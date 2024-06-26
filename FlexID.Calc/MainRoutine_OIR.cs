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

            // 計算時間メッシュ
            var calcTimes = calcTimeMesh.Start();
            long calcPreT;
            long calcNowT = calcTimes.Current;

            // inputの初期値を各臓器に振り分ける
            SubRoutine.Init(Act, data);

            // 出力時間メッシュ
            var outTimes = outTimeMesh.Start();
            long outPreT;
            long outNowT = outTimes.Current;

            // 処理中の出力メッシュにおける臓器毎の積算放射能
            var OutMeshTotal = new double[data.Organs.Count];

            double WholeBody = 0;  // 積算線量
            double preBody = 0;
            var Result = new double[43];  // 組織毎の計算結果
            var preResult = new double[43];

            int outIter;

            void ClearOutMeshTotal()
            {
                foreach (var organ in data.Organs)
                {
                    OutMeshTotal[organ.Index] = 0;
                    Act.Excreta[organ.Index] = 0;
                }

                outIter = 0;
            }
            ClearOutMeshTotal();    // 各臓器の積算放射能として0を設定する

            // 初期配分された残留放射能をテンポラリファイルに出力
            CalcOut.ActivityOut(0.0, Act, OutMeshTotal, 0);

            outTimes.MoveNext();
            outPreT = outNowT;
            outNowT = outTimes.Current;

            while (calcTimes.MoveNext())
            {
                // 不要な前ステップのデータを削除
                Act.NextTime(data);

                calcPreT = calcNowT;
                calcNowT = calcTimes.Current;

                // 預託期間を超える計算は行わない
                if (commitmentPeriod < calcNowT)
                    break;

                var calcNowDay = TimeMesh.SecondsToDays(calcNowT);

                // ΔT[sec]
                var calcDeltaT = calcNowT - calcPreT;
                var calcDeltaDay = TimeMesh.SecondsToDays(calcDeltaT);

                #region 1つの計算時間メッシュ内で収束計算を繰り返す
                for (int iter = 1; iter <= iterMax; iter++)
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
                            SubRoutine.Accumulation(calcDeltaDay, organ, Act, calcNowDay);
                        }
                        else if (func == OrganFunc.mix) // 混合
                        {
                            SubRoutine.Mix(organ, Act);
                        }
                        else if (func == OrganFunc.exc) // 排泄物
                        {
                            SubRoutine.Excretion(organ, Act, calcDeltaDay);
                        }

                        Act.Now[organ.Index].ini = Act.rNow[organ.Index].ini;
                        Act.Now[organ.Index].ave = Act.rNow[organ.Index].ave;
                        Act.Now[organ.Index].end = Act.rNow[organ.Index].end;

                        Act.Now[organ.Index].total = Act.rNow[organ.Index].total;

                        // 臓器毎の積算放射能算出
                        Act.IntakeQuantityNow[organ.Index] =
                            Act.IntakeQuantityPre[organ.Index] + Act.Now[organ.Index].total;
                    }

                    // 前回との差が収束するまで計算を繰り返す
                    if (iter > 1)
                    {
                        var flgIter = true;
                        foreach (var o in data.Organs)
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

                            if (s1 > convergence || s2 > convergence || s3 > convergence)
                            {
                                flgIter = false;
                                break;
                            }
                        }
                        // 前回との差が全ての臓器で収束した場合
                        if (flgIter)
                        {
                            // 出力メッシュと終端が一致する計算メッシュにおける反復回数を保存する。
                            outIter = iter;

                            // // 出力メッシュ内での総反復回数を保存する。
                            // // outIter += iter;
                            break;
                        }
                    }

                    Act.NextIter(data);
                }
                #endregion

                // 時間メッシュ毎の放射能を足していく
                foreach (var organ in data.Organs)
                {
                    OutMeshTotal[organ.Index] += Act.Now[organ.Index].total;
                    Act.Excreta[organ.Index] += Act.PreExcreta[organ.Index];
                }

                foreach (var organ in data.Organs)
                {
                    if (organ.Name.Contains("mix"))
                        continue;

                    var nuclide = organ.Nuclide;
                    var nucDecay = nuclide.Ramd;

                    // タイムステップごとの放射能　
                    var activity = Act.Now[organ.Index].end * calcDeltaT * nucDecay;
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

                            Result[indexT] += equivalentDose;
                            WholeBody += effectiveDose;
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
                            Act.Now[organ.Index].end = Act.Excreta[organ.Index] / outDeltaDay;
                    }

                    // 残留放射能をテンポラリファイルに出力
                    CalcOut.ActivityOut(outNowDay, Act, OutMeshTotal, outIter);

                    CalcOut.CommitmentOut(outNowDay, outPreDay, WholeBody, preBody, Result, preResult);

                    ClearOutMeshTotal();

                    // これ以上出力時間メッシュが存在しないならば、計算を終了する。
                    if (!outTimes.MoveNext())
                        break;
                    outPreT = outNowT;
                    outNowT = outTimes.Current;

                    preBody = WholeBody;
                    Array.Copy(Result, preResult, Result.Length);
                }
            }
        }
    }
}
