using System;
using System.Collections.Generic;
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

        private Activity Act { get; } = new Activity();

        private CalcOut CalcOut { get; } = new CalcOut();

        public void Main()
        {
            string TmpFile = Path.GetTempFileName();

            var fileReader = new FileReader();
            var Input = fileReader.InfoReader(InputPath);
            var CalcTimeMesh = fileReader.MeshReader(CalcTimeMeshPath);
            var OutTimeMesh = fileReader.OutReader(OutTimeMeshPath);

            var data = DataClass.Read(Input, CalcProgeny);

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

                MainCalc(CalcTimeMesh, OutTimeMesh, data);
            }

            // テンポラリファイルを並び替えて出力
            CalcOut.ActivityOut(RetentionPath, CumulativePath, TmpFile, data);

            // Iter出力
            //CalcOut.IterOut(CalcTimeMesh, iterLog, IterPath);

            File.Delete(TmpFile);
        }

        private void MainCalc(List<double> CalcTimeMesh, List<double> OutTimeMesh, DataClass data)
        {
            const double convergence = 1E-8; // 収束値
            const int iterMax = 1500;  // iterationの最大回数

            // 預託期間[day]を取得。
            var commitmentDays = SubRoutine.CommitmentPeriodToDays(CommitmentPeriod);

            // 標的領域の組織加重係数を取得。
            var targetWeights = data.TargetWeights;

            // 標的領域の名称をヘッダーとして出力。
            CalcOut.CommitmentTarget(data);

            // 経過時間=0での計算結果を処理する
            int ctime = 0;  // 計算時間メッシュのインデックス
            int otime;      // 出力時間メッシュのインデックス
            {
                var calcNowT = CalcTimeMesh[ctime];

                // inputの初期値を各臓器に振り分ける
                SubRoutine.Init(Act, data);
            }

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
            CalcOut.TemporaryOut(0.0, data, Act, OutMeshTotal, 0);

            ctime = 1;
            otime = 1;
            for (; ctime < CalcTimeMesh.Count; ctime++)
            {
                // 不要な前ステップのデータを削除
                Act.NextTime(data);

                var calcPreT = CalcTimeMesh[ctime - 1];
                var calcNowT = CalcTimeMesh[ctime - 0];

                // 預託期間を超える計算は行わない
                if (calcNowT > commitmentDays)
                    break;

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
                            SubRoutine.Accumulation(calcNowT - calcPreT, organ, Act, calcNowT);
                        }
                        else if (func == OrganFunc.mix) // 混合
                        {
                            SubRoutine.Mix(organ, Act);
                        }
                        else if (func == OrganFunc.exc) // 排泄物
                        {
                            SubRoutine.Excretion(organ, Act, calcNowT - calcPreT);
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

                // 出力時間メッシュを超える計算は行わない
                if (OutTimeMesh.Count <= otime)
                    break;

                // ΔT[sec]
                var deltaT = (calcNowT - calcPreT) * 24 * 3600;

                foreach (var organ in data.Organs)
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

                if (calcNowT == OutTimeMesh[otime])
                {
                    var outPreT = OutTimeMesh[otime - 1];
                    var outNowT = OutTimeMesh[otime - 0];

                    foreach (var organ in data.Organs)
                    {
                        if (organ.Func == OrganFunc.exc)
                            Act.Now[organ.Index].end = Act.Excreta[organ.Index] / (outNowT - outPreT);
                    }

                    // 残留放射能をテンポラリファイルに出力
                    CalcOut.TemporaryOut(outNowT, data, Act, OutMeshTotal, outIter);

                    CalcOut.CommitmentOut(outNowT, outPreT, WholeBody, preBody, Result, preResult);

                    ClearOutMeshTotal();

                    preBody = WholeBody;
                    Array.Copy(Result, preResult, Result.Length);
                    otime++;
                }
            }
        }
    }
}
