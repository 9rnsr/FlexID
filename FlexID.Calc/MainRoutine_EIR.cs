using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

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

        private CalcOut CalcOut { get; } = new CalcOut();

        // 反復回数カウント
        private Dictionary<double, int> iterLog = new Dictionary<double, int>();

        // EIR計算時の切替年齢
        private int month3 = 100;
        private int year1 = 365;
        private int year5 = 1825;
        private int year10 = 3650;
        private int year15 = 5475;
        public static int adult = 9125; // 現在はSrしか計算しないため25歳で決め打ち、今後インプット等で成人の年齢を読み込む必要あり？

        public void Main()
        {
            string TmpFile = Path.GetTempFileName();

            var fileReader = new FileReader();
            var Input = fileReader.InfoReader(InputPath);
            var CalcTimeMesh = fileReader.MeshReader(CalcTimeMeshPath);
            var OutTimeMesh = fileReader.OutReader(OutTimeMeshPath);

            var dataList = DataClass.Read_EIR(Input, CalcProgeny);
            var wT = SubRoutine.WeightTissue(@"lib\EIR\wT.txt");

            var RetentionPath = OutputPath + "_Retention.out";
            var CumulativePath = OutputPath + "_Cumulative.out";
            var DosePath = OutputPath + "_Dose.out";
            var DoseRatePath = OutputPath + "_DoseRate.out";
            //var IterPath = OutputPath + "_IterLog.out";

            Directory.CreateDirectory(Path.GetDirectoryName(RetentionPath));

            using (var wTmp = new StreamWriter(TmpFile)) // テンポラリファイル
            using (var dCom = new StreamWriter(DosePath, false, Encoding.UTF8)) // 実効線量
            using (var rCom = new StreamWriter(DoseRatePath, false, Encoding.UTF8)) // 線量率
            {
                CalcOut.wTmp = wTmp;
                CalcOut.dCom = dCom;
                CalcOut.rCom = rCom;

                MainCalc(CalcTimeMesh, OutTimeMesh, dataList, wT);
            }

            // テンポラリファイルを並び替えて出力
            CalcOut.ActivityOut(RetentionPath, CumulativePath, TmpFile, dataList[0]);

            // Iter出力
            //CalcOut.IterOut(CalcTimeMesh, iterLog, IterPath);

            File.Delete(TmpFile);
        }

        private void MainCalc(List<double> CalcTimeMesh, List<double> OutTimeMesh, List<DataClass> dataList, Dictionary<string, double> wT)
        {
            DataClass dataLo;
            DataClass dataHi;

            // 移行係数以外は変わらないので、とりあえず3monthのデータを入れる
            dataLo = dataList[0];

            const double convergence = 1E-14; // 収束値
            const int iterMax = 1500;  // iterationの最大回数

            // 預託期間[day]を取得。
            var commitmentDays = SubRoutine.CommitmentPeriodToDays(CommitmentPeriod);

            // 被ばく年齢を"days"に変換
            int ExposureDays = 0;
            if (ExposureAge == "3months old")
                ExposureDays = 100; //100日と考える
            else if (ExposureAge == "1years old")
                ExposureDays = 1 * 365;
            else if (ExposureAge == "5years old")
                ExposureDays = 5 * 365;
            else if (ExposureAge == "10years old")
                ExposureDays = 10 * 365;
            else if (ExposureAge == "15years old")
                ExposureDays = 15 * 365;
            else if (ExposureAge == "adult")
                //ExposureDays = 20 * 365;
                ExposureDays = 25 * 365;
            else
                throw Program.Error("Please select the age at the time of exposure.");

            // 流入割合がマイナスの時の処理は親からの分岐比*親の崩壊定数とする
            foreach (var data in dataList)
            {
                foreach (var organ in data.Organs)
                {
                    foreach (var inflow in organ.Inflows)
                    {
                        if (inflow.Organ == null)
                            continue;

                        var nucDecay = inflow.Organ.NuclideDecay;

                        if (inflow.Rate < 0)
                            inflow.Rate = organ.Nuclide.DecayRate * nucDecay;
                    }
                }
            }

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

                        // 各標的組織への補間されたS係数値が書き込まれる配列を確保(または再利用)する。
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
            void InterporationReset()
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
            void InterpopationCalc(double nowT)
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

                            // 各標的組織への補間されたS係数値を書き込むための配列を取得する。
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
                    InterporationReset();
                }
            }

            // 標的組織の名称リストを(親核種のS係数データから)取得。
            var targetTissues = dataList[0].Nuclides[0].TargetTissues;
            var targetWeights = targetTissues.Select(t => wT[t]).ToArray();

            // 標的組織の名称をヘッダーとして出力。
            CalcOut.CommitmentTarget(targetTissues.ToList(), dataList[0]);

            // 経過時間=0での計算結果を処理する
            int ctime = 0;  // 計算時間メッシュのインデックス
            int otime = 0;  // 出力時間メッシュのインデックス
            {
                var calcNowT = CalcTimeMesh[ctime];

                // inputの初期値を各臓器に振り分ける
                SubRoutine.Init(Act, dataLo);

                iterLog[calcNowT] = 0;

                var outNowT = calcNowT;

                // 計算結果をテンポラリファイルに出力
                var flgTime = true;
                foreach (var organ in dataLo.Organs)
                {
                    var nucDecay = organ.NuclideDecay;

                    CalcOut.TemporaryOut(
                        outNowT, flgTime, organ.ID,
                        Act.Now[organ.Index].end * nucDecay,
                        Act.Now[organ.Index].total * nucDecay,
                        Act.IntakeQuantityNow[organ.Index] * nucDecay,
                        iterLog[outNowT]);
                    flgTime = false;
                }
            }

            // 処理中の出力メッシュにおける臓器毎の積算放射能
            var OutMeshTotal = new double[dataLo.Organs.Count];

            double WholeBody = 0;  // 積算線量
            double preBody = 0;
            var Result = new double[31];  // 組織毎の計算結果
            var preResult = new double[31];

            void ClearOutMeshTotal()
            {
                foreach (var organ in dataLo.Organs)
                {
                    OutMeshTotal[organ.Index] = 0;
                }
            }
            ClearOutMeshTotal();    // 各臓器の積算放射能として0を設定する

            ctime = 1;
            otime = 1;
            for (; ctime < CalcTimeMesh.Count; ctime++)
            {
                // 不要な前ステップのデータを削除
                Act.NextTime(dataLo);

                var calcPreT = CalcTimeMesh[ctime - 1];
                var calcNowT = CalcTimeMesh[ctime - 0];

                // 預託期間を超える計算は行わない
                if (calcNowT > commitmentDays)
                    break;

                // 生まれてからの日数によってLoとHiを変える
                if (calcNowT + ExposureDays <= year1)
                {
                    dataLo = dataList[0];
                    dataHi = dataList[1];
                }
                else if (calcNowT + ExposureDays <= year5)
                {
                    dataLo = dataList[1];
                    dataHi = dataList[2];
                }
                else if (calcNowT + ExposureDays <= year10)
                {
                    dataLo = dataList[2];
                    dataHi = dataList[3];
                }
                else if (calcNowT + ExposureDays <= year15)
                {
                    dataLo = dataList[3];
                    dataHi = dataList[4];
                }
                else if (calcNowT + ExposureDays <= adult)
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
                for (int iter = 1; iter <= iterMax; iter++)
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
                            SubRoutine.Accumulation_EIR(calcNowT - calcPreT, organLo, organHi, Act, calcNowT + ExposureDays, daysLo, daysHi);
                        }
                        else if (func == OrganFunc.mix) // 混合
                        {
                            SubRoutine.Mix(organLo, Act);
                        }
                        else if (func == OrganFunc.exc) // 排泄物
                        {
                            SubRoutine.Excretion(organLo, Act, calcNowT - calcPreT);
                        }

                        Act.Now[organLo.Index].ini = Act.rNow[organLo.Index].ini;
                        Act.Now[organLo.Index].ave = Act.rNow[organLo.Index].ave;
                        Act.Now[organLo.Index].end = Act.rNow[organLo.Index].end;

                        Act.Now[organLo.Index].total = Act.rNow[organLo.Index].total;

                        // 臓器毎の積算放射能算出
                        Act.IntakeQuantityNow[organLo.Index] =
                            Act.IntakeQuantityPre[organLo.Index] + Act.Now[organLo.Index].total;
                    }

                    // 前回との差が収束するまで計算を繰り返す
                    if (iter > 1)
                    {
                        var flgIter = true;
                        foreach (var o in dataLo.Organs)
                        {
                            double s1 = 0;
                            double s2 = 0;
                            double s3 = 0;

                            if (Act.rNow[o.Index].ini != 0)
                                s1 = Math.Abs((Act.rNow[o.Index].ini - Act.rPre[o.Index].ini) / Act.rNow[o.Index].ini);
                            if (Act.rNow[o.Index].ave != 0)
                                s2 = Math.Abs((Act.rNow[o.Index].ave - Act.rPre[o.Index].ave) / Act.rNow[o.Index].ave);
                            if (Act.rNow[o.Index].end != 0)
                                s3 = Math.Abs((Act.rNow[o.Index].end - Act.rPre[o.Index].end) / Act.rNow[o.Index].end);
                            else
                                continue;   // todo: s3==0のときにs1,s2の差を無視してしまう？

                            if (s1 > convergence || s2 > convergence || s3 > convergence)
                            {
                                flgIter = false;
                                break;
                            }
                        }
                        // 前回との差が全ての臓器で収束した場合
                        if (flgIter)
                        {
                            iterLog[calcNowT] = iter;
                            break;
                        }
                    }

                    Act.NextIter(dataLo);
                }
                #endregion

                // 時間メッシュ毎の放射能を足していく
                foreach (var organ in dataLo.Organs)
                {
                    OutMeshTotal[organ.Index] += Act.Now[organ.Index].total;
                    Act.Excreta[organ.Index] += Act.PreExcreta[organ.Index];
                }

                // 出力時間メッシュを超える計算は行わない
                if (OutTimeMesh.Count <= otime)
                    break;

                // S係数の補間計算を実施する。
                InterpopationCalc(calcNowT + ExposureDays);

                // ΔT[sec]
                var deltaT = (calcNowT - calcPreT) * 24 * 3600;

                foreach (var organ in dataLo.Organs)
                {
                    if (organ.Name.Contains("mix"))
                        continue;

                    var nuclide = organ.Nuclide;
                    var nucDecay = nuclide.Ramd;

                    // タイムステップごとの放射能　
                    var activity = Act.Now[organ.Index].end * deltaT * nucDecay;
                    if (activity == 0)
                        continue;

                    if (organ.SourceRegion != null)
                    {
                        // コンパートメントから各標的組織への預託線量を計算する。
                        for (int indexT = 0; indexT < 31; indexT++)
                        {
                            // 標的組織の部分的な重量。
                            var targetWeight = targetWeights[indexT];

                            // S係数(補間あり)。
                            var scoeff = organsSee[organ.Index][indexT];

                            // 等価線量 = 放射能 * S係数
                            var equivalentDose = activity * scoeff;

                            // 実効線量 = 等価線量 * wT
                            var effectiveDose = equivalentDose * targetWeight;

                            Result[indexT] += equivalentDose;
                            WholeBody += effectiveDose;
                        }
                    }
                }

                if (calcNowT == OutTimeMesh[otime])
                {
                    var outPreT = OutTimeMesh[otime - 1];
                    var outNowT = OutTimeMesh[otime - 0];

                    #region 残留放射能をテンポラリファイルに出力
                    var flgTime = true;
                    foreach (var organ in dataLo.Organs)
                    {
                        if (organ.Func == OrganFunc.exc)
                            Act.Now[organ.Index].end = Act.Excreta[organ.Index] / (outNowT - outPreT);

                        if (!iterLog.ContainsKey(outNowT))
                            continue;   // iterの上限を超える場合　iter上限を挙げている為現状到達しないはず

                        var nucDecay = organ.NuclideDecay;

                        CalcOut.TemporaryOut(
                            outNowT, flgTime, organ.ID,
                            Act.Now[organ.Index].end * nucDecay,
                            OutMeshTotal[organ.Index] * nucDecay,
                            Act.IntakeQuantityNow[organ.Index] * nucDecay,
                            iterLog[outNowT]);
                        flgTime = false;
                    }
                    ClearOutMeshTotal();
                    #endregion

                    CalcOut.CommitmentOut(outNowT, outPreT, WholeBody, preBody, Result, preResult);
                    preBody = WholeBody;
                    Array.Copy(Result, preResult, Result.Length);
                    otime++;
                }
            }
        }
    }
}
