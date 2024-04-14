using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using TextIO;

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

        // 反復回数カウント
        private Dictionary<double, int> iterLog = new Dictionary<double, int>();

        public void Main()
        {
            string TmpFile = Path.GetTempFileName();

            var fileReader = new FileReader();
            var Input = fileReader.InfoReader(InputPath);
            var CalcTimeMesh = fileReader.MeshReader(CalcTimeMeshPath);
            var OutTimeMesh = fileReader.OutReader(OutTimeMeshPath);

            var data = DataClass.Read(Input, CalcProgeny);
            var wT = SubRoutine.WeightTissue(@"lib\OIR\wT.txt");

            var RetentionPath = OutputPath + "_Retention.out";
            var CumulativePath = OutputPath + "_Cumulative.out";
            var DosePath = OutputPath + "_Dose.out";
            var DoseRatePath = OutputPath + "_DoseRate.out";
            var IterPath = OutputPath + "_IterLog.out";

            Directory.CreateDirectory(Path.GetDirectoryName(RetentionPath));

            using (var wTmp = new StreamWriter(TmpFile)) // テンポラリファイル
            using (var dCom = new StreamWriter(DosePath, false, Encoding.UTF8)) // 実効線量
            using (var rCom = new StreamWriter(DoseRatePath, false, Encoding.UTF8)) // 線量率
            {
                CalcOut.wTmp = wTmp;
                CalcOut.dCom = dCom;
                CalcOut.rCom = rCom;

                MainCalc(CalcTimeMesh, OutTimeMesh, data, wT);
            }

            // テンポラリファイルを並び替えて出力
            CalcOut.ActivityOut(RetentionPath, CumulativePath, TmpFile, data);
            // Iter出力
            //CalcOut.IterOut(CalcTimeMesh, iterLog, IterPath);
            File.Delete(TmpFile);
        }

        private void MainCalc(List<double> CalcTimeMesh, List<double> OutTimeMesh, DataClass data, Dictionary<string, double> wT)
        {
            const double convergence = 1E-8; // 収束値
            const int iterMax = 1500;  // iterationの最大回数

            // 預託期間[day]を取得。
            var commitmentDays = SubRoutine.CommitmentPeriodToDays(CommitmentPeriod);

            // 流入割合がマイナスの時の処理は親からの分岐比*親の崩壊定数とする
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

            // 経過時間=0での計算結果を処理する
            int ctime = 0;  // 計算時間メッシュのインデックス
            int otime = 0;  // 出力時間メッシュのインデックス
            {
                var calcNowT = CalcTimeMesh[ctime];

                // inputの初期値を各臓器に振り分ける
                SubRoutine.Init(Act, data);

                iterLog[calcNowT] = 0;

                var outNowT = calcNowT;

                // 計算結果をテンポラリファイルに出力
                var flgTime = true;
                foreach (var organ in data.Organs)
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

            // 線源臓器リストの抽出
            var source = new Dictionary<string, string>();
            for (int i = 0; i < data.Nuclides.Count; i++)
            {
                var nuclide = data.Nuclides[i];
                var nuc = nuclide.Nuclide;
                var prg = nuclide.IsProgeny ? "_prg" : "";
                source[nuc + "AM"] = File.ReadLines($@"lib\OIR\{nuc}_AM{prg}_S-Coefficient.txt").First();
                source[nuc + "AF"] = File.ReadLines($@"lib\OIR\{nuc}_AF{prg}_S-Coefficient.txt").First();
            }
            // 処理中の出力メッシュにおける臓器毎の積算放射能
            var OutMeshTotal = new double[data.Organs.Count];

            double WholeBody = 0;  // 積算線量
            double preBody = 0;
            var Result = new double[43];  // 組織毎の計算結果
            var preResult = new double[43];

            void ClearOutMeshTotal()
            {
                foreach (var organ in data.Organs)
                {
                    OutMeshTotal[organ.Index] = 0;
                    Act.Excreta[organ.Index] = 0;
                }
            }
            ClearOutMeshTotal();    // 各臓器の積算放射能として0を設定する

            var flgTarget = true;   // 預託線量ヘッダー出力用フラグ

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
                            iterLog[calcNowT] = iter;
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

                // S-Coefficient読込
                var S_coe = new Dictionary<string, StreamLineReader>();
                for (int i = 0; i < data.Nuclides.Count; i++)
                {
                    var n = data.Nuclides[i];
                    var nuc = n.Nuclide;
                    if (n.IsProgeny)
                    {
                        S_coe[nuc + "AM"] = new StreamLineReader(@"lib\OIR\" + nuc + "_AM_prg_S-Coefficient.txt");
                        S_coe[nuc + "AF"] = new StreamLineReader(@"lib\OIR\" + nuc + "_AF_prg_S-Coefficient.txt");
                    }
                    else
                    {
                        S_coe[nuc + "AM"] = new StreamLineReader(@"lib\OIR\" + nuc + "_AM_S-Coefficient.txt");
                        S_coe[nuc + "AF"] = new StreamLineReader(@"lib\OIR\" + nuc + "_AF_S-Coefficient.txt");
                    }
                }

                // ΔT[sec]
                var deltaT = (calcNowT - calcPreT) * 24 * 3600;
                string[] sourceAM = new string[0];
                string[] sourceAF = new string[0];
                string lineAM = "";
                string lineAF = "";
                // ヘッダーを送る
                for (int i = 0; i < data.Nuclides.Count; i++)
                {
                    var nuc = data.Nuclides[i].Nuclide;

                    var r = S_coe[nuc + "AM"];
                    lineAM = r.ReadLine();
                    sourceAM = lineAM.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);
                    r = S_coe[nuc + "AF"];
                    lineAF = r.ReadLine();
                    sourceAF = lineAF.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);
                }

                var TargetList = new List<string>();

                var nuclide = data.Nuclides[0]; // 現在対象としている核種
                var nucId = nuclide.Nuclide;
                var S_coeAM = new string[0];
                var S_coeAF = new string[0];
                int oCount = 0;

                while (true)
                {
                    double totalAM = 0;
                    double totalAF = 0;
                    bool flgScoe = true;

                    foreach (var organ in data.Organs)
                    {
                        StreamLineReader r;

                        if (organ.Name.Contains("mix"))
                            continue;

                        if (flgScoe)
                        {
                            nuclide = organ.Nuclide;
                            nucId = nuclide.Nuclide;
                            var am = source[nucId + "AM"];
                            sourceAM = am.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);
                            var af = source[nucId + "AF"];
                            sourceAF = af.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);

                            r = S_coe[nucId + "AM"];
                            lineAM = r.ReadLine();
                            r = S_coe[nucId + "AF"];
                            lineAF = r.ReadLine();
                            if (lineAM == null)
                                break;

                            S_coeAM = lineAM.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);
                            S_coeAF = lineAF.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);
                            TargetList.Add(S_coeAM[0]);
                            flgScoe = false;
                        }

                        // 対象としてる核種が変わったら見るS係数ファイルを変える
                        if (nuclide != organ.Nuclide)
                        {
                            nuclide = organ.Nuclide;
                            nucId = nuclide.Nuclide;
                            var am = source[nucId + "AM"];
                            sourceAM = am.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);
                            var af = source[nucId + "AF"];
                            sourceAF = af.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);

                            r = S_coe[nucId + "AM"];
                            lineAM = r.ReadLine();
                            r = S_coe[nucId + "AF"];
                            lineAF = r.ReadLine();

                            S_coeAM = lineAM.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);
                            S_coeAF = lineAF.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);
                        }

                        var nucDecay = nuclide.Ramd;

                        // タイムステップごとの放射能　
                        var Act = this.Act.Now[organ.Index].end * deltaT * nucDecay;
                        if (Act == 0)
                            continue;

                        // 放射能*S係数
                        int indexAM = Array.IndexOf(sourceAM, data.CorrNum[(organ.Nuclide.Nuclide, organ.Name)]);
                        int indexAF = Array.IndexOf(sourceAF, data.CorrNum[(organ.Nuclide.Nuclide, organ.Name)]);
                        if (indexAM > 0) // indexが1より下は組織と対応するS係数無し
                            totalAM += Act * double.Parse(S_coeAM[indexAM]);
                        if (indexAF > 0)
                            totalAF += Act * double.Parse(S_coeAF[indexAF]);
                    }

                    if (lineAF == null)
                        break;

                    Result[oCount] += (totalAM + totalAF) / 2;
                    WholeBody += ((totalAM * wT[S_coeAM[0]]) + (totalAF * wT[S_coeAM[0]])) / 2; // 実効線量 =（男性等価線量*wT+女性等価線量*wT）/2
                    oCount++;
                }

                // 初回のみヘッダーの標的組織出力
                if (flgTarget)
                {
                    CalcOut.CommitmentTarget(TargetList, data);
                    flgTarget = false;
                }

                if (calcNowT == OutTimeMesh[otime])
                {
                    var outPreT = OutTimeMesh[otime - 1];
                    var outNowT = OutTimeMesh[otime - 0];

                    #region 残留放射能をテンポラリファイルに出力
                    var flgTime = true;
                    foreach (var organ in data.Organs)
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
