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

        public void Main(List<InputData> dataList)
        {
            var calcTimeMesh = new TimeMesh(CalcTimeMeshPath);
            var outTimeMesh = new TimeMesh(OutTimeMeshPath);
            if (!calcTimeMesh.Cover(outTimeMesh))
                throw Program.Error("Calculation time mesh does not cover all boundaries of output time mesh.");

            using (CalcOut = new CalcOut(dataList[0], OutputPath))
            {
                // コンパートメントの名称をヘッダ―として出力。
                CalcOut.ActivityHeader();

                // 標的領域の名称をヘッダーとして出力。
                CalcOut.CommitmentHeader();

                MainCalc(calcTimeMesh, outTimeMesh, dataList);
            }
        }

        private void MainCalc(TimeMesh calcTimeMesh, TimeMesh outTimeMesh, List<InputData> dataList)
        {
            InputData dataLo;
            InputData dataHi;

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

            InputData dataLoInterp = null;  // 年齢区間の切り替わり検出用。
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

            const long Delta24hourT = 24 * 60 * 60;
            long outLastExcretaT = outPreT; // excコンパートメントで最後に数値を出力した時間メッシュ
            long outBefore24hourT = outNowT - Delta24hourT;

            // 計算時間メッシュを進める。
            while (calcTimes.MoveNext())
            {
                calcPreT = calcNowT;
                calcNowT = calcTimes.Current;

                // 預託期間を超える計算は行わない
                if (commitmentPeriod < calcNowT)
                    break;

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

                Act.NextCalc(dataLo);

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
                    }

                    // 前回との差が収束するまで計算を繰り返す
                    if (Act.NextIter(dataLo, convergence))
                        continue;

                    // 出力メッシュと終端が一致する計算メッシュにおける反復回数を保存する。
                    outIter = calcIter;

                    // // 出力メッシュ内での総反復回数を保存する。
                    // outIter += calcIter;
                    break;
                }
                #endregion

                Act.FinishIter();

                // 時間メッシュ毎の放射能を足していく
                foreach (var organ in dataLo.Organs)
                {
                    var calcNowTotal = Act.CalcNow[organ.Index].total;

                    // 今回の出力時間メッシュにおける積算放射能。
                    if (organ.IsExcretaCompatibleWithOIR && calcPreT < outBefore24hourT)
                    {
                        // 計算時間メッシュが、次回の出力時間メッシュから24-hour前の位置を跨いでいる場合。
                        if (outBefore24hourT < calcNowT)
                        {
                            // 24-hourの範囲内となる分だけを加算する。
                            var rate = (double)(calcNowT - outBefore24hourT) / calcDeltaT;
                            Act.OutNow[organ.Index].total += calcNowTotal * rate;
                        }
                    }
                    else
                        Act.OutNow[organ.Index].total += calcNowTotal;

                    // 摂取時からの積算放射能。
                    Act.OutTotalFromIntake[organ.Index] += calcNowTotal;
                }

                // S係数の補間計算を実施する。
                InterpolationCalc(ageDay);

                foreach (var organ in dataLo.Organs)
                {
                    // コンパートメントが線源領域に対応しない場合は何もしない。
                    if (organ.SourceRegion is null)
                        continue;

                    // コンパートメントの残留放射能がゼロの場合は何もしない。
                    var activity = Act.CalcNow[organ.Index].ave * calcDeltaT;
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

                    if (outDeltaT < Delta24hourT)
                    {
                        // 24時間より小さい幅を持つ出力時間メッシュは、それらを合わせた時間幅が
                        // 前回の'exc'コンパートメントに対する数値出力から24-hour経過した時間と一致することをここで確認する。
                        // - 'exc'の残留放射能の出力に寄与しない出力時間メッシュがないことを保証する。
                        // - これを許した場合、excの積算放射能を構成する複数の出力時間メッシュのうち、24-hourを超える古い側の
                        //   メッシュの寄与を後から減算しなければならず、この面倒な処理を避けるという理由もある。
                        // - 本来は計算開始前にTimeMesh入力検証で確認すべきだが、現在はOIRの'exc'に特有の処理であるためここで確認している。
                        if ((outNowT - outLastExcretaT) > Delta24hourT)
                            throw new Exception("sum of out time meshes those are smaller than 24-hour delta should match to 24-hour, "
                                              + "for the 'exc' compartment output compatibility with OIR.");
                    }

                    // OIR互換排泄コンパートメントについて、前回の数値出力からの
                    // 経過時間が24時間に満たない場合は、数値出力を抑制する。
                    var maskExcreta = outNowT < outLastExcretaT + Delta24hourT;

                    // 出力時間メッシュにおける平均と末期の残留放射能を計算する。
                    foreach (var organ in dataLo.Organs)
                    {
                        if (organ.IsExcretaCompatibleWithOIR && !maskExcreta)
                        {
                            // excのtotalは24-hourの時間幅で計算するため、total=aveとなる。
                            Act.OutNow[organ.Index].ave = Act.OutNow[organ.Index].total;
                        }
                        else
                            Act.OutNow[organ.Index].ave = Act.OutNow[organ.Index].total / outDeltaDay;

                        Act.OutNow[organ.Index].end = Act.CalcNow[organ.Index].end;
                    }

                    // 放射能をファイルに出力する。
                    CalcOut.ActivityOut(outNowDay, Act, outIter, maskExcreta);

                    // 線量をファイルに出力する。
                    CalcOut.CommitmentOut(outNowDay, outPreDay, wholeBodyNow, wholeBodyPre, resultNow, resultPre);

                    // これ以上出力時間メッシュが存在しないならば、計算を終了する。
                    if (!outTimes.MoveNext())
                        break;

                    // 出力時間メッシュを進める。
                    outPreT = outNowT;
                    outNowT = outTimes.Current;
                    outIter = 0;

                    if (!maskExcreta)
                        outLastExcretaT = outPreT;
                    outBefore24hourT = outNowT - Delta24hourT;

                    Act.NextOut(dataLo, maskExcreta);

                    wholeBodyPre = wholeBodyNow;
                    Array.Copy(resultNow, resultPre, resultNow.Length);
                }
            }

            // 計算完了の出力を行う。
            CalcOut.FinishOut();
        }
    }
}
