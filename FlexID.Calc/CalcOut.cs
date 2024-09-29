using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace FlexID.Calc
{
    class CalcOut : IDisposable
    {
        private readonly InputData data;

        public string DosePath { get; }
        public string DoseRatePath { get; }
        public string RetentionPath { get; }
        public string CumulativePath { get; }

        // 線量の出力ファイル用
        private StreamWriter wDose;

        // 線量率の出力ファイル用
        private StreamWriter wRate;

        // 残留放射能の出力ファイル用
        private StreamWriter[] wsRete;

        // 積算放射能の出力ファイル用
        private StreamWriter[] wsCumu;

        private StreamWriter[] wsOrgansRete;
        private StreamWriter[] wsOrgansCumu;

        /// <summary>
        /// 計算処理が正常に終了した場合に<c>true</c>を設定する。
        /// </summary>
        private bool IsFinished = false;

        /// <summary>
        /// コンストラクタ。
        /// </summary>
        /// <param name="data"></param>
        /// <param name="outputPath"></param>
        public CalcOut(InputData data, string outputPath)
        {
            this.data = data;

            var outputDir = Path.GetDirectoryName(outputPath);
            Directory.CreateDirectory(outputDir);

            DosePath = outputPath + "_Dose.out";
            DoseRatePath = outputPath + "_DoseRate.out";
            RetentionPath = outputPath + "_Retention.out";
            CumulativePath = outputPath + "_Cumulative.out";

            // 預託線量の出力ファイルを用意する。
            wDose = new StreamWriter(DosePath, false, Encoding.UTF8);
            wRate = new StreamWriter(DoseRatePath, false, Encoding.UTF8);

            // 残留放射能の出力ファイルを用意する。
            wsRete = CreateWriters(RetentionPath).ToArray();
            wsCumu = CreateWriters(CumulativePath).ToArray();

            IEnumerable<StreamWriter> CreateWriters(string basePath)
            {
                // 親核種の出力ファイルはそのまま結果ファイルになる。
                yield return new StreamWriter(basePath, false, Encoding.UTF8);

                // 子孫核種の出力ファイルは一時ファイルとし、計算出力時に削除する。
                for (int n = 1; n < data.Nuclides.Count; n++)
                {
                    var path = basePath + $".{n}";

                    // 前回実行時の一時ファイルが残ってしまった場合を想定して、
                    // 同名のファイルが存在する場合はこの隠し属性を外しておく。
                    if (File.Exists(path))
                        File.SetAttributes(path, File.GetAttributes(path) & ~FileAttributes.Hidden);

                    var writer = new StreamWriter(path, false, Encoding.UTF8);

                    // 隠しファイル属性を設定しておく。
                    File.SetAttributes(path, FileAttributes.Hidden);

                    yield return writer;
                }
            }

            // 残留放射能の数値を出力するStreamWriterを
            // コンパートメント毎にorgan.Indexでアクセスできるようにする。
            wsOrgansRete = GetOrganWriters(wsRete).ToArray();
            wsOrgansCumu = GetOrganWriters(wsCumu).ToArray();

            IEnumerable<StreamWriter> GetOrganWriters(StreamWriter[] ws)
            {
                foreach (var organ in data.Organs)
                {
                    var indexN = data.Nuclides.IndexOf(organ.Nuclide);
                    yield return ws[indexN];
                }
            }
        }

        /// <summary>
        /// 集合コンパートメントについての準備を行う。
        /// </summary>
        public void PrepareCompositeCompartments()
        {
            // 消化管を構成する、OIRの線源領域の名称リスト。
            var sregionsAtract = new[]
            {
                "St-cont", "St-wall",
                "SI-cont", "SI-wall",
                "RC-cont", "RC-wall",
                "LC-cont", "LC-wall",
                "RS-cont", "RS-wall",
            };

            // 肺を構成する、OIRにおける線源領域の名称リスト。
            var sregionsLungs = new[]
            {
                // Extrathoracic region
                // "ET1-sur",  "ET2-sur",  "ET2-bnd",  "ET2-seq",

                // Thoracic region (Lung)
                "Bronchi",  "Bronchi-b",  "Bronchi-q",
                "Brchiole", "Brchiole-b", "Brchiole-q",
                "ALV", "LN-Th", "Lung-Tis",
            };

            // 骨格を構成する、OIRにおける線源領域の名称リスト。
            var sregionsSkeleton = new[]
            {
                "C-bone-S", "C-bone-V", "T-bone-S", "T-bone-V",
                "C-marrow", "T-marrow", "R-marrow", "Y-marrow",
            };

            // 肝臓を構成する、OIRにおける線源領域の名称リスト。
            var sregionsLiver = new[] { "Liver" };

            // 甲状腺を構成する、OIRにおける線源領域の名称リスト。
            var sregionsThyroid = new[] { "Thyroid" };

            // 血液(輸送コンパートメント)を構成する、OIRにおける線源領域の名称リスト。
            var sregionsBlood = new[] { "Blood" };

            foreach (var nuclide in data.Nuclides)
            {
                var compartmentsAcc = data.Organs
                    .Where(o => o.Func == OrganFunc.acc && o.Nuclide == nuclide && o.SourceRegion != null);

                nuclide.AtractIndexes = compartmentsAcc
                    .Where(o => sregionsAtract.Contains(o.SourceRegion)).Select(o => o.Index).ToArray();

                nuclide.LungsIndexes = compartmentsAcc
                    .Where(o => sregionsLungs.Contains(o.SourceRegion)).Select(o => o.Index).ToArray();

                nuclide.SkeletonIndexes = compartmentsAcc
                    .Where(o => sregionsSkeleton.Contains(o.SourceRegion)).Select(o => o.Index).ToArray();

                nuclide.LiverIndexes = compartmentsAcc
                    .Where(o => sregionsLiver.Contains(o.SourceRegion)).Select(o => o.Index).ToArray();

                nuclide.ThyroidIndexes = compartmentsAcc
                    .Where(o => sregionsThyroid.Contains(o.SourceRegion)).Select(o => o.Index).ToArray();

                nuclide.BloodIndexes = compartmentsAcc
                    .Where(o => sregionsBlood.Contains(o.SourceRegion)).Select(o => o.Index).ToArray();
            }
        }

        /// <summary>
        /// 残留放射能の出力ファイルにヘッダーを書き出す。
        /// </summary>
        public void ActivityHeader()
        {
            // Retention
            foreach (var nuclide in data.Nuclides)
            {
                var indexN = data.Nuclides.IndexOf(nuclide);
                var wRete = wsRete[indexN];

                if (indexN == 0)
                {
                    wRete.WriteLine("FlexID output: RetentionActivity");
                    wRete.WriteLine(data.Title);
                    wRete.WriteLine();
                    wRete.WriteLine("Radionuclide: " + string.Join(", ", data.Nuclides.Select(n => n.Nuclide)));
                    wRete.WriteLine("Units: day, Bq/Bq");
                    wRete.WriteLine();
                }

                wRete.WriteLine(nuclide.Nuclide);
                wRete.Write("  Time         ");
                wRete.Write("  {0,-14}", "WholeBody");

                if (nuclide.AtractIndexes  /**/.Length != 0) wRete.Write("{0,-16}", "AlimentaryTract*");
                if (nuclide.LungsIndexes   /**/.Length != 0) wRete.Write("  {0,-14}", "Lungs*");
                if (nuclide.SkeletonIndexes/**/.Length != 0) wRete.Write("  {0,-14}", "Skeleton*");
                if (nuclide.LiverIndexes   /**/.Length != 0) wRete.Write("  {0,-14}", "Liver*");
                if (nuclide.ThyroidIndexes /**/.Length != 0) wRete.Write("  {0,-14}", "Thyroid*");

                foreach (var organ in data.Organs.Where(o => o.Nuclide == nuclide))
                {
                    if (organ.Func == OrganFunc.inp)
                        continue;

                    wRete.Write("  {0,-14}", organ.Name);
                }
                wRete.WriteLine();
            }

            // Cumulative
            foreach (var nuclide in data.Nuclides)
            {
                var indexN = data.Nuclides.IndexOf(nuclide);
                var wCumu = wsCumu[indexN];

                if (indexN == 0)
                {
                    wCumu.WriteLine("FlexID output: CumulativeActivity");
                    wCumu.WriteLine(data.Title);
                    wCumu.WriteLine();
                    wCumu.WriteLine("Radionuclide: " + string.Join(", ", data.Nuclides.Select(n => n.Nuclide)));
                    wCumu.WriteLine("Units: day, Bq");
                    wCumu.WriteLine();
                }

                wCumu.WriteLine(nuclide.Nuclide);
                wCumu.Write("  Time         ");
                wCumu.Write("  {0,-14}", "WholeBody");

                if (nuclide.AtractIndexes  /**/.Length != 0) wCumu.Write("{0,-16}", "AlimentaryTract*");
                if (nuclide.LungsIndexes   /**/.Length != 0) wCumu.Write("  {0,-14}", "Lungs*");
                if (nuclide.SkeletonIndexes/**/.Length != 0) wCumu.Write("  {0,-14}", "Skeleton*");
                if (nuclide.LiverIndexes   /**/.Length != 0) wCumu.Write("  {0,-14}", "Liver*");
                if (nuclide.ThyroidIndexes /**/.Length != 0) wCumu.Write("  {0,-14}", "Thyroid*");

                foreach (var organ in data.Organs.Where(o => o.Nuclide == nuclide))
                {
                    if (organ.Func == OrganFunc.inp)
                        continue;

                    wCumu.Write("  {0,-14}", organ.Name);
                }
                wCumu.WriteLine();
            }
        }

        /// <summary>
        /// 預託線量の出力ファイルにヘッダーを書き出す。
        /// </summary>
        public void CommitmentHeader()
        {
            var nuclide = data.Nuclides[0];
            var targets = data.TargetRegions;

            // Dose
            {
                wDose.WriteLine("FlexID output: Dose");
                wDose.WriteLine(data.Title);
                wDose.WriteLine();
                wDose.WriteLine("Radionuclide: " + string.Join(", ", data.Nuclides.Select(n => n.Nuclide)));
                wDose.WriteLine("Units: day, Sv/Bq");
                wDose.WriteLine();

                wDose.WriteLine(nuclide.Nuclide);
                wDose.Write("  Time         ");
                wDose.Write("  WholeBody   ");
                foreach (var t in targets) wDose.Write("  {0,-12:n}", t);
                wDose.WriteLine();
            }

            // DoseRate
            {
                wRate.WriteLine("FlexID output: DoseRate");
                wRate.WriteLine(data.Title);
                wRate.WriteLine();
                wRate.WriteLine("Radionuclide: " + string.Join(", ", data.Nuclides.Select(n => n.Nuclide)));
                wRate.WriteLine("Units: day, Sv/h");
                wRate.WriteLine();

                wRate.WriteLine(nuclide.Nuclide);
                wRate.Write("  Time         ");
                wRate.Write("  WholeBody   ");
                foreach (var t in targets) wRate.Write("  {0,-12:n}", t);
                wRate.WriteLine();
            }
        }

        /// <summary>
        /// 出力時間メッシュにおける残留放射能を出力。
        /// </summary>
        /// <param name="outT"></param>
        /// <param name="Act"></param>
        /// <param name="iter"></param>
        public void ActivityOut(double outT, Activity Act, int iter)
        {
            foreach (var w in wsRete) w.Write("  {0:0.000000E+00} ", outT);
            foreach (var w in wsCumu) w.Write("  {0:0.000000E+00} ", outT);

            // 核種毎にaccコンパートメントの数値を合算したものを
            // 全身の放射能の数値として出力する。
            for (int i = 0; i < data.Nuclides.Count; i++)
            {
                var nuclide = data.Nuclides[i];
                var reteWholeBody = 0.0;
                var cumuWholeBody = 0.0;

                foreach (var organ in data.Organs.Where(o => o.Nuclide == nuclide))
                {
                    if (organ.Func != OrganFunc.acc)
                        continue;

                    var nucDecay = organ.NuclideDecay;

                    var retention = Act.OutNow[organ.Index].end * nucDecay;
                    var cumulative = Act.OutTotalFromIntake[organ.Index] * nucDecay;

                    reteWholeBody += retention;
                    cumuWholeBody += cumulative;
                }

                wsRete[i].Write("  {0:0.00000000E+00}", reteWholeBody);
                wsCumu[i].Write("  {0:0.00000000E+00}", cumuWholeBody);

                var reteBlood = nuclide.BloodIndexes.Select(oi => Act.OutNow[oi].end * data.Organs[oi].NuclideDecay).Sum();
                var cumuBlood = nuclide.BloodIndexes.Select(oi => Act.OutTotalFromIntake[oi] * data.Organs[oi].NuclideDecay).Sum();

                void WriteOutSum(int[] indexes, double bloodFraction)
                {
                    if (indexes.Length == 0)
                        return;
                    var rete = indexes.Select(oi => Act.OutNow[oi].end * data.Organs[oi].NuclideDecay).Sum();
                    var cumu = indexes.Select(oi => Act.OutTotalFromIntake[oi] * data.Organs[oi].NuclideDecay).Sum();

                    wsRete[i].Write("  {0:0.00000000E+00}", rete + reteBlood * bloodFraction);
                    wsCumu[i].Write("  {0:0.00000000E+00}", cumu + cumuBlood * bloodFraction);
                }

                WriteOutSum(nuclide.AtractIndexes, 0.07);
                WriteOutSum(nuclide.LungsIndexes, 0.125);
                WriteOutSum(nuclide.SkeletonIndexes, 0.07);
                WriteOutSum(nuclide.LiverIndexes, 0.1);
                WriteOutSum(nuclide.ThyroidIndexes, 0.0006);
            }

            foreach (var organ in data.Organs)
            {
                if (organ.Func == OrganFunc.inp)
                    continue;

                var nucDecay = organ.NuclideDecay;

                var retention = Act.OutNow[organ.Index].end * nucDecay;
                var cumulative = Act.OutTotalFromIntake[organ.Index] * nucDecay;

                if (organ.ExcretaCompatibleWithOIR)
                {
                    // OIR互換出力を行うexcコンパートメントでは、24-hour sample出力値を模擬して
                    // 出力時間メッシュから24-hour前までの各計算時間メッシュにおけるtotalの総計、
                    // その一日あたりの平均値(これはtotalと等しい)を残留放射能として出力する。
                    retention = Act.OutNow[organ.Index].ave * nucDecay;
                }

                var wrRete = wsOrgansRete[organ.Index];
                var wrCumu = wsOrgansCumu[organ.Index];

                if (double.IsNaN(retention))
                    wrRete.Write("       ----     ");
                else
                    wrRete.Write("  {0:0.00000000E+00}", retention);

                if (double.IsNaN(cumulative))
                    wrCumu.Write("       ----     ");
                else
                    wrCumu.Write("  {0:0.00000000E+00}", cumulative);
            }

            foreach (var w in wsRete) w.WriteLine();
            foreach (var w in wsCumu) w.WriteLine();
        }

        /// <summary>
        /// 出力時間メッシュにおける預託線量を出力。
        /// </summary>
        /// <param name="nowT">今回の出力時刻。</param>
        /// <param name="preT">前回の出力時刻。</param>
        /// <param name="wholeBodyNow">今回出力における全身の実効線量。</param>
        /// <param name="wholeBodyPre">前回出力における全身の実効線量。</param>
        /// <param name="resultNow">今回出力における標的組織毎の等価線量。</param>
        /// <param name="resultPre">前回出力における標的組織毎の等価線量。</param>
        public void CommitmentOut(double nowT, double preT, double wholeBodyNow, double wholeBodyPre, double[] resultNow, double[] resultPre)
        {
            wDose.Write("{0,14:0.000000E+00}  ", nowT);
            wDose.Write("{0,13:0.000000E+00}", wholeBodyNow);
            wRate.Write("{0,14:0.000000E+00}  ", nowT);
            wRate.Write("{0,13:0.000000E+00}", (wholeBodyNow - wholeBodyPre) / ((nowT - preT) * 24));
            for (int i = 0; i < resultNow.Length; i++)
            {
                wDose.Write("  {0,12:0.000000E+00}", resultNow[i]);
                wRate.Write("  {0,12:0.000000E+00}", (resultNow[i] - resultPre[i]) / ((nowT - preT) * 24));
            }
            wDose.WriteLine();
            wRate.WriteLine();
        }

        /// <summary>
        /// 計算完了時のメッセージを出力する。
        /// </summary>
        public void FinishOut()
        {
            // 現在はメッセージ出力は存在せず、終了フラグを設定するだけとなっている。
            IsFinished = true;
        }

        public void Dispose()
        {
            if (!IsFinished)
            {
                // 計算が未完了の場合は、中断メッセージを出力する。
                const string message = "[Abort Calculation]";

                wDose.WriteLine(message);
                wRate.WriteLine(message);
                foreach (var w in wsRete) w.WriteLine(message);
                foreach (var w in wsCumu) w.WriteLine(message);
            }

            wDose.Dispose();
            wRate.Dispose();
            foreach (var w in wsRete) w.Dispose();
            foreach (var w in wsCumu) w.Dispose();

            // 預託線量について、子孫核種の出力ファイルの内容を親核種の出力ファイルに追記していく。
            var nuclideCount = data.Nuclides.Count;
            if (nuclideCount >= 2)
            {
                using (var wRete = new StreamWriter(RetentionPath, append: true))
                {
                    for (int n = 1; n < nuclideCount; n++)
                    {
                        wRete.WriteLine();

                        var progenyRetentionFile = RetentionPath + $".{n}";
                        foreach (var ln in File.ReadLines(progenyRetentionFile))
                            wRete.WriteLine(ln);
                        File.Delete(progenyRetentionFile);
                    }
                }

                using (var wCumu = new StreamWriter(CumulativePath, append: true))
                {
                    for (int n = 1; n < nuclideCount; n++)
                    {
                        wCumu.WriteLine();

                        var progenyCumulativeFile = CumulativePath + $".{n}";
                        foreach (var ln in File.ReadLines(progenyCumulativeFile))
                            wCumu.WriteLine(ln);
                        File.Delete(progenyCumulativeFile);
                    }
                }
            }

            // Iter出力
            //IterOut(CalcTimeMesh, iterLog, IterPath);
        }

        public void IterOut(List<(double time, int iter)> iterLog, string IterPath)
        {
            using (var w = new StreamWriter(IterPath, false, Encoding.UTF8))
            {
                w.WriteLine("   time(day)    Iteration");
                foreach (var (time, iter) in iterLog)
                {
                    w.WriteLine("  {0:0.00000E+00}     {1,3:0}", time, iter);
                }
            }
        }
    }
}
