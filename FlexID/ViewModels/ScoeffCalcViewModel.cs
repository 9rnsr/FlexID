using FlexID.Calc;
using Prism.Mvvm;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using System;
using System.IO;
using System.Linq;
using System.Reactive.Disposables;
using System.Threading.Tasks;
using System.Windows;

namespace FlexID.ViewModels
{
    public class ScoeffCalcViewModel : BindableBase
    {
        private CompositeDisposable Disposables { get; } = new CompositeDisposable();

        public ReactivePropertySlim<bool> CalcMale { get; } = new ReactivePropertySlim<bool>(true);

        public ReactivePropertySlim<bool> CalcPchip { get; } = new ReactivePropertySlim<bool>(true);

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
            RunCommand = calcStatus.CanExecute.ToAsyncReactiveCommand().WithSubscribe(async () =>
            {
                try
                {
                    var sex = (CalcMale.Value ? Sex.Male : Sex.Female);

                    var interpolationMethod = CalcPchip.Value ? "PCHIP" : "線形補間";

                    // 1行＝計算対象の核種名としてファイルから読み出す。
                    var nuclides = File.ReadLines(NuclideListFilePath).ToArray();

                    await Run(sex, interpolationMethod, nuclides);

                    MessageBox.Show("Finish", "S-Coefficient", MessageBoxButton.OK);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }).AddTo(Disposables);
        }

        private static Task Run(Sex sex, string interpolationMethod, string[] nuclides)
        {
            var safdata = SAFDataReader.ReadSAF(sex);
            if (safdata is null)
                throw new Exception("There are multiple files of the same type.");

            var calcS = new CalcScoeff(safdata);

            calcS.InterpolationMethod = interpolationMethod;

            return Task.WhenAll(nuclides.Select(nuc => Task.Run(() =>
            {
                calcS.CalcS(nuc);

                var target = $@"{nuc}_{(sex == Sex.Male ? "AM" : "AF")}_S-Coefficient";

                var scoeffExcelFilePath = Path.Combine("out", target + ".xlsx");
                calcS.WriteCalcResult(scoeffExcelFilePath);

                var scoeffTextFilePath = Path.Combine("out", target + ".txt");
                calcS.WriteOutTotalResult(scoeffTextFilePath);
            })).ToArray());
        }
    }
}
