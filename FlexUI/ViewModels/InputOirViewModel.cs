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
        Task.Run(async () => await BuiltinTargets.AddRangeAsync(InputTarget.GetInputsOIR(@"inp\OIR")));
    }

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(Targets))]
    [NotifyCanExecuteChangedFor(nameof(RunCommand))]
    public partial bool IsBuiltin { get; set; } = true;

    public CheckableItemsView<InputTarget, InputTargetViewModel> Targets =>
        IsBuiltin ? BuiltinTargets : ExternalTargets;

    public CheckableItemsView<InputTarget, InputTargetViewModel> BuiltinTargets { get; }

    public CheckableItemsView<InputTarget, InputTargetViewModel> ExternalTargets { get; }

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

            foreach (var inputFile in selected)
            {
                if (ExternalTargets.FilteredItems.Any(
                    targetVM => Path.GetFullPath(targetVM.InputTarget.FilePath)
                             == Path.GetFullPath(inputFile)))
                {
                    continue;
                }

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

                ExternalTargets.Add(new InputTarget(inputFile, data), initialCheck: true);
            }
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
    private partial bool IsBusy { get; set; }

    private bool CanRun => !IsBusy && Targets.IsCheckedAny;

    [RelayCommand(CanExecute = nameof(CanRun))]
    private async Task Run()
    {
        try
        {
            WeakReferenceMessenger.Default.Send(new BusyState(true));

            // 各パラメータの入力確認
            if (OutputDirectory == "")
                throw new Exception("Please enter the Output File Path.");
            //if (string.IsNullOrWhiteSpace(Path.GetFileName(OutputFilePath)))
            //    throw new Exception("Please enter file name in the Output File Path");
            //if (SelectedNuclide is null)
            //    throw new Exception("Please select Nuclide.");
            //if (SelectedInput is null)
            //    throw new Exception("Please select Route of Intake.");
            if (ComputeTimeMeshFilePath == "")
                throw new Exception("Please enter the Computational Time Mesh file path.");
            if (OutputTimeMeshFilePath == "")
                throw new Exception("Please enter the Output Time Mesh file path.");
            if (CommitmentPeriod <= 0)
                throw new Exception("Please enter the positive Commitment Period.");
            if (SelectedCommitmentPeriodUnit is null)
                throw new Exception("Please select Commitment Period Unit.");

            //await RunAndView(SelectedInput.InputTarget);

            var message = "All tasks completed successfully.";
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

    private async Task RunAndView(InputTarget target)
    {
        // FlexID.Calcアセンブリがない場合はこのメソッドに入った直後に例外が発生する。

        var data = new InputDataReader_OIR(target.FilePath, calcProgeny: true).Read();

        var outputDir           /**/= OutputDirectory;
        var outputFile          /**/= target.Name;
        var computeTimeMeshPath /**/= ComputeTimeMeshFilePath;
        var outputTimeMeshPath  /**/= OutputTimeMeshFilePath;
        var commitmentPeriod    /**/= CommitmentPeriod + SelectedCommitmentPeriodUnit;

        if (!Path.IsPathFullyQualified(outputDir))
            outputDir = Path.Combine(AppResource.ProcessDir, outputDir);
        if (!Path.IsPathFullyQualified(computeTimeMeshPath))
            computeTimeMeshPath = Path.Combine(AppResource.BaseDir, computeTimeMeshPath);
        if (!Path.IsPathFullyQualified(outputTimeMeshPath))
            outputTimeMeshPath = Path.Combine(AppResource.BaseDir, outputTimeMeshPath);

        var main = new MainRoutine_OIR()
        {
            OutputDirectory     /**/= outputDir,
            OutputFileName      /**/= outputFile,
            ComputeTimeMeshPath /**/= computeTimeMeshPath,
            OutputTimeMeshPath  /**/= outputTimeMeshPath,
            CommitmentPeriod    /**/= commitmentPeriod,
        };

        await Task.Run(() => main.Main(data));

        // // ファイルパスを引数にして出力GUI実行
        // var p = Process.Start("FlexID.Viewer.exe", outputPath + "_Retention.out");
        // p.WaitForExit();
    }
}
