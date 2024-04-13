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
            var IterPath = OutputPath + "_IterLog.out";

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
            DataClass dataLow;
            DataClass dataHigh;

            // 移行係数以外は変わらないので、とりあえず3monthのデータを入れる
            dataLow = dataList[0];

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
                            inflow.Rate = data.DecayRate[organ.Nuclide] * nucDecay;
                    }
                }
            }

            // 経過時間=0での計算結果を処理する
            int ctime = 0;  // 計算時間メッシュのインデックス
            int otime = 0;  // 出力時間メッシュのインデックス
            {
                var calcNowT = CalcTimeMesh[ctime];

                // inputの初期値を各臓器に振り分ける
                SubRoutine.Init(Act, dataLow);

                iterLog[calcNowT] = 0;

                var outNowT = calcNowT;

                // 計算結果をテンポラリファイルに出力
                var flgTime = true;
                foreach (var organ in dataLow.Organs)
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
            var OutMeshTotal = new double[dataLow.Organs.Count];

            double WholeBody = 0;  // 積算線量
            double preBody = 0;
            var Result = new double[31];  // 組織毎の計算結果
            var preResult = new double[31];

            void ClearOutMeshTotal()
            {
                foreach (var organ in dataLow.Organs)
                {
                    OutMeshTotal[organ.Index] = 0;
                }
            }
            ClearOutMeshTotal();    // 各臓器の積算放射能として0を設定する

            var flgTarget = true;   // 預託線量ヘッダー出力用フラグ

            ctime = 1;
            otime = 1;

            for (; ctime < CalcTimeMesh.Count; ctime++)
            {
                // 不要な前ステップのデータを削除
                Act.NextTime(dataLow);

                var calcPreT = CalcTimeMesh[ctime - 1];
                var calcNowT = CalcTimeMesh[ctime - 0];

                int LowDays = 0;
                int HighDays = 0;
                // 生まれてからの日数によってLowとHighを変える
                if (calcNowT + ExposureDays <= year1)
                {
                    dataLow = dataList[0];
                    dataHigh = dataList[1];
                    LowDays = month3;
                    HighDays = year1;
                }
                else if (calcNowT + ExposureDays <= year5)
                {
                    dataLow = dataList[1];
                    dataHigh = dataList[2];
                    LowDays = year1;
                    HighDays = year5;
                }
                else if (calcNowT + ExposureDays <= year10)
                {
                    dataLow = dataList[2];
                    dataHigh = dataList[3];
                    LowDays = year5;
                    HighDays = year10;
                }
                else if (calcNowT + ExposureDays <= year15)
                {
                    dataLow = dataList[3];
                    dataHigh = dataList[4];
                    LowDays = year10;
                    HighDays = year15;
                }
                else if (calcNowT + ExposureDays <= adult)
                {
                    dataLow = dataList[4];
                    dataHigh = dataList[5];
                    LowDays = year15;
                    HighDays = adult;
                }
                else
                {
                    dataLow = dataList[5];
                    dataHigh = dataList[5];
                    LowDays = adult;
                    HighDays = adult;
                }

                // 預託期間を超える計算は行わない
                if (calcNowT > commitmentDays)
                    break;

                #region 1つの計算時間メッシュ内で収束計算を繰り返す
                for (int iter = 1; iter <= iterMax; iter++)
                {
                    for (int i = 0; i < dataLow.Organs.Count; i++)
                    {
                        var organLow = dataLow.Organs[i];
                        var organHigh = dataHigh.Organs[i];

                        var func = organLow.Func; // 臓器機能

                        // 臓器機能ごとに異なる処理をする
                        if (func == OrganFunc.inp) // 入力
                        {
                            SubRoutine.Input(organLow, Act);
                        }
                        else if (func == OrganFunc.acc) // 蓄積
                        {
                            SubRoutine.Accumulation_EIR(calcNowT - calcPreT, organLow, organHigh, Act, calcNowT + ExposureDays, LowDays, HighDays);
                        }
                        else if (func == OrganFunc.mix) // 混合
                        {
                            SubRoutine.Mix(organLow, Act);
                        }
                        else if (func == OrganFunc.exc) // 排泄物
                        {
                            SubRoutine.Excretion(organLow, Act, calcNowT - calcPreT);
                        }

                        Act.Now[organLow.Index].ini = Act.rNow[organLow.Index].ini;
                        Act.Now[organLow.Index].ave = Act.rNow[organLow.Index].ave;
                        Act.Now[organLow.Index].end = Act.rNow[organLow.Index].end;

                        Act.Now[organLow.Index].total = Act.rNow[organLow.Index].total;

                        // 臓器毎の積算放射能算出
                        Act.IntakeQuantityNow[organLow.Index] =
                            Act.IntakeQuantityPre[organLow.Index] + Act.Now[organLow.Index].total;
                    }

                    // 前回との差が収束するまで計算を繰り返す
                    if (iter > 1)
                    {
                        var flgIter = true;
                        foreach (var o in dataLow.Organs)
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

                    Act.NextIter(dataLow);
                }
                #endregion

                // 時間メッシュ毎の放射能を足していく
                foreach (var organ in dataLow.Organs)
                {
                    OutMeshTotal[organ.Index] += Act.Now[organ.Index].total;
                    Act.Excreta[organ.Index] += Act.PreExcreta[organ.Index];
                }

                // 出力時間メッシュを超える計算は行わない
                if (OutTimeMesh.Count <= otime)
                    break;

                // S-Coefficient読込
                var SEE = new Dictionary<string, string[]>();
                for (int i = 0; i < dataLow.TargetNuc.Count; i++)
                {
                    var nuclide = dataLow.TargetNuc[i];
                    SEE[nuclide] = File.ReadAllLines("lib\\EIR\\" + nuclide + "SEE.txt");
                }

                // ΔT[sec]
                var deltaT = (calcNowT - calcPreT) * 24 * 3600;

                var TargetList = new List<string>();

                var nucId = ""; // 現在対象としている核種
                var _see = new List<string>();
                string line = "";
                int oCount = 0;
                List<string> SEElow = new List<string>();
                List<string> SEEhigh = new List<string>();
                string[] S_low;
                string[] S_high;
                int S_count = 0;
                string[] source = new string[0];

                while (true)
                {
                    double total = 0;
                    bool flgScoe = true;

                    foreach (var organ in dataLow.Organs)
                    {
                        if (organ.Name.Contains("mix"))
                            continue;

                        if (flgScoe)
                        {
                            nucId = organ.Nuclide;
                            var lines = SEE[nucId].ToArray();
                            source = lines[1].Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);

                            _see = new List<string>();
                            (SEElow, SEEhigh) = SEE_select(SEE[nucId], calcNowT, ExposureDays);
                            S_low = SEElow[S_count].Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);
                            S_high = SEEhigh[S_count].Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);
                            _see.Add(S_low[0]);
                            if (calcNowT + ExposureDays < 6205)
                            {
                                for (int i = 1; i < S_low.Length; i++)
                                {
                                    _see.Add(Interpolation(calcNowT + ExposureDays, double.Parse(S_low[i]), double.Parse(S_high[i]), LowDays, HighDays).ToString());
                                }
                            }
                            else
                            {
                                for (int i = 1; i < S_low.Length; i++)
                                {
                                    _see.Add(S_low[i]);
                                }
                            }

                            if (line == null)
                                break;

                            TargetList.Add(_see[0]);
                            flgScoe = false;
                        }

                        // 対象としてる核種が変わったら見るS係数ファイルを変える
                        if (nucId != organ.Nuclide)
                        {
                            nucId = organ.Nuclide;
                            var lines = SEE[nucId].ToArray();
                            source = lines[1].Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);

                            _see = new List<string>();
                            (SEElow, SEEhigh) = SEE_select(SEE[nucId], calcNowT, ExposureDays);
                            S_low = SEElow[S_count].Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);
                            S_high = SEEhigh[S_count].Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);
                            _see.Add(S_low[0]);
                            if (calcNowT + ExposureDays < 6205)
                            {
                                for (int i = 1; i < S_low.Length; i++)
                                {
                                    _see.Add(Interpolation(calcNowT + ExposureDays, double.Parse(S_low[i]), double.Parse(S_high[i]), LowDays, HighDays).ToString());
                                }
                            }
                            else
                            {
                                for (int i = 1; i < S_low.Length; i++)
                                {
                                    _see.Add(S_low[i]);
                                }
                            }
                        }

                        var nucDecay = dataLow.Ramd[nucId];

                        // タイムステップごとの放射能　
                        var Act = this.Act.Now[organ.Index].end * deltaT * nucDecay;
                        if (Act == 0)
                            continue;

                        // 放射能*S係数
                        int index = Array.IndexOf(source, dataLow.CorrNum[(organ.Nuclide, organ.Name)]);
                        if (index > 0) // indexが1より下は組織と対応するS係数無し
                            total += Act * double.Parse(_see[index]);
                    }

                    if (line == null)
                        break;

                    Result[oCount] += total;
                    WholeBody += total * wT[_see[0]];  // 実効線量 = 男性等価線量*wT
                    oCount++;
                    S_count++;

                    if (S_count >= SEElow.Count)
                        break;
                }

                // 初回のみヘッダーの標的組織出力
                if (flgTarget)
                {
                    CalcOut.CommitmentTarget(TargetList, dataLow);
                    flgTarget = false;
                }

                if (calcNowT == OutTimeMesh[otime])
                {
                    var outPreT = OutTimeMesh[otime - 1];
                    var outNowT = OutTimeMesh[otime - 0];

                    #region 残留放射能をテンポラリファイルに出力
                    var flgTime = true;
                    foreach (var organ in dataLow.Organs)
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

        public static double Interpolation(double day, double LowValue, double HighValue, int LowDays, int HighDays)
        {
            double value = 0;
            value = LowValue + (day - LowDays) * (HighValue - LowValue) / (HighDays - LowDays);
            return value;
        }

        private (List<string>, List<string>) SEE_select(string[] data, double calcNowT, int ExposureDays)
        {
            List<string> dataLow = new List<string>();
            List<string> dataHigh = new List<string>();

            List<string> Data = new List<string>(data);

            if (calcNowT + ExposureDays <= year1)
            {
                dataLow = DataClass.Read_See(Data, "Age:3month");
                dataHigh = DataClass.Read_See(Data, "Age:1year");
            }
            else if (calcNowT + ExposureDays <= year5)
            {
                dataLow = DataClass.Read_See(Data, "Age:1year");
                dataHigh = DataClass.Read_See(Data, "Age:5year");
            }
            else if (calcNowT + ExposureDays <= year10)
            {
                dataLow = DataClass.Read_See(Data, "Age:5year");
                dataHigh = DataClass.Read_See(Data, "Age:10year");
            }
            else if (calcNowT + ExposureDays <= year15)
            {
                dataLow = DataClass.Read_See(Data, "Age:10year");
                dataHigh = DataClass.Read_See(Data, "Age:15year");
            }
            else if (calcNowT + ExposureDays <= adult)
            {
                dataLow = DataClass.Read_See(Data, "Age:15year");
                dataHigh = DataClass.Read_See(Data, "Age:adult");
            }
            else
            {
                dataLow = DataClass.Read_See(Data, "Age:adult");
                dataHigh = DataClass.Read_See(Data, "Age:adult");
            }
            return (dataLow, dataHigh);
        }
    }
}
