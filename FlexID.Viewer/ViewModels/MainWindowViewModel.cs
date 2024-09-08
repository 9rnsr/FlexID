using Microsoft.Win32;
using OxyPlot;
using Prism.Mvvm;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using System;
using System.Collections.Generic;
using System.Reactive.Linq;

namespace FlexID.Viewer.ViewModels
{
    public class MainWindowViewModel : BindableBase
    {
        private readonly Model model;

        // 入力GUIから受け取るファイルパス
        public static string OutPath = "";

        #region コンター表示

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

        // データグリッドに表示する生の計算値
        public ReadOnlyReactiveCollection<CalcData> DataValues { get; }

        // 臓器ごとの色情報
        public ReactiveProperty<Dictionary<string, string>> OrganColors { get; } = new ReactiveProperty<Dictionary<string, string>>();

        // モデル図に表示するために合算された値
        public ReactiveProperty<Dictionary<string, double>> OrganValues { get; set; } = new ReactiveProperty<Dictionary<string, double>>();

        #region 出力ファイル情報

        public ReadOnlyReactiveCollection<string> ComboList { get; }
        public ReactiveProperty<string> SelectCombo { get; } = new ReactiveProperty<string>();

        public ReactivePropertySlim<string> ResultFilePath { get; }

        public ReactiveCommandSlim<string[]> SelectResultFilePathCommand { get; }

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

        public ReactiveCommand PlotRun { get; } = new ReactiveCommand();

        public ReactiveProperty<string> RadioNuclide { get; }
        public ReactiveProperty<string> IntakeRoute { get; }
        public ReadOnlyReactiveProperty<string> GraphLabel { get; }
        public ReadOnlyReactiveCollection<GraphList> GraphList { get; }

        #region グラフ表示

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
            this.model.ResultFilePath = OutPath;

            DataValues = this.model._dataValues.ToReadOnlyReactiveCollection();
            ComboList = this.model._comboList.ToReadOnlyReactiveCollection();
            GraphList = this.model._graphList.ToReadOnlyReactiveCollection();

            OrganValues = this.model.ObserveProperty(x => x.OrganValues).ToReactiveProperty();
            OrganColors = this.model.ObserveProperty(x => x.OrganColors).ToReactiveProperty();
            RadioNuclide = this.model.ObserveProperty(x => x.RadioNuclide).ToReactiveProperty();
            IntakeRoute = this.model.ObserveProperty(x => x.IntakeRoute).ToReactiveProperty();

            GraphLabel = this.model.ObserveProperty(x => x.GraphLabel).ToReadOnlyReactiveProperty();

            #region 出力ファイル情報

            ResultFilePath = this.model.ToReactivePropertySlimAsSynchronized(x => x.ResultFilePath);

            SelectResultFilePathCommand = new ReactiveCommandSlim<string[]>().WithSubscribe(paths =>
            {
                var selected = paths?[0] ?? SelectResultFile();
                if (selected is string path)
                    ResultFilePath.Value = path;
            });

            // テキストボックスの内容を変更後0.5秒後にイベント発生
            ResultFilePath.Throttle(TimeSpan.FromSeconds(0.5)).Subscribe(_ =>
            {
                try
                {
                    this.model.Reader();
                }
                catch { }
            });

            #endregion

            #region コンター表示

            ContourMax = this.model.ToReactivePropertySlimAsSynchronized(x => x.ContourMax);
            ContourMin = this.model.ToReactivePropertySlimAsSynchronized(x => x.ContourMin);
            ContourUnit = this.model.ObserveProperty(x => x.ContourUnit).ToReadOnlyReactivePropertySlim();

            #endregion

            #region 出力タイムステップスライダー

            TimeSteps = this.model.ObserveProperty(x => x.TimeSteps).ToReadOnlyReactivePropertySlim();
            StartTimeStep = TimeSteps.Select(ts => ts.Count == 0 ? 0 : ts[0]).ToReadOnlyReactivePropertySlim();
            EndTimeStep = TimeSteps.Select(ts => ts.Count == 0 ? 0 : ts[ts.Count - 1]).ToReadOnlyReactivePropertySlim();

            CurrentTimeStep = this.model.ToReactivePropertyAsSynchronized(x => x.CurrentTimeStep);

            CurrentTimeStep.Subscribe(_ => this.model.GetValues());

            IsPlaying = this.model.ObserveProperty(x => x.IsPlaying).ToReadOnlyReactiveProperty();

            PlayCommand.Subscribe(() => this.model.Playing());
            NextStepCommand.Subscribe(() => this.model.NextStep());
            PreviousStepCommand.Subscribe(() => this.model.PreviousStep());

            #endregion

            #region グラフ表示

            IsLogAxisX = this.model.ToReactivePropertySlimAsSynchronized(m => m.IsLogAxisX);
            IsLogAxisY = this.model.ToReactivePropertySlimAsSynchronized(m => m.IsLogAxisY);

            #endregion

            PlotRun.Subscribe(() => this.model.Graph());

            // コンボボックスでパターンを選択
            SelectCombo.Subscribe(str =>
            {
                if (str == null)
                    return;

                this.model.SelectPattern(SelectCombo.Value);
            });

            // コンターの上限・下限設定
            ContourMax.Subscribe(_ =>
            {
                if (DataValues.Count != 0)
                    this.model.SetColor();
            });
            ContourMin.Subscribe(_ =>
            {
                if (DataValues.Count != 0)
                    this.model.SetColor();
            });
        }

        /// <summary>
        /// ファイルダイアログ操作
        /// </summary>
        private string SelectResultFile()
        {
            var dialog = new OpenFileDialog();
            dialog.ShowDialog();

            if (dialog.FileName != "")
                return dialog.FileName;

            return null;
        }
    }
}
