using System.Collections.ObjectModel;
using System.IO;
using System.Reactive.Disposables;
using System.Windows;
using FlexID.Calc;
using Microsoft.Win32;
using Prism.Mvvm;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;

namespace FlexID.ViewModels
{
    public class NuclideItem : BindableBase
    {
        public string Nuclide
        {
            get => _nuclide;
            set => SetProperty(ref _nuclide, value);
        }
        private string _nuclide;

        public bool IsChecked
        {
            get => _isChecked;
            set => SetProperty(ref _isChecked, value);
        }
        private bool _isChecked;
    }

    public class ScoeffCalcViewModel : BindableBase
    {
        private CompositeDisposable Disposables { get; } = new CompositeDisposable();

        public ReactivePropertySlim<string> OutputFilePath { get; } = new ReactivePropertySlim<string>();

        public ReactivePropertySlim<bool> CalcMale { get; } = new ReactivePropertySlim<bool>(true);

        public ReactivePropertySlim<bool> CalcFemale { get; } = new ReactivePropertySlim<bool>(true);

        public ReactivePropertySlim<bool> CalcPchip { get; } = new ReactivePropertySlim<bool>(true);

        public ReactivePropertySlim<bool> IdacDoseCompatible { get; } = new ReactivePropertySlim<bool>(false);

        public ObservableCollection<NuclideItem> Nuclides { get; } = new ObservableCollection<NuclideItem>();

        public ReactiveCommandSlim SelectOutputFilePathCommand { get; }

        public AsyncReactiveCommand RunCommand { get; }

        /// <summary>
        /// 計算対象となる核種群の名前を格納したファイルのパス
        /// </summary>
        private const string NuclideListFilePath = @"lib\NuclideList.txt";

        /// <summary>
        /// コンストラクタ。
        /// </summary>
        public ScoeffCalcViewModel(CalcState calcStatus)
        {
            OutputFilePath.Value = @"out\";

            Nuclides.AddRange(SAFDataReader.ReadRadNuclides().Select(nuc => new NuclideItem { Nuclide = nuc }));

            SelectOutputFilePathCommand = new ReactiveCommandSlim().WithSubscribe(() =>
            {
                var dialog = new SaveFileDialog();
                dialog.InitialDirectory = Environment.CurrentDirectory;
                if (dialog.ShowDialog() == true)
                    OutputFilePath.Value = dialog.FileName;

            }).AddTo(Disposables);

            RunCommand = calcStatus.CanExecute.ToAsyncReactiveCommand().WithSubscribe(async () =>
            {
                try
                {
                    var outPath = OutputFilePath.Value;
                    Directory.CreateDirectory(outPath);

                    var calcAM = CalcMale.Value;
                    var calcAF = CalcFemale.Value;
                    if (!calcAM && !calcAF)
                        throw new Exception("Select target male and/or female.");

                    var nuclides = Nuclides.Where(ni => ni.IsChecked).Select(ni => ni.Nuclide).ToArray();
                    if (!nuclides.Any())
                        throw new Exception("No nuclides selected.");

                    var interpolationMethod = CalcPchip.Value ? "PCHIP" : "線形補間";

                    var isIdacDoseCompatible = IdacDoseCompatible.Value;

                    await Task.WhenAll(
                        calcAM ? Run(Sex.Male, interpolationMethod, nuclides, outPath, isIdacDoseCompatible) : Task.CompletedTask,
                        calcAF ? Run(Sex.Female, interpolationMethod, nuclides, outPath, isIdacDoseCompatible) : Task.CompletedTask);

                    MessageBox.Show("Finish", "S-Coefficient", MessageBoxButton.OK);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }).AddTo(Disposables);
        }

        private static Task Run(Sex sex, string interpolationMethod, string[] nuclides, string outPath, bool isIdacDoseCompatible)
        {
            var safdata = SAFDataReader.ReadSAF(sex);
            if (safdata is null)
                throw new Exception("There are multiple files of the same type.");

            return Task.WhenAll(nuclides.Select(nuc => Task.Run(() =>
            {
                var calcS = new CalcScoeff(safdata);

                calcS.InterpolationMethod = interpolationMethod;

                calcS.CalcS(nuc);

                var target = $@"{nuc}_{(sex == Sex.Male ? "AM" : "AF")}";

                if (isIdacDoseCompatible)
                {
                    var scoeffFilePath = Path.Combine(outPath, target + ".csv");
                    calcS.WriteOutIdacDoseCompatibleResult(scoeffFilePath, sex);
                }
                else
                {
                    var scoeffFilePath = Path.Combine(outPath, target + ".txt");
                    calcS.WriteOutTotalResult(scoeffFilePath);
                }

            })).ToArray());
        }
    }
}
