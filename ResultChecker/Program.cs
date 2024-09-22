using FlexID.Calc;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ResultChecker
{
    internal partial class Program
    {
        static int Main(string[] args)
        {
            Regex pattern = null;
            if (args.Length == 1)
            {
                try
                {
                    pattern = new Regex(args[0], RegexOptions.IgnoreCase);
                }
                catch
                {
                    Console.WriteLine("target pattern is not correct.");
                    return -1;
                }
            }

            var targets = GetTargets().Where(t => pattern?.IsMatch(t.Target) ?? true).ToArray();
            var results = new ConcurrentBag<Result>();

            // 並列に計算を実施する。
            Parallel.ForEach(targets, x =>
            {
                var (target, material) = x;

                try
                {
                    var result = CalcAndSummary(target, material);
                    results.Add(result);
                    Console.WriteLine($"done: {target}");
                }
                catch
                {
                    // 何らかのエラーが発生した場合。
                    results.Add(new Result { Target = target, HasErrors = true });
                    Console.WriteLine($"error: {target}");
                }
            });

            var sortedResults = results.OrderBy(r => r.Target).ToArray();

            //WriteSummaryCsv(sortedResults);

            WriteSummaryExcel("summary.xlsx", sortedResults);

            return 0;
        }

        // 計算および比較の結果。
        struct Result
        {
            public string Target;
            public string Material;

            public bool HasErrors;

            public string ExpectEffectiveDose;
            public string ActualEffectiveDose;
            public double FractionEffectiveDose;

            public (double Min, double Max) FractionsWholeBody;
            public (double Min, double Max) FractionsUrine;
            public (double Min, double Max) FractionsFaeces;
        }

        static Result CalcAndSummary(string target, string material)
        {
            var nuclide = target.Split('_')[0];
            var inputPath = Path.Combine("inp", "OIR", nuclide, target + ".inp");

            var outputDir = "out";
            Directory.CreateDirectory(outputDir);

            var outputPath = Path.Combine(outputDir, target);

            // 計算時間メッシュはFlexID.Calcに付属のものを使用する。
            var cTimeMeshFile = @"lib\TimeMesh\time.dat";

            // 出力時間メッシュは、OIRの残留放射能データと同じになるように設定する。
            var oTimeMeshFile = Path.Combine("out-OIR.dat");

            var commitmentPeriod = "50years";

            var main = new MainRoutine_OIR();
            main.InputPath        /**/= inputPath;
            main.OutputPath       /**/= outputPath;
            main.CalcTimeMeshPath /**/= cTimeMeshFile;
            main.OutTimeMeshPath  /**/= oTimeMeshFile;
            main.CommitmentPeriod /**/= commitmentPeriod;
            main.CalcProgeny      /**/= true;

            main.Main();

            var result = new Result() { Target = target, Material = material };

            // 50年預託実行線量の比較。
            var actualDose = main.WholeBodyEffectiveDose;
            var expectDose = ReadDosePerIntake(target, material);
            result.ActualEffectiveDose = $"{actualDose:0.000000E+00}";
            result.ExpectEffectiveDose = expectDose;
            result.FractionEffectiveDose = actualDose / double.Parse(expectDose);

            // 50年の預託期間における、各出力時間メッシュにおける数値の比較。
            // 要約として、期待値に対する下振れ率と上振れ率の最大値を算出する。
            var actualActs = GetResultRetentions(target);
            var expectActs = GetExpectRetentions(target, material);
            var fractionsWholeBody /**/= (min: double.PositiveInfinity, max: double.NegativeInfinity);
            var fractionsUrine     /**/= (min: double.PositiveInfinity, max: double.NegativeInfinity);
            var fractionsFaeces    /**/= (min: double.PositiveInfinity, max: double.NegativeInfinity);

            foreach (var (actualAct, expectAct) in actualActs.Zip(expectActs, (a, e) => (a, e)))
            {
                //Console.WriteLine(
                //    $"{expectAct.EndTime,8}," +
                //    $"{expectAct.WholeBody:0.0E+00}," +
                //    $"{(expectAct.Urine != null ? $"{expectAct.Urine:0.0E+00}" : "-"),-7}," +
                //    $"{(expectAct.Faeces != null ? $"{expectAct.Faeces:0.0E+00}" : "-"),-7}," +
                //    $"{actualAct.WholeBody:0.00000000E+00}," +
                //    $"{(actualAct.Urine != null ? $"{actualAct.Urine:0.00000000E+00}" : "-"),-14}," +
                //    $"{(actualAct.Faeces != null ? $"{actualAct.Faeces:0.00000000E+00}" : "-"),-14}");

                var fracWholeBody = actualAct.WholeBody / expectAct.WholeBody;
                fractionsWholeBody = (Math.Min(fractionsWholeBody.min, fracWholeBody),
                                      Math.Max(fractionsWholeBody.max, fracWholeBody));

                if (expectAct.Urine is double expectUrine &&
                    actualAct.Urine is double actualUrine)
                {
                    var fracUrine = actualUrine / expectUrine;
                    fractionsUrine = (Math.Min(fractionsUrine.min, fracUrine),
                                      Math.Max(fractionsUrine.max, fracUrine));
                }
                if (expectAct.Faeces is double expectFaeces &&
                    actualAct.Faeces is double actualFaeces)
                {
                    var fracFaeces = actualFaeces / expectFaeces;
                    fractionsFaeces = (Math.Min(fractionsFaeces.min, fracFaeces),
                                       Math.Max(fractionsFaeces.max, fracFaeces));
                }
            }

            result.FractionsWholeBody /**/= fractionsWholeBody;
            result.FractionsUrine     /**/= fractionsUrine;
            result.FractionsFaeces    /**/= fractionsFaeces;

            return result;
        }

        /// <summary>
        /// OIR Data Viewerの「Dose per Intake」＞「Material」から取得した、
        /// ある核種の50年預託実効線量 e(50) [Sv/Bq]データを保存したファイルから、
        /// 指定のmaterialに対応する数値を取得する。
        /// </summary>
        /// <param name="target"></param>
        /// <param name="material"></param>
        /// <returns></returns>
        /// <exception cref="InvalidDataException"></exception>
        static string ReadDosePerIntake(string target, string material)
        {
            var nuclide = target.Split('_')[0];
            var filePath = $"Expect/{nuclide}.dat";
            var (routeOfIntake, _, _) = DecomposeMaterial(material);

            using (var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read))
            using (var reader = new StreamReader(stream, Encoding.UTF8))
            {
                string[] columns;

                // ファイルの内容が対象核種のものか確認する。
                columns = reader.ReadLine().Split('\t');
                if (columns[1] != nuclide)
                    throw new InvalidDataException();

                // 数値の単位が[Sv/Bq]であるかを確認する。
                columns = reader.ReadLine().Split('\t');
                if (columns[1] != "Sv per Bq")
                    throw new InvalidDataException();

                reader.ReadLine();  // (empty line)
                reader.ReadLine();  // (table header)

                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    columns = line.Split('\t');

                    if (columns[0] == material ||
                        columns[0] == routeOfIntake)    // Injectionの場合
                    {
                        var dose = columns[1];
                        return dose;
                    }

                    reader.ReadLine();
                }
            }

            return null;
        }

        // 残留放射能の取得結果。
        struct Retention
        {
            public double StartTime;
            public double EndTime;

            public double WholeBody;
            public double? Urine;
            public double? Faeces;
        }

        /// <summary>
        /// FlexIDが出力した各出力時間メッシュにおける残留放射能の出力ファイル
        /// *_Retention.outから、Whole Bodyの数値列を読み込む。
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        static List<Retention> GetResultRetentions(string target)
        {
            var nuclide = target.Split('_')[0];
            var filePath = $"out/{target}_Retention.out";

            var retentions = new List<Retention>();

            using (var reader = new OutputDataReader(filePath))
            {
                var data = reader.Read();
                var result = data.Nuclides[0];

                if (nuclide == "Cs-137")
                {
                    // 子孫核種であるBa-137mの結果を読み出す。
                    result = data.Nuclides[1];
                }

                var compartments = result.Compartments.Select(c => c.Name).ToArray();

                OutputCompartmentData GetCompartmentData(string name)
                {
                    var index = Array.IndexOf(compartments, name);
                    return index != -1 ? result.Compartments[index] : null;
                }

                var resultWholeBody /**/= GetCompartmentData("WholeBody");
                var resultUrine     /**/= GetCompartmentData("Urine");
                var resultFaeces    /**/= GetCompartmentData("Faeces");

                if (resultWholeBody is null)
                    throw new InvalidDataException();

                // 経過時間ゼロでの残留放射能＝初期配分の結果を読み飛ばす。
                for (int istep = 1; istep < data.TimeSteps.Count; istep++)
                {
                    double? GetValue(OutputCompartmentData res) =>
                        res?.Values[istep] is double v && !double.IsNaN(v) ? v : default(double?);

                    retentions.Add(new Retention
                    {
                        StartTime /**/= data.TimeSteps[istep - 1],
                        EndTime   /**/= data.TimeSteps[istep],
                        WholeBody /**/= GetValue(resultWholeBody).Value,
                        Urine     /**/= GetValue(resultUrine),
                        Faeces    /**/= GetValue(resultFaeces),
                    });
                }
            }

            return retentions;
        }

        /// <summary>
        /// OIR Data Viewerの「Dose per Content & Reference Bioassay Functions」から取得した、
        /// 核種・摂取形態・化学形態・及び粒子径に対応する残留放射能データを保存したファイルから
        /// 全身の残留放射能の数値を取得する。
        /// </summary>
        /// <param name="target"></param>
        /// <param name="mat"></param>
        /// <returns></returns>
        /// <exception cref="InvalidDataException"></exception>
        static List<Retention> GetExpectRetentions(string target, string mat)
        {
            var nuclide = target.Split('_')[0];
            var filePath = $"Expect/{target}.dat";

            var (routeOfIntake, material, particleSize) = DecomposeMaterial(mat);

            var retentions = new List<Retention>();

            using (var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read))
            using (var reader = new StreamReader(stream, Encoding.UTF8))
            {
                string[] columns;

                // ファイルの内容が対象核種のものか確認する。
                columns = reader.ReadLine().Split('\t');
                if (columns[1] != nuclide)
                    throw new InvalidDataException();

                // 対象の摂取形態かどうか確認する。
                columns = reader.ReadLine().Split('\t');
                if (columns[1] != routeOfIntake)
                    throw new InvalidDataException();

                // 対象の化学形態かどうか確認する。
                columns = reader.ReadLine().Split('\t');
                if (columns[1] != material &&
                    columns[1] != $"{routeOfIntake}, {material}")   // Injectionの場合
                    throw new InvalidDataException();

                // 対象の粒子サイズかどうか確認する。
                columns = reader.ReadLine().Split('\t');
                if (columns[1] != particleSize)
                    throw new InvalidDataException();

                reader.ReadLine();  // (empty line)

                // Bq/Bqで出力された数値データかどうか確認する。
                if (reader.ReadLine() != "Content in an Organ or Excreta Sample per Intake (Reference Bioassay Functions m(t)), Bq per Bq")
                    throw new InvalidDataException();

                columns = reader.ReadLine().Split('\t');  // (table header)
                var indexWholeBody /**/= columns.IndexOf(s => s.Contains("Whole Body"));
                var indexUrine     /**/= columns.IndexOf(s => s.Contains("Urine"));
                var indexFaeces    /**/= columns.IndexOf(s => s.Contains("Faeces"));

                var startTime = 0.0;

                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    columns = line.Split('\t');

                    var endTime   /**/= double.Parse(columns[0]);

                    double? GetValue(int index) =>
                        index == -1 || columns[index] == "-" ? default(double?) : double.Parse(columns[index]);

                    retentions.Add(new Retention
                    {
                        StartTime /**/= startTime,
                        EndTime   /**/= endTime,
                        WholeBody /**/= GetValue(indexWholeBody).Value,
                        Urine     /**/= GetValue(indexUrine),
                        Faeces    /**/= GetValue(indexFaeces),
                    });

                    startTime = endTime;
                }
            }

            return retentions;
        }
    }

    static class LinqExtensions
    {
        public static int IndexOf<T>(this IEnumerable<T> sources, Func<T, bool> predicate)
        {
            int index = 0;
            foreach (var value in sources)
            {
                if (predicate(value))
                    return index;
                index++;
            }
            return -1;
        }
    }
}
