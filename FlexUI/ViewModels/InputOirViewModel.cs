using System.Collections.Concurrent;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using FlexID.Models;
using FlexID.Services;
using R3;

namespace FlexID.ViewModels;

public partial class InputOirViewModel : ViewModelBase
{
    /// <summary>
    /// コンストラクタ。
    /// </summary>
    public InputOirViewModel()
    {
        BuiltinTargets = new(target => new InputTargetViewModel(target));
        ExternalTargets = new(target => new InputTargetViewModel(target));

        CommitmentPeriod = 50;
        SelectedCommitmentPeriodUnit = CommitmentPeriodUnits[^1];
        ComputeTimeMeshFilePath = @"lib\TimeMesh\time.dat";
        OutputTimeMeshFilePath = @"lib\TimeMesh\out-time-OIR.dat";
        OutputDirectory = @"out\";

        WeakReferenceMessenger.Default.Register<BusyState>(this, (r, m) => IsBusy = m.Value);

        this.ObservePropertyChanged(vm => vm.Targets)
            .Select(targets => targets.ObservePropertyChanged(m => m.IsCheckedAny)).Switch()
            .Subscribe(_ => RunCommand.NotifyCanExecuteChanged())
            .AddTo(disposables);

        // OIR用のリソースとして配置されたインプットファイルの一覧を取得する。
        Task.Run(async () => await LoadBuiltinInputs());
    }

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(Targets))]
    [NotifyCanExecuteChangedFor(nameof(RunCommand))]
    public partial bool IsBuiltin { get; set; } = true;

    public CheckableItemsView<InputTarget, InputTargetViewModel> Targets =>
        IsBuiltin ? BuiltinTargets : ExternalTargets;

    public CheckableItemsView<InputTarget, InputTargetViewModel> BuiltinTargets { get; }

    public CheckableItemsView<InputTarget, InputTargetViewModel> ExternalTargets { get; }

    private static async IAsyncEnumerable<InputTarget> GetTargets(IEnumerable<string> inputs)
    {
        foreach (var inputFile in inputs)
        {
            var data = await Task.Run(() =>
            {
                try
                {
                    var reader = new InputDataReader_OIR(inputFile);
                    return reader.ReadRough();
                }
                catch
                {
                    // 読み込みに失敗した。
                    return null;
                }
            });
            if (data is null)
                continue;

            var parentNuclide = data.Nuclides.FirstOrDefault();
            if (parentNuclide is null)
                continue;

            yield return new InputTarget(inputFile, data);
        }
    }

    /// <summary>
    /// 組み込みのインプット群を列挙する。
    /// </summary>
    public async Task LoadBuiltinInputs()
    {
        IEnumerable<string> inputFiles;
        try
        {
            var inputDir = Path.Combine(AppResource.BaseDir, @"inp\OIR");
            inputFiles = Directory.EnumerateFiles(inputDir, "*.inp", SearchOption.AllDirectories);
        }
        catch (Exception e) when (e is IOException || e is SystemException)
        {
            // inputDirが存在しない場合など。
            return;
        }

        var inputs = GetTargets(inputFiles);
        await BuiltinTargets.AddRangeAsync(inputs);
    }

    [RelayCommand]
    private async Task AddInputFiles(string[] paths)
    {
        var selected = paths;
        if (selected is null)
        {
            var appId = App.Current.AppWindow!.Id;
            var picker = new Microsoft.Windows.Storage.Pickers.FileOpenPicker(appId)
            {
                //SuggestedFolder = Environment.CurrentDirectory, // Windows App SDK 2.0
            };

            var results = await picker.PickMultipleFilesAsync();

            selected = results?.Select(item => item.Path).ToArray();
        }
        if (selected is not null)
        {
            IsBuiltin = false;

            var inputs = GetTargets(selected.Where(inputFile =>
            {
                inputFile = Path.GetFullPath(inputFile);
                return ExternalTargets.FilteredItems
                    .All(targetVM => Path.GetFullPath(targetVM.InputTarget.FilePath) != inputFile);
            }));

            await ExternalTargets.AddRangeAsync(inputs, initialCheck: true);
        }
    }

    [RelayCommand]
    private async Task RemoveInputFiles(IReadOnlyList<object> inputVMs)
    {
        var items = inputVMs.OfType<InputTargetViewModel>()
            .Select(inputVM => inputVM.InputTarget);
        ExternalTargets.RemoveRange(items);
    }

    [ObservableProperty]
    public partial int CommitmentPeriod { get; set; }

    public IReadOnlyList<string> CommitmentPeriodUnits { get; } =
    [
        "days",
        "months",
        "years",
    ];

    [ObservableProperty]
    public partial string SelectedCommitmentPeriodUnit { get; set; }

    [ObservableProperty]
    public partial bool IsCompareWithOir { get; set; } = true;

    [ObservableProperty]
    public partial string ComputeTimeMeshFilePath { get; set; }

    [RelayCommand]
    private async Task SelectComputeTimeMeshFilePath(string[] paths)
    {
        var selected = paths?[0];
        if (selected is null)
        {
            var appId = App.Current.AppWindow!.Id;
            var picker = new Microsoft.Windows.Storage.Pickers.FileOpenPicker(appId)
            {
                //SuggestedFolder = Environment.CurrentDirectory, // Windows App SDK 2.0
            };

            var result = await picker.PickSingleFileAsync();

            selected = result?.Path;
        }
        if (selected is not null)
            ComputeTimeMeshFilePath = selected;
    }

    [ObservableProperty]
    public partial string OutputTimeMeshFilePath { get; set; }

    [RelayCommand]
    private async Task SelectOutputTimeMeshFilePath(string[] paths)
    {
        var selected = paths?[0];
        if (selected is null)
        {
            var appId = App.Current.AppWindow!.Id;
            var picker = new Microsoft.Windows.Storage.Pickers.FileOpenPicker(appId)
            {
                //SuggestedFolder = Environment.CurrentDirectory, // Windows App SDK 2.0
            };

            var result = await picker.PickSingleFileAsync();

            selected = result?.Path;
        }
        if (selected is not null)
            OutputTimeMeshFilePath = selected;
    }

    [ObservableProperty]
    public partial string OutputDirectory { get; set; }

    [RelayCommand]
    private async Task SelectOutputDirectory(string[] paths)
    {
        var selected = paths?[0];
        if (selected is null)
        {
            var appId = App.Current.AppWindow!.Id;
            var picker = new Microsoft.Windows.Storage.Pickers.FolderPicker(appId)
            {
                SuggestedStartLocation = Microsoft.Windows.Storage.Pickers.PickerLocationId.Unspecified,
                //SuggestedFolder = Environment.CurrentDirectory, // Windows App SDK 2.0
            };

            var result = await picker.PickSingleFolderAsync();

            selected = result?.Path;
        }
        if (selected is not null)
            OutputDirectory = selected;
    }

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(RunCommand))]
    public partial bool IsBusy { get; private set; }

    private bool CanRun => !IsBusy && Targets.IsCheckedAny;

    public ProgressViewModel ProgressViewModel { get; } = new();

    [RelayCommand(CanExecute = nameof(CanRun))]
    private async Task Run(CancellationToken cancellationToken)
    {
        try
        {
            WeakReferenceMessenger.Default.Send(new BusyState(true));

            // 各パラメータの入力確認
            if (OutputDirectory == "")
                throw new Exception("Please enter the Output File Path.");
            if (ComputeTimeMeshFilePath == "")
                throw new Exception("Please enter the Computational Time Mesh file path.");
            if (OutputTimeMeshFilePath == "")
                throw new Exception("Please enter the Output Time Mesh file path.");
            if (CommitmentPeriod <= 0)
                throw new Exception("Please enter the positive Commitment Period.");
            if (SelectedCommitmentPeriodUnit is null)
                throw new Exception("Please select Commitment Period Unit.");

            var outputDir           /**/= OutputDirectory;
            var computeTimeMeshPath /**/= ComputeTimeMeshFilePath;
            var outputTimeMeshPath  /**/= OutputTimeMeshFilePath;
            var commitmentPeriod    /**/= CommitmentPeriod + SelectedCommitmentPeriodUnit;

            if (!Path.IsPathFullyQualified(outputDir))
                outputDir = Path.Combine(AppResource.ProcessDir, outputDir);
            if (!Path.IsPathFullyQualified(computeTimeMeshPath))
                computeTimeMeshPath = Path.Combine(AppResource.BaseDir, computeTimeMeshPath);
            if (!Path.IsPathFullyQualified(outputTimeMeshPath))
                outputTimeMeshPath = Path.Combine(AppResource.BaseDir, outputTimeMeshPath);

            var targets = Targets.FilteredItems
                .Where(targetVM => targetVM.IsChecked)
                .Select(targetVM => new ProgressTargetViewModel(targetVM.InputTarget)).ToArray();

            var outputs = targets.Select(target => Path.Combine(outputDir, target.Name));

            var expectDir = Path.Combine(AppResource.BaseDir, @"expect");
            var expects = Directory.EnumerateFiles(expectDir, "*.dat", SearchOption.AllDirectories)
                .Select(path => KeyValuePair.Create(Path.GetFileNameWithoutExtension(path), path))
                .Where(expect => expect.Key.Contains('_'))  // Leave the expect retention files only (e.g. 'H-3_Injection.dat').
                .ToDictionary();

            var reports = new ConcurrentDictionary<ProgressTargetViewModel, ReportData>();

            var runner = new ParallelRunner<ProgressTargetViewModel>(targets);

            ProgressViewModel.Connect(runner);

            await runner.StartAsync((target, cancellationToken) =>
            {
                var data = new InputDataReader_OIR(target.InputFilePath, calcProgeny: true).Read();

                var main = new MainRoutine_OIR()
                {
                    OutputDirectory     /**/= outputDir,
                    OutputFileName      /**/= target.Name,
                    ComputeTimeMeshPath /**/= computeTimeMeshPath,
                    OutputTimeMeshPath  /**/= outputTimeMeshPath,
                    CommitmentPeriod    /**/= commitmentPeriod,
                };

                main.Main(data);

                // // ファイルパスを引数にして出力GUI実行
                // var p = Process.Start("FlexID.Viewer.exe", outputPath + "_Retention.out");
                // p.WaitForExit();

                var output = Path.Combine(outputDir, target.Name);

                (string Name, string Path)? compare = null;
                if (IsCompareWithOir && expects.TryGetValue(target.Name, out var expectPath))
                    compare = (Name: target.Name, Path: expectPath);

                var report = new ReportData(new FileInfo(outputDir), output, compare);

                report.LoadResult();
                if (report.HasErrors)
                    throw new Exception(string.Join("\n", report.Errors));

                ReportGenerator.WriteReport(report.ReportPath, report);

                reports.TryAdd(target, report);
            }, cancellationToken);

            //if (!errors)
            {
                var summaryFile = "summary.xlsx";
                summaryFile = Path.Combine(outputDir, summaryFile);

                var sortedReports = targets
                    .Select(target => reports.TryGetValue(target, out var report) ? report : null)
                    .OfType<ReportData>().ToArray();

                ReportGenerator.WriteSummary(summaryFile, sortedReports);
            }

            var failedCount = targets.Count(target => target.IsFailure);
            var message = failedCount == 0 ? "All tasks completed successfully."
                : $"{failedCount} / {ProgressViewModel.Targets.Count} tasks encounted errors during processing.";
            MessageService.Confirm("Caculation Finished", message);
        }
        catch (Exception ex)
        {
            var message = "Unexpected error has occurred during processing: " + ex.Message;
            MessageService.Error("Calculation Stopped", message);
        }
        finally
        {
            WeakReferenceMessenger.Default.Send(new BusyState(false));
        }
    }

    [RelayCommand]
    private void ProgressAbortOrClose()
    {
        if (IsBusy)
            RunCommand.Cancel();
        else
            ProgressViewModel.Disconnect();
    }
}
