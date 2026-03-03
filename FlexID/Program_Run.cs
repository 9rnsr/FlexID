using System.CommandLine;
using System.CommandLine.Parsing;
using System.Text.RegularExpressions;
using FlexID.Models;

namespace FlexID;

internal class Program_Run
{
    /// <summary>
    /// 'run' command.
    /// </summary>
    public static Command Command { get; }

    static readonly Option<FileInfo[]> InputFileOption = new("--input", "-i")
    {
        HelpName = "path",
        Description = "The input file for the calculation",
        AllowMultipleArgumentsPerToken = true,
        Arity = ArgumentArity.OneOrMore,
    };

    static readonly Option<Regex[]> InputPatternOption = new("--pattern", "-p")
    {
        HelpName = "regex",
        Description = "The regular-expression pattern to match built-in input files",
        AllowMultipleArgumentsPerToken = true,
        Arity = ArgumentArity.OneOrMore,
        CustomParser = result => [.. result.Tokens.Select(pattern => new Regex(pattern.Value))],
    };

    static readonly Option<string> OutputDirectoryOption = new("--output-dir", "-od")
    {
        HelpName = "path",
        Description = "The leading part of all output file paths",
    };

    static readonly Option<string[]> OutputFileOption = new("--output", "-o")
    {
        HelpName = "path",
        Description = "The trailing part of an output file path (no suffix and extension)",
        AllowMultipleArgumentsPerToken = true,
        Arity = ArgumentArity.OneOrMore,
    };

    static readonly Option<FileInfo> ComputeTimeMeshFileOption = new("--compute-mesh", "-mo")
    {
        HelpName = "path",
        Description = "The computational time mesh file",
    };

    static readonly Option<FileInfo> OutputTimieMeshFileOption = new("--output-mesh", "-mo")
    {
        HelpName = "path",
        Description = "The output time mesh file",
    };

    static readonly Option<string> CommitmentPeriodOption = new("--commitment-period", "-cp")
    {
        HelpName = "num+unit",
        Description = "The commitment period, unit=[days, months, years]",
    };

    static Program_Run()
    {
        InputFileOption.AcceptExistingOnly();
        OutputDirectoryOption.AcceptLegalFilePathsOnly();
        OutputFileOption.AcceptLegalFilePathsOnly();
        ComputeTimeMeshFileOption.AcceptExistingOnly();
        OutputTimieMeshFileOption.AcceptExistingOnly();

        Command = new("run", "Run input and get results as output.")
        {
            InputFileOption,
            InputPatternOption,
            OutputDirectoryOption,
            OutputFileOption,
            ComputeTimeMeshFileOption,
            OutputTimieMeshFileOption,
            CommitmentPeriodOption,
        };

        Command.Validators.Add(Validate);
        Command.SetAction(Execute);
    }

    private static void Validate(CommandResult result)
    {
        var options = result.Children.OfType<OptionResult>().Select(or => or.Option);
        if (options.Contains(InputFileOption) && options.Contains(InputPatternOption))
        {
            result.AddError($"Options {InputFileOption.Name} and {InputPatternOption.Name} cannot be used together");
            return;
        }

        var inputsCount = result.GetResult(InputFileOption)?.Tokens.Count ?? 0;
        var outputsCount = result.GetResult(OutputFileOption)?.Tokens.Count ?? 0;
        if (inputsCount != 0)
        {
            if (outputsCount != 0 && outputsCount != inputsCount)
            {
                result.AddError($"{InputFileOption.Name} and {OutputFileOption.Name} should be equal in number");
                return;
            }
        }
        else
        {
            if (outputsCount != 0)
            {
                result.AddError($"{InputPatternOption.Name} and {OutputFileOption.Name} cannot be used together");
            }
        }
    }

    private static async Task<int> Execute(ParseResult parseResult)
    {
        var inputs = parseResult.GetValue(InputFileOption) ?? [];
        var patterns = parseResult.GetValue(InputPatternOption) ?? [];

        var outputDir = Path.GetFullPath(parseResult.GetValue(OutputDirectoryOption) ?? Environment.CurrentDirectory);
        var outputs = (parseResult.GetValue(OutputFileOption) ?? []).AsEnumerable();

        var computeTimeMeshPath = parseResult.GetValue(ComputeTimeMeshFileOption) ??
                new FileInfo(Path.Combine(AppResource.BaseDir, @"lib\TimeMesh\time.dat"));
        var outputTimeMeshPath = parseResult.GetValue(OutputTimieMeshFileOption) ??
                new FileInfo(Path.Combine(AppResource.BaseDir, @"lib\TimeMesh\out-time-OIR.dat"));
        var commitmentPeriod = parseResult.GetValue(CommitmentPeriodOption) ?? "50years";

        // TODO: customize with options.
        var outputDose = true;
        var outputDoseRate = true;
        var outputRetention = true;
        var outputCumulative = true;

        if (patterns.Length != 0)
        {
            bool IsMatch(string path) =>
                patterns.Any(pattern => pattern.IsMatch(Path.GetFileNameWithoutExtension(path)));
            try
            {
                var dirPath = Path.Combine(AppResource.BaseDir, @"inp\OIR");
                inputs = Directory.EnumerateFiles(dirPath, "*.inp", SearchOption.AllDirectories)
                    .Where(IsMatch).Select(path => new FileInfo(path)).ToArray();
            }
            catch (Exception e) when (e is IOException || e is SystemException)
            {
                // dirPathが存在しない場合など。
                throw new Exception("Failed to enumerate built-in inputs");
            }

            outputDoseRate = false;
            outputCumulative = false;
        }

        if (outputs.Count() == 0)
            outputs = inputs.Select(input => Path.GetFileNameWithoutExtension(input.Name));
        outputs = outputs.Select(output => Path.Combine(outputDir, output));

        var cts = new CancellationTokenSource();

        // UIとそのほかのプロセスのために1コアだけ残して他を使用する。
        var parallelCount = Math.Max(1, Environment.ProcessorCount - 1);
        var semaphore = new SemaphoreSlim(parallelCount);
        var presenter = new ProgressPresenter(inputs.Length, cts.Token);

        var errors = false;

        await Task.WhenAll(inputs.Zip(outputs).Select((pair, i) =>
        {
            // 同時に起動・実行されるタスク数を制限する。
            semaphore.Wait();

            var (input, output) = pair;

            return Task.Run(() => RunSingle(input, output))
                       .ContinueWith(_ => semaphore.Release());
        }));

        await presenter.WaitForExit();
        Console.WriteLine();

        void RunSingle(FileInfo input, string output)
        {
            var outDir = Path.GetDirectoryName(output)!;
            var outName = Path.GetFileNameWithoutExtension(output);

            try
            {
                presenter.Start(input.Name);

                var reader = new InputDataReader_OIR(input.FullName);
                var data = reader.Read();
                data.OutputDose = outputDose;
                data.OutputDoseRate = outputDoseRate;
                data.OutputRetention = outputRetention;
                data.OutputCumulative = outputCumulative;

                var target = new InputTarget(input.FullName, data);

                var main = new MainRoutine_OIR()
                {
                    OutputDirectory     /**/= outDir,
                    OutputFileName      /**/= outName,
                    ComputeTimeMeshPath /**/= computeTimeMeshPath.FullName,
                    OutputTimeMeshPath  /**/= outputTimeMeshPath.FullName,
                    CommitmentPeriod    /**/= commitmentPeriod,
                };

                main.Main(data);

                presenter.Stop(input.Name, $"\x1B[36mOK\x1B[0m");
            }
            catch (Exception exception)
            {
                // 何らかのエラーが発生した場合。
                errors = true;
                presenter.Stop(input.Name, $"\x1B[31mNG\x1B[0m", exception.Message);
            }

            // 1ケースの計算が終了するごとにGCを呼ばないと、並列計算数がだんだん減ってしまう。
            GC.Collect();
        }

        return errors ? 1 : 0;
    }
}
