using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Media;

namespace FlexID.Viewer
{
    public class Model : BindableBase
    {
        /// <summary>
        /// コンストラクタ。
        /// </summary>
        public Model()
        {
            // モデル図に表示するための統一臓器名リスト
            var FixListLines = File.ReadAllLines(@"lib/FixList.txt");

            IntegratedOrgans = new List<string>();

            FixList = new Dictionary<string, string>();

            foreach (var x in FixListLines)
            {
                var Lines = x.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);
                IntegratedOrgans.Add(Lines[0]);
                for (int i = 1; i < Lines.Length; i++)
                {
                    FixList.Add(Lines[i], Lines[0]);
                }
            }

            #region グラフ表示

            LogAxisX = new LogarithmicAxis()
            {
                Position = AxisPosition.Bottom,
                MajorGridlineStyle = LineStyle.Automatic,
                MajorGridlineColor = OxyColor.FromRgb(0, 0, 0),
                MinorGridlineStyle = LineStyle.Dot,
                MinorGridlineColor = OxyColor.FromRgb(128, 128, 128),
                TitleFontSize = 14,
                Title = "Days after Intake"
            };
            LogAxisY = new LogarithmicAxis()
            {
                Position = AxisPosition.Left,
                MajorGridlineStyle = LineStyle.Automatic,
                MajorGridlineColor = OxyColor.FromRgb(0, 0, 0),
                MinorGridlineStyle = LineStyle.Dot,
                MinorGridlineColor = OxyColor.FromRgb(128, 128, 128),
                TitleFontSize = 14,
                AxisTitleDistance = 10,
            };

            LinAxisX = new LinearAxis()
            {
                Position = AxisPosition.Bottom,
                MajorGridlineStyle = LineStyle.Automatic,
                MajorGridlineColor = OxyColor.FromRgb(0, 0, 0),
                MinorGridlineStyle = LineStyle.Dot,
                MinorGridlineColor = OxyColor.FromRgb(128, 128, 128),
                TitleFontSize = 14,
                Title = "Days after Intake"
            };
            LinAxisY = new LinearAxis()
            {
                Position = AxisPosition.Left,
                MajorGridlineStyle = LineStyle.Automatic,
                MajorGridlineColor = OxyColor.FromRgb(0, 0, 0),
                MinorGridlineStyle = LineStyle.Dot,
                MinorGridlineColor = OxyColor.FromRgb(128, 128, 128),
                TitleFontSize = 14,
                AxisTitleDistance = 10,
            };

            PlotModel.Axes.Add(LogAxisX);
            PlotModel.Axes.Add(LogAxisY);

            #endregion
        }

        // 表示する出力ファイル
        private IEnumerable<string> TargetFile;

        // 対象臓器
        string[] Organs;

        // モデル図に表示するための統一臓器名リスト
        private List<string> IntegratedOrgans { get; }

        // 臓器名をfixするためにテキストから読込んだリスト 
        private Dictionary<string, string> FixList { get; }

        public ObservableCollection<CalcData> _dataValues = new ObservableCollection<CalcData>();
        public ObservableCollection<GraphList> _graphList = new ObservableCollection<GraphList>();
        public ObservableCollection<string> _comboList = new ObservableCollection<string>();

        private string pattern = "";
        private string patternUnit = "";

        #region 出力ファイル情報

        //　解析結果のファイルパス
        public string ResultFilePath
        {
            get => resultFilePath;
            set => SetProperty(ref resultFilePath, value);
        }
        private string resultFilePath;

        #endregion

        // モデル図に表示するために合算された値
        Dictionary<string, double> organValues = new Dictionary<string, double>();
        public Dictionary<string, double> OrganValues
        {
            get => organValues;
            set => SetProperty(ref organValues, value);
        }

        // 現在スライダーが示している時間
        double onValue = 0;
        public double OnValue
        {
            get => onValue;
            set => SetProperty(ref onValue, value);
        }

        #region コンター表示

        /// <summary>
        /// コンターの上限値。
        /// </summary>
        public double ContourMax
        {
            get => contourMax;
            set => SetProperty(ref contourMax, value);
        }
        private double contourMax;

        /// <summary>
        /// コンターの下限値。
        /// </summary>
        public double ContourMin
        {
            get => contourMin;
            set => SetProperty(ref contourMin, value);
        }
        private double contourMin;

        /// <summary>
        /// コンターに表示される単位。
        /// </summary>
        public string ContourUnit
        {
            get => contourUnit;
            set => SetProperty(ref contourUnit, value);
        }
        private string contourUnit;

        #endregion

        #region 出力タイムステップスライダー
        DoubleCollection timeStep;
        public DoubleCollection TimeStep
        {
            get => timeStep;
            set => SetProperty(ref timeStep, value);
        }

        double startStep;
        public double StartStep
        {
            get => startStep;
            set => SetProperty(ref startStep, value);
        }

        double endStep;
        public double EndStep
        {
            get => endStep;
            set => SetProperty(ref endStep, value);
        }
        #endregion

        // True：再生中　False：停止中
        bool isPlaying;
        public bool IsPlaying
        {
            get => isPlaying;
            set => SetProperty(ref isPlaying, value);
        }

        string radioNuclide = "";
        public string RadioNuclide
        {
            get => radioNuclide;
            set => SetProperty(ref radioNuclide, value);
        }

        string intakeRoute = "";
        public string IntakeRoute
        {
            get => intakeRoute;
            set => SetProperty(ref intakeRoute, value);
        }

        // 臓器ごとの色情報
        Dictionary<string, string> organColors = new Dictionary<string, string>();
        public Dictionary<string, string> OrganColors
        {
            get => organColors;
            set => SetProperty(ref organColors, value);
        }

        #region グラフ表示

        string graphLabel;
        public string GraphLabel
        {
            get => graphLabel;
            set
            {
                if (value == "Dose[Sv/Bq]")
                    value = "Effective/Equivalent Dose[Sv/Bq]";
                LogAxisY.Title = value;
                LinAxisY.Title = value;
                PlotModel.InvalidatePlot(false);
                SetProperty(ref graphLabel, value);
            }
        }

        public PlotModel PlotModel { get; } = new PlotModel();

        private LogarithmicAxis LogAxisX { get; }
        private LogarithmicAxis LogAxisY { get; }
        private LinearAxis LinAxisX { get; }
        private LinearAxis LinAxisY { get; }

        public bool IsLogAxisX
        {
            get => isLogAxisX;
            set
            {
                PlotModel.Axes[0] = (value ? (Axis)LogAxisX : LinAxisX);
                PlotModel.InvalidatePlot(false);
                SetProperty(ref isLogAxisX, value);
            }
        }
        private bool isLogAxisX = true;

        public bool IsLogAxisY
        {
            get => isLogAxisY;
            set
            {
                PlotModel.Axes[1] = (value ? (Axis)LogAxisY : LinAxisY);
                PlotModel.InvalidatePlot(false);
                SetProperty(ref isLogAxisY, value);
            }
        }
        private bool isLogAxisY = true;

        #endregion

        /// <summary>
        /// 再生・停止制御
        /// </summary>
        public async void Playing()
        {
            if (TimeStep == null)
                return;

            if (!IsPlaying) // false＝停止時のみ処理される
            {
                IsPlaying = true;
                foreach (var x in TimeStep)
                {
                    if (OnValue >= x)
                        continue;

                    OnValue = x;
                    await Task.Delay(200);

                    if (!IsPlaying) // 再生中にボタンが押されると再生処理を終了する
                        break;
                }
            }
            else if (IsPlaying) // true＝再生時のみ処理される
                IsPlaying = false;

            IsPlaying = false;
        }

        /// <summary>
        /// 次のタイムステップヘ進む
        /// </summary>
        public void NextStep()
        {
            if (TimeStep == null)
                return;
            if (OnValue == EndStep)
                return;

            IsPlaying = false;
            for (int i = 0; i < TimeStep.Count; i++)
            {
                if (OnValue == TimeStep[i])
                {
                    OnValue = TimeStep[i + 1];
                    break;
                }
            }
        }

        /// <summary>
        /// 1つ前のタイムステップに戻る
        /// </summary>
        public void PreviousStep()
        {
            if (TimeStep == null)
                return;
            if (OnValue == StartStep)
                return;

            IsPlaying = false;
            for (int i = 0; i < TimeStep.Count; i++)
            {
                if (OnValue == TimeStep[i])
                {
                    OnValue = TimeStep[i - 1];
                    break;
                }
            }
        }

        /// <summary>
        /// ファイル名から表示可能なパターンを検索
        /// </summary>
        public void Reader()
        {
            RadioNuclide = "";
            IntakeRoute = "";
            ContourMax = 0;
            ContourMin = 0;
            ContourUnit = "-";
            OnValue = 0;
            TimeStep = null;
            StartStep = 0;
            EndStep = 0;
            _dataValues.Clear();
            _graphList.Clear();
            _comboList.Clear();
            OrganValues = new Dictionary<string, double>();
            OrganColors = new Dictionary<string, string>();
            PlotModel.Series.Clear();

            if (ResultFilePath != "")
            {
                var path = ResultFilePath.Replace("_Retention.out", "");
                path = path.Replace("_Cumulative.out", "");
                path = path.Replace("_Dose.out", "");
                path = path.Replace("_DoseRate.out", "");
                path = Path.GetFullPath(path);

                _comboList.Clear();
                if (File.Exists(path + "_Retention.out"))
                    _comboList.Add("Retention");
                if (File.Exists(path + "_Cumulative.out"))
                    _comboList.Add("CumulativeActivity");
                if (File.Exists(path + "_Dose.out"))
                    _comboList.Add("Dose");
                if (File.Exists(path + "_DoseRate.out"))
                    _comboList.Add("DoseRate");
                if (_comboList.Count > 0)
                    ReadHeader(path);
            }
        }

        /// <summary>
        /// 選択されたファイルのヘッダー読み取り
        /// </summary>
        /// <param name="Path"></param>
        private void ReadHeader(string Path)
        {
            var Extension = "_" + _comboList[0].Replace("Activity", "") + ".out";

            Path += Extension;
            var file = File.ReadLines(Path);
            foreach (var x in file)
            {
                var line = x.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);
                RadioNuclide = line[1];
                IntakeRoute = line[2];
                break;
            }
        }

        /// <summary>
        /// 4つの中から描画するパターンを設定
        /// </summary>
        public void SelectPattern(string selectCombo)
        {
            PlotModel.Series.Clear();
            GraphLabel = "";
            pattern = selectCombo;

            if (selectCombo != null)
            {
                var path = ResultFilePath;

                switch (selectCombo)
                {
                    case "Dose":
                        ContourUnit = "Sv/Bq";
                        break;

                    case "DoseRate":
                        ContourUnit = "Sv/h";
                        break;

                    case "Retention":
                        ContourUnit = "Bq/Bq";
                        break;

                    case "CumulativeActivity":
                        ContourUnit = "Bq";
                        break;
                }

                // ファイル名を固定していいならハードコーディングでいい？
                path = path.Replace("_Retention.out", "");
                path = path.Replace("_Cumulative.out", "");
                path = path.Replace("_Dose.out", "");
                path = path.Replace("_DoseRate.out", "");

                var Target = path + "_" + selectCombo.Replace("Activity", "") + ".out";
                TargetFile = File.ReadLines(Target);

                foreach (var x in TargetFile.Skip(1))
                {
                    Organs = x.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);
                    break;
                }
                Contour();
                GetStep();
                GetValues();
                ReadResults();
            }
        }

        /// <summary>
        /// コンターの上限・下限値取得
        /// </summary>
        private void Contour()
        {
            double max = 0;
            double min = 0;

            foreach (var x in TargetFile.Skip(3)) // ヘッダーを飛ばすために3行飛ばす
            {
                var line = x.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);
                if (line.Length < 6)
                    continue;

                List<double> values = new List<double>();
                for (int i = 2; i < line.Length; i++) // 最初の2つを除くので2からスタート
                {
                    if (double.TryParse(line[i], out double num))
                    {
                        if (num > 0)
                            values.Add(num);
                    }
                    else
                        break;
                }
                if (values.Count < 1)
                    continue;
                if (max == 0)
                {
                    max = values.Max();
                    min = values.Min();
                }
                else
                {
                    if (max < values.Max())
                        max = values.Max();

                    if (min > values.Min())
                        min = values.Min();
                }
            }

            // 1E**に合わせる
            max = Math.Log10(max);
            max = Math.Ceiling(max);
            ContourMax = Math.Pow(10, max);

            min = Math.Log10(min);
            min = Math.Floor(min);
            ContourMin = Math.Pow(10, min);
        }

        /// <summary>
        /// 選択されたファイルのタイムステップ取得
        /// </summary>
        private void GetStep()
        {
            TimeStep = new DoubleCollection();
            foreach (var x in TargetFile.Skip(3))
            {
                if (x == "")
                    break;
                var line = x.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);
                if (line.Length < 1)
                    continue;
                TimeStep.Add(double.Parse(line[0]));
            }
            OnValue = TimeStep.First();
            StartStep = TimeStep.First();
            EndStep = TimeStep.Last();
        }

        /// <summary>
        /// ファイルから臓器名および計算値取得
        /// </summary>
        public void GetValues()
        {
            if (TargetFile == null)
                return;

            foreach (var x in TargetFile.Skip(3))
            {
                var line = x.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);
                if (line.Length < 1)
                    continue;
                if (OnValue == double.Parse(line[0]))
                {
                    _dataValues.Clear();
                    _graphList.Clear();
                    for (int i = 2; i < Organs.Length; i++)
                        _dataValues.Add(new CalcData(Organs[i], double.Parse(line[i])));
                    for (int i = 1; i < Organs.Length; i++) // グラフタブに表示するリストはWholeBodyを入れるため別処理
                        _graphList.Add(new GraphList(Organs[i], false));
                    break;
                }
            }
            SetColor();
        }

        /// <summary>
        /// 臓器ごとに全ステップの計算値取得
        /// </summary>
        private void ReadResults()
        {
            var regions = TargetFile.Skip(1).Take(1)
                .SelectMany(x => x.Split(' ').Where(s => !string.IsNullOrWhiteSpace(s)))
                .ToList();
            var units = TargetFile.Skip(2).Take(1)
                .SelectMany(x => x.Split(' ').Where(s => !string.IsNullOrWhiteSpace(s)))
                .ToList();

            var calcTimes = default(List<double>);

            patternUnit = units[1];

            PlotModel.Series.Clear();

            for (int i = 0; i < regions.Count; i++)
            {
                var values = new List<double>();
                foreach (var y in TargetFile.Skip(3))
                {
                    if (y == "")
                        break;
                    var value = y.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);
                    values.Add(double.Parse(value[i]));
                }

                if (i == 0)
                {
                    calcTimes = values;
                    continue;
                }

                var serie = new ScatterSeries()
                {
                    Title = regions[i],
                };
                for (int j = 0; j < calcTimes.Count; j++)
                {
                    if (calcTimes[j] == 0)
                        continue;
                    serie.Points.Add(new ScatterPoint(calcTimes[j], values[j]));
                }
                PlotModel.Series.Add(serie);
            }
        }

        /// <summary>
        /// コンターの上限下限から計算値の割合を算出し、モデル図の色を設定する
        /// </summary>
        public void SetColor()
        {
            OrganValues = new Dictionary<string, double>();
            foreach (var x in IntegratedOrgans)
            {
                OrganValues.Add(x, 0);
            }

            foreach (var x in _dataValues)
            {
                if (FixList.TryGetValue(x.OrganName, out var integratedOrgan))
                    OrganValues[integratedOrgan] += x.Value;
            }

            OrganColors = new Dictionary<string, string>();
            foreach (var x in OrganValues)
            {
                Color ColorCode;
                byte R, G, B = 0;

                //          R    G    B
                // Red     255   0    0
                // Orange  255  165   0
                // Yellow  255  255   0
                // Lime     0   255   0
                // Blue     0    0   255

                if (x.Value == 0) // 計算値が0の時は別処理をする
                {
                    ColorCode = Color.FromRgb(211, 211, 211);

                    OrganColors.Add(x.Key, ColorCode.ToString());
                    continue;
                }

                double frac = (Math.Log(x.Value) - Math.Log(ContourMin)) / (Math.Log(ContourMax) - Math.Log(ContourMin));

                if (frac > 1) // コンターの上限を超える
                {
                    ColorCode = Color.FromRgb(255, 0, 0);
                }
                else if (frac > 0.75) // Red～Orangeの間
                {
                    frac = (frac - 0.75) * 4;
                    //R = (byte)(255 + (0 * frac));
                    G = (byte)(165 - (165 * frac));
                    //B = (byte)(0 + (0 * frac));
                    ColorCode = Color.FromRgb(255, G, 0);
                }
                else if (frac > 0.5) // Orange～Yellowの間
                {
                    frac = (frac - 0.5) * 4;
                    //R = (byte)(255 + (0 * frac));
                    G = (byte)(255 - (90 * frac));
                    //B = (byte)(0 + (0 * frac));
                    ColorCode = Color.FromRgb(255, G, 0);
                }
                else if (frac > 0.25) // Yellow～Limeの間
                {
                    frac = (frac - 0.25) * 4;
                    R = (byte)(0 + (255 * frac));
                    //G = (byte)(255 + (0 * frac));
                    //B = (byte)(0 + (0 * frac));
                    ColorCode = Color.FromRgb(R, 255, 0);
                }
                else if (frac > 0) // Lime～Blueの間
                {
                    frac = (frac - 0) * 4;
                    //R = (byte)(0 + (0 * frac));
                    G = (byte)(0 + (255 * frac));
                    B = (byte)(255 - (255 * frac));
                    ColorCode = Color.FromRgb(0, G, B);
                }
                else  // コンターの下限を下回る
                    ColorCode = Color.FromRgb(0, 0, 255);

                OrganColors.Add(x.Key, ColorCode.ToString());
            }
        }

        /// <summary>
        /// プロットデータ取得
        /// </summary>
        public void Graph()
        {
            GraphLabel = pattern + patternUnit;

            foreach (var serie in PlotModel.Series)
            {
                var region = _graphList.First(n => n.OrganName == serie.Title);
                serie.IsVisible = region.IsChecked;
            }

            // グラフがFitする範囲などを更新するためにupdateData: trueが必要。
            PlotModel.InvalidatePlot(updateData: true);
        }
    }

    public class CalcData
    {
        public CalcData(string organName, double value)
        {
            OrganName = organName;
            Value = value;
        }
        public string OrganName { get; set; }
        public double Value { get; set; }
    }

    public class GraphList
    {
        public GraphList(string organName, bool isChecked)
        {
            OrganName = organName;
            IsChecked = isChecked;
        }
        public string OrganName { get; set; }
        public bool IsChecked { get; set; }
    }
}
