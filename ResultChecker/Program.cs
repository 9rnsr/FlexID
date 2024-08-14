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
    internal class Program
    {
        static void Main(string[] args)
        {
            var targets = GetTargets().ToArray();
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

            var summaryHeaders = new[]
            {
                "Summary",
                "",
                "Target,Whole Body Effective Dose,,,,Whole Body,,Urine,,Faeces",
                ",OIR,FlexID,Diff (FlexID/OIR),,Diff (min),Diff (max),Diff (min),Diff (max),Diff (min),Diff (max)",
            };
            var summaryLines = results.OrderBy(r => r.Target).Select(res =>
            {
                var line = $"{res.Target}";
                if (res.HasErrors)
                    line += ",-,-,-,-,-";
                else
                {
                    line += $",{res.ExpectEffectiveDose}" +
                            $",{res.ActualEffectiveDose}" +
                            $",{res.FractionEffectiveDose:0.00%},";
                    line += $",{res.WholeBodyActivityFractionMin:0.00%}" +
                            $",{res.WholeBodyActivityFractionMax:0.00%}" +
                            $",{res.UrineActivityFractionMin:0.00%}" +
                            $",{res.UrineActivityFractionMax:0.00%}" +
                            $",{res.FaecesActivityFractionMin:0.00%}" +
                            $",{res.FaecesActivityFractionMax:0.00%}";
                }
                return line;
            });
            File.WriteAllLines("summary.csv", summaryHeaders.Concat(summaryLines));
        }

        // 計算および比較の結果。
        struct Result
        {
            public string Target;

            public bool HasErrors;

            public string ExpectEffectiveDose;
            public string ActualEffectiveDose;
            public double FractionEffectiveDose;

            public double WholeBodyActivityFractionMin;
            public double WholeBodyActivityFractionMax;

            public double UrineActivityFractionMin;
            public double UrineActivityFractionMax;

            public double FaecesActivityFractionMin;
            public double FaecesActivityFractionMax;
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

            var result = new Result() { Target = target };

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
            var minWholeBodyFrac = double.PositiveInfinity;
            var maxWholeBodyFrac = double.NegativeInfinity;
            var oneDayActualUrine = 0.0;
            var oneDayActualFaeces = 0.0;
            var minUrineFrac = double.PositiveInfinity;
            var maxUrineFrac = double.NegativeInfinity;
            var minFaecesFrac = double.PositiveInfinity;
            var maxFaecesFrac = double.NegativeInfinity;
            foreach (var (actualAct, expectAct) in
                actualActs.Zip(expectActs, (a, b) => (a, b)))
            {
                var fractionWholeBody = actualAct.WholeBody / expectAct.WholeBody;
                minWholeBodyFrac = Math.Min(minWholeBodyFrac, fractionWholeBody);
                maxWholeBodyFrac = Math.Max(maxWholeBodyFrac, fractionWholeBody);

                var duration = expectAct.EndTime - expectAct.StartTime;
                if (duration < 1.0)
                {
                    // 時間メッシュ幅が1日未満の場合は、1日分の積算放射能を計算する。
                    oneDayActualUrine += actualAct.Urine.Value * duration;
                    oneDayActualFaeces += actualAct.Faeces.Value * duration;

                    // OIR側は24-hour sample値なので、時間メッシュの終了時刻が
                    // 1日の倍数になった位置で比較を行う。
                    if (expectAct.EndTime % 1.0 == 0)
                    {
                        var fractionUrine = oneDayActualUrine / expectAct.Urine.Value;
                        minUrineFrac = Math.Min(minUrineFrac, fractionUrine);
                        maxUrineFrac = Math.Max(maxUrineFrac, fractionUrine);

                        var fractionFaeces = oneDayActualFaeces / expectAct.Faeces.Value;
                        minFaecesFrac = Math.Min(minFaecesFrac, fractionFaeces);
                        maxFaecesFrac = Math.Max(maxFaecesFrac, fractionFaeces);

                        oneDayActualUrine = 0;
                        oneDayActualFaeces = 0;
                    }
                }
                else
                {
                    if (expectAct.Urine is double expectUrine)
                    {
                        var fractionUrine = actualAct.Urine.Value / expectUrine;
                        minUrineFrac = Math.Min(minUrineFrac, fractionUrine);
                        maxUrineFrac = Math.Max(maxUrineFrac, fractionUrine);
                    }
                    if (expectAct.Faeces is double expectFaeces)
                    {
                        var fractionFaeces = actualAct.Faeces.Value / expectFaeces;
                        minFaecesFrac = Math.Min(minFaecesFrac, fractionFaeces);
                        maxFaecesFrac = Math.Max(maxFaecesFrac, fractionFaeces);
                    }
                }
            }
            result./**/ WholeBodyActivityFractionMin = minWholeBodyFrac;
            result./**/ WholeBodyActivityFractionMax = maxWholeBodyFrac;
            result./**/     UrineActivityFractionMin = minUrineFrac;
            result./**/     UrineActivityFractionMax = maxUrineFrac;
            result./**/    FaecesActivityFractionMin = minFaecesFrac;
            result./**/    FaecesActivityFractionMax = maxFaecesFrac;

            return result;
        }

        /// <summary>
        /// FlexIDに整備されているインプットと、OIRデータにおけるMaterialの対応を定義する。
        /// (核種についてはTargetの名称に含める仕様となっている)
        /// </summary>
        /// <returns></returns>
        static IEnumerable<(string Target, string Material)> GetTargets()
        {
            yield return ("Ba-133_ing-Insoluble",       /**/"Ingestion, Insoluble forms: sulphate, titanate, fA=1E-4");
            yield return ("Ba-133_ing-Soluble",         /**/"Ingestion, Soluble forms, fA=0.2");
            yield return ("Ba-133_inh-TypeF",           /**/"Inhalation, Aerosols Type F, Barium chloride, carbonate, fA=0.2, 5 µm");
            yield return ("Ba-133_inh-TypeM",           /**/"Inhalation, Aerosols Type M, Barium sulphate, all unspecified forms, fA=4E-2, 5 µm");
            yield return ("Ba-133_inh-TypeS",           /**/"Inhalation, Aerosols Type S, fA=2E-3, 5 µm");

            yield return ("C-14_ing",                   /**/"Ingestion, All chemical forms, fA=0.99");
            yield return ("C-14_inh-TypeF",             /**/"Inhalation, Aerosols Type F, fA=0.99, 5 µm");
            yield return ("C-14_inh-TypeF-Barium",      /**/"Inhalation, Aerosols Barium carbonate, fA=0.99, 5 µm");
            yield return ("C-14_inh-TypeF-Gas",         /**/"Inhalation, Gas or vapour Type F, Unspecified , fA=0.99");
            yield return ("C-14_inh-TypeM",             /**/"Inhalation, Aerosols Type M, All unspecified forms, fA=0.2, 5 µm");
            yield return ("C-14_inh-TypeS",             /**/"Inhalation, Aerosols Type S, Elemental carbon, carbon tritide, fA=1E-2, 5 µm");

            yield return ("Ca-45_ing",                  /**/"Ingestion, All unspecified forms, fA=0.4");
            yield return ("Ca-45_inh-TypeF",            /**/"Inhalation, Aerosols Type F, Calcium chloride, fA=0.4, 5 µm");
            yield return ("Ca-45_inh-TypeM",            /**/"Inhalation, Aerosols Type M, All unspecified forms, fA=8E-2, 5 µm");
            yield return ("Ca-45_inh-TypeS",            /**/"Inhalation, Aerosols Type S, fA=4E-3, 5 µm");

            yield return ("Cs-134_ing-Insoluble",       /**/"Ingestion, Relatively insoluble forms, irradiated fuel fragments, fA=0.1");
            yield return ("Cs-134_ing-Unspecified",     /**/"Ingestion, Caesium chloride, nitrate, sulphate; all unspecified compounds, fA=0.99");
            yield return ("Cs-134_inh-TypeF",           /**/"Inhalation, Aerosols Type F, Caesium chloride, nitrate, sulphate, fA=0.99, 5 µm");
            yield return ("Cs-134_inh-TypeM",           /**/"Inhalation, Aerosols Type M, Irradiated fuel fragments, all unspecified forms, fA=0.2, 5 µm");
            yield return ("Cs-134_inh-TypeS",           /**/"Inhalation, Aerosols Type S, fA=1E-2, 5 µm");

            yield return ("Cs-137_ing-Insoluble",       /**/"Ingestion, Relatively insoluble forms, irradiated fuel fragments, fA=0.1");
            yield return ("Cs-137_ing-Unspecified",     /**/"Ingestion, Caesium chloride, nitrate, sulphate; all unspecified compounds, fA=0.99");
            yield return ("Cs-137_inh-TypeF",           /**/"Inhalation, Aerosols Type F, Caesium chloride, nitrate, sulphate, fA=0.99, 5 µm");
            yield return ("Cs-137_inh-TypeM",           /**/"Inhalation, Aerosols Type M, Irradiated fuel fragments, all unspecified forms, fA=0.2, 5 µm");
            yield return ("Cs-137_inh-TypeS",           /**/"Inhalation, Aerosols Type S, fA=1E-2, 5 µm");

            yield return ("Fe-59_ing",                  /**/"Ingestion, All unspecified forms, fA=0.1");
            yield return ("Fe-59_inh-TypeF",            /**/"Inhalation, Aerosols Type F, fA=0.1, 5 µm");
            yield return ("Fe-59_inh-TypeM",            /**/"Inhalation, Aerosols Type M, Ferric chloride, ferric oxide, all unspecified forms, fA=2E-2, 5 µm");
            yield return ("Fe-59_inh-TypeS",            /**/"Inhalation, Aerosols Type S, Corrosion products, fA=1E-3, 5 µm");

            yield return ("H-3_ing-Organic",            /**/"Ingestion,  Biogenic forms, fA=0.99");
            yield return ("H-3_ing-Insoluble",          /**/"Ingestion, Relatively insoluble forms, fA=0.1");
            yield return ("H-3_ing-Soluble",            /**/"Ingestion, Soluble forms, fA=0.99");
            yield return ("H-3_inh-TypeF-Gas",          /**/"Inhalation, Gas or vapour Type F, Unspecified , fA=0.99");
            yield return ("H-3_inh-TypeF-Organic",      /**/"Inhalation, Aerosols Biogenic organic compounds, fA=0.99, 5 µm");
            yield return ("H-3_inh-TypeF-Tritide",      /**/"Inhalation, Aerosols Type F, LaNiAl tritide, fA=0.99, 5 µm");
            yield return ("H-3_inh-TypeM",              /**/"Inhalation, Aerosols Type M, All unspecified compounds, glass fragments, luminous paint, titanium tritide, zirconium tritide, fA=0.2, 5 µm");
            yield return ("H-3_inh-TypeS",              /**/"Inhalation, Aerosols Type S, Carbon tritide, hafnium tritide, fA=1E-2, 5 µm");

            yield return ("I-129_ing",                  /**/"Ingestion, All unspecified forms, fA=0.99");
            yield return ("I-129_inh-TypeF",            /**/"Inhalation, Aerosols Type F, Sodium iodide, caesium chloride vector, silver iodide, all unspecified forms, fA=0.99, 5 µm");
            yield return ("I-129_inh-TypeM",            /**/"Inhalation, Aerosols Type M, fA=0.2, 5 µm");
            yield return ("I-129_inh-TypeS",            /**/"Inhalation, Aerosols Type S, fA=1E-2, 5 µm");

            yield return ("Pu-238_ing-Insoluble",       /**/"Ingestion, Insoluble forms: oxides, fA=1E-5");
            yield return ("Pu-238_ing-Unidentified",    /**/"Ingestion, Soluble forms: nitrate, chloride, bicarbonates, all other unidentified chemical forms, fA=5E-4");
            yield return ("Pu-238_inh-TypeF",           /**/"Inhalation, Aerosols Type F, fA=5E-4, 5 µm");
            yield return ("Pu-238_inh-TypeM",           /**/"Inhalation, Aerosols Type M, Plutonium citrate, plutonium tri-butyl-phosphate, plutonium chloride, fA=1E-4, 5 µm");
            yield return ("Pu-238_inh-TypeS",           /**/"Inhalation, Aerosols Type S, fA=5E-6, 5 µm");

            yield return ("Pu-239_ing-Insoluble",       /**/"Ingestion, Insoluble forms: oxides, fA=1E-5");
            yield return ("Pu-239_ing-Unidentified",    /**/"Ingestion, Soluble forms: nitrate, chloride, bicarbonates, all other unidentified chemical forms, fA=5E-4");
            yield return ("Pu-239_inh-TypeF",           /**/"Inhalation, Aerosols Type F, fA=5E-4, 5 µm");
            yield return ("Pu-239_inh-TypeM",           /**/"Inhalation, Aerosols Type M, Plutonium citrate, plutonium tri-butyl-phosphate, plutonium chloride, fA=1E-4, 5 µm");
            yield return ("Pu-239_inh-TypeS",           /**/"Inhalation, Aerosols Type S, fA=5E-6, 5 µm");
            yield return ("Pu-239_inj",                 /**/"Injection, fA=5E-4");

            yield return ("Pu-240_ing-Insoluble",       /**/"Ingestion, Insoluble forms: oxides, fA=1E-5");
            yield return ("Pu-240_ing-Unidentified",    /**/"Ingestion, Soluble forms: nitrate, chloride, bicarbonates, all other unidentified chemical forms, fA=5E-4");
            yield return ("Pu-240_inh-TypeF",           /**/"Inhalation, Aerosols Type F, fA=5E-4, 5 µm");
            yield return ("Pu-240_inh-TypeM",           /**/"Inhalation, Aerosols Type M, Plutonium citrate, plutonium tri-butyl-phosphate, plutonium chloride, fA=1E-4, 5 µm");
            yield return ("Pu-240_inh-TypeS",           /**/"Inhalation, Aerosols Type S, fA=5E-6, 5 µm");

            yield return ("Pu-241_ing-Insolube",        /**/"Ingestion, Insoluble forms: oxides, fA=1E-5");
            yield return ("Pu-241_ing-Unidentified",    /**/"Ingestion, Soluble forms: nitrate, chloride, bicarbonates, all other unidentified chemical forms, fA=5E-4");
            yield return ("Pu-241_inh-TypeF",           /**/"Inhalation, Aerosols Type F, fA=5E-4, 5 µm");
            yield return ("Pu-241_inh-TypeM",           /**/"Inhalation, Aerosols Type M, Plutonium citrate, plutonium tri-butyl-phosphate, plutonium chloride, fA=1E-4, 5 µm");
            yield return ("Pu-241_inh-TypeS",           /**/"Inhalation, Aerosols Type S, fA=5E-6, 5 µm");

            yield return ("Pu-242_ing-Insoluble",       /**/"Ingestion, Insoluble forms: oxides, fA=1E-5");
            yield return ("Pu-242_ing-Unidentified",    /**/"Ingestion, Soluble forms: nitrate, chloride, bicarbonates, all other unidentified chemical forms, fA=5E-4");
            yield return ("Pu-242_inh-TypeF",           /**/"Inhalation, Aerosols Type F, fA=5E-4, 5 µm");
            yield return ("Pu-242_inh-TypeM",           /**/"Inhalation, Aerosols Type M, Plutonium citrate, plutonium tri-butyl-phosphate, plutonium chloride, fA=1E-4, 5 µm");
            yield return ("Pu-242_inh-TypeS",           /**/"Inhalation, Aerosols Type S, fA=5E-6, 5 µm");

            yield return ("Ra-223_inh-TypeF",           /**/"Inhalation, Aerosols Type F, Nitrate, fA=0.2, 5 µm");

            yield return ("Ra-226_ing",                 /**/"Ingestion, All forms, fA=0.2");
            yield return ("Ra-226_inh-TypeF",           /**/"Inhalation, Aerosols Type F, Nitrate, fA=0.2, 5 µm");
            yield return ("Ra-226_inh-TypeM",           /**/"Inhalation, Aerosols Type M, All unspecified forms, fA=4E-2, 5 µm");
            yield return ("Ra-226_inh-TypeS",           /**/"Inhalation, Aerosols Type S, fA=2E-3, 5 µm");

            yield return ("Sr-90_ing-Titanate",         /**/"Ingestion, Strontium titanate, fA=1E-2");
            yield return ("Sr-90_ing-Other",            /**/"Ingestion, All other chemical forms, fA=0.25");
            yield return ("Sr-90_inh-TypeF",            /**/"Inhalation, Aerosols Type F, Strontium chloride, sulphate and carbonate, fA=0.25, 5 µm");
            yield return ("Sr-90_inh-TypeM",            /**/"Inhalation, Aerosols Type M, Fuel fragments, all unspecified forms, fA=5E-2, 5 µm");
            yield return ("Sr-90_inh-TypeS",            /**/"Inhalation, Aerosols Type S, FAP, PSL, fA=2.5E-3, 5 µm");

            yield return ("Tc-99_ing",                  /**/"Ingestion, All forms, fA=0.9");
            yield return ("Tc-99_inh-TypeF",            /**/"Inhalation, Aerosols Type F, Pertechnetate, Tc-DTPA, fA=0.9, 5 µm");
            yield return ("Tc-99_inh-TypeM",            /**/"Inhalation, Aerosols Type M, All unspecified forms, fA=0.18, 5 µm");
            yield return ("Tc-99_inh-TypeS",            /**/"Inhalation, Aerosols Type S, fA=9E-3, 5 µm");

            yield return ("Zn-65_ing",                  /**/"Ingestion, All forms, fA=0.5");
            yield return ("Zn-65_inh-TypeF",            /**/"Inhalation, Aerosols Type F, Oxide, chromate, fA=0.5, 5 µm");
            yield return ("Zn-65_inh-TypeM",            /**/"Inhalation, Aerosols Type M, Nitrate, phosphate, all unspecified compounds, fA=0.1, 5 µm");
            yield return ("Zn-65_inh-TypeS",            /**/"Inhalation, Aerosols Type S, Corrosion products, fA=5E-3, 5 µm");
        }

        static Regex patternMaterial = new Regex(
           @"^(?<RouteOfIntake>Injection|Ingestion|Inhalation), (?<Material>.+?)(?:, (?<ParticleSize>[\d\.]+) µm)?$");

        static (string routeOfIntake, string material, string particleSize) DecomposeMaterial(string mat)
        {
            var m = patternMaterial.Match(mat);
            if (!m.Success)
                throw new InvalidDataException();

            var routeOfIntake = m.Groups["RouteOfIntake"].Value;
            var particleSize = m.Groups["ParticleSize"].Value;
            var material = m.Groups["Material"].Value;
            if (particleSize.Length == 0)
                particleSize = "-";

            return (routeOfIntake, material, particleSize);
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

            string line;
            using (var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read))
            using (var reader = new StreamReader(stream, Encoding.UTF8))
            {
                if (nuclide == "Cs-137")
                {
                    // 子孫核種であるBa-137mの結果を読み出す。
                    while ((line = reader.ReadLine()) != "")
                        continue;
                }

                reader.ReadLine();

                var compartments = reader.ReadLine()
                    .Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                var indexUrine = Array.IndexOf(compartments, "Urine");
                var indexFaeces = Array.IndexOf(compartments, "Faeces");
                if (indexUrine == -1) throw new InvalidDataException();
                if (indexFaeces == -1) throw new InvalidDataException();

                reader.ReadLine();

                // 経過時間ゼロでの残留放射能＝初期配分の結果を読み飛ばす。
                reader.ReadLine();

                var startTime = 0.0;

                while ((line = reader.ReadLine()) != null)
                {
                    if (line.Length == 0)
                        break;

                    var columns = line.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                    var endTime = double.Parse(columns[0]);
                    var wholeBody /**/= double.Parse(columns[1]);
                    var urine     /**/= double.Parse(columns[indexUrine]);
                    var faeces    /**/= double.Parse(columns[indexFaeces]);
                    retentions.Add(new Retention
                    {
                        StartTime /**/= startTime,
                        EndTime   /**/= endTime,
                        WholeBody /**/= wholeBody,
                        Urine     /**/= urine,
                        Faeces    /**/= faeces,
                    });

                    startTime = endTime;
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

                reader.ReadLine();  // (table header)

                var startTime = 0.0;

                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    columns = line.Split('\t');
                    var endTime   /**/= double.Parse(columns[0]);
                    var wholeBody /**/= double.Parse(columns[1]);
                    var urine     /**/= columns[2] == "-" ? default(double?) : double.Parse(columns[2]);
                    var faeces    /**/= columns[3] == "-" ? default(double?) : double.Parse(columns[3]);
                    retentions.Add(new Retention
                    {
                        StartTime /**/= startTime,
                        EndTime   /**/= endTime,
                        WholeBody /**/= wholeBody,
                        Urine     /**/= urine,
                        Faeces    /**/= faeces,
                    });

                    startTime = endTime;
                }
            }

            return retentions;
        }
    }
}
