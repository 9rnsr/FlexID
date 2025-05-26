namespace ResultChecker;

using System.Collections.Concurrent;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using FlexID.Calc;

internal partial class Program
{
    /// <summary>
    /// OIRデータが格納されたディレクトリ。
    /// </summary>
    static readonly string ExpectDir = "Expect";

    /// <summary>
    /// 処理結果の出力ディレクトリ。
    /// </summary>
    static string OutputDir;

    /// <summary>
    /// 処理結果の出力ファイル名。
    /// </summary>
    static string OutputFileName;

    /// <summary>
    /// 結果取得するための計算を実行するかどうか。
    /// </summary>
    static bool RunCalculation;

    static async Task<int> Main(string[] args)
    {
        List<Regex> patterns = null;

        OutputDir = "out";
        OutputFileName = "summary.xlsx";
        RunCalculation = true;

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

                if (arg is ['@', .. var file])
                {
                    var head = args.AsSpan(0, i);
                    var tail = args.AsSpan(i + 1);
                    var newArgs = File.ReadAllText(file)
                        .Split(Array.Empty<char>(), StringSplitOptions.RemoveEmptyEntries);
                    args = [.. head, .. newArgs, .. tail];
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
                    OutputDir = Path.GetDirectoryName(output);
                    OutputFileName = Path.GetFileName(output);
                    continue;
                }
                if (GetOption(out var outDir, "-od", "--output-dir"))
                {
                    OutputDir = outDir;
                    continue;
                }
                if (GetOption(out var outFile, "-of", "--output-file"))
                {
                    OutputFileName = outFile;
                    continue;
                }

                if (IsOption("--run"))
                {
                    RunCalculation = true;
                    continue;
                }
                if (IsOption("--no-run"))
                {
                    RunCalculation = false;
                    continue;
                }

                if (arg is ['-', _] or ['-', '-', ..])
                {
                    Console.Error.WriteLine($"error: unknown option '{arg}'");
                    Usage();
                    return -1;
                }

            Lpattern:
                var pattern = arg;
                try
                {
                    patterns ??= [];
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

        (string target, string inputPath)[] targets;
        if (RunCalculation)
        {
            // パターンに合致するインプットを計算対象として収集する。
            targets = [.. GetInputs()
                .Select(inputPath => (target: Path.GetFileNameWithoutExtension(inputPath), inputPath))
                .Where(x => patterns?.Any(pattern => pattern.IsMatch(x.target)) ?? true)];
        }
        else
        {
            // 出力ディレクトリにあるログファイルから、処理対象を収集する。
            targets = [.. Directory.EnumerateFiles(OutputDir, "*.log")
                .Select(logFile => (target: Path.GetFileNameWithoutExtension(logFile), inputPath: ""))
                .Where(x => patterns?.Any(pattern => pattern.IsMatch(x.target)) ?? true)];
        }
        if (targets.Length == 0)
        {
            Console.Error.WriteLine($"error: there is no targets.");
            return -1;
        }

        Directory.CreateDirectory(OutputDir);

        var results = new ConcurrentBag<Result>();

        // 1コアは画面更新とそのほかのプロセスのために占有しない。
        var maxParallel = Math.Max(1, Environment.ProcessorCount - 1);
        var lcts = new LimitedConcurrencyLevelTaskScheduler(maxParallel);
        var factory = new TaskFactory(lcts);

        var presenter = new ProgressPresenter(targets.Length);

        if (RunCalculation)
        {
            // 並列処理で計算を実施する。
            Task.WaitAll([.. targets.Select(x => factory.StartNew(() => Process(x.target, x.inputPath)))]);
        }
        else
        {
            // 同期処理で出力の取得と進捗表示を実施する。
            presenter.Update();
            foreach (var (target, inputPath) in targets)
            {
                Process(target, inputPath);
                presenter.Update();
            }
            presenter.Update();
        }

        void Process(string target, string inputPath)
        {
            try
            {
                presenter.Start(target);

                var result = RunCalculation
                    ? CalcAndSummary(target, inputPath, OutputDir)
                    : GetResult(target);
                results.Add(result);

                presenter.Stop(target, $"\x1B[36mOK\x1B[0m");
            }
            catch (Exception ex)
            {
                // 何らかのエラーが発生した場合。
                results.Add(new Result { Target = target, HasErrors = true });

                presenter.Stop(target, $"\x1B[31mNG\x1B[0m", ex.Message);
            }

            // 1ケースの計算が終了するごとにGCを呼ばないと、並列計算数がだんだん減ってしまう。
            if (RunCalculation)
                GC.Collect();
        }

        await presenter.WaitForExit();

        var outputFilePath = Path.Combine(OutputDir, OutputFileName);
        var sortedResults = results.OrderBy(r => r.Target).ToArray();

        Console.Write($"\nGenerate {OutputFileName} ...");
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
    /// 計算および比較の結果。
    /// </summary>
    struct Result
    {
        public string Target;

        public bool HasErrors;

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

    static Result CalcAndSummary(string target, string inputPath, string outputDir)
    {
        var outputPath = Path.Combine(outputDir, target);

        // 計算時間メッシュはFlexID.Calcに付属のものを使用する。
        var cTimeMeshFile = @"lib\TimeMesh\time.dat";

        // 出力時間メッシュは、OIRの残留放射能データと同じになるように設定する。
        var oTimeMeshFile = @"lib\TimeMesh\out-time-OIR.dat";

        var commitmentPeriod = "50years";

        var data = new InputDataReader_OIR(inputPath).Read();
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

        return GetResult(target);
    }

    static Result GetResult(string target)
    {
        var result = new Result() { Target = target };

        // 50年の預託期間における、各出力時間メッシュにおける数値の比較。
        // 要約として、期待値に対する下振れ率と上振れ率の最大値を算出する。
        var expectActs = GetExpectRetentions(target, out var mat, out var retentionNuc);
        var actualActs = GetResultRetentions(target, retentionNuc);
        var fractionsWholeBody /**/= (min: double.PositiveInfinity, max: double.NegativeInfinity);
        var fractionsUrine     /**/= (min: double.PositiveInfinity, max: double.NegativeInfinity);
        var fractionsFaeces    /**/= (min: double.PositiveInfinity, max: double.NegativeInfinity);
        var fractionsAtract    /**/= (min: double.PositiveInfinity, max: double.NegativeInfinity);
        var fractionsLungs     /**/= (min: double.PositiveInfinity, max: double.NegativeInfinity);
        var fractionsSkeleton  /**/= (min: double.PositiveInfinity, max: double.NegativeInfinity);
        var fractionsLiver     /**/= (min: double.PositiveInfinity, max: double.NegativeInfinity);
        var fractionsThyroid   /**/= (min: double.PositiveInfinity, max: double.NegativeInfinity);

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
        result.ExpectDose = GetExpectDoses(target, mat);
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
    /// <param name="mat"></param>
    /// <returns></returns>
    /// <exception cref="InvalidDataException"></exception>
    static Dose GetExpectDoses(string target, string mat)
    {
        var nuclide = target.Split('_')[0];
        var filePath = Path.Combine(ExpectDir, $"{nuclide}.dat");
        var (routeOfIntake, _, _) = DecomposeMaterial(mat);

        using var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);
        using var reader = new StreamReader(stream, Encoding.UTF8);

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

                var equivalentDosesMale = columns.Skip(3).Select(double.Parse).ToArray();

                line = reader.ReadLine();
                columns = line.Split('\t');
                var equivalentDosesFemale = columns.Skip(3).Select(double.Parse).ToArray();

                return new Dose
                {
                    EffectiveDose = effectiveDose,
                    EquivalentDosesMale = equivalentDosesMale,
                    EquivalentDosesFemale = equivalentDosesFemale,
                };
            }

            reader.ReadLine();
        }

        throw new InvalidDataException($"Missing data for Material '{mat}' and Route of Intake '{routeOfIntake}'.");
    }

    /// <summary>
    /// FlexIDが出力した各出力時間メッシュにおける線量の出力ファイル
    /// *_Dose.outから、預託実効線量と預託等価線量の数値を読み込む。
    /// </summary>
    /// <param name="target"></param>
    /// <returns></returns>
    static Dose GetResultDoses(string target)
    {
        var nuclide = target.Split('_')[0];
        var filePath = Path.Combine(OutputDir, $"{target}_Dose.out");

        // 組織加重係数データを読み込む。
        var (ts, ws) = InputDataReaderBase.ReadTissueWeights(Path.Combine("lib", "OIR", "wT.txt"));

        using var reader = new OutputDataReader(filePath);

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
            EquivalentDosesMale = [.. equivalentDoses.Select(d => d.Male)],
            EquivalentDosesFemale = [.. equivalentDoses.Select(d => d.Female)],
        };
    }

    /// <summary>
    /// 残留放射能の取得結果。
    /// </summary>
    struct Retention
    {
        public double StartTime;
        public double EndTime;

        public double WholeBody;
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
    /// <param name="resultNuc"></param>
    /// <returns></returns>
    static List<Retention> GetResultRetentions(string target, string resultNuc)
    {
        var nuclide = target.Split('_')[0];
        var filePath = Path.Combine(OutputDir, $"{target}_Retention.out");

        var retentions = new List<Retention>();

        using var reader = new OutputDataReader(filePath);

        var data = reader.Read();
        var result = data.Blocks[0];

        if (resultNuc != nuclide)
        {
            // 子孫核種の結果を読み出す。
            result = data.Blocks.Where(n => n.Header == resultNuc).FirstOrDefault();
            if (result is null)
                throw new InvalidDataException($"Missing retention data of progeny nuclide '{resultNuc}'.");
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
        var resultAtract    /**/= GetCompartmentData("AlimentaryTract*");
        var resultLungs     /**/= GetCompartmentData("Lungs*");
        var resultSkeleton  /**/= GetCompartmentData("Skeleton*");
        var resultLiver     /**/= GetCompartmentData("Liver*");
        var resultThyroid   /**/= GetCompartmentData("Thyroid*");

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
                Atract    /**/= GetValue(resultAtract),
                Lungs     /**/= GetValue(resultLungs),
                Skeleton  /**/= GetValue(resultSkeleton),
                Liver     /**/= GetValue(resultLiver),
                Thyroid   /**/= GetValue(resultThyroid),
            });
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
    /// <param name="retentionNuc">残留放射能データが対応する核種名</param>
    /// <returns></returns>
    /// <exception cref="InvalidDataException"></exception>
    static List<Retention> GetExpectRetentions(string target, out string mat, out string retentionNuc)
    {
        var nuclide = target.Split('_')[0];
        var filePath = Path.Combine(ExpectDir, $"{target}.dat");

        mat = "";

        var retentions = new List<Retention>();

        using var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);
        using var reader = new StreamReader(stream, Encoding.UTF8);

        string[] columns;

        // ファイルの内容が対象核種のものか確認する。
        columns = reader.ReadLine().Split('\t');
        if (columns[1] != nuclide)
            throw new InvalidDataException();

        // 対象の摂取形態。
        var routeOfIntake = reader.ReadLine().Split('\t')[1];

        // 対象の化学形態。
        var chemicalForm = reader.ReadLine().Split('\t')[1];

        // 対象の粒子サイズ。
        var particleSize = reader.ReadLine().Split('\t')[1];

        // 預託線量の期待値を引くための文字列を組み立てる。
        mat = $"{routeOfIntake}, {chemicalForm}";
        if (particleSize != "-")
            mat += $", {particleSize} µm";

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
        var headerWholeBody = columns[indexWholeBody];
        var m = Regex.Match(headerWholeBody, @"Whole Body *\((?<nuc>[^ ]+)\)");
        if (m.Success)
            retentionNuc = m.Groups["nuc"].Value;
        else
            retentionNuc = nuclide;

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
                Atract    /**/= GetValue(indexAtract),
                Lungs     /**/= GetValue(indexLungs),
                Skeleton  /**/= GetValue(indexSkeleton),
                Liver     /**/= GetValue(indexLiver),
                Thyroid   /**/= GetValue(indexThyroid),
            });

            startTime = endTime;
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
