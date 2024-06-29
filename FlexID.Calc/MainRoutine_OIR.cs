using System;

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

        private Activity Act { get; } = new Activity();

        private CalcOut CalcOut { get; set; }

        public void Main()
        {
            var calcTimeMesh = new TimeMesh(CalcTimeMeshPath);
            var outTimeMesh = new TimeMesh(OutTimeMeshPath);
            if (!calcTimeMesh.Cover(outTimeMesh))
                throw Program.Error("Calculation time mesh does not cover all boundaries of output time mesh.");

            var data = DataClass.Read(InputPath, CalcProgeny);

            using (CalcOut = new CalcOut(data, OutputPath))
            {
                MainCalc(calcTimeMesh, outTimeMesh, data);
            }
        }

        private void MainCalc(TimeMesh calcTimeMesh, TimeMesh outTimeMesh, DataClass data)
        {
            const double convergence = 1E-8; // 収束値
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

            // 計算時間メッシュを進める。
            while (calcTimes.MoveNext())
            {
                calcPreT = calcNowT;
                calcNowT = calcTimes.Current;

                // 預託期間を超える計算は行わない
                if (commitmentPeriod < calcNowT)
                    break;

                Act.NextCalc(data);

                var calcNowDay = TimeMesh.SecondsToDays(calcNowT);

                // ΔT[sec]
                var calcDeltaT = calcNowT - calcPreT;
                var calcDeltaDay = TimeMesh.SecondsToDays(calcDeltaT);

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
                            SubRoutine.Accumulation_OIR(calcDeltaDay, organ, Act);
                        }
                        else if (func == OrganFunc.mix) // 混合
                        {
                            SubRoutine.Mix(organ, Act);
                        }
                        else if (func == OrganFunc.exc) // 排泄物
                        {
                            SubRoutine.Excretion(organ, Act, calcDeltaDay);
                        }

                        Act.CalcNow[organ.Index].ini = Act.IterNow[organ.Index].ini;
                        Act.CalcNow[organ.Index].ave = Act.IterNow[organ.Index].ave;
                        Act.CalcNow[organ.Index].end = Act.IterNow[organ.Index].end;

                        Act.CalcNow[organ.Index].total = Act.IterNow[organ.Index].total;

                        // 臓器毎の積算放射能算出
                        Act.IntakeQuantityNow[organ.Index] =
                            Act.IntakeQuantityPre[organ.Index] + Act.CalcNow[organ.Index].total;
                    }

                    // 前回との差が収束するまで計算を繰り返す
                    if (calcIter > 1)
                    {
                        var converged = true;
                        foreach (var o in data.Organs)
                        {
                            double s1 = 0;
                            double s2 = 0;
                            double s3 = 0;

                            if (Act.IterNow[o.Index].ini != 0)
                                s1 = Math.Abs((Act.IterNow[o.Index].ini - Act.IterPre[o.Index].ini) / Act.IterNow[o.Index].ini);
                            if (Act.IterNow[o.Index].ave != 0)
                                s2 = Math.Abs((Act.IterNow[o.Index].ave - Act.IterPre[o.Index].ave) / Act.IterNow[o.Index].ave);
                            if (Act.IterNow[o.Index].end != 0)
                                s3 = Math.Abs((Act.IterNow[o.Index].end - Act.IterPre[o.Index].end) / Act.IterNow[o.Index].end);

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

                    Act.NextIter(data);
                }
                #endregion

                // 時間メッシュ毎の放射能を足していく
                foreach (var organ in data.Organs)
                {
                    Act.OutTotalNow[organ.Index] += Act.CalcNow[organ.Index].total;
                    Act.Excreta[organ.Index] += Act.PreExcreta[organ.Index];
                }

                foreach (var organ in data.Organs)
                {
                    if (organ.Name.Contains("mix"))
                        continue;

                    var nucDecay = organ.NuclideDecay;

                    // タイムステップごとの放射能　
                    var activity = Act.CalcNow[organ.Index].end * calcDeltaT * nucDecay;
                    if (activity == 0)
                        continue;

                    if (organ.SourceRegion != null)
                    {
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
                }

                if (calcNowT == outNowT)
                {
                    var outDeltaT = outNowT - outPreT;

                    var outNowDay = TimeMesh.SecondsToDays(outNowT);
                    var outPreDay = TimeMesh.SecondsToDays(outPreT);
                    var outDeltaDay = TimeMesh.SecondsToDays(outDeltaT);
                    foreach (var organ in data.Organs)
                    {
                        if (organ.Func == OrganFunc.exc)
                            Act.CalcNow[organ.Index].end = Act.Excreta[organ.Index] / outDeltaDay;
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

                    Act.NextOut(data);

                    wholeBodyPre = wholeBodyNow;
                    Array.Copy(resultNow, resultPre, resultNow.Length);
                }
            }

            // 計算完了の出力を行う。
            CalcOut.FinishOut();
        }
    }
}
