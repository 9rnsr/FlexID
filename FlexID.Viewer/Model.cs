using FlexID.Calc;
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
            #region コンター表示

            organs = new List<string>();
            organMap = new Dictionary<string, string>();

            unsetOrganValues = new Dictionary<string, double>();
            unsetOrganColors = new Dictionary<string, string>();

            foreach (var line in File.ReadLines(@"lib/FixList.txt"))
            {
                var parts = line.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);
                var organ = parts[0];

                organs.Add(organ);
                for (int i = 1; i < parts.Length; i++)
                    organMap.Add(parts[i], organ);

                unsetOrganValues.Add(organ, 0);
                unsetOrganColors.Add(organ, unsetColorCode);
            }

            actualOrganValues = new Dictionary<string, double>(unsetOrganValues);
            actualOrganColors = new Dictionary<string, string>(unsetOrganColors);

            OrganValues = unsetOrganValues;
            OrganColors = unsetOrganColors;

            #endregion

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

        #region 出力ファイル情報

        /// <summary>
        /// 出力ファイルリストのベースとなるパス文字列。
        /// </summary>
        private string BasePath { get; set; }

        /// <summary>
        /// 検出された出力ファイル種別のリスト。
        /// </summary>
        public ObservableCollection<OutputType> OutputTypes { get; } = new ObservableCollection<OutputType>();

        /// <summary>
        /// インプットのタイトル。
        /// </summary>
        public string Title
        {
            get => title;
            set => SetProperty(ref title, value);
        }
        private string title = "";

        /// <summary>
        /// 表示中の出力ファイルのデータ。
        /// </summary>
        private OutputData CurrentOutput { get; set; }

        /// <summary>
        /// 表示中の核種のデータ。
        /// </summary>
        private OutputNuclideData CurrentNuclide { get; set; }

        /// <summary>
        /// 核種リスト。
        /// </summary>
        public ObservableCollection<string> Nuclides { get; } = new ObservableCollection<string>();

        #endregion

        #region コンター表示

        /// <summary>
        /// モデル図に表示するための、統一臓器名のリスト。
        /// </summary>
        private readonly List<string> organs;

        /// <summary>
        /// コンパートメント名をキーに、対応する統一臓器名を値にした辞書。
        /// </summary>
        private readonly Dictionary<string, string> organMap;

        private readonly Dictionary<string, double> unsetOrganValues;
        private readonly Dictionary<string, string> unsetOrganColors;

        private readonly Dictionary<string, double> actualOrganValues;
        private readonly Dictionary<string, string> actualOrganColors;

        // モデル図で値が未設定の場合の色情報。
        private readonly string unsetColorCode = Color.FromRgb(211, 211, 211).ToString();

        /// <summary>
        /// 現在の出力タイムステップにおける各コンパートメントの数値。
        /// </summary>
        public ObservableCollection<CalcData> DataValues { get; } = new ObservableCollection<CalcData>();

        /// <summary>
        /// モデル図に表示するための、統一臓器名とその合算された数値。
        /// </summary>
        public Dictionary<string, double> OrganValues
        {
            get => organValues;
            set => SetProperty(ref organValues, value);
        }
        private Dictionary<string, double> organValues;

        /// <summary>
        /// モデル図に表示するための、統一臓器名とその色情報。
        /// </summary>
        public Dictionary<string, string> OrganColors
        {
            get => organColors;
            set => SetProperty(ref organColors, value);
        }
        private Dictionary<string, string> organColors;

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

        /// <summary>
        /// 出力タイムステップ。
        /// </summary>
        public IReadOnlyList<double> TimeSteps
        {
            get => timeSteps;
            private set => SetProperty(ref timeSteps, value);
        }
        private IReadOnlyList<double> timeSteps = Array.Empty<double>();

        /// <summary>
        /// 現在スライダーが示している出力タイムステップのインデックス。
        /// </summary>
        private int CurrentTimeIndex { get; set; } = -1;

        /// <summary>
        /// 現在スライダーが示している出力タイムステップ。
        /// </summary>
        public double CurrentTimeStep
        {
            get => currentTimeStep;
            set
            {
                if (TimeSteps.Count == 0)
                {
                    CurrentTimeIndex = -1;
                    value = 0;
                }
                else if ((TimeSteps[0] is var start) && value < start)
                {
                    // 下限側の範囲外。
                    CurrentTimeIndex = 0;
                    value = start;
                }
                else if ((TimeSteps[TimeSteps.Count - 1] is var end) && end <= value)
                {
                    // 上限側の範囲外。
                    CurrentTimeIndex = TimeSteps.Count - 1;
                    value = end;
                }
                else
                {
                    // [s, e) の区間内にある時間が設定された場合は、sの時間に設定する。
                    for (int i = 1; i < TimeSteps.Count; i++)
                    {
                        if (value < TimeSteps[i])
                        {
                            CurrentTimeIndex = i - 1;
                            value = TimeSteps[i - 1];
                            break;
                        }
                    }
                }
                SetProperty(ref currentTimeStep, value);
            }
        }
        private double currentTimeStep = 0;

        /// <summary>
        /// アニメーション再生状態を示す。<see langword="true"/>：再生中、<see langword="false"/>：停止中。
        /// </summary>
        public bool IsPlaying
        {
            get => isPlaying;
            set => SetProperty(ref isPlaying, value);
        }
        private bool isPlaying;

        #endregion

        #region グラフ表示

        public ObservableCollection<RegionData> Regions { get; } = new ObservableCollection<RegionData>();

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
            if (TimeSteps.Count == 0)
                return;

            if (!IsPlaying)
            {
                // 停止時の処理。
                IsPlaying = true;
                for (var i = CurrentTimeIndex + 1; i < TimeSteps.Count; i++)
                {
                    CurrentTimeIndex = i;
                    CurrentTimeStep = TimeSteps[CurrentTimeIndex];
                    await Task.Delay(200);

                    if (!IsPlaying) // 再生中にボタンが押されると再生処理を終了する
                        break;
                }
            }
            else
            {
                // 再生時の処理。
                IsPlaying = false;
            }

            IsPlaying = false;
        }

        /// <summary>
        /// 次のタイムステップヘ進む
        /// </summary>
        public void NextStep()
        {
            if (TimeSteps.Count == 0)
                return;
            if (CurrentTimeIndex == TimeSteps.Count - 1)
                return;

            IsPlaying = false;
            CurrentTimeIndex++;
            CurrentTimeStep = TimeSteps[CurrentTimeIndex];
        }

        /// <summary>
        /// 1つ前のタイムステップに戻る
        /// </summary>
        public void PreviousStep()
        {
            if (TimeSteps.Count == 0)
                return;
            if (CurrentTimeIndex == 0)
                return;

            IsPlaying = false;
            CurrentTimeIndex--;
            CurrentTimeStep = TimeSteps[CurrentTimeIndex];
        }

        private string GetBasePath(string path)
        {
            if (string.IsNullOrEmpty(path))
                return null;

            path = path.Replace("_Retention.out", "");
            path = path.Replace("_Cumulative.out", "");
            path = path.Replace("_Dose.out", "");
            path = path.Replace("_DoseRate.out", "");
            try
            {
                return Path.GetFullPath(path);
            }
            catch
            {
                return null;
            }
        }

        private static string GetSuffix(OutputType type)
        {
            switch (type)
            {
                case OutputType.RetentionActivity: return "Retention";
                case OutputType.CumulativeActivity: return "Cumulative";
                case OutputType.Dose: return "Dose";
                case OutputType.DoseRate: return "DoseRate";

                default:
                case OutputType.Unknown:
                    throw new NotSupportedException();
            }
        }

        private void ClearTypes()
        {
            OutputTypes.Clear();
            ClearOutput();
        }

        private void ClearOutput()
        {
            if (CurrentOutput is null)
                return;
            CurrentOutput = null;

            Title = "";
            Nuclides.Clear();
            ClearNuclide();
        }

        private void ClearNuclide()
        {
            if (CurrentNuclide is null)
                return;
            CurrentNuclide = null;

            ContourMax = 0;
            ContourMin = 0;
            ContourUnit = "-";

            TimeSteps = Array.Empty<double>();
            CurrentTimeIndex = -1;
            CurrentTimeStep = 0;

            ClearValues();

            Regions.Clear();
            PlotModel.Series.Clear();
        }

        private void ClearValues()
        {
            DataValues.Clear();
            OrganValues = unsetOrganValues;
            OrganColors = unsetOrganColors;
        }

        /// <summary>
        /// ファイル名から描画可能な出力データ群を検索する。
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public OutputType? SetTypes(string path)
        {
            var newBasePath = GetBasePath(path);
            if (newBasePath != BasePath)
            {
                BasePath = newBasePath;
                ClearTypes();

                if (BasePath != null)
                {
                    if (File.Exists(BasePath + "_Retention.out"))
                        OutputTypes.Add(OutputType.RetentionActivity);
                    if (File.Exists(BasePath + "_Cumulative.out"))
                        OutputTypes.Add(OutputType.CumulativeActivity);
                    if (File.Exists(BasePath + "_Dose.out"))
                        OutputTypes.Add(OutputType.Dose);
                    if (File.Exists(BasePath + "_DoseRate.out"))
                        OutputTypes.Add(OutputType.DoseRate);
                }
            }
            return OutputTypes.FirstOrDefault(type =>
                path.EndsWith($"_{GetSuffix(type)}.out", StringComparison.OrdinalIgnoreCase));
        }

        /// <summary>
        /// 見つかっている出力データ群から描画対象を設定する。
        /// </summary>
        public void SetOutput(OutputType type)
        {
            if (CurrentOutput?.Type == type)
                return;

            OutputData newOutput = null;
            if (OutputTypes.Contains(type))
            {
                var targetPath = $"{BasePath}_{GetSuffix(type)}.out";
                try
                {
                    using (var reader = new OutputDataReader(targetPath))
                        newOutput = reader.Read();
                }
                catch (InvalidDataException) { }
            }

            ClearOutput();
            if (newOutput is null)
                return;

            CurrentOutput = newOutput;

            Title = $"{CurrentOutput.Nuclide}, {CurrentOutput.IntakeRoute}";
            Nuclides.AddRange(CurrentOutput.Nuclides.Select(n => n.Nuclide));
        }

        /// <summary>
        /// 崩壊系列を構成する核種から描画対象を設定する。
        /// </summary>
        public void SetNuclide(string nuc)
        {
            if (CurrentNuclide?.Nuclide == nuc)
                return;

            var index = Nuclides.IndexOf(nuc);
            if (index == -1)
            {
                ClearNuclide();
                return;
            }

            CurrentNuclide = CurrentOutput.Nuclides[index];

            SetMinMax();
            SetSteps();
            SetPlot();

            SetValues();
        }

        /// <summary>
        /// 出力データからコンターの上限・下限値を設定する。
        /// </summary>
        private void SetMinMax()
        {
            var max = CurrentNuclide.Compartments.SelectMany(c => c.Values).Where(v => !double.IsNaN(v)).Max();
            var min = CurrentNuclide.Compartments.SelectMany(c => c.Values).Where(v => !double.IsNaN(v)).Min();

            // 1E**に合わせる
            ContourMax = Math.Pow(10, Math.Ceiling(Math.Log10(max)));
            ContourMin = Math.Pow(10, Math.Floor(Math.Log10(min)));

            ContourUnit = CurrentOutput.DataValueUnit;
        }

        /// <summary>
        /// 出力データからタイムステップを設定する。
        /// </summary>
        private void SetSteps()
        {
            TimeSteps = CurrentOutput.TimeSteps;

            var anySteps = TimeSteps.Count != 0;
            CurrentTimeIndex = anySteps ? 0 : -1;
            CurrentTimeStep = anySteps ? TimeSteps[CurrentTimeIndex] : 0;
        }

        /// <summary>
        /// 出力データから全タイムステップにおけるコンパートメント毎の計算値をグラフ設定する。
        /// </summary>
        private void SetPlot()
        {
            var calcTimes = CurrentOutput.TimeSteps;

            var compartments = CurrentNuclide.Compartments;
            for (int i = 0; i < compartments.Length; i++)
            {
                var compartment = compartments[i];
                var values = compartment.Values;

                var serie = new ScatterSeries()
                {
                    Title = compartment.Name,
                    IsVisible = false,  // 初期状態は非表示。
                };
                for (int j = 0; j < calcTimes.Count; j++)
                {
                    if (calcTimes[j] == 0)
                        continue;
                    serie.Points.Add(new ScatterPoint(calcTimes[j], values[j]));
                }

                Regions.Add(new RegionData(serie, compartment.Name));
                PlotModel.Series.Add(serie);
            }

            var graphLabel =
                CurrentOutput.Type == OutputType.RetentionActivity ? "Retention" :
                CurrentOutput.Type == OutputType.CumulativeActivity ? "CumulativeActivity" :
                CurrentOutput.Type == OutputType.Dose ? "Effective/Equivalent Dose" :
                CurrentOutput.Type == OutputType.DoseRate ? "DoseRate" :
                throw new NotSupportedException();
            graphLabel += $"[{ContourUnit}]";

            LogAxisY.Title = graphLabel;
            LinAxisY.Title = graphLabel;

            // グラフがFitする範囲などを更新するためにupdateData: trueが必要。
            PlotModel.InvalidatePlot(updateData: true);
        }

        /// <summary>
        /// 現在の出力タイムステップにおける計算値を取得し、モデルの図の値を設定する。
        /// </summary>
        public void SetValues()
        {
            if (CurrentNuclide is null)
                return;
            if (TimeSteps.Count == 0)
                return;

            DataValues.Clear();
            OrganValues = unsetOrganValues;

            foreach (var organ in organs)
            {
                actualOrganValues[organ] = 0;
            }

            var compartments = CurrentNuclide.Compartments;
            for (int i = 0; i < compartments.Length; i++)
            {
                var compartment = compartments[i];
                var value = compartment.Values[CurrentTimeIndex];

                DataValues.Add(new CalcData(compartment.Name, value));

                if (organMap.TryGetValue(compartment.Name, out var organ))
                    actualOrganValues[organ] += value;
            }

            OrganValues = actualOrganValues;

            // 数値が変わるので、色も再設定が必要。
            SetColors();
        }

        /// <summary>
        /// 現在の出力タイムステップにおける計算値の割合をコンターの上限下限から算出し、モデル図の色を設定する。
        /// </summary>
        public void SetColors()
        {
            if (CurrentNuclide is null)
                return;
            if (TimeSteps.Count == 0)
                return;

            OrganColors = unsetOrganColors;

            foreach (var organ in organs)
            {
                var contourVal = OrganValues[organ];

                if (contourVal == 0) // 計算値が0の時は別処理をする
                {
                    actualOrganColors[organ] = unsetColorCode;
                    continue;
                }

                var frac = (Math.Log(contourVal) - Math.Log(ContourMin))
                         / (Math.Log(ContourMax) - Math.Log(ContourMin));

                Color color;
                byte R, G, B;

                //          R    G    B
                // Red     255   0    0
                // Orange  255  165   0
                // Yellow  255  255   0
                // Lime     0   255   0
                // Blue     0    0   255

                if (frac > 1) // コンターの上限を超える
                {
                    color = Color.FromRgb(255, 0, 0);
                }
                else if (frac > 0.75) // Red～Orangeの間
                {
                    frac = (frac - 0.75) * 4;
                    //R = (byte)(255 + (0 * frac));
                    G = (byte)(165 - (165 * frac));
                    //B = (byte)(0 + (0 * frac));
                    color = Color.FromRgb(255, G, 0);
                }
                else if (frac > 0.5) // Orange～Yellowの間
                {
                    frac = (frac - 0.5) * 4;
                    //R = (byte)(255 + (0 * frac));
                    G = (byte)(255 - (90 * frac));
                    //B = (byte)(0 + (0 * frac));
                    color = Color.FromRgb(255, G, 0);
                }
                else if (frac > 0.25) // Yellow～Limeの間
                {
                    frac = (frac - 0.25) * 4;
                    R = (byte)(0 + (255 * frac));
                    //G = (byte)(255 + (0 * frac));
                    //B = (byte)(0 + (0 * frac));
                    color = Color.FromRgb(R, 255, 0);
                }
                else if (frac > 0) // Lime～Blueの間
                {
                    frac = (frac - 0) * 4;
                    //R = (byte)(0 + (0 * frac));
                    G = (byte)(0 + (255 * frac));
                    B = (byte)(255 - (255 * frac));
                    color = Color.FromRgb(0, G, B);
                }
                else  // コンターの下限を下回る
                {
                    color = Color.FromRgb(0, 0, 255);
                }

                actualOrganColors[organ] = color.ToString();
            }

            OrganColors = actualOrganColors;
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

    public class RegionData : BindableBase
    {
        public RegionData(ScatterSeries serie, string name)
        {
            this.serie = serie;
            Name = name;
        }

        private readonly ScatterSeries serie;

        public string Name { get; }

        public bool IsVisible
        {
            get => serie.IsVisible;
            set
            {
                if (serie.IsVisible == value)
                    return;
                serie.IsVisible = value;
                serie.PlotModel.InvalidatePlot(true);
            }
        }
    }
}
