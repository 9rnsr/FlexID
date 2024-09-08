using Microsoft.Win32;
using OxyPlot;
using Prism.Mvvm;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using System.Windows.Media;

namespace FlexID.Viewer.ViewModels
{
    public class MainWindowViewModel : BindableBase
    {
        private readonly Model model;

        // 入力GUIから受け取るファイルパス
        public static string OutPath = "";

        // 現在スライダーが示している時間
        public ReactiveProperty<double> OnValue { get; }

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
        public ReactiveProperty<DoubleCollection> TimeStep { get; }
        public ReadOnlyReactiveProperty<double> StartStep { get; }
        public ReadOnlyReactiveProperty<double> EndStep { get; }
        #endregion

        public ReactiveCommand PlayCommand { get; } = new ReactiveCommand();
        public ReactiveCommand NextStepCommand { get; } = new ReactiveCommand();
        public ReactiveCommand PreviousStepCommand { get; } = new ReactiveCommand();

        public ReactiveCommand PlotRun { get; } = new ReactiveCommand();

        // True：再生中　False：停止中
        public ReadOnlyReactiveProperty<bool> IsPlaying { get; }
        public ReactiveProperty<string> RadioNuclide { get; }
        public ReactiveProperty<string> IntakeRoute { get; }
        public ReadOnlyReactiveProperty<string> GraphLabel { get; }
        public ReadOnlyReactiveCollection<GraphList> GraphList { get; }
        public ReadOnlyReactiveProperty<PlotModel> PlotModel { get; }

        public ReactiveProperty<bool> AxisX { get; }
        public ReactiveProperty<bool> AxisY { get; }

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
            TimeStep = this.model.ObserveProperty(x => x.TimeStep).ToReactiveProperty();
            IsPlaying = this.model.ObserveProperty(x => x.IsPlaying).ToReadOnlyReactiveProperty();
            StartStep = this.model.ObserveProperty(x => x.StartStep).ToReadOnlyReactiveProperty();
            EndStep = this.model.ObserveProperty(x => x.EndStep).ToReadOnlyReactiveProperty();
            GraphLabel = this.model.ObserveProperty(x => x.GraphLabel).ToReadOnlyReactiveProperty();
            PlotModel = this.model.ObserveProperty(x => x.PlotModel).ToReadOnlyReactiveProperty();

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

            OnValue = this.model.ToReactivePropertyAsSynchronized(x => x.OnValue);

            #region コンター表示

            ContourMax = this.model.ToReactivePropertySlimAsSynchronized(x => x.ContourMax);
            ContourMin = this.model.ToReactivePropertySlimAsSynchronized(x => x.ContourMin);
            ContourUnit = this.model.ObserveProperty(x => x.ContourUnit).ToReadOnlyReactivePropertySlim();

            #endregion

            AxisX = this.model.ToReactivePropertyAsSynchronized(x => x.AxisX);
            AxisY = this.model.ToReactivePropertyAsSynchronized(x => x.AxisY);

            PlayCommand.Subscribe(() => this.model.Playing());
            NextStepCommand.Subscribe(() => this.model.NextStep());
            PreviousStepCommand.Subscribe(() => this.model.PreviousStep());
            PlotRun.Subscribe(() => this.model.Graph());

            // OnValueの値が変更されたらイベント発生
            OnValue.Subscribe(_ =>
            {
                this.model.GetValues();
            });

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

            AxisX.Subscribe(_ =>
            {
                if (PlotModel.Value == null)
                    return;
                if (PlotModel.Value.Series.Count < 1)
                    return;
                this.model.Graph();
            });
            AxisY.Subscribe(_ =>
            {
                if (PlotModel.Value == null)
                    return;
                if (PlotModel.Value.Series.Count < 1)
                    return;
                this.model.Graph();
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
