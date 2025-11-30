using System.Collections.ObjectModel;
using System.IO;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FlexID.Calc;
using Microsoft.Win32;

namespace FlexID.Viewer.ViewModels;

public partial class MainViewModel : ObservableObject
{
    /// <summary>
    /// コンストラクタ。
    /// </summary>
    public MainViewModel()
    {
    }

    /// <summary>
    /// コンター表示用のViewModel。
    /// </summary>
    public ContourViewModel Contour { get; } = new();

    /// <summary>
    /// グラフ表示用のViewModel。
    /// </summary>
    public GraphViewModel Graph { get; } = new();

    /// <summary>
    /// 出力ファイルのパス文字列。
    /// </summary>
    [ObservableProperty]
    public partial string OutputFilePath { get; set; } = "";

    /// <summary>
    /// 出力ファイルの選択処理。
    /// </summary>
    [RelayCommand]
    private void SelectOutputFilePath(string[] paths)
    {
        var selected = paths?[0];
        if (selected is null)
        {
            var dialog = new OpenFileDialog();
            dialog.ShowDialog();

            if (dialog.FileName != "")
                selected = dialog.FileName;
        }
        if (selected is string path)
            OutputFilePath = path;
    }

    /// <summary>
    /// 表示中の出力ファイルのデータ。
    /// </summary>
    [ObservableProperty]
    public partial OutputData SelectedOutput { get; set; }

    /// <summary>
    /// 出力ファイルリストのベースとなるパス文字列。
    /// </summary>
    private string BasePath { get; set; }

    /// <summary>
    /// 検出された出力ファイル種別のリスト。
    /// </summary>
    public ObservableCollection<OutputType> OutputTypes { get; } = [];

    /// <summary>
    /// 選択された出力ファイル種別。
    /// </summary>
    [ObservableProperty]
    public partial OutputType? SelectedOutputType { get; set; }

    /// <summary>
    /// 表示中の核種のデータ。
    /// </summary>
    [ObservableProperty]
    public partial OutputBlockData SelectedBlock { get; set; }

    /// <summary>
    /// 出力ファイルの読み込み。
    /// </summary>
    /// <param name="path">ファイルのパス文字列。</param>
    /// <returns>ファイルパスが有効でなかったり、読み込みに失敗した場合は <see langword="null"/> を返す。</returns>
    private static OutputData ReadOutputData(string path)
    {
        if (!string.IsNullOrWhiteSpace(path))
        {
            try
            {
                using var reader = new OutputDataReader(path);
                return reader.Read();
            }
            catch (Exception ex) when (ex is IOException or InvalidDataException) { }
        }
        return null;
    }

    /// <summary>
    /// 出力ファイルのパスから、出力ファイル種別を示す接尾辞を除いた部分をフルパスで返す。
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    private static string GetBasePath(string path)
    {
        if (string.IsNullOrEmpty(path))
            return null;

        static string StripEndsWith(string value, string pattern)
        {
            if (value.EndsWith(pattern))
                value = value.Substring(0, value.Length - pattern.Length);
            return value;
        }

        path = StripEndsWith(path, "_Retention.out");
        path = StripEndsWith(path, "_Cumulative.out");
        path = StripEndsWith(path, "_Dose.out");
        path = StripEndsWith(path, "_DoseRate.out");
        try
        {
            return Path.GetFullPath(path);
        }
        catch
        {
            return null;
        }
    }

    /// <summary>
    /// 出力ファイル種別に対応する接尾辞を返す。
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    /// <exception cref="NotSupportedException"></exception>
    private static string GetSuffix(OutputType type)
    {
        switch (type)
        {
            case OutputType.RetentionActivity:  /**/return "Retention";
            case OutputType.CumulativeActivity: /**/return "Cumulative";
            case OutputType.Dose:               /**/return "Dose";
            case OutputType.DoseRate:           /**/return "DoseRate";

            default:
            case OutputType.Unknown:
                throw new NotSupportedException();
        }
    }

    private static readonly IReadOnlyList<OutputType> candidateTypes =
    [
        OutputType.RetentionActivity,
        OutputType.CumulativeActivity,
        OutputType.Dose,
        OutputType.DoseRate,
    ];

    partial void OnOutputFilePathChanged(string value)
    {
        SelectedOutput = ReadOutputData(value);
        if (SelectedOutput is null)
        {
            BasePath = null;
            OutputTypes.Clear();
            return;
        }

        // 設定された出力ファイル名から表示可能な出力データ群を列挙する。
        var newBasePath = GetBasePath(value);
        if (newBasePath is not null && newBasePath != BasePath)
        {
            OutputTypes.Replace(candidateTypes
                .Where(t => File.Exists($"{newBasePath}_{GetSuffix(t)}.out")));
        }
        BasePath = newBasePath;

        SelectedOutputType = SelectedOutput.Type;
    }

    partial void OnSelectedOutputTypeChanged(OutputType? value)
    {
        SelectedBlock = null;

        if (value is not OutputType t)
            return;
        if (!OutputTypes.Contains(t))
            return;

        var block = SelectedBlock;
        var tstep = Contour.CurrentTimeStep;

        // 見つかっている出力データ群から表示対象を設定する。
        OutputFilePath = $"{BasePath}_{GetSuffix(t)}.out";

        // 直前に選択されていたものと同名のヘッダーを持つ
        // ブロックデータが存在する場合は、これを優先して再選択する。
        if (block is not null)
            block = SelectedOutput?.Blocks.FirstOrDefault(b => b.Header == block.Header);
        if (block is null)
            block = SelectedOutput?.Blocks.FirstOrDefault();

        SelectedBlock = block;
        Contour.CurrentTimeStep = tstep;
    }

    partial void OnSelectedBlockChanged(OutputBlockData value)
    {
        // コンター表示とグラフ表示を更新する。
        Contour.SetBlock(SelectedOutput, value);
        Graph.SetBlock(SelectedOutput, value);
    }
}
