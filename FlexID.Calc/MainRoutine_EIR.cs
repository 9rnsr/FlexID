using System;
using System.Collections.Generic;
using System.Linq;

namespace FlexID.Calc
{
    /// <summary>
    /// 公衆の構成員による放射性核種の摂取(EIR:  Environmental Intakes of Radionuclides)における
    /// 残留放射能および預託線量の計算。
    /// </summary>
    public class MainRoutine_EIR
    {
        /// <summary>
        /// 出力ファイルパス。
        /// </summary>
        public string OutputPath { get; set; }

        /// <summary>
        /// 核種情報ファイルパス。
        /// </summary>
        public string InputPath { get; set; }

        /// <summary>
        /// 計算時間メッシュファイルパス。
        /// </summary>
        public string CalcTimeMeshPath { get; set; }

        /// <summary>
        /// 出力メッシュファイルパス。
        /// </summary>
        public string OutTimeMeshPath { get; set; }

        /// <summary>
        /// 預託期間。
        /// </summary>
        public string CommitmentPeriod { get; set; }

        /// <summary>
        /// 子孫核種の計算を行うかどうか。
        /// </summary>
        public bool CalcProgeny { get; set; }

        /// <summary>
        /// 被ばく時の年齢。
        /// </summary>
        public string ExposureAge { get; set; }

        private Activity Act { get; } = new Activity();

        private CalcOut CalcOut { get; set; }

        // EIR計算時の切替年齢
        public const int Age3month = 100;       // 100日と考える
        public const int Age1year = 1 * 365;
        public const int Age5year = 5 * 365;
        public const int Age10year = 10 * 365;
        public const int Age15year = 15 * 365;
        public const int AgeAdult = 25 * 365;   // TODO: 現在はSrしか計算しないため25歳で決め打ち

        public void Main()
        {
            var calcTimeMesh = new TimeMesh(CalcTimeMeshPath);
            var outTimeMesh = new TimeMesh(OutTimeMeshPath);
            if (!calcTimeMesh.Cover(outTimeMesh))
                throw Program.Error("Calculation time mesh does not cover all boundaries of output time mesh.");

            var dataList = new DataReader(InputPath, CalcProgeny).Read_EIR();

            using (CalcOut = new CalcOut(dataList[0], OutputPath))
            {
                MainCalc(calcTimeMesh, outTimeMesh, dataList);
            }
        }

        private void MainCalc(TimeMesh calcTimeMesh, TimeMesh outTimeMesh, List<DataClass> dataList)
        {
            DataClass dataLo;
            DataClass dataHi;

            // 移行係数以外は変わらないので、とりあえず3monthのデータを入れる
            dataLo = dataList[0];

            const double convergence = 1E-14; // 収束値
            const int iterMax = 1500;  // iterationの最大回数

            // 預託期間[sec]を取得。
            var commitmentPeriod = TimeMesh.CommitmentPeriodToSeconds(CommitmentPeriod);

            var age3monthT /**/= TimeMesh.DaysToSeconds(Age3month);
            var age1yearT  /**/= TimeMesh.DaysToSeconds(Age1year);
            var age5yearT  /**/= TimeMesh.DaysToSeconds(Age5year);
            var age10yearT /**/= TimeMesh.DaysToSeconds(Age10year);
            var age15yearT /**/= TimeMesh.DaysToSeconds(Age15year);
            var ageAdultT  /**/= TimeMesh.DaysToSeconds(AgeAdult);

            // 被ばく年齢[sec]を取得。
            var exposureAge =
                ExposureAge == /**/"3months old" ? age3monthT :
                ExposureAge == /**/ "1years old" ? age1yearT :
                ExposureAge == /**/ "5years old" ? age5yearT :
                ExposureAge == /**/"10years old" ? age10yearT :
                ExposureAge == /**/"15years old" ? age15yearT :
                ExposureAge == /**/"Adult" ? ageAdultT :
                throw Program.Error("Please select the age at the time of exposure.");

            DataClass dataLoInterp = null;  // 年齢区間の切り替わり検出用。
            double[][] sourcesSee = null;
            double[][] organsSee = null;

            // S係数の補間のための領域を確保する。
            void InterpolationAlloc()
            {
                if (dataLo == dataLoInterp)
                    return;
                dataLoInterp = dataLo;

                // 実際の所、EIRでは全ての年齢区間で線源領域は同じのはずで、
                // また体内動態モデルを定義するコンパートメント群も同じになるようだが
                // 現状のデータの持ち方としてこれらが年齢区間毎に異なり得ると想定している。
                Array.Resize(ref sourcesSee, dataLo.Nuclides.Select(n => n.SourceRegions.Length).Sum());
                Array.Resize(ref organsSee, dataLo.Organs.Count);

                int offsetS = 0;
                foreach (var nuclide in dataLo.Nuclides)
                {
                    var sourcesCount = nuclide.SourceRegions.Length;

                    for (int indexS = 0; indexS < sourcesCount; indexS++)
                    {
                        // ある核種における線源領域の1つ。
                        var sourceRegion = nuclide.SourceRegions[indexS];

                        // 各標的領域への補間されたS係数値が書き込まれる配列を確保(または再利用)する。
                        var see = sourcesSee[offsetS + indexS];
                        see = see ?? (sourcesSee[offsetS + indexS] = new double[31]);

                        // 線源領域に対応する全てのコンパートメントに、
                        // 補間されたS係数値が書き込まれる予定の配列を割り当てる。
                        foreach (var organ in dataLo.Organs
                            .Where(o => o.Nuclide == nuclide && o.SourceRegion == sourceRegion))
                        {
                            organsSee[organ.Index] = see;
                        }
                    }

                    offsetS += sourcesCount;
                }
            }

            // S係数の補間をやめて、コンパートメント毎に設定されたS係数を使用するよう切り替える。
            void InterpolationReset()
            {
                if (dataLo == dataLoInterp && sourcesSee is null)
                    return;
                dataLoInterp = dataLo;

                sourcesSee = null;
                Array.Resize(ref organsSee, dataLo.Organs.Count);

                foreach (var organ in dataLo.Organs)
                {
                    organsSee[organ.Index] = organ.S_Coefficients;
                }
            }

            // 指定の計算時間メッシュにおけるS係数の補間計算を実施する。
            void InterpolationCalc(double nowT)
            {
                if (nowT < 6205)
                {
                    // 17歳未満の被ばく期間では、SEEデータを補間する。
                    InterpolationAlloc();

                    int offsetS = 0;
                    foreach (var nuclide in dataLo.Nuclides)
                    {
                        var sourcesCount = nuclide.SourceRegions.Length;

                        int indexN = dataLo.Nuclides.IndexOf(nuclide);
                        var tableLo = dataLo.SCoeffTables[indexN];
                        var tableHi = dataHi.SCoeffTables[indexN];

                        for (int indexS = 0; indexS < sourcesCount; indexS++)
                        {
                            // ある核種における線源領域の1つ。
                            var sourceRegion = nuclide.SourceRegions[indexS];

                            // 各標的領域への補間されたS係数値を書き込むための配列を取得する。
                            var see = sourcesSee[offsetS + indexS];

                            var seeLo = tableLo[sourceRegion];
                            var seeHi = tableHi[sourceRegion];
                            for (int indexT = 0; indexT < 31; indexT++)
                            {
                                var scoeffLo = seeLo[indexT];
                                var scoeffHi = seeHi[indexT];
                                var scoeff = SubRoutine.Interpolation(nowT, scoeffLo, scoeffHi, dataLo.StartAge, dataHi.StartAge);
                                see[indexT] = scoeff;
                            }
                        }
                        offsetS += sourcesCount;
                    }
                }
                else
                {
                    InterpolationReset();
                }
            }

            // 標的領域の組織加重係数を取得。
            var targetWeights = dataList[0].TargetWeights;

            // 計算時間メッシュを準備する。
            var calcTimes = calcTimeMesh.Start();
            long calcPreT;
            long calcNowT = calcTimes.Current;
            int calcIter;   // 計算時間メッシュ毎の収束計算回数

            // 出力時間メッシュを準備する。
            var outTimes = outTimeMesh.Start();
            long outPreT;
            long outNowT = outTimes.Current;
            int outIter;    // 出力時間メッシュ毎の収束計算回数

            var wholeBodyNow = 0.0; // 今回の出力時間メッシュにおける全身の積算線量。
            var wholeBodyPre = 0.0; // 前回の出力時間メッシュにおける全身の積算線量。
            var resultNow = new double[31]; // 今回の出力時間メッシュにおける組織毎の計算結果。
            var resultPre = new double[31]; // 前回の出力時間メッシュにおける組織毎の計算結果。

            // inputの初期値を各コンパートメントに振り分ける。
            SubRoutine.Init(Act, dataLo);

            // 初期配分された放射能をファイルに出力する。
            CalcOut.ActivityOut(0.0, Act, 0);

            // 出力時間メッシュを進める。
            outTimes.MoveNext();
            outPreT = outNowT;
            outNowT = outTimes.Current;
            outIter = 0;

            // 計算時間メッシュを進める。
            while (calcTimes.MoveNext())
            {
                calcPreT = calcNowT;
                calcNowT = calcTimes.Current;

                // 預託期間を超える計算は行わない
                if (commitmentPeriod < calcNowT)
                    break;

                Act.NextCalc(dataLo);

                var calcNowDay = TimeMesh.SecondsToDays(calcNowT);

                // ΔT[sec]
                var calcDeltaT = calcNowT - calcPreT;
                var calcDeltaDay = TimeMesh.SecondsToDays(calcDeltaT);

                // 生まれてからの日数によってLoとHiを変える
                var ageT = exposureAge + calcNowT;
                var ageDay = TimeMesh.SecondsToDays(ageT);
                if (ageT <= age1yearT)
                {
                    dataLo = dataList[0];
                    dataHi = dataList[1];
                }
                else if (ageT <= age5yearT)
                {
                    dataLo = dataList[1];
                    dataHi = dataList[2];
                }
                else if (ageT <= age10yearT)
                {
                    dataLo = dataList[2];
                    dataHi = dataList[3];
                }
                else if (ageT <= age15yearT)
                {
                    dataLo = dataList[3];
                    dataHi = dataList[4];
                }
                else if (ageT <= ageAdultT)
                {
                    dataLo = dataList[4];
                    dataHi = dataList[5];
                }
                else
                {
                    dataLo = dataList[5];
                    dataHi = dataList[5];
                }
                int daysLo = dataLo.StartAge;
                int daysHi = dataHi.StartAge;

                #region 1つの計算時間メッシュ内で収束計算を繰り返す
                for (calcIter = 1; calcIter <= iterMax; calcIter++)
                {
                    for (int i = 0; i < dataLo.Organs.Count; i++)
                    {
                        var organLo = dataLo.Organs[i];
                        var organHi = dataHi.Organs[i];

                        var func = organLo.Func; // 臓器機能

                        // 臓器機能ごとに異なる処理をする
                        if (func == OrganFunc.inp) // 入力
                        {
                            SubRoutine.Input(organLo, Act);
                        }
                        else if (func == OrganFunc.acc) // 蓄積
                        {
                            SubRoutine.Accumulation_EIR(organLo, organHi, Act, calcDeltaDay, ageDay, daysLo, daysHi);
                        }
                        else if (func == OrganFunc.mix) // 混合
                        {
                            SubRoutine.Mix(organLo, Act);
                        }
                        else if (func == OrganFunc.exc) // 排泄
                        {
                            SubRoutine.Excretion(organLo, Act, calcDeltaDay);
                        }

                        Act.CalcNow[organLo.Index].ini = Act.IterNow[organLo.Index].ini;
                        Act.CalcNow[organLo.Index].ave = Act.IterNow[organLo.Index].ave;
                        Act.CalcNow[organLo.Index].end = Act.IterNow[organLo.Index].end;

                        Act.CalcNow[organLo.Index].total = Act.IterNow[organLo.Index].total;
                    }

                    // 前回との差が収束するまで計算を繰り返す
                    if (calcIter > 1)
                    {
                        var converged = true;
                        foreach (var o in dataLo.Organs)
                        {
                            ref var iterNow = ref Act.IterNow[o.Index];
                            ref var iterPre = ref Act.IterPre[o.Index];

                            var s1 = iterNow.ini != 0 ? Math.Abs((iterNow.ini - iterPre.ini) / iterNow.ini) : 0;
                            var s2 = iterNow.ave != 0 ? Math.Abs((iterNow.ave - iterPre.ave) / iterNow.ave) : 0;
                            var s3 = iterNow.end != 0 ? Math.Abs((iterNow.end - iterPre.end) / iterNow.end) : 0;

                            if (s1 > convergence || s2 > convergence || s3 > convergence)
                            {
                                converged = false;
                                break;
                            }
                        }
                        // 前回との差が全ての臓器で収束した場合
                        if (converged)
                        {
                            // 出力メッシュと終端が一致する計算メッシュにおける反復回数を保存する。
                            outIter = calcIter;

                            // // 出力メッシュ内での総反復回数を保存する。
                            // outIter += iter;
                            break;
                        }
                    }

                    Act.NextIter(dataLo);
                }
                #endregion

                // 時間メッシュ毎の放射能を足していく
                foreach (var organ in dataLo.Organs)
                {
                    // 今回の出力時間メッシュにおける積算放射能。
                    Act.OutNow[organ.Index].total += Act.CalcNow[organ.Index].total;

                    // 摂取時からの積算放射能。
                    Act.OutTotalFromIntake[organ.Index] += Act.CalcNow[organ.Index].total;
                }

                // S係数の補間計算を実施する。
                InterpolationCalc(ageDay);

                foreach (var organ in dataLo.Organs)
                {
                    // コンパートメントが線源領域に対応しない場合は何もしない。
                    if (organ.SourceRegion is null)
                        continue;

                    if (organ.Name.Contains("mix"))
                        continue;

                    var nucDecay = organ.NuclideDecay;

                    // コンパートメントの残留放射能がゼロの場合は何もしない。
                    var activity = Act.CalcNow[organ.Index].end * calcDeltaT * nucDecay;
                    if (activity == 0)
                        continue;

                    // コンパートメントから各標的領域への預託線量を計算する。
                    for (int indexT = 0; indexT < 31; indexT++)
                    {
                        // 標的領域の部分的な重量。
                        var targetWeight = targetWeights[indexT];

                        // S係数(補間あり)。
                        var scoeff = organsSee[organ.Index][indexT];

                        // 等価線量 = 放射能 * S係数
                        var equivalentDose = activity * scoeff;

                        // 実効線量 = 等価線量 * wT
                        var effectiveDose = equivalentDose * targetWeight;

                        resultNow[indexT] += equivalentDose;
                        wholeBodyNow += effectiveDose;
                    }
                }

                if (calcNowT == outNowT)
                {
                    var outDeltaT = outNowT - outPreT;

                    var outNowDay = TimeMesh.SecondsToDays(outNowT);
                    var outPreDay = TimeMesh.SecondsToDays(outPreT);
                    var outDeltaDay = TimeMesh.SecondsToDays(outDeltaT);

                    // 出力時間メッシュにおける平均と末期の残留放射能を計算する。
                    foreach (var organ in dataLo.Organs)
                    {
                        Act.OutNow[organ.Index].ave = Act.OutNow[organ.Index].total / outDeltaDay;
                        Act.OutNow[organ.Index].end = Act.CalcNow[organ.Index].end;
                    }

                    // 放射能をファイルに出力する。
                    CalcOut.ActivityOut(outNowDay, Act, outIter);

                    // 線量をファイルに出力する。
                    CalcOut.CommitmentOut(outNowDay, outPreDay, wholeBodyNow, wholeBodyPre, resultNow, resultPre);

                    // これ以上出力時間メッシュが存在しないならば、計算を終了する。
                    if (!outTimes.MoveNext())
                        break;

                    // 出力時間メッシュを進める。
                    outPreT = outNowT;
                    outNowT = outTimes.Current;
                    outIter = 0;

                    Act.NextOut(dataLo);

                    wholeBodyPre = wholeBodyNow;
                    Array.Copy(resultNow, resultPre, resultNow.Length);
                }
            }

            // 計算完了の出力を行う。
            CalcOut.FinishOut();
        }
    }
}
