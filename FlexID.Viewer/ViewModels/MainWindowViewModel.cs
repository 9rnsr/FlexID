using System.Collections.ObjectModel;
using System.Reactive.Linq;
using FlexID.Calc;
using Microsoft.Win32;
using OxyPlot;
using Prism.Mvvm;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;

namespace FlexID.Viewer.ViewModels
{
    public class MainWindowViewModel : BindableBase
    {
        private readonly Model model;

        // 入力GUIから受け取るファイルパス
        public static string OutPath = "";

        #region 出力ファイル情報

        public ReactivePropertySlim<string> OutputFilePath { get; }

        public ReactiveCommandSlim<string[]> SelectOutputFilePathCommand { get; }

        public ObservableCollection<OutputType> OutputTypes => this.model.OutputTypes;

        public ReactivePropertySlim<OutputType?> SelectedOutputType { get; } = new ReactivePropertySlim<OutputType?>();

        public ReadOnlyReactivePropertySlim<string> Title { get; }

        public ObservableCollection<string> Nuclides => this.model.Nuclides;

        public ReactivePropertySlim<string> SelectedNuclide { get; } = new ReactivePropertySlim<string>();

        #endregion

        #region コンター表示

        /// <summary>
        /// DataGridに表示するコンパートメント毎の計算値。
        /// </summary>
        public ReadOnlyReactiveCollection<CalcData> DataValues { get; }

        /// <summary>
        /// モデル図に表示するための、統一臓器名とその合算された数値。
        /// </summary>
        public ReadOnlyReactivePropertySlim<Dictionary<string, double>> OrganValues { get; }

        /// <summary>
        /// モデル図に表示するための、統一臓器名とその色情報。
        /// </summary>
        public ReadOnlyReactivePropertySlim<Dictionary<string, string>> OrganColors { get; }

        /// <summary>
        /// コンターの上限値。
        /// </summary>
        public ReactivePropertySlim<double> ContourMax { get; }

        /// <summary>
        /// コンターの下限値。
        /// </summary>
        public ReactivePropertySlim<double> ContourMin { get; }

        /// <summary>
        /// コンターに表示される単位。
        /// </summary>
        public ReadOnlyReactivePropertySlim<string> ContourUnit { get; }

        #endregion

        #region 出力タイムステップスライダー

        public ReadOnlyReactivePropertySlim<IReadOnlyList<double>> TimeSteps { get; }
        public ReadOnlyReactivePropertySlim<double> StartTimeStep { get; }
        public ReadOnlyReactivePropertySlim<double> EndTimeStep { get; }

        /// <summary>
        /// 現在スライダーが示している時間。
        /// </summary>
        public ReactiveProperty<double> CurrentTimeStep { get; }

        /// <summary>
        /// アニメーション再生状態を示す。<see langword="true"/>：再生中、<see langword="false"/>：停止中。
        /// </summary>
        public ReadOnlyReactiveProperty<bool> IsPlaying { get; }

        public ReactiveCommand PlayCommand { get; } = new ReactiveCommand();
        public ReactiveCommand NextStepCommand { get; } = new ReactiveCommand();
        public ReactiveCommand PreviousStepCommand { get; } = new ReactiveCommand();

        #endregion

        #region グラフ表示

        public ReadOnlyReactiveCollection<RegionData> Regions { get; }

        public PlotModel PlotModel => this.model.PlotModel;

        public ReactivePropertySlim<bool> IsLogAxisX { get; }
        public ReactivePropertySlim<bool> IsLogAxisY { get; }

        #endregion

        /// <summary>
        /// コンストラクタ。
        /// </summary>
        /// <param name="model"></param>
        public MainWindowViewModel(Model model)
        {
            this.model = model;

            #region 出力ファイル情報

            OutputFilePath = new ReactivePropertySlim<string>("");

            SelectOutputFilePathCommand = new ReactiveCommandSlim<string[]>().WithSubscribe(paths =>
            {
                var selected = paths?[0] ?? SelectOutputFile();
                if (selected is string path)
                    OutputFilePath.Value = path;
            });

            OutputFilePath
                .Select(path => this.model.SetTypes(path))
                .ObserveOnUIDispatcher()
                .Subscribe(type => SelectedOutputType.Value = type);

            SelectedOutputType.Subscribe(type =>
            {
                if (type is OutputType t)
                {
                    var nuc = SelectedNuclide.Value;
                    var tstep = CurrentTimeStep.Value;

                    this.model.SetOutput(t);

                    if (!this.model.Nuclides.Contains(nuc))
                        nuc = this.model.Nuclides.FirstOrDefault();
                    SelectedNuclide.Value = nuc;
                    CurrentTimeStep.Value = tstep;
                }
            });

            Title = this.model.ObserveProperty(x => x.Title).ToReadOnlyReactivePropertySlim();

            SelectedNuclide.Subscribe(nuc => this.model.SetNuclide(nuc));

            #endregion

            #region コンター表示

            DataValues = this.model.DataValues.ToReadOnlyReactiveCollection();

            OrganValues = this.model.ObserveProperty(x => x.OrganValues).ToReadOnlyReactivePropertySlim();
            OrganColors = this.model.ObserveProperty(x => x.OrganColors).ToReadOnlyReactivePropertySlim();

            ContourMax = this.model.ToReactivePropertySlimAsSynchronized(x => x.ContourMax);
            ContourMin = this.model.ToReactivePropertySlimAsSynchronized(x => x.ContourMin);
            ContourUnit = this.model.ObserveProperty(x => x.ContourUnit).ToReadOnlyReactivePropertySlim();

            ContourMax.Subscribe(_ => this.model.SetColors());
            ContourMin.Subscribe(_ => this.model.SetColors());

            #endregion

            #region 出力タイムステップスライダー

            TimeSteps = this.model.ObserveProperty(x => x.TimeSteps).ToReadOnlyReactivePropertySlim();
            StartTimeStep = TimeSteps.Select(ts => ts.Count == 0 ? 0 : ts[0]).ToReadOnlyReactivePropertySlim();
            EndTimeStep = TimeSteps.Select(ts => ts.Count == 0 ? 0 : ts[ts.Count - 1]).ToReadOnlyReactivePropertySlim();

            CurrentTimeStep = this.model.ToReactivePropertyAsSynchronized(x => x.CurrentTimeStep);

            CurrentTimeStep.Subscribe(_ => this.model.SetValues());

            IsPlaying = this.model.ObserveProperty(x => x.IsPlaying).ToReadOnlyReactiveProperty();

            PlayCommand.Subscribe(() => this.model.Playing());
            NextStepCommand.Subscribe(() => this.model.NextStep());
            PreviousStepCommand.Subscribe(() => this.model.PreviousStep());

            #endregion

            #region グラフ表示

            Regions = this.model.Regions.ToReadOnlyReactiveCollection();

            IsLogAxisX = this.model.ToReactivePropertySlimAsSynchronized(m => m.IsLogAxisX);
            IsLogAxisY = this.model.ToReactivePropertySlimAsSynchronized(m => m.IsLogAxisY);

            #endregion

            if (!string.IsNullOrEmpty(OutPath))
                OutputFilePath.Value = OutPath;
        }

        /// <summary>
        /// ファイルダイアログ操作
        /// </summary>
        private string SelectOutputFile()
        {
            var dialog = new OpenFileDialog();
            dialog.ShowDialog();

            if (dialog.FileName != "")
                return dialog.FileName;

            return null;
        }
    }
}
