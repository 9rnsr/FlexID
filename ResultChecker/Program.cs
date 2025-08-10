using System.Collections.Concurrent;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using FlexID.Calc;

namespace ResultChecker
{
    internal partial class Program
    {
        static async Task<int> Main(string[] args)
        {
            // 処理結果の出力ディレクトリ。
            var outputDir = "out";

            // 処理結果の出力ファイル名。
            var outputFileName = "summary.xlsx";

            // 結果取得するための計算を実行するかどうか。
            var runCalculation = true;

            List<Regex> patterns = null;

            if (args.Length >= 1)
            {
                var processOptions = true;

                for (int i = 0; i < args.Length; i++)
                {
                    var arg = args[i];

                    if (!processOptions)
                        goto Lpattern;

                    bool IsOption(params string[] opts) => opts.Contains(arg);

                    bool GetOption(out string value, params string[] opts)
                    {
                        if (IsOption(opts) && i + 1 < args.Length)
                        {
                            value = args[++i];
                            return true;
                        }
                        value = null;
                        return false;
                    }

                    if (IsOption("--"))
                    {
                        processOptions = false;
                        continue;
                    }

                    if (arg[0] == '@')
                    {
                        var head = args.AsSpan(0, i).ToArray();
                        var tail = args.AsSpan(i + 1).ToArray();
                        var newArgs = File.ReadAllText(arg.Substring(1))
                            .Split(Array.Empty<char>(), StringSplitOptions.RemoveEmptyEntries);
                        args = head.Concat(newArgs).Concat(tail).ToArray();
                        i--;
                        continue;
                    }

                    if (IsOption("-h", "--help"))
                    {
                        Usage();
                        return -1;
                    }

                    if (GetOption(out var output, "-o", "--out"))
                    {
                        // オプション値から出力ディレクトリと出力ファイル名の両方を設定する。
                        outputDir = Path.GetDirectoryName(output);
                        outputFileName = Path.GetFileName(output);
                        continue;
                    }
                    if (GetOption(out var outDir, "-od", "--output-dir"))
                    {
                        outputDir = outDir;
                        continue;
                    }
                    if (GetOption(out var outFile, "-of", "--output-file"))
                    {
                        outputFileName = outFile;
                        continue;
                    }

                    if (IsOption("--run"))
                    {
                        runCalculation = true;
                        continue;
                    }
                    if (IsOption("--no-run"))
                    {
                        runCalculation = false;
                        continue;
                    }

                    if (arg.Length == 2 && arg[0] == '-' || arg.StartsWith("--"))
                    {
                        Console.Error.WriteLine($"error: unknown option '{arg}'");
                        Usage();
                        return -1;
                    }

                Lpattern:
                    var pattern = arg;
                    try
                    {
                        if (patterns is null)
                            patterns = new List<Regex>();
                        patterns.Add(new Regex(pattern, RegexOptions.IgnoreCase));
                    }
                    catch
                    {
                        var n = patterns.Count;
                        var nth = n == 1 ? "1st" : n == 2 ? "2nd" : $"{n}th";
                        Console.Error.WriteLine($"{nth} target pattern is not correct.");
                        return -1;
                    }
                }
            }

            // パターンに合致する処理対象を収集する。
            var targets = GetTargets(outputDir, runCalculation)
                .Where(target => patterns?.Any(pattern => pattern.IsMatch(target.Name)) ?? true)
                .ToArray();
            if (targets.Length == 0)
            {
                Console.Error.WriteLine($"error: there is no targets.");
                return -1;
            }

            Directory.CreateDirectory(outputDir);

            var results = new ConcurrentBag<Result>();

            // 1コアは画面更新とそのほかのプロセスのために占有しない。
            var maxParallel = Math.Max(1, Environment.ProcessorCount - 1);
            var lcts = new LimitedConcurrencyLevelTaskScheduler(maxParallel);
            var factory = new TaskFactory(lcts);

            var presenter = new ProgressPresenter(targets.Length);

            if (runCalculation)
            {
                // 並列処理で計算を実施する。
                Task.WaitAll(targets.Select(target => factory.StartNew(() => Process(target))).ToArray());
            }
            else
            {
                // 同期処理で出力の取得と進捗表示を実施する。
                presenter.Update();
                foreach (var target in targets)
                {
                    Process(target);
                    presenter.Update();
                }
                presenter.Update();
            }

            void Process(Target target)
            {
                try
                {
                    presenter.Start(target.Name);

                    if (runCalculation)
                        RunCalc(target, outputDir);

                    var result = GetResult(target);
                    results.Add(result);

                    presenter.Stop(target.Name, $"\x1B[36mOK\x1B[0m");
                }
                catch (Exception ex)
                {
                    // 何らかのエラーが発生した場合。
                    results.Add(new Result { Target = target, HasErrors = true });

                    presenter.Stop(target.Name, $"\x1B[31mNG\x1B[0m", ex.Message);
                }

                // 1ケースの計算が終了するごとにGCを呼ばないと、並列計算数がだんだん減ってしまう。
                if (runCalculation)
                    GC.Collect();
            }

            await presenter.WaitForExit();

            var outputFilePath = Path.Combine(outputDir, outputFileName);
            var sortedResults = results.OrderBy(r => r.Target.Name).ToArray();

            Console.Write($"\nGenerate {outputFileName} ...");
            WriteSummaryExcel(outputFilePath, sortedResults);
            Console.WriteLine($"done");

            return 0;
        }

        private static void Usage()
        {
            var exeName = Path.GetFileNameWithoutExtension(Assembly.GetExecutingAssembly().Location);

            Console.Error.WriteLine($"usage: {exeName} [options] [<pattern> ...]");
            Console.Error.WriteLine();
            Console.Error.WriteLine("options:");
            Console.Error.WriteLine("    -o, --output <path>           set the output directory and output file name");
            Console.Error.WriteLine("    -od, --output-dir <path>      set the output directory");
            Console.Error.WriteLine("    -of, --output-file <name>     set the output summary Excel file name");
            Console.Error.WriteLine("    --[no-]run                    run calculation");
            Console.Error.WriteLine("    -h, --help                    print help information");
            Console.Error.WriteLine("    @file                         read command-line options from the file");
            Console.Error.WriteLine();
            Console.Error.WriteLine("<pattern>");
            Console.Error.WriteLine("    target input name pattern as regular expression.");
            Console.Error.WriteLine();
        }

        /// <summary>
        /// 計算および比較の対象。
        /// </summary>
        class Target
        {
            public string Name;

            public string Nuclide => Name.Split('_')[0];

            public string RouteOfIntake;
            public string ChemicalForm;
            public string ParticleSize;

            public string NuclideWholeBody;
            public string NuclideUrine;
            public string NuclideFaeces;
            public string NuclideAtract;
            public string NuclideLungs;
            public string NuclideSkeleton;
            public string NuclideLiver;
            public string NuclideThyroid;

            public string TargetPath;

            public string ExpectDosePath;

            public string ExpectRetentionPath;

            public string ResultDosePath;

            public string ResultRetentionPath;
        }

        /// <summary>
        /// 計算および比較の結果。
        /// </summary>
        class Result
        {
            public Target Target;

            public bool HasErrors;

            public IReadOnlyList<Retention> ExpectActs;
            public IReadOnlyList<Retention> ActualActs;

            public Dose ExpectDose;
            public Dose ActualDose;

            public (double Min, double Max) FractionsWholeBody;
            public (double Min, double Max) FractionsUrine;
            public (double Min, double Max) FractionsFaeces;
            public (double Min, double Max) FractionsAtract;
            public (double Min, double Max) FractionsLungs;
            public (double Min, double Max) FractionsSkeleton;
            public (double Min, double Max) FractionsLiver;
            public (double Min, double Max) FractionsThyroid;
        }

        /// <summary>
        /// 計算を実行する。
        /// </summary>
        /// <param name="target"></param>
        /// <param name="outputDir"></param>
        static void RunCalc(Target target, string outputDir)
        {
            var nuclide = target.Nuclide;

            var outputPath = Path.Combine(outputDir, target.Name);

            // 計算時間メッシュはFlexID.Calcに付属のものを使用する。
            var cTimeMeshFile = @"lib\TimeMesh\time.dat";

            // 出力時間メッシュは、OIRの残留放射能データと同じになるように設定する。
            var oTimeMeshFile = @"lib\TimeMesh\out-time-OIR.dat";

            var commitmentPeriod = "50years";

            var data = new InputDataReader_OIR(target.TargetPath).Read();
            data.OutputDose = true;
            data.OutputDoseRate = false;
            data.OutputRetention = true;
            data.OutputCumulative = false;

            var main = new MainRoutine_OIR();
            main.OutputPath       /**/= outputPath;
            main.CalcTimeMeshPath /**/= cTimeMeshFile;
            main.OutTimeMeshPath  /**/= oTimeMeshFile;
            main.CommitmentPeriod /**/= commitmentPeriod;

            main.Main(data);
        }

        /// <summary>
        /// 計算と比較の結果を取得する。
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        static Result GetResult(Target target)
        {
            var result = new Result() { Target = target };

            result.ExpectActs = GetExpectRetentions(target);
            result.ActualActs = GetResultRetentions(target);

            // 50年の預託期間における、各出力時間メッシュにおける数値の比較。
            // 要約として、期待値に対する下振れ率と上振れ率の最大値を算出する。
            var fractionsWholeBody /**/= (min: double.PositiveInfinity, max: double.NegativeInfinity);
            var fractionsUrine     /**/= (min: double.PositiveInfinity, max: double.NegativeInfinity);
            var fractionsFaeces    /**/= (min: double.PositiveInfinity, max: double.NegativeInfinity);
            var fractionsAtract    /**/= (min: double.PositiveInfinity, max: double.NegativeInfinity);
            var fractionsLungs     /**/= (min: double.PositiveInfinity, max: double.NegativeInfinity);
            var fractionsSkeleton  /**/= (min: double.PositiveInfinity, max: double.NegativeInfinity);
            var fractionsLiver     /**/= (min: double.PositiveInfinity, max: double.NegativeInfinity);
            var fractionsThyroid   /**/= (min: double.PositiveInfinity, max: double.NegativeInfinity);

            var activities = result.ActualActs.Zip(result.ExpectActs, (a, e) => (a, e));
            foreach (var (actualAct, expectAct) in activities)
            {
                //Console.WriteLine(
                //    $"{expectAct.EndTime,8}," +
                //    $"{expectAct.WholeBody:0.0E+00}," +
                //    $"{(expectAct.Urine != null ? $"{expectAct.Urine:0.0E+00}" : "-"),-7}," +
                //    $"{(expectAct.Faeces != null ? $"{expectAct.Faeces:0.0E+00}" : "-"),-7}," +
                //    $"{actualAct.WholeBody:0.00000000E+00}," +
                //    $"{(actualAct.Urine != null ? $"{actualAct.Urine:0.00000000E+00}" : "-"),-14}," +
                //    $"{(actualAct.Faeces != null ? $"{actualAct.Faeces:0.00000000E+00}" : "-"),-14}");

                if (expectAct.WholeBody is double expectWholeBody &&
                    actualAct.WholeBody is double actualWholeBody)
                {
                    var fracWholeBody = actualWholeBody / expectWholeBody;
                    fractionsWholeBody = (Math.Min(fractionsWholeBody.min, fracWholeBody),
                                          Math.Max(fractionsWholeBody.max, fracWholeBody));
                }
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
                if (expectAct.Atract is double expectAtract &&
                    actualAct.Atract is double actualAtract)
                {
                    var fracAtract = actualAtract / expectAtract;
                    fractionsAtract = (Math.Min(fractionsAtract.min, fracAtract),
                                       Math.Max(fractionsAtract.max, fracAtract));
                }
                if (expectAct.Lungs is double expectLungs &&
                    actualAct.Lungs is double actualLungs)
                {
                    var fracLungs = actualLungs / expectLungs;
                    fractionsLungs = (Math.Min(fractionsLungs.min, fracLungs),
                                      Math.Max(fractionsLungs.max, fracLungs));
                }
                if (expectAct.Skeleton is double expectSkeleton &&
                    actualAct.Skeleton is double actualSkeleton)
                {
                    var fracSkeleton = actualSkeleton / expectSkeleton;
                    fractionsSkeleton = (Math.Min(fractionsSkeleton.min, fracSkeleton),
                                         Math.Max(fractionsSkeleton.max, fracSkeleton));
                }
                if (expectAct.Liver is double expectLiver &&
                    actualAct.Liver is double actualLiver)
                {
                    var fracLiver = actualLiver / expectLiver;
                    fractionsLiver = (Math.Min(fractionsLiver.min, fracLiver),
                                      Math.Max(fractionsLiver.max, fracLiver));
                }
                if (expectAct.Thyroid is double expectThyroid &&
                    actualAct.Thyroid is double actualThyroid)
                {
                    var fracThyroid = actualThyroid / expectThyroid;
                    fractionsThyroid = (Math.Min(fractionsThyroid.min, fracThyroid),
                                        Math.Max(fractionsThyroid.max, fracThyroid));
                }
            }

            result.FractionsWholeBody /**/= fractionsWholeBody;
            result.FractionsUrine     /**/= fractionsUrine;
            result.FractionsFaeces    /**/= fractionsFaeces;
            result.FractionsAtract    /**/= fractionsAtract;
            result.FractionsLungs     /**/= fractionsLungs;
            result.FractionsSkeleton  /**/= fractionsSkeleton;
            result.FractionsLiver     /**/= fractionsLiver;
            result.FractionsThyroid   /**/= fractionsThyroid;

            // 預託実効線量と預託等価線量を取得。
            result.ExpectDose = GetExpectDoses(target);
            result.ActualDose = GetResultDoses(target);

            return result;
        }

        /// <summary>
        /// 線量の取得結果。
        /// </summary>
        struct Dose
        {
            public double EffectiveDose { get; set; }
            public double[] EquivalentDosesMale { get; set; }
            public double[] EquivalentDosesFemale { get; set; }
        }

        /// <summary>
        /// OIR Data Viewerの「Dose per Intake」＞「Material」から取得した、
        /// ある核種の50年預託実効線量 e(50) [Sv/Bq]データを保存したファイルから、
        /// 指定のmaterialに対応する数値を取得する。
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        /// <exception cref="InvalidDataException"></exception>
        static Dose GetExpectDoses(Target target)
        {
            var nuclide = target.Nuclide;
            var filePath = target.ExpectDosePath
                ?? throw new FileNotFoundException("expect dose file", $"{nuclide}.dat");

            var routeOfIntake = target.RouteOfIntake;

            // 預託線量の期待値を引くための文字列を組み立てる。
            var mat = $"{target.RouteOfIntake}, {target.ChemicalForm}";
            if (target.ParticleSize != "-")
                mat += $", {target.ParticleSize} µm";

            using (var reader = new StreamReader(File.OpenRead(filePath), Encoding.UTF8))
            {
                string[] columns;

                // ファイルの内容が対象核種のものか確認する。
                columns = reader.ReadLine().Split('\t');
                if (columns[1] != nuclide)
                    throw new InvalidDataException($"Nuclide '{nuclide}' expected, but {columns[1]}");

                // 数値の単位が[Sv/Bq]であるかを確認する。
                columns = reader.ReadLine().Split('\t');
                if (columns[1] != "Sv per Bq")
                    throw new InvalidDataException($"Measurement unit 'Sv per Bq' expected, but '{columns[1]}'.");

                reader.ReadLine();  // (empty line)
                reader.ReadLine();  // (table header)

                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    columns = line.Split('\t');

                    if (columns[0] == mat ||
                        columns[0] == routeOfIntake)    // Injectionの場合
                    {
                        var effectiveDose = double.Parse(columns[1]);

                        var equivalentDosesMale = columns.Skip(3).Select(v => double.Parse(v)).ToArray();

                        line = reader.ReadLine();
                        columns = line.Split('\t');
                        var equivalentDosesFemale = columns.Skip(3).Select(v => double.Parse(v)).ToArray();

                        return new Dose
                        {
                            EffectiveDose = effectiveDose,
                            EquivalentDosesMale = equivalentDosesMale,
                            EquivalentDosesFemale = equivalentDosesFemale,
                        };
                    }

                    reader.ReadLine();
                }
            }

            throw new InvalidDataException($"Missing data for Material '{mat}' and Route of Intake '{routeOfIntake}'.");
        }

        /// <summary>
        /// FlexIDが出力した各出力時間メッシュにおける線量の出力ファイル
        /// *_Dose.outから、預託実効線量と預託等価線量の数値を読み込む。
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        static Dose GetResultDoses(Target target)
        {
            var nuclide = target.Nuclide;
            var filePath = target.ResultDosePath;

            // 組織加重係数データを読み込む。
            var (ts, ws) = InputDataReaderBase.ReadTissueWeights(Path.Combine("lib", "OIR", "wT.txt"));

            using (var reader = new OutputDataReader(filePath))
            {
                var data = reader.Read();
                var resultM = data.Blocks[0];
                var resultF = data.Blocks[1];

                // 列データの最終行＝評価期間の最後における線量値を取得する。
                double GetDose(OutputBlockData result, string targetRegion)
                    => result.Compartments.FirstOrDefault(c => c.Name == targetRegion)?.Values.Last()
                    ?? throw new InvalidDataException($"Missing '{targetRegion}' data column");

                double GetTissueWeight(string targetRegion)
                    => ws[Array.IndexOf(ts, targetRegion)];

                (double Male, double Female) GetResult(string targetRegion, params string[] moreTargetRegions)
                {
                    var doseM = GetDose(resultM, targetRegion);
                    var doseF = GetDose(resultF, targetRegion);
                    if (moreTargetRegions.Length == 0)
                        return (doseM, doseF);

                    // 複数の標的領域の等価線量を組織加重係数で加重平均したものを返す。
                    var wT = GetTissueWeight(targetRegion);
                    var sumOfWeightedDoseM = wT * doseM;
                    var sumOfWeightedDoseF = wT * doseF;
                    var sumOfTissueWeight = wT;
                    foreach (var moreTargetRegion in moreTargetRegions)
                    {
                        wT = GetTissueWeight(moreTargetRegion);
                        doseM = GetDose(resultM, moreTargetRegion);
                        doseF = GetDose(resultF, moreTargetRegion);
                        sumOfWeightedDoseM += wT * doseM;
                        sumOfWeightedDoseF += wT * doseF;
                        sumOfTissueWeight += wT;
                    }
                    doseM = sumOfWeightedDoseM / sumOfTissueWeight;
                    doseF = sumOfWeightedDoseF / sumOfTissueWeight;
                    return (doseM, doseF);
                }

                // Effective Dose
                var resultWholeBody = GetDose(resultM, "WholeBody");

                // Equivalent Dose
                var equivalentDoses = new[]
                {
                    // OIR Data Viewerで提示されている預託等価線量の領域名と、
                    // それらに対応する標的領域(1つ以上)の名称。
                    /* Bone marrow     */ GetResult("R-marrow"),
                    /* Colon           */ GetResult("RC-stem", "LC-stem", "RS-stem"),
                    /* Lung            */ GetResult("Bronch-bas", "Bronch-sec", "Bchiol-sec", "AI"),
                    /* Stomach         */ GetResult("St-stem"),
                    /* Breast          */ GetResult("Breast"),
                    /* Ovaries         */ GetResult("Ovaries"),
                    /* Testes          */ GetResult("Testes"),
                    /* Urinary bladder */ GetResult("UB-wall"),
                    /* Oesophagus      */ GetResult("Oesophagus"),
                    /* Liver           */ GetResult("Liver"),
                    /* Thyroid         */ GetResult("Thyroid"),
                    /* Bone Surface    */ GetResult("Endost-BS"),
                    /* Brain           */ GetResult("Brain"),
                    /* Salivary glands */ GetResult("S-glands"),
                    /* Skin            */ GetResult("Skin"),
                    /* Adrenals        */ GetResult("Adrenals"),
                    /* ET of HRTM      */ GetResult("ET1-bas", "ET2-bas"),
                    /* Gall bladder    */ GetResult("GB-wall"),
                    /* Heart           */ GetResult("Ht-wall"),
                    /* Kidneys         */ GetResult("Kidneys"),
                    /* Lymphatic nodes */ GetResult("LN-Sys", "LN-Th", "LN-ET"),
                    /* Muscle          */ GetResult("Muscle"),
                    /* Oral mucosa     */ GetResult("O-mucosa"),
                    /* Pancreas        */ GetResult("Pancreas"),
                    /* Prostate        */ GetResult("Prostate"),
                    /* Small intestine */ GetResult("SI-stem"),
                    /* Spleen          */ GetResult("Spleen"),
                    /* Thymus          */ GetResult("Thymus"),
                    /* Uterus          */ GetResult("Uterus"),
                };

                return new Dose
                {
                    EffectiveDose = resultWholeBody,
                    EquivalentDosesMale = equivalentDoses.Select(d => d.Male).ToArray(),
                    EquivalentDosesFemale = equivalentDoses.Select(d => d.Female).ToArray(),
                };
            }
        }

        /// <summary>
        /// 残留放射能の取得結果。
        /// </summary>
        struct Retention
        {
            public double StartTime;
            public double EndTime;

            public double? WholeBody;
            public double? Urine;
            public double? Faeces;
            public double? Atract;
            public double? Lungs;
            public double? Skeleton;
            public double? Liver;
            public double? Thyroid;
        }

        /// <summary>
        /// FlexIDが出力した各出力時間メッシュにおける残留放射能の出力ファイル
        /// *_Retention.outから、Whole Bodyの数値列を読み込む。
        /// </summary>
        /// <param name="target"></param>
        /// <returns>時間メッシュ毎の残留放射能データ。</returns>
        static List<Retention> GetResultRetentions(Target target)
        {
            var filePath = target.ResultRetentionPath;

            var retentions = new List<Retention>();

            using (var reader = new OutputDataReader(filePath))
            {
                var data = reader.Read();

                OutputCompartmentData GetCompartmentData(string nuclide, string name)
                {
                    if (nuclide is null)
                        return null;

                    // 対応する核種の結果を読み出す。
                    var result = data.Blocks.Where(n => n.Header == nuclide).FirstOrDefault();
                    if (result is null)
                        throw new InvalidDataException($"Missing retention data of nuclide '{nuclide}'.");

                    var index = result.Compartments.IndexOf(c => c.Name == name);
                    return index != -1 ? result.Compartments[index] : null;
                }

                var resultWholeBody /**/= GetCompartmentData(target.NuclideWholeBody, /**/"WholeBody");
                var resultUrine     /**/= GetCompartmentData(target.NuclideUrine,     /**/"Urine");
                var resultFaeces    /**/= GetCompartmentData(target.NuclideFaeces,    /**/"Faeces");
                var resultAtract    /**/= GetCompartmentData(target.NuclideAtract,    /**/"AlimentaryTract*");
                var resultLungs     /**/= GetCompartmentData(target.NuclideLungs,     /**/"Lungs*");
                var resultSkeleton  /**/= GetCompartmentData(target.NuclideSkeleton,  /**/"Skeleton*");
                var resultLiver     /**/= GetCompartmentData(target.NuclideLiver,     /**/"Liver*");
                var resultThyroid   /**/= GetCompartmentData(target.NuclideThyroid,   /**/"Thyroid*");

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
                        WholeBody /**/= GetValue(resultWholeBody),
                        Urine     /**/= GetValue(resultUrine),
                        Faeces    /**/= GetValue(resultFaeces),
                        Atract    /**/= GetValue(resultAtract),
                        Lungs     /**/= GetValue(resultLungs),
                        Skeleton  /**/= GetValue(resultSkeleton),
                        Liver     /**/= GetValue(resultLiver),
                        Thyroid   /**/= GetValue(resultThyroid),
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
        /// <returns>時間メッシュ毎の残留放射能データ。</returns>
        /// <exception cref="InvalidDataException"></exception>
        static List<Retention> GetExpectRetentions(Target target)
        {
            var nuclide = target.Nuclide;
            var filePath = target.ExpectRetentionPath
                ?? throw new FileNotFoundException("expect retention file", $"{target.Name}.dat");

            var retentions = new List<Retention>();

            using (var reader = new StreamReader(File.OpenRead(filePath), Encoding.UTF8))
            {
                string[] columns;

                // ファイルの内容が対象核種のものか確認する。
                columns = reader.ReadLine().Split('\t');
                if (columns[1] != nuclide)
                    throw new InvalidDataException();

                // 対象の摂取形態。
                target.RouteOfIntake = reader.ReadLine().Split('\t')[1];

                // 対象の化学形態。
                target.ChemicalForm = reader.ReadLine().Split('\t')[1];

                // 対象の粒子サイズ。
                target.ParticleSize = reader.ReadLine().Split('\t')[1];

                reader.ReadLine();  // (empty line)

                // Bq/Bqで出力された数値データかどうか確認する。
                if (reader.ReadLine() != "Content in an Organ or Excreta Sample per Intake (Reference Bioassay Functions m(t)), Bq per Bq")
                    throw new InvalidDataException();

                columns = reader.ReadLine().Split('\t');  // (table header)
                var indexWholeBody /**/= columns.IndexOf(s => s.Contains("Whole Body"));
                var indexUrine     /**/= columns.IndexOf(s => s.Contains("Urine"));
                var indexFaeces    /**/= columns.IndexOf(s => s.Contains("Faeces"));
                var indexAtract    /**/= columns.IndexOf(s => s.Contains("Alimentary Tract"));
                var indexLungs     /**/= columns.IndexOf(s => s.Contains("Lungs"));
                var indexSkeleton  /**/= columns.IndexOf(s => s.Contains("Skeleton"));
                var indexLiver     /**/= columns.IndexOf(s => s.Contains("Liver"));
                var indexThyroid   /**/= columns.IndexOf(s => s.Contains("Thyroid"));

                // 残留放射能データが子孫核種のものである場合、その名前が
                // ヘッダに括弧書きされているためこれを取り出す。
                string GetRetentionNuclide(int index, string header)
                {
                    if (index == -1)
                        return null;
                    var m = Regex.Match(columns[index], Regex.Escape(header) + @" *\((?<nuc>[^ ]+)\)");
                    return m.Success ? m.Groups["nuc"].Value : nuclide;
                }
                target.NuclideWholeBody /**/= GetRetentionNuclide(indexWholeBody, /**/"Whole Body");
                target.NuclideUrine     /**/= GetRetentionNuclide(indexUrine,     /**/"Urine (24-hour sample)");
                target.NuclideFaeces    /**/= GetRetentionNuclide(indexFaeces,    /**/"Faeces (24-hour sample)");
                target.NuclideAtract    /**/= GetRetentionNuclide(indexAtract,    /**/"Alimentary Tract*");
                target.NuclideLungs     /**/= GetRetentionNuclide(indexLungs,     /**/"Lungs*");
                target.NuclideSkeleton  /**/= GetRetentionNuclide(indexSkeleton,  /**/"Skeleton*");
                target.NuclideLiver     /**/= GetRetentionNuclide(indexLiver,     /**/"Liver*");
                target.NuclideThyroid   /**/= GetRetentionNuclide(indexThyroid,   /**/"Thyroid*");

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
                        WholeBody /**/= GetValue(indexWholeBody),
                        Urine     /**/= GetValue(indexUrine),
                        Faeces    /**/= GetValue(indexFaeces),
                        Atract    /**/= GetValue(indexAtract),
                        Lungs     /**/= GetValue(indexLungs),
                        Skeleton  /**/= GetValue(indexSkeleton),
                        Liver     /**/= GetValue(indexLiver),
                        Thyroid   /**/= GetValue(indexThyroid),
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
