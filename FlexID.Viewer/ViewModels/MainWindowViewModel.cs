using System.Collections.ObjectModel;
using System.IO;
using System.Reactive.Linq;
using FlexID.Calc;
using Microsoft.Win32;
using Prism.Mvvm;
using Reactive.Bindings;

namespace FlexID.Viewer.ViewModels;

public class MainWindowViewModel : BindableBase
{
    /// <summary>
    /// コンストラクタ。
    /// </summary>
    public MainWindowViewModel()
    {
        OutputFilePath.Subscribe(OnOutputFilePathChanged);

        SelectOutputFilePathCommand = new ReactiveCommandSlim<string[]>().WithSubscribe(SelectOutputFilePath);

        SelectedOutputType.Subscribe(OnSelectedOutputTypeChanged);

        SelectedBlock.Subscribe(OnSelectedBlockChanged);
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
    public ReactivePropertySlim<string> OutputFilePath { get; } = new("");

    /// <summary>
    /// 出力ファイルの選択処理。
    /// </summary>
    public ReactiveCommandSlim<string[]> SelectOutputFilePathCommand { get; }

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
            OutputFilePath.Value = path;
    }

    /// <summary>
    /// 表示中の出力ファイルのデータ。
    /// </summary>
    public ReactivePropertySlim<OutputData> SelectedOutput { get; } = new();

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
    public ReactivePropertySlim<OutputType?> SelectedOutputType { get; } = new();

    /// <summary>
    /// 表示中の核種のデータ。
    /// </summary>
    public ReactivePropertySlim<OutputBlockData> SelectedBlock { get; } = new();

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

    private void OnOutputFilePathChanged(string value)
    {
        SelectedOutput.Value = ReadOutputData(value);
        if (SelectedOutput.Value is null)
        {
            BasePath = null;
            OutputTypes.Clear();
            return;
        }

        // 設定された出力ファイル名から表示可能な出力データ群を列挙する。
        var newBasePath = GetBasePath(value);
        if (newBasePath is not null && newBasePath != BasePath)
        {
            OutputTypes.Clear();
            OutputTypes.AddRange(candidateTypes
                .Where(t => File.Exists($"{newBasePath}_{GetSuffix(t)}.out")));
        }
        BasePath = newBasePath;

        SelectedOutputType.Value = SelectedOutput.Value.Type;
    }

    private void OnSelectedOutputTypeChanged(OutputType? value)
    {
        SelectedBlock.Value = null;

        if (value is not OutputType t)
            return;
        if (!OutputTypes.Contains(t))
            return;

        var block = SelectedBlock.Value;
        var tstep = Contour.CurrentTimeStep;

        // 見つかっている出力データ群から表示対象を設定する。
        OutputFilePath.Value = $"{BasePath}_{GetSuffix(t)}.out";

        // 直前に選択されていたものと同名のヘッダーを持つ
        // ブロックデータが存在する場合は、これを優先して再選択する。
        if (block is not null)
            block = SelectedOutput.Value?.Blocks.FirstOrDefault(b => b.Header == block.Header);
        if (block is null)
            block = SelectedOutput.Value?.Blocks.FirstOrDefault();

        SelectedBlock.Value = block;
        Contour.CurrentTimeStep = tstep;
    }

    public void OnSelectedBlockChanged(OutputBlockData value)
    {
        // コンター表示とグラフ表示を更新する。
        Contour.SetBlock(SelectedOutput.Value, value);
        Graph.SetBlock(SelectedOutput.Value, value);
    }
}
