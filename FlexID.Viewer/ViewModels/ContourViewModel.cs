using System.Collections.ObjectModel;
using System.IO;
using System.Windows.Media;
using FlexID.Calc;
using Prism.Mvvm;
using Reactive.Bindings;

namespace FlexID.Viewer.ViewModels;

public class ContourViewModel : BindableBase
{
    /// <summary>
    /// コンストラクタ。
    /// </summary>
    public ContourViewModel()
    {
        var listFilePath = Path.Combine(AppContext.BaseDirectory, "lib", "FixList.txt");
        foreach (var line in File.ReadLines(listFilePath))
        {
            var parts = line.Split([" "], StringSplitOptions.RemoveEmptyEntries);
            var organ = parts[0];

            organs.Add(organ);
            for (int i = 1; i < parts.Length; i++)
                organMap.Add(parts[i], organ);

            unsetOrganValues.Add(organ, 0);
            unsetOrganColors.Add(organ, unsetColorCode);
        }

        actualOrganValues = new(unsetOrganValues);
        actualOrganColors = new(unsetOrganColors);

        OrganValues = unsetOrganValues;
        OrganColors = unsetOrganColors;

        PlayCommand = new ReactiveCommand().WithSubscribe(Play);
        NextStepCommand = new ReactiveCommand().WithSubscribe(NextStep);
        PreviousStepCommand = new ReactiveCommand().WithSubscribe(PreviousStep);
    }

    /// <summary>
    /// 表示中のブロックデータ(核種毎の残留放射能や男女別の線量)。
    /// </summary>
    private OutputBlockData SelectedBlock { get; set; }

    #region コンター表示

    /// <summary>
    /// モデル図に表示するための、統一臓器名のリスト。
    /// </summary>
    private readonly List<string> organs = [];

    /// <summary>
    /// コンパートメント名をキーに、対応する統一臓器名を値にした辞書。
    /// </summary>
    private readonly Dictionary<string, string> organMap = [];

    private readonly Dictionary<string, double> unsetOrganValues = [];
    private readonly Dictionary<string, string> unsetOrganColors = [];

    private readonly Dictionary<string, double> actualOrganValues;
    private readonly Dictionary<string, string> actualOrganColors;

    // モデル図で値が未設定の場合の色情報。
    private readonly string unsetColorCode = Color.FromRgb(211, 211, 211).ToString();

    /// <summary>
    /// 現在の出力タイムステップにおける各コンパートメントの数値。
    /// </summary>
    public ObservableCollection<CalcData> DataValues { get; } = [];

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
        set
        {
            SetProperty(ref contourMax, value);
            SetColors();
        }
    }
    private double contourMax;

    /// <summary>
    /// コンターの下限値。
    /// </summary>
    public double ContourMin
    {
        get => contourMin;
        set
        {
            SetProperty(ref contourMin, value);
            SetColors();
        }
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
        private set
        {
            SetProperty(ref timeSteps, value);
            RaisePropertyChanged(nameof(StartTimeStep));
            RaisePropertyChanged(nameof(EndTimeStep));
        }
    }
    private IReadOnlyList<double> timeSteps = [];

    public double StartTimeStep => TimeSteps.FirstOrDefault();

    public double EndTimeStep => TimeSteps.LastOrDefault();

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

            SetValues();
        }
    }
    private double currentTimeStep = 0;

    /// <summary>
    /// アニメーション再生状態を示す。
    /// </summary>
    public bool IsPlaying
    {
        get => isPlaying;
        set => SetProperty(ref isPlaying, value);
    }
    private bool isPlaying;

    #endregion

    public ReactiveCommand PlayCommand { get; }
    public ReactiveCommand NextStepCommand { get; }
    public ReactiveCommand PreviousStepCommand { get; }

    /// <summary>
    /// 再生・停止制御
    /// </summary>
    public async void Play()
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

    /// <summary>
    /// 崩壊系列を構成する核種から描画対象を設定する。
    /// </summary>
    public void SetBlock(OutputData output, OutputBlockData block)
    {
        if (SelectedBlock == block)
            return;

        SelectedBlock = null;

        ContourMax = 0;
        ContourMin = 0;
        ContourUnit = "-";

        TimeSteps = [];
        CurrentTimeIndex = -1;
        CurrentTimeStep = 0;

        DataValues.Clear();
        OrganValues = unsetOrganValues;
        OrganColors = unsetOrganColors;

        if (block is null)
            return;

        SelectedBlock = block;

        SetMinMax();
        ContourUnit = output.DataValueUnit;

        TimeSteps = output.TimeSteps;
        SetSteps();

        SetValues();
    }

    /// <summary>
    /// 出力データからコンターの上限・下限値を設定する。
    /// </summary>
    private void SetMinMax()
    {
        var max = SelectedBlock.Compartments.SelectMany(c => c.Values).Where(v => !double.IsNaN(v)).Max();
        var min = SelectedBlock.Compartments.SelectMany(c => c.Values).Where(v => !double.IsNaN(v)).Min();

        // 1E**に合わせる
        ContourMax = Math.Pow(10, Math.Ceiling(Math.Log10(max)));
        ContourMin = Math.Pow(10, Math.Floor(Math.Log10(min)));
    }

    /// <summary>
    /// 出力データからタイムステップを設定する。
    /// </summary>
    private void SetSteps()
    {
        var anySteps = TimeSteps.Count != 0;
        CurrentTimeIndex = anySteps ? 0 : -1;
        CurrentTimeStep = anySteps ? TimeSteps[CurrentTimeIndex] : 0;
    }

    /// <summary>
    /// 現在の出力タイムステップにおける計算値を取得し、モデルの図の値を設定する。
    /// </summary>
    public void SetValues()
    {
        if (SelectedBlock is null)
            return;
        if (TimeSteps.Count == 0)
            return;

        DataValues.Clear();
        OrganValues = unsetOrganValues;

        foreach (var organ in organs)
        {
            actualOrganValues[organ] = 0;
        }

        var compartments = SelectedBlock.Compartments;
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
        if (SelectedBlock is null)
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
