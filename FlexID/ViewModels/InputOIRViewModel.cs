using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using FlexID.Calc;
using Microsoft.Win32;

namespace FlexID.ViewModels;

public partial class InputOIRViewModel : ObservableObject
{
    /// <summary>
    /// コンストラクタ。
    /// </summary>
    public InputOIRViewModel()
    {
        OutputFilePath = @"out\";
        CalcTimeMeshFilePath = @"lib\TimeMesh\time.dat";
        OutTimeMeshFilePath = @"lib\TimeMesh\out-time-OIR.dat";
        CommitmentPeriod = "50";

        SelectedCommitmentPeriodUnit = CommitmentPeriodUnits.Last();

        Task.Run(() =>
        {
            // OIR用のインプットフォルダ配下に置かれたインプットファイルと、それらの核種の一覧を取得する。
            var inputDirPath = Path.Combine(AppContext.BaseDirectory, "inp", "OIR");

            foreach (var input in InputData.GetInputsOIR(inputDirPath))
            {
                var nuc = input.Nuclide;
                if (!cacheNucInps.TryGetValue(nuc, out var inputs))
                {
                    inputs = [];
                    cacheNucInps.Add(nuc, inputs);
                }
                inputs.Add(input);
            }

            Application.Current.Dispatcher.Invoke(() =>
            {
                Nuclides.AddRange(cacheNucInps.Keys.OrderBy(nuc => nuc));
                SelectedNuclide = Nuclides.FirstOrDefault();
            });
        });

        WeakReferenceMessenger.Default.Register<BusyState>(this, (r, m) => IsBusy = m.Value);
    }

    public ObservableCollection<string> Nuclides { get; } = [];

    [ObservableProperty]
    public partial string SelectedNuclide { get; set; }

    private readonly Dictionary<string, List<InputData>> cacheNucInps = [];

    public ObservableCollection<InputData> Inputs { get; } = [];

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(RunCommand))]
    public partial InputData SelectedInput { get; set; }

    partial void OnSelectedInputChanged(InputData value)
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
    public partial string CalcTimeMeshFilePath { get; set; }

    [RelayCommand]
    private void SelectCalcTimeMeshFilePath(string[] paths)
    {
        var selected = paths?[0];
        if (selected is null)
        {
            var dialog = new OpenFileDialog();
            dialog.InitialDirectory = Environment.CurrentDirectory;
            if (dialog.ShowDialog() == true)
                selected = dialog.FileName;
        }
        CalcTimeMeshFilePath = selected;
    }

    [ObservableProperty]
    public partial string OutTimeMeshFilePath { get; set; }

    [RelayCommand]
    private void SelectOutTimeMeshFilePath(string[] paths)
    {
        var selected = paths?[0];
        if (selected is null)
        {
            var dialog = new OpenFileDialog();
            dialog.InitialDirectory = Environment.CurrentDirectory;
            if (dialog.ShowDialog() == true)
                selected = dialog.FileName;
        }
        OutTimeMeshFilePath = selected;
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
            var dialog = new SaveFileDialog();
            dialog.InitialDirectory = Environment.CurrentDirectory;
            if (dialog.ShowDialog() == true)
                selected = dialog.FileName;
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
            if (SelectedNuclide is null)
                throw new Exception("Please select Nuclide.");
            if (SelectedInput is null)
                throw new Exception("Please select Route of Intake.");
            if (CalcTimeMeshFilePath == "")
                throw new Exception("Please enter the Calculation Time Mesh file path.");
            if (OutTimeMeshFilePath == "")
                throw new Exception("Please enter the Output Time Mesh file path.");
            if (!int.TryParse(CommitmentPeriod, out _))
                throw new Exception("Please enter Commitment Period.");
            if (SelectedCommitmentPeriodUnit is null)
                throw new Exception("Please select Commitment Period.");

            await RunAndView();
        }
        catch (Exception error)
        {
            MessageBox.Show(error.Message);
        }
        finally
        {
            WeakReferenceMessenger.Default.Send(new BusyState(false));
        }
    }

    private async Task RunAndView()
    {
        // FlexID.Calcアセンブリがない場合はこのメソッドに入った直後に例外が発生する。

        var data = new InputDataReader_OIR(SelectedInput.FilePath, CalcProgeny).Read();

        var main = new MainRoutine_OIR();
        main.OutputPath       /**/= OutputFilePath;
        main.CalcTimeMeshPath /**/= CalcTimeMeshFilePath;
        main.OutTimeMeshPath  /**/= OutTimeMeshFilePath;
        main.CommitmentPeriod /**/= CommitmentPeriod + SelectedCommitmentPeriodUnit;

        await Task.Run(() => main.Main(data));

        // ファイルパスを引数にして出力GUI実行
        var p = Process.Start("FlexID.Viewer.exe", main.OutputPath + "_Retention.out");
        p.WaitForExit();
    }
}
