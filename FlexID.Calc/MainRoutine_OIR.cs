using System;
using System.IO;
using System.Linq;
using System.Text;

namespace FlexID.Calc
{
    /// <summary>
    /// 放射性核種の職業上の摂取(OIR: Occupational Intakes of Radionuclides)における
    /// 残留放射能および預託線量の計算。
    /// </summary>
    public class MainRoutine_OIR
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

        public Activity Act { get; } = new Activity();

        private CalcOut CalcOut { get; set; }

        public void Main()
        {
            var calcTimeMesh = new TimeMesh(CalcTimeMeshPath);
            var outTimeMesh = new TimeMesh(OutTimeMeshPath);
            if (!calcTimeMesh.Cover(outTimeMesh))
                throw Program.Error("Calculation time mesh does not cover all boundaries of output time mesh.");

            var data = new InputDataReader(InputPath, CalcProgeny).Read_OIR();

            using (CalcOut = new CalcOut(data, OutputPath))
            {
                // ログファイルを出力する。
                WriteOutLog(data);

                // OIRでは、集合コンパートメントを処理するための準備を行う。
                CalcOut.PrepareCompositeCompartments();

                // コンパートメントの名称をヘッダ―として出力。
                CalcOut.ActivityHeader();

                // 標的領域の名称をヘッダーとして出力。
                CalcOut.CommitmentHeader();

                MainCalc(calcTimeMesh, outTimeMesh, data);
            }
        }

        private void WriteOutLog(InputData data)
        {
            var logPath = OutputPath + ".log";

            using (var stream = new FileStream(logPath, FileMode.Create, FileAccess.Write, FileShare.Read))
            using (var writer = new StreamWriter(stream, Encoding.UTF8))
            {
                var otherSourceRegion = "Other";

                foreach (var (nuclide, scoeffTable) in data.Nuclides.Zip(data.SCoeffTables))
                {
                    writer.WriteLine();
                    writer.WriteLine($"Nuclide: {nuclide.Nuclide}");
                    writer.WriteLine();
                    writer.WriteLine($"Source regions those are part of '{otherSourceRegion}':");
                    writer.WriteLine(string.Join(",", nuclide.OtherSourceRegions));
                    writer.WriteLine();
                    writer.WriteLine($"S-Coefficient values from '{otherSourceRegion}' to each target regions:");
                    writer.WriteLine($"{"  T/S",-10} {otherSourceRegion,-14}");

                    var scoeffOther = scoeffTable[otherSourceRegion];

                    foreach (var (targetRegion, scoeff) in data.TargetRegions.Zip(scoeffOther))
                    {
                        writer.WriteLine($"{targetRegion,-10} {scoeff:0.00000000E+00}");
                    }
                }
            }
        }

        private void MainCalc(TimeMesh calcTimeMesh, TimeMesh outTimeMesh, InputData data)
        {
            const double convergence = 1E-10; // 収束値
            const int iterMax = 1500;  // iterationの最大回数

            // 預託期間[sec]を取得。
            var commitmentPeriod = TimeMesh.CommitmentPeriodToSeconds(CommitmentPeriod);

            // 標的領域の組織加重係数を取得。
            var targetWeights = data.TargetWeights;

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
            var resultNow = new double[43]; // 今回の出力時間メッシュにおける組織毎の計算結果。
            var resultPre = new double[43]; // 前回の出力時間メッシュにおける組織毎の計算結果。

            // inputの初期値を各コンパートメントに振り分ける。
            SubRoutine.Init(Act, data);

            // 初期配分された放射能をファイルに出力する。
            CalcOut.ActivityOut(0.0, Act, 0);

            // 出力時間メッシュを進める。
            outTimes.MoveNext();
            outPreT = outNowT;
            outNowT = outTimes.Current;
            outIter = 0;

            const long delta24hourT = 24 * 60 * 60;
            long outExcLastT = outPreT; // excコンパートメントで最後に数値を出力した時間メッシュ
            long outBefore24hourT = outNowT - delta24hourT;

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

                Act.NextCalc(data);

                #region 1つの計算時間メッシュ内で収束計算を繰り返す
                for (calcIter = 1; calcIter <= iterMax; calcIter++)
                {
                    foreach (var organ in data.Organs)
                    {
                        var func = organ.Func; // 臓器機能

                        // 臓器機能ごとに異なる処理をする
                        if (func == OrganFunc.inp) // 入力
                        {
                            SubRoutine.Input(organ, Act);
                        }
                        else if (func == OrganFunc.acc) // 蓄積
                        {
                            SubRoutine.Accumulation_OIR(organ, Act, calcDeltaDay);
                        }
                        else if (func == OrganFunc.mix) // 混合
                        {
                            SubRoutine.Mix(organ, Act);
                        }
                        else if (func == OrganFunc.exc) // 排泄
                        {
                            SubRoutine.Excretion(organ, Act, calcDeltaDay);
                        }
                    }

                    // 前回との差が収束するまで計算を繰り返す
                    if (Act.NextIter(data, convergence))
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
                foreach (var organ in data.Organs)
                {
                    var calcNowTotal = Act.CalcNow[organ.Index].total;

                    // 今回の出力時間メッシュにおける積算放射能。
                    if (organ.ExcretaCompatibleWithOIR && calcPreT < outBefore24hourT)
                    {
                        // 計算時間メッシュが、次回の出力時間メッシュから24-hour前の位置を跨いでいる場合。
                        if (outBefore24hourT < calcNowT)
                        {
                            // 24-hourの範囲内となる分だけを加算する。
                            var rate = (double)(calcNowT - outBefore24hourT) / calcDeltaT;
                            Act.OutNow[organ.Index].total.KahanSum(calcNowTotal * rate, ref Act.OutNowTotalComp[organ.Index]);
                        }
                    }
                    else
                        Act.OutNow[organ.Index].total.KahanSum(calcNowTotal, ref Act.OutNowTotalComp[organ.Index]);

                    // 摂取時からの積算放射能。
                    Act.OutTotalFromIntake[organ.Index].KahanSum(calcNowTotal, ref Act.OutTotalCompFromIntake[organ.Index]);
                }

                foreach (var organ in data.Organs)
                {
                    // コンパートメントが線源領域に対応しない場合は何もしない。
                    if (organ.SourceRegion is null)
                        continue;

                    // コンパートメントの残留放射能がゼロの場合は何もしない。
                    var activity = Act.CalcNow[organ.Index].end * calcDeltaT;
                    if (activity == 0)
                        continue;

                    // コンパートメントから各標的領域への預託線量を計算する。
                    for (int indexT = 0; indexT < 43; indexT++)
                    {
                        // 標的領域の部分的な重量。
                        var targetWeight = targetWeights[indexT];

                        // S係数(男女平均)。
                        var scoeff = organ.S_Coefficients[indexT];

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

                    if (outDeltaT < delta24hourT)
                    {
                        // 24時間より小さい幅を持つ出力時間メッシュは、それらを合わせた時間幅が
                        // 前回の'exc'コンパートメントに対する数値出力から24-hour経過した時間と一致することをここで確認する。
                        // - 'exc'の残留放射能の出力に寄与しない出力時間メッシュがないことを保証する。
                        // - これを許した場合、excの積算放射能を構成する複数の出力時間メッシュのうち、24-hourを超える古い側の
                        //   メッシュの寄与を後から減算しなければならず、この面倒な処理を避けるという理由もある。
                        // - 本来は計算開始前にTimeMesh入力検証で確認すべきだが、現在はOIRの'exc'に特有の処理であるためここで確認している。
                        if ((outNowT - outExcLastT) > delta24hourT)
                            throw new Exception("sum of out time meshes those are smaller than 24-hour delta should match to 24-hour, "
                                              + "for the 'exc' compartment output compatibility with OIR.");
                    }

                    // excにおける今回の出力時間が、前回の出力時間から24-hour以上経過しているか。
                    var outExc = outExcLastT <= outBefore24hourT;

                    // 出力時間メッシュにおける平均と末期の残留放射能を計算する。
                    foreach (var organ in data.Organs)
                    {
                        if (organ.ExcretaCompatibleWithOIR)
                        {
                            if (outExc)
                            {
                                // excのtotalは24-hourの時間幅で計算するため、total=aveとなる。
                                Act.OutNow[organ.Index].ave = Act.OutNow[organ.Index].total;
                            }
                            else
                            {
                                // excにおける残留放射能の出力処理を特別扱いするためのフラグとしてNaNを使う。
                                Act.OutNow[organ.Index].ave = double.NaN;
                            }
                        }
                        else
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

                    if (outExc)
                        outExcLastT = outPreT;
                    outBefore24hourT = outNowT - delta24hourT;

                    Act.NextOut(data);

                    wholeBodyPre = wholeBodyNow;
                    Array.Copy(resultNow, resultPre, resultNow.Length);
                }
            }

            this.WholeBodyEffectiveDose = wholeBodyNow;

            // 計算完了の出力を行う。
            CalcOut.FinishOut();
        }

        public double WholeBodyEffectiveDose { get; private set; }
    }
}
