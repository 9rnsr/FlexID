using FlexID.Calc;
using Microsoft.Win32;
using Prism.Mvvm;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace FlexID.ViewModels
{
    public class InputEIRViewModel : BindableBase
    {
        private CompositeDisposable Disposables { get; } = new CompositeDisposable();

        public ReactivePropertySlim<string> OutputFilePath { get; } = new ReactivePropertySlim<string>();

        public ObservableCollection<string> Nuclides { get; } = new ObservableCollection<string>();

        public ReactivePropertySlim<string> SelectedNuclide { get; } = new ReactivePropertySlim<string>();

        public ReadOnlyReactivePropertySlim<bool> HasProgeny { get; }

        public ReactivePropertySlim<bool> CalcProgeny { get; } = new ReactivePropertySlim<bool>();

        public ObservableCollection<string> ModelTypes { get; } = new ObservableCollection<string>();

        public ReactivePropertySlim<string> SelectedModelType { get; } = new ReactivePropertySlim<string>();

        public ReactivePropertySlim<string> CalcTimeMeshFilePath { get; } = new ReactivePropertySlim<string>();

        public ReactivePropertySlim<string> OutTimeMeshFilePath { get; } = new ReactivePropertySlim<string>();

        public ReactivePropertySlim<string> CommitmentPeriod { get; } = new ReactivePropertySlim<string>();

        public IReadOnlyList<string> CommitmentPeriodUnits { get; } = new List<string>
        {
            "days",
            "months",
            "years",
        }.AsReadOnly();

        public ReactivePropertySlim<string> SelectedCommitmentPeriodUnit { get; }

        public IReadOnlyList<string> IntakeAges { get; } = new List<string>
        {
            "3months old",
            "1years old",
            "5years old",
            "10years old",
            "15years old",
            "Adult",
        }.AsReadOnly();

        public ReactivePropertySlim<string> SelectedIntakeAge { get; }

        public ReactiveCommandSlim SelectOutputFilePathCommand { get; }

        public ReactiveCommandSlim SelectCalcTimeMeshFilePathCommand { get; }

        public ReactiveCommandSlim SelectOutTimeMeshFilePathCommand { get; }

        public AsyncReactiveCommand RunCommand { get; }

        /// <summary>
        /// コンストラクタ。
        /// </summary>
        public InputEIRViewModel(CalcState calcStatus)
        {
            OutputFilePath.Value = @"out\";
            CalcTimeMeshFilePath.Value = @"lib\time.dat";
            OutTimeMeshFilePath.Value = @"lib\out-time.dat";
            CommitmentPeriod.Value = "50";

            const string InputDirPath = @"inp\EIR";
            var cacheNucInps = new Dictionary<string, List<InputData>>();

            // EIR用のインプットフォルダにある核種の一覧を取得する。
            try
            {
                foreach (var x in Directory.EnumerateDirectories(InputDirPath))
                {
                    var nuc = Path.GetFileName(x);
                    Nuclides.Add(nuc);
                }
            }
            catch (Exception e) when (e is IOException || e is SystemException)
            {
                // InputDirPathがない場合はNuclidesが空になる。
            }
            SelectedNuclide.Value = Nuclides.FirstOrDefault();

            // 核種に対応する、選択されたインプット群。
            var selectedInputs = SelectedNuclide.Select(nuc =>
            {
                if (nuc is null)
                    return new List<InputData>();
                if (!cacheNucInps.TryGetValue(nuc, out var inputs))
                {
                    inputs = InputData.GetInputsEIR(InputDirPath, nuc);
                    cacheNucInps.Add(nuc, inputs);
                }
                return inputs;
            });

            selectedInputs.Subscribe(inputs =>
            {
                // インプットの種別(被ばく経路/化学形態)の一覧を更新する。
                var types = inputs.Select(inp => inp.ModelType);
                ModelTypes.Clear();
                ModelTypes.AddRange(types);

                SelectedModelType.Value = types.FirstOrDefault();
            }).AddTo(Disposables);

            // 核種とインプットの種別(被ばく経路/化学形態)に対応する、選択されたインプット情報。
            var selectedInput = selectedInputs.CombineLatest(SelectedModelType,
                (inputs, type) => inputs?.FirstOrDefault(inp => inp.ModelType == type))
                .ToReadOnlyReactivePropertySlim().AddTo(Disposables);

            // 子孫核種を持つインプットに対してのみ、子孫核種の計算を選択可能にする。
            HasProgeny = selectedInput.Select(inp => inp?.HasProgeny ?? false)
                .ToReadOnlyReactivePropertySlim().AddTo(Disposables);

            // 子孫核種の有無が切り替わった場合に、これを子孫核種の計算有無に反映する。
            HasProgeny.Subscribe(
                x => CalcProgeny.Value = x).AddTo(Disposables);

            SelectedCommitmentPeriodUnit = new ReactivePropertySlim<string>(CommitmentPeriodUnits.Last());

            SelectedIntakeAge = new ReactivePropertySlim<string>(IntakeAges.First());

            SelectOutputFilePathCommand = new ReactiveCommandSlim().WithSubscribe(() =>
            {
                var dialog = new SaveFileDialog();
                dialog.InitialDirectory = Environment.CurrentDirectory;
                if (dialog.ShowDialog() == true)
                    OutputFilePath.Value = dialog.FileName;

            }).AddTo(Disposables);

            SelectCalcTimeMeshFilePathCommand = new ReactiveCommandSlim().WithSubscribe(() =>
            {
                var dialog = new OpenFileDialog();
                dialog.InitialDirectory = Environment.CurrentDirectory;
                if (dialog.ShowDialog() == true)
                    CalcTimeMeshFilePath.Value = dialog.FileName;

            }).AddTo(Disposables);

            SelectOutTimeMeshFilePathCommand = new ReactiveCommandSlim().WithSubscribe(() =>
            {
                var dialog = new OpenFileDialog();
                dialog.InitialDirectory = Environment.CurrentDirectory;
                if (dialog.ShowDialog() == true)
                    OutTimeMeshFilePath.Value = dialog.FileName;

            }).AddTo(Disposables);

            RunCommand = calcStatus.CanExecute.ToAsyncReactiveCommand().WithSubscribe(async () =>
            {
                try
                {
                    CheckParam();
                    await RunAndView(selectedInput.Value);
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

            var main = new MainRoutine_EIR();
            main.OutputPath = OutputFilePath.Value;
            main.InputPath = selectedInput.FilePath;
            main.CalcTimeMeshPath = CalcTimeMeshFilePath.Value;
            main.OutTimeMeshPath = OutTimeMeshFilePath.Value;
            main.CommitmentPeriod = CommitmentPeriod.Value + SelectedCommitmentPeriodUnit.Value;
            main.CalcProgeny = CalcProgeny.Value;
            main.ExposureAge = SelectedIntakeAge.Value;

            await Task.Run(() => main.Main());

            // ファイルパスを引数にして出力GUI実行
            var p = Process.Start("FlexID.Viewer.exe", main.OutputPath);
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
            if (SelectedModelType.Value is null)
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
}
