namespace FlexID;

internal partial class Program
{
    static async Task<int> Main(string[] args)
    {
        return 0;
    }
#if false
    static async Task<int> Main(string[] args)
    {
        if (args.Length == 0)
            return Usage();

        // 処理結果の出力ディレクトリ。
        var outputDir = Path.Combine(AppResource.ProcessDir, "out");

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
                        patterns = [];
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

    private static int Usage()
    {
        var exeName = Path.GetFileNameWithoutExtension(Environment.ProcessPath);

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
        return 0;
    }

    /// <summary>
    /// 計算を実行する。
    /// </summary>
    /// <param name="target"></param>
    /// <param name="outputDir"></param>
    static void RunCalc(ReportTarget target, string outputDir)
    {
        var nuclide = target.Nuclide;

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

        var main = new MainRoutine_OIR()
        {
            OutputDirectory  /**/= outputDir,
            OutputFileName   /**/= target.Name,
            CalcTimeMeshPath /**/= cTimeMeshFile,
            OutTimeMeshPath  /**/= oTimeMeshFile,
            CommitmentPeriod /**/= commitmentPeriod,
        };

        main.Main(data);
    }
#endif
}
