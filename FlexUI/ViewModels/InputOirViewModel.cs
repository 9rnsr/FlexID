using System.Collections.ObjectModel;
using System.Diagnostics;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using FlexID.Models;

namespace FlexID.ViewModels;

public partial class InputOirViewModel : ObservableObject
{
    /// <summary>
    /// コンストラクタ。
    /// </summary>
    public InputOirViewModel()
    {
        OutputFilePath = @"out\";
        ComputeTimeMeshFilePath = @"lib\TimeMesh\time.dat";
        OutputTimeMeshFilePath = @"lib\TimeMesh\out-time-OIR.dat";
        CommitmentPeriod = "50";

        SelectedCommitmentPeriodUnit = CommitmentPeriodUnits.Last();

        WeakReferenceMessenger.Default.Register<BusyState>(this, (r, m) => IsBusy = m.Value);

#if false
        Task.Run(async () =>
        {
            // OIR用のインプットフォルダ配下に置かれたインプットファイルと、それらの核種の一覧を取得する。
            await foreach (var input in InputTarget.GetInputsOIR(@"inp\OIR"))
            {
                var nuc = input.Nuclide;
                if (!cacheNucInps.TryGetValue(nuc, out var inputs))
                {
                    inputs = [];
                    cacheNucInps.Add(nuc, inputs);
                }
                inputs.Add(new InputTargetViewModel(input));
            }

            Application.Current.Dispatcher.Invoke(() =>
            {
                Nuclides.AddRange(cacheNucInps.Keys.OrderBy(nuc => nuc));
                SelectedNuclide = Nuclides.FirstOrDefault();
            });
        });
#endif
    }

    public ObservableCollection<string> Nuclides { get; } = [];

    [ObservableProperty]
    public partial string SelectedNuclide { get; set; }

    private readonly Dictionary<string, List<InputTargetViewModel>> cacheNucInps = [];

    public ObservableCollection<InputTargetViewModel> Inputs { get; } = [];

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(RunCommand))]
    public partial InputTargetViewModel SelectedInput { get; set; }

    partial void OnSelectedInputChanged(InputTargetViewModel value)
    {
        // 子孫核種の有無が切り替わった場合に、これを子孫核種の計算有無に反映する。
        HasProgeny = value?.HasProgeny ?? false;
        CalcProgeny = HasProgeny;
    }

    [ObservableProperty]
    public partial bool HasProgeny { get; private set; }

    [ObservableProperty]
    public partial bool CalcProgeny { get; set; }

    // 核種に対応する、選択されたインプット群。
    partial void OnSelectedNuclideChanged(string value)
    {
        var inputs = value is null ? [] : cacheNucInps[value];

        // インプットの一覧を更新する。
        Inputs.Replace(inputs);

        SelectedInput = inputs.FirstOrDefault();
    }

    [ObservableProperty]
    public partial string ComputeTimeMeshFilePath { get; set; }

    [RelayCommand]
    private void SelectComputeTimeMeshFilePath(string[] paths)
    {
        var selected = paths?[0];
        if (selected is null)
        {
            //var dialog = new OpenFileDialog();
            //dialog.InitialDirectory = Environment.CurrentDirectory;
            //if (dialog.ShowDialog() == true)
            //    selected = dialog.FileName;
        }
        ComputeTimeMeshFilePath = selected;
    }

    [ObservableProperty]
    public partial string OutputTimeMeshFilePath { get; set; }

    [RelayCommand]
    private void SelectOutputTimeMeshFilePath(string[] paths)
    {
        var selected = paths?[0];
        if (selected is null)
        {
            //var dialog = new OpenFileDialog();
            //dialog.InitialDirectory = Environment.CurrentDirectory;
            //if (dialog.ShowDialog() == true)
            //    selected = dialog.FileName;
        }
        OutputTimeMeshFilePath = selected;
    }

    [ObservableProperty]
    public partial string CommitmentPeriod { get; set; }

    public IReadOnlyList<string> CommitmentPeriodUnits { get; } =
    [
        "days",
        "months",
        "years",
    ];

    [ObservableProperty]
    public partial string SelectedCommitmentPeriodUnit { get; set; }

    [ObservableProperty]
    public partial string OutputFilePath { get; set; }

    [RelayCommand]
    private void SelectOutputFilePath(string[] paths)
    {
        var selected = paths?[0];
        if (selected is null)
        {
            //var dialog = new SaveFileDialog();
            //dialog.InitialDirectory = Environment.CurrentDirectory;
            //if (dialog.ShowDialog() == true)
            //    selected = dialog.FileName;
        }
        OutputFilePath = selected;
    }

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(RunCommand))]
    private partial bool IsBusy { get; set; }

    private bool CanRun => !IsBusy && SelectedInput is not null;

    [RelayCommand(CanExecute = nameof(CanRun))]
    private async Task Run()
    {
        try
        {
            WeakReferenceMessenger.Default.Send(new BusyState(true));

            // 各パラメータの入力確認
            if (OutputFilePath == "")
                throw new Exception("Please enter the Output File Path.");
            if (string.IsNullOrWhiteSpace(Path.GetFileName(OutputFilePath)))
                throw new Exception("Please enter file name in the Output File Path");
            if (SelectedNuclide is null)
                throw new Exception("Please select Nuclide.");
            if (SelectedInput is null)
                throw new Exception("Please select Route of Intake.");
            if (ComputeTimeMeshFilePath == "")
                throw new Exception("Please enter the Computational Time Mesh file path.");
            if (OutputTimeMeshFilePath == "")
                throw new Exception("Please enter the Output Time Mesh file path.");
            if (!int.TryParse(CommitmentPeriod, out _))
                throw new Exception("Please enter Commitment Period.");
            if (SelectedCommitmentPeriodUnit is null)
                throw new Exception("Please select Commitment Period.");

            await RunAndView();
        }
        catch (Exception error)
        {
            //MessageBox.Show(error.Message);
        }
        finally
        {
            WeakReferenceMessenger.Default.Send(new BusyState(false));
        }
    }

    private async Task RunAndView()
    {
        // FlexID.Calcアセンブリがない場合はこのメソッドに入った直後に例外が発生する。

        var data = new InputDataReader_OIR(SelectedInput.InputTarget.FilePath, CalcProgeny).Read();

        var outputPath          /**/= OutputFilePath;
        var computeTimeMeshPath /**/= ComputeTimeMeshFilePath;
        var outputTimeMeshPath  /**/= OutputTimeMeshFilePath;
        var commitmentPeriod    /**/= CommitmentPeriod + SelectedCommitmentPeriodUnit;

        if (!Path.IsPathFullyQualified(outputPath))
            outputPath = Path.Combine(AppResource.ProcessDir, outputPath);
        if (!Path.IsPathFullyQualified(computeTimeMeshPath))
            computeTimeMeshPath = Path.Combine(AppResource.BaseDir, computeTimeMeshPath);
        if (!Path.IsPathFullyQualified(outputTimeMeshPath))
            outputTimeMeshPath = Path.Combine(AppResource.BaseDir, outputTimeMeshPath);

        var outputDir = Path.GetDirectoryName(outputPath);
        var outputFile = Path.GetFileName(outputPath);

        var main = new MainRoutine_OIR()
        {
            OutputDirectory     /**/= outputDir,
            OutputFileName      /**/= outputFile,
            ComputeTimeMeshPath /**/= computeTimeMeshPath,
            OutputTimeMeshPath  /**/= outputTimeMeshPath,
            CommitmentPeriod    /**/= commitmentPeriod,
        };

        await Task.Run(() => main.Main(data));

        // ファイルパスを引数にして出力GUI実行
        var p = Process.Start("FlexID.Viewer.exe", outputPath + "_Retention.out");
        p.WaitForExit();
    }
}
