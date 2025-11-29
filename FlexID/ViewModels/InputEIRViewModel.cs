using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Windows;
using FlexID.Calc;
using Microsoft.Win32;
using Prism.Mvvm;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;

namespace FlexID.ViewModels;

public class InputEIRViewModel : BindableBase
{
    private CompositeDisposable Disposables { get; } = [];

    public ReactivePropertySlim<string> OutputFilePath { get; } = new();

    public ObservableCollection<string> Nuclides { get; } = [];

    public ReactivePropertySlim<string> SelectedNuclide { get; } = new();

    public ReadOnlyReactivePropertySlim<bool> HasProgeny { get; }

    public ReactivePropertySlim<bool> CalcProgeny { get; } = new();

    public ObservableCollection<InputData> Inputs { get; } = [];

    public ReactivePropertySlim<InputData> SelectedInput { get; } = new();

    public ReactivePropertySlim<string> CalcTimeMeshFilePath { get; } = new();

    public ReactivePropertySlim<string> OutTimeMeshFilePath { get; } = new();

    public ReactivePropertySlim<string> CommitmentPeriod { get; } = new();

    public IReadOnlyList<string> CommitmentPeriodUnits { get; } =
    [
        "days",
        "months",
        "years",
    ];

    public ReactivePropertySlim<string> SelectedCommitmentPeriodUnit { get; }

    public IReadOnlyList<string> IntakeAges { get; } =
    [
        "3months old",
        "1years old",
        "5years old",
        "10years old",
        "15years old",
        "Adult",
    ];

    public ReactivePropertySlim<string> SelectedIntakeAge { get; }

    public ReactiveCommandSlim<string[]> SelectOutputFilePathCommand { get; }

    public ReactiveCommandSlim<string[]> SelectCalcTimeMeshFilePathCommand { get; }

    public ReactiveCommandSlim<string[]> SelectOutTimeMeshFilePathCommand { get; }

    public AsyncReactiveCommand RunCommand { get; }

    /// <summary>
    /// コンストラクタ。
    /// </summary>
    public InputEIRViewModel(CalcState calcStatus)
    {
        OutputFilePath.Value = @"out\";
        CalcTimeMeshFilePath.Value = @"lib\TimeMesh\time.dat";
        OutTimeMeshFilePath.Value = @"lib\TimeMesh\out-time.dat";
        CommitmentPeriod.Value = "50";

        // 子孫核種を持つインプットに対してのみ、子孫核種の計算を選択可能にする。
        HasProgeny = SelectedInput.Select(inp => inp?.HasProgeny ?? false)
            .ToReadOnlyReactivePropertySlim().AddTo(Disposables);

        // 子孫核種の有無が切り替わった場合に、これを子孫核種の計算有無に反映する。
        HasProgeny.Subscribe(
            x => CalcProgeny.Value = x).AddTo(Disposables);

        SelectedCommitmentPeriodUnit = new ReactivePropertySlim<string>(CommitmentPeriodUnits.Last());

        SelectedIntakeAge = new ReactivePropertySlim<string>(IntakeAges.First());

        SelectOutputFilePathCommand = new ReactiveCommandSlim<string[]>().WithSubscribe(paths =>
        {
            var selected = paths?[0];
            if (selected is null)
            {
                var dialog = new SaveFileDialog();
                dialog.InitialDirectory = Environment.CurrentDirectory;
                if (dialog.ShowDialog() == true)
                    selected = dialog.FileName;
            }
            OutputFilePath.Value = selected;

        }).AddTo(Disposables);

        SelectCalcTimeMeshFilePathCommand = new ReactiveCommandSlim<string[]>().WithSubscribe(paths =>
        {
            var selected = paths?[0];
            if (selected is null)
            {
                var dialog = new OpenFileDialog();
                dialog.InitialDirectory = Environment.CurrentDirectory;
                if (dialog.ShowDialog() == true)
                    selected = dialog.FileName;
            }
            CalcTimeMeshFilePath.Value = selected;

        }).AddTo(Disposables);

        SelectOutTimeMeshFilePathCommand = new ReactiveCommandSlim<string[]>().WithSubscribe(paths =>
        {
            var selected = paths?[0];
            if (selected is null)
            {
                var dialog = new OpenFileDialog();
                dialog.InitialDirectory = Environment.CurrentDirectory;
                if (dialog.ShowDialog() == true)
                    selected = dialog.FileName;
            }
            OutTimeMeshFilePath.Value = selected;

        }).AddTo(Disposables);

        Task.Run(() =>
        {
            // EIR用のインプットフォルダ配下に置かれたインプットファイルと、それらの核種の一覧を取得する。
            var inputDirPath = Path.Combine(AppContext.BaseDirectory, "inp", "EIR");

            var cacheNucInps = new Dictionary<string, List<InputData>>();

            foreach (var input in InputData.GetInputsEIR(inputDirPath))
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
                SelectedNuclide.Value = Nuclides.FirstOrDefault();

                // 核種に対応する、選択されたインプット群。
                SelectedNuclide.Subscribe(nuc =>
                {
                    var inputs = nuc is null ? [] : cacheNucInps[nuc];

                    // インプットの一覧を更新する。
                    Inputs.Clear();
                    Inputs.AddRange(inputs);

                    SelectedInput.Value = inputs.FirstOrDefault();
                }).AddTo(Disposables);
            });
        });

        var canRun = calcStatus.CanExecute.CombineLatest(SelectedInput, (b, inp) => b && inp != null);
        RunCommand = canRun.ToAsyncReactiveCommand().WithSubscribe(async () =>
        {
            try
            {
                CheckParam();
                await RunAndView(SelectedInput.Value);
            }
            catch (Exception error)
            {
                MessageBox.Show(error.Message);
            }
        }).AddTo(Disposables);
    }

    private async Task RunAndView(InputData selectedInput)
    {
        // FlexID.Calcアセンブリがない場合はこのメソッドに入った直後に例外が発生する。

        var dataList = new InputDataReader_EIR(selectedInput.FilePath, CalcProgeny.Value).Read();

        var main = new MainRoutine_EIR();
        main.OutputPath       /**/= OutputFilePath.Value;
        main.CalcTimeMeshPath /**/= CalcTimeMeshFilePath.Value;
        main.OutTimeMeshPath  /**/= OutTimeMeshFilePath.Value;
        main.CommitmentPeriod /**/= CommitmentPeriod.Value + SelectedCommitmentPeriodUnit.Value;
        main.ExposureAge      /**/= SelectedIntakeAge.Value;

        await Task.Run(() => main.Main(dataList));

        // ファイルパスを引数にして出力GUI実行
        var p = Process.Start("FlexID.Viewer.exe", main.OutputPath + "_Retention.out");
        p.WaitForExit();
    }

    /// <summary>
    /// 各パラメータの入力確認
    /// </summary>
    private void CheckParam()
    {
        if (OutputFilePath.Value == "")
        {
            throw new Exception("Please enter the Output File Path.");
        }
        if (SelectedNuclide.Value is null)
        {
            throw new Exception("Please select Nuclide.");
        }
        if (SelectedInput.Value is null)
        {
            throw new Exception("Please select Route of Intake.");
        }
        if (CalcTimeMeshFilePath.Value == "")
        {
            throw new Exception("Please enter the Calculation Time Mesh file path.");
        }
        if (OutTimeMeshFilePath.Value == "")
        {
            throw new Exception("Please enter the Output Time Mesh file path.");
        }
        if (!int.TryParse(CommitmentPeriod.Value, out _))
        {
            throw new Exception("Please enter Commitment Period.");
        }
        if (SelectedCommitmentPeriodUnit.Value is null)
        {
            throw new Exception("Please select Commitment Period.");
        }
        if (SelectedIntakeAge.Value is null)
        {
            throw new Exception("Please select Exposure Age.");
        }
    }
}
