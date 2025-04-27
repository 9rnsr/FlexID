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
        private TextWriter wDose;

        // 線量率の出力ファイル用
        private TextWriter wRate;

        // 残留放射能の出力ファイル用
        private TextWriter[] wsRete;

        // 積算放射能の出力ファイル用
        private TextWriter[] wsCumu;

        private TextWriter[] wsOrgansRete;
        private TextWriter[] wsOrgansCumu;

        /// <summary>
        /// 計算処理が正常に終了した場合に<c>true</c>を設定する。
        /// </summary>
        private bool IsFinished = false;

        // StreamWriter.Nullはスレッドセーフでなくテストで問題を発生し得るため、
        // ここでスレッドセーフなラッパーを作り、これを計算処理で使用する。
        private static TextWriter NullWriter { get; } = TextWriter.Synchronized(StreamWriter.Null);

        private static IEnumerable<TextWriter> NullWriters(int n) => Enumerable.Repeat(NullWriter, n);

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
            wDose = data.OutputDose ? new StreamWriter(DosePath, false, Encoding.UTF8) : NullWriter;
            wRate = data.OutputDoseRate ? new StreamWriter(DoseRatePath, false, Encoding.UTF8) : NullWriter;

            // 残留放射能の出力ファイルを用意する。
            wsRete = (data.OutputRetention ? CreateWriters(RetentionPath) : NullWriters(data.Nuclides.Count)).ToArray();
            wsCumu = (data.OutputCumulative ? CreateWriters(CumulativePath) : NullWriters(data.Nuclides.Count)).ToArray();

            IEnumerable<TextWriter> CreateWriters(string basePath)
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

            // 残留放射能の数値を出力するTextWriterを
            // コンパートメント毎にorgan.Indexでアクセスできるようにする。
            wsOrgansRete = GetOrganWriters(wsRete).ToArray();
            wsOrgansCumu = GetOrganWriters(wsCumu).ToArray();

            IEnumerable<TextWriter> GetOrganWriters(TextWriter[] ws)
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

            foreach (var nuclide in data.Nuclides)
            {
                var compartmentsAcc = data.Organs
                    .Where(o => o.Func == OrganFunc.acc && o.Nuclide == nuclide && o.SourceRegion != null).ToArray();

                var otherIndexes = compartmentsAcc
                    .Where(o => o.SourceRegion == "Other").Select(o => o.Index).ToArray();

                var bloodIndexes = compartmentsAcc
                    .Where(o => o.SourceRegion == "Blood").Select(o => o.Index).ToArray();

                var otherSourceRegions = nuclide.OtherSourceRegions.Select(sr => data.SourceRegions.First(s => s.Name == sr)).ToArray();
                var massOtherAM = otherSourceRegions.Select(s => s.MaleMass).Sum();
                var massOtherAF = otherSourceRegions.Select(s => s.FemaleMass).Sum();
                var massOther = (massOtherAM + massOtherAF) / 2;    // 男女平均

                (int, double)[] GetIndexes(string[] sregions, double bloodFraction, bool considerOther = true)
                {
                    // コンパートメントとして明示されている集合コンパートメントの構成要素を得る。
                    var explicits = compartmentsAcc.Where(o => sregions.Contains(o.SourceRegion)).ToArray();
                    if (!explicits.Any())   // 構成要素が1つも明示されていない＝集合コンパートメントなしとする。
                        return Array.Empty<(int, double)>();

                    // 線源領域Otherに含まれる集合コンパートメント構成要素の、Other全体に対する質量比を得る。
                    var otherFraction = sregions
                        .Where(sr => nuclide.OtherSourceRegions.Contains(sr) && !explicits.Any(s => s.SourceRegion == sr))
                        .Select(sr =>
                        {
                            var sourceRegion = data.SourceRegions.First(s => s.Name == sr);
                            var massRateAM = sourceRegion.MaleMass / massOtherAM;
                            var massRateAF = sourceRegion.FemaleMass / massOtherAF;
                            var massRate = (massRateAM + massRateAF) / 2;
                            return massRate;
                        }).Sum();

                    // 明示されたコンパートメントの残留放射能を1.0で、
                    // 血液コンパートメントの残留放射能をbloodFractionで、
                    // 線源領域Otherのコンパートメントの残留放射能をotherFractionで、それぞれ加算する。
                    var indexes = explicits.Select(o => (o.Index, 1.0))
                        .Concat(bloodIndexes.Select(i => (i, bloodFraction)));
                    if (considerOther)
                        indexes = indexes.Concat(otherIndexes.Select(i => (i, otherFraction)));
                    return indexes.ToArray();
                }

                nuclide.AtractIndexes   /**/= GetIndexes(sregionsAtract,   /**/ 0.07);
                nuclide.LungsIndexes    /**/= GetIndexes(sregionsLungs,    /**/ 0.125);
                nuclide.SkeletonIndexes /**/= GetIndexes(sregionsSkeleton, /**/ 0.07, considerOther: false);
                nuclide.LiverIndexes    /**/= GetIndexes(sregionsLiver,    /**/ 0.1);
                nuclide.ThyroidIndexes  /**/= GetIndexes(sregionsThyroid,  /**/ 0.0006);
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
                    wRete.WriteLine("Radionuclide: " + string.Join(", ", data.Nuclides.Select(n => n.Name)));
                    wRete.WriteLine("Units: day, Bq/Bq");
                    wRete.WriteLine();
                }

                wRete.WriteLine(nuclide.Name);
                wRete.Write("  Time         ");
                wRete.Write("  {0,-14}", "WholeBody");

                if (nuclide.AtractIndexes  /**/.Length != 0) wRete.Write("{0,-16}", "AlimentaryTract*");
                if (nuclide.LungsIndexes   /**/.Length != 0) wRete.Write("  {0,-14}", "Lungs*");
                if (nuclide.SkeletonIndexes/**/.Length != 0) wRete.Write("  {0,-14}", "Skeleton*");
                if (nuclide.LiverIndexes   /**/.Length != 0) wRete.Write("  {0,-14}", "Liver*");
                if (nuclide.ThyroidIndexes /**/.Length != 0) wRete.Write("  {0,-14}", "Thyroid*");

                foreach (var organ in data.Organs.Where(o => o.Nuclide == nuclide))
                {
                    if (organ.IsDecayCompartment)
                        continue;
                    if (organ.IsZeroInflow)
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
                    wCumu.WriteLine("Radionuclide: " + string.Join(", ", data.Nuclides.Select(n => n.Name)));
                    wCumu.WriteLine("Units: day, Bq");
                    wCumu.WriteLine();
                }

                wCumu.WriteLine(nuclide.Name);
                wCumu.Write("  Time         ");
                wCumu.Write("  {0,-14}", "WholeBody");

                if (nuclide.AtractIndexes  /**/.Length != 0) wCumu.Write("{0,-16}", "AlimentaryTract*");
                if (nuclide.LungsIndexes   /**/.Length != 0) wCumu.Write("  {0,-14}", "Lungs*");
                if (nuclide.SkeletonIndexes/**/.Length != 0) wCumu.Write("  {0,-14}", "Skeleton*");
                if (nuclide.LiverIndexes   /**/.Length != 0) wCumu.Write("  {0,-14}", "Liver*");
                if (nuclide.ThyroidIndexes /**/.Length != 0) wCumu.Write("  {0,-14}", "Thyroid*");

                foreach (var organ in data.Organs.Where(o => o.Nuclide == nuclide))
                {
                    if (organ.IsDecayCompartment)
                        continue;
                    if (organ.IsZeroInflow)
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
                wDose.WriteLine("Radionuclide: " + string.Join(", ", data.Nuclides.Select(n => n.Name)));
                wDose.WriteLine("Units: day, Sv/Bq");
                wDose.WriteLine();

                wDose.WriteLine(nuclide.Name);
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
                wRate.WriteLine("Radionuclide: " + string.Join(", ", data.Nuclides.Select(n => n.Name)));
                wRate.WriteLine("Units: day, Sv/h");
                wRate.WriteLine();

                wRate.WriteLine(nuclide.Name);
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

                    var retention = Act.OutNow[organ.Index].end;
                    var cumulative = Act.OutTotalFromIntake[organ.Index];

                    reteWholeBody += retention;
                    cumuWholeBody += cumulative;
                }

                wsRete[i].Write("  {0:0.00000000E+00}", reteWholeBody);
                wsCumu[i].Write("  {0:0.00000000E+00}", cumuWholeBody);

                void WriteOutSum((int index, double Rate)[] indexes)
                {
                    if (indexes.Length == 0)
                        return;
                    var rete = indexes.Select(x => Act.OutNow[x.index].end * x.Rate).Sum();
                    var cumu = indexes.Select(x => Act.OutTotalFromIntake[x.index] * x.Rate).Sum();

                    wsRete[i].Write("  {0:0.00000000E+00}", rete);
                    wsCumu[i].Write("  {0:0.00000000E+00}", cumu);
                }

                WriteOutSum(nuclide.AtractIndexes);
                WriteOutSum(nuclide.LungsIndexes);
                WriteOutSum(nuclide.SkeletonIndexes);
                WriteOutSum(nuclide.LiverIndexes);
                WriteOutSum(nuclide.ThyroidIndexes);
            }

            foreach (var organ in data.Organs)
            {
                if (organ.IsDecayCompartment)
                    continue;
                if (organ.IsZeroInflow)
                    continue;

                var retention = Act.OutNow[organ.Index].end;
                var cumulative = Act.OutTotalFromIntake[organ.Index];

                if (organ.ExcretaCompatibleWithOIR)
                {
                    // OIR互換出力を行うexcコンパートメントでは、24-hour sample出力値を模擬して
                    // 出力時間メッシュから24-hour前までの各計算時間メッシュにおけるtotalの総計、
                    // その一日あたりの平均値(これはtotalと等しい)を残留放射能として出力する。
                    retention = Act.OutNow[organ.Index].ave;
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

            var nuclideCount = data.Nuclides.Count;
            if (nuclideCount >= 2 && data.OutputRetention)
            {
                // 残留放射能について、子孫核種の出力を親核種の出力ファイルに追記していく。
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
            }
            if (nuclideCount >= 2 && data.OutputCumulative)
            {
                // 積算放射能について、子孫核種の出力を親核種の出力ファイルに追記していく。
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
