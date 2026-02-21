using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using FlexID.Models;
using FlexID.Services;
using R3;

namespace FlexID.ViewModels;

public partial class InputEirViewModel : ViewModelBase
{
    /// <summary>
    /// コンストラクタ。
    /// </summary>
    public InputEirViewModel()
    {
        BuiltinTargets = new(target => new InputTargetViewModel(target));
        ExternalTargets = new(target => new InputTargetViewModel(target));

        CommitmentPeriod = 50;
        SelectedCommitmentPeriodUnit = CommitmentPeriodUnits[^1];
        SelectedIntakeAge = IntakeAges[0];
        ComputeTimeMeshFilePath = @"lib\TimeMesh\time.dat";
        OutputTimeMeshFilePath = @"lib\TimeMesh\out-time.dat";
        OutputDirectory = @"out\";

        WeakReferenceMessenger.Default.Register<BusyState>(this, (r, m) => IsBusy = m.Value);

        this.ObservePropertyChanged(vm => vm.Targets)
            .Select(targets => targets.ObservePropertyChanged(m => m.IsCheckedAny)).Switch()
            .Subscribe(_ => RunCommand.NotifyCanExecuteChanged())
            .AddTo(disposables);

        // EIR用のリソースとして配置されたインプットファイルの一覧を取得する。
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
                    var reader = new InputDataReader_EIR(inputFile);
                    return reader.Read().FirstOrDefault();
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
            var inputDir = Path.Combine(AppResource.BaseDir, @"inp\EIR");
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

    public IReadOnlyList<string> IntakeAges { get; } =
    [
        "3months old",
        "1years old",
        "5years old",
        "10years old",
        "15years old",
        "Adult",
    ];

    [ObservableProperty]
    public partial string SelectedIntakeAge { get; set; }

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
            if (ComputeTimeMeshFilePath == "")
                throw new Exception("Please enter the Computational Time Mesh file path.");
            if (OutputTimeMeshFilePath == "")
                throw new Exception("Please enter the Output Time Mesh file path.");
            if (CommitmentPeriod <= 0)
                throw new Exception("Please enter the positive Commitment Period.");
            if (SelectedCommitmentPeriodUnit is null)
                throw new Exception("Please select Commitment Period Unit.");
            if (SelectedIntakeAge is null)
                throw new Exception("Please select Exposure Age.");

            var targets = Targets.FilteredItems
                .Where(targetVM => targetVM.IsChecked)
                .Select(targetVM => targetVM.InputTarget).ToArray();

            var cts = new CancellationTokenSource();

            var runner = new ParallelRunner<InputTarget>(targets);

            await runner.StartAsync(RunSingle(), cts.Token);

            MessageService.Confirm("Caculation finished.");
        }
        catch (Exception ex)
        {
            MessageService.Error(ex.Message);
        }
        finally
        {
            WeakReferenceMessenger.Default.Send(new BusyState(false));
        }
    }

    private Action<InputTarget, CancellationToken> RunSingle()
    {
        var outputDir           /**/= OutputDirectory;
        var computeTimeMeshPath /**/= ComputeTimeMeshFilePath;
        var outputTimeMeshPath  /**/= OutputTimeMeshFilePath;
        var commitmentPeriod    /**/= CommitmentPeriod + SelectedCommitmentPeriodUnit;
        var intakeAge           /**/= SelectedIntakeAge;

        if (!Path.IsPathFullyQualified(outputDir))
            outputDir = Path.Combine(AppResource.ProcessDir, outputDir);
        if (!Path.IsPathFullyQualified(computeTimeMeshPath))
            computeTimeMeshPath = Path.Combine(AppResource.BaseDir, computeTimeMeshPath);
        if (!Path.IsPathFullyQualified(outputTimeMeshPath))
            outputTimeMeshPath = Path.Combine(AppResource.BaseDir, outputTimeMeshPath);

        return (target, cancellationToken) =>
        {
            var dataList = new InputDataReader_EIR(target.FilePath, calcProgeny: true).Read();

            var main = new MainRoutine_EIR()
            {
                OutputDirectory     /**/= outputDir,
                OutputFileName      /**/= target.Name,
                ComputeTimeMeshPath /**/= computeTimeMeshPath,
                OutputTimeMeshPath  /**/= outputTimeMeshPath,
                CommitmentPeriod    /**/= commitmentPeriod,
                ExposureAge         /**/= intakeAge,
            };

            main.Main(dataList);

            // // ファイルパスを引数にして出力GUI実行
            // var p = Process.Start("FlexID.Viewer.exe", outputPath + "_Retention.out");
            // p.WaitForExit();
        };
    }
}
