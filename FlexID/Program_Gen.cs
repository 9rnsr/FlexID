using System.CommandLine;
using System.CommandLine.Parsing;
using Sprache;

namespace FlexID;

internal partial class Program_Gen
{
    /// <summary>
    /// 'gen' command.
    /// </summary>
    public static Command Command { get; }

    static readonly Option<FileInfo> OutputDirectoryOption = new("--output-dir", "-od")
    {
        HelpName = "path",
        Description = "The directory that contains all output files",
    };

    static readonly Option<string[]> OutputFileOption = new("--output", "-o")
    {
        HelpName = "path",
        Description = "The output files to be processed (one of the set of *.out files, or *.log file)",
        AllowMultipleArgumentsPerToken = true,
        Arity = ArgumentArity.OneOrMore,
    };

    static readonly Option<bool> CompareAllOption = new("--compare-all", "-C")
    {
        Description = "Compare with the built-in expect data for all output files by using the deduced names",
    };

    static readonly Option<string[]> CompareNameOption = new("--compare", "-c")
    {
        HelpName = "name",
        Description = "The built-in expected output data name being compared with the corresponding output file",
        AllowMultipleArgumentsPerToken = true,
        Arity = ArgumentArity.OneOrMore,
    };

    const string DefaultSummaryFileName = "summary.xlsx";

    static readonly Option<string> SummaryFileOption = new("--summary", "-s")
    {
        HelpName = "path",
        Description = $"The summary file being created for the comparison results [default: {DefaultSummaryFileName}]",
        Arity = ArgumentArity.ZeroOrOne,
    };

    static Program_Gen()
    {
        OutputDirectoryOption.AcceptExistingOnly();

        Command = new("gen", "Generate additional files from the outputs")
        {
            OutputDirectoryOption,
            OutputFileOption,
            CompareAllOption,
            CompareNameOption,
            SummaryFileOption,
        };

        Command.Validators.Add(Validate);
        Command.SetAction(Execute);
    }

    private static void Validate(CommandResult result)
    {
        var options = result.Children.OfType<OptionResult>().Select(or => or.Option);

        // -odと-oは少なくともどちらか一方の指定が必要。
        if (!options.Contains(OutputDirectoryOption) && !options.Contains(OutputFileOption))
        {
            result.AddError($"Options {OutputDirectoryOption.Name} or {OutputFileOption.Name} should be specified at least");
            return;
        }

        // -Cと-cは同時に使用できない。
        if (options.Contains(CompareAllOption) && options.Contains(CompareNameOption))
        {
            result.AddError($"Options {CompareAllOption.Name} and {CompareNameOption.Name} cannot be used together");
            return;
        }

        // -c <name>を使用する場合、これらは-o <path>と同数存在する必要がある。
        var outputsCount = result.GetResult(OutputFileOption)?.Tokens.Count ?? 0;
        var comparesCount = result.GetResult(CompareNameOption)?.Tokens.Count ?? 0;
        if (comparesCount != 0 && outputsCount != comparesCount)
        {
            result.AddError($"Option {CompareNameOption.Name} should be used with {OutputFileOption.Name} and equal in number");
            return;
        }
    }

    private static async Task<int> Execute(ParseResult parseResult)
    {
        var currentDir = new FileInfo(Environment.CurrentDirectory);
        var outputDir = parseResult.GetValue(OutputDirectoryOption);
        var outputs = GetOutputs(parseResult.GetValue(OutputFileOption), outputDir ?? currentDir);

        var compareNames =
                parseResult.GetValue(CompareNameOption) is [_, ..] names ? names :
                parseResult.GetValue(CompareAllOption) ? [.. outputs.Select(output => Path.GetFileNameWithoutExtension(output))] :
                null;

        (string Name, string Path)?[]? compares = null;
        if (compareNames is not null)
        {
            var expectDir = Path.Combine(AppResource.BaseDir, @"expect");
            var expects = Directory.EnumerateFiles(expectDir, "*.dat", SearchOption.AllDirectories)
                .Select(path => KeyValuePair.Create(Path.GetFileNameWithoutExtension(path), path))
                .Where(expect => expect.Key.Contains('_'))  // Leave the expect retention files only (e.g. 'H-3_Injection.dat').
                .ToDictionary();

            compares = [.. compareNames.Select(name =>
            {
                (string Name, string Path)? compare = null;
                if (expects.TryGetValue(name, out var expectPath))
                    compare = (Name: name, Path: expectPath);
                return compare;
            })];
        }

        var reports = outputs.Select((output, i) => new ReportData(outputDir, output, compares?[i])).ToArray();

        var cts = new CancellationTokenSource();

        var errors = false;
        var runner = new ParallelRunner<ReportData>(reports);
        var presenter = new ProgressPresenter(outputs.Length, cts.Token);
        runner.StartItem += report => presenter.Start(report.OutputName);
        runner.SuccessItem += report => presenter.Stop(report.OutputName, $"\x1B[36mOK\x1B[0m");
        runner.FailureItem += (report, exception) =>
        {
            errors = true;
            presenter.Stop(report.OutputName, $"\x1B[31mNG\x1B[0m", exception.Message);
        };

        await runner.StartAsync((report, cancellationToken) =>
        {
            report.LoadResult();
            if (report.HasErrors)
                throw new Exception(string.Join("\n", report.Errors));

            ReportGenerator.WriteReport(report.ReportPath, report);
        }, cts.Token);

        await presenter.WaitForExit();
        Console.WriteLine();

        if (!errors)
        {
            var summaryFile = parseResult.GetValue(SummaryFileOption) ?? DefaultSummaryFileName;
            summaryFile = Path.Combine((outputDir ?? currentDir).FullName, summaryFile);

            var sortedReports = reports.OrderBy(r => r.SortKey).ToArray();

            Console.Write($"Generate {summaryFile} ...");
            ReportGenerator.WriteSummary(summaryFile, sortedReports);
            Console.WriteLine($"done");
        }

        return errors ? 1 : 0;
    }

    static readonly string[] suffixes =
    [
        ".log",
        "_Retention.out",
        "_Cumulative.out",
        "_Dose.out",
        "_DoseRate.out",
    ];

    /// <summary>
    /// -oで指定された出力ファイル、もしくは-odオプションで指定されたディレクトリに含まれるファイルの各フルパスから、
    /// 拡張子と接尾辞を除いたものを列挙する。
    /// </summary>
    /// <param name="outputFiles"></param>
    /// <param name="outputDir"></param>
    /// <returns></returns>
    private static string[] GetOutputs(string[]? outputFiles, FileInfo outputDir)
    {
        // ワイルドカードを含む場合は、カレントディレクトリを基準にしてこれを展開する。
        static IEnumerable<string> ExpandWildCards(string path) =>
            path.Contains('*') || path.Contains('?')
                ? Directory.EnumerateFiles(Environment.CurrentDirectory, path, SearchOption.TopDirectoryOnly)
                : [Path.GetFullPath(path)];

        IEnumerable<string> EnumerateCandidates()
        {
            var outputs = outputFiles?.Length > 0 ?
                    outputFiles.SelectMany(ExpandWildCards) :
                    Directory.EnumerateFiles(outputDir.FullName, "*.log");
            foreach (var path in outputs.Where(File.Exists))
            {
                var parent = Path.GetDirectoryName(path)!;
                var filename = Path.GetFileName(path);

                var suffix = suffixes.FirstOrDefault(suffix => filename.EndsWith(suffix, StringComparison.OrdinalIgnoreCase));
                if (suffix is not null)
                    yield return Path.Combine(parent, filename[..^suffix.Length]);
                else
                    continue;
            }
        }

        return EnumerateCandidates().Distinct(StringComparer.OrdinalIgnoreCase).ToArray();
    }
}
