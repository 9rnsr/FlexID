using System.Diagnostics;
using System.Text.RegularExpressions;

namespace FlexID.Calc;

/// <summary>
/// 流入経路を表現する
/// </summary>
public class Inflow
{
    /// <summary>
    /// 流入元の臓器番号
    /// </summary>
    public int ID;

    /// <summary>
    /// 流入元の臓器
    /// </summary>
    public Organ Organ;

    /// <summary>
    /// 流入割合
    /// </summary>
    public double Rate;

    public override string ToString() => $"{Organ} =>{Rate}";
}

/// <summary>
/// 臓器機能。
/// </summary>
public enum OrganFunc
{
    /// <summary>
    /// 入力。
    /// </summary>
    inp,

    /// <summary>
    /// 蓄積。
    /// </summary>
    acc,

    /// <summary>
    /// 混合。
    /// </summary>
    mix,

    /// <summary>
    /// 排泄。
    /// </summary>
    exc,
}

/// <summary>
/// 臓器(コンパートメント)を表現する。
/// </summary>
[DebuggerDisplay(@"\{{Func}, {ToString(),nq}\}")]
public class Organ
{
    /// <summary>
    /// 臓器が対象とする核種。
    /// </summary>
    public NuclideData Nuclide;

    /// <summary>
    /// 崩壊定数[/day]。(＝ ln(2) / 半減期[day])
    /// </summary>
    public double NuclideDecay => Nuclide.Lambda;

    /// <summary>
    /// 臓器番号。
    /// </summary>
    public int ID;

    /// <summary>
    /// 臓器毎のデータを配列から引くためのインデックス。
    /// </summary>
    public int Index;

    /// <summary>
    /// 臓器名。
    /// </summary>
    public string Name;

    /// <summary>
    /// 臓器機能。
    /// </summary>
    public OrganFunc Func;

    /// <summary>
    /// 生物学的崩壊定数[/day]。
    /// 蓄積コンパートメントのみで意味を持ち、それ以外では1.0となる。
    /// </summary>
    public double BioDecay;

    /// <summary>
    /// 流入経路。
    /// </summary>
    public List<Inflow> Inflows = [];

    /// <summary>
    /// コンパートメントへの流入がない場合に<see langword="true"/>。
    /// </summary>
    public bool IsZeroInflow;

    /// <summary>
    /// コンパートメントからの流出が即時に処理される場合に<see langword="true"/>。
    /// </summary>
    public bool IsInstantOutflow => Func == OrganFunc.inp || Func == OrganFunc.mix;

    /// <summary>
    /// コンパートメントが壊変経路のために自動定義された場合に<see langword="true"/>。
    /// </summary>
    public bool IsDecayCompartment;

    /// <summary>
    /// excコンパートメントにおいて、OIRにおける24-hour smaple値を模擬した
    /// 残留放射能を出力する場合に <see langword="true"/>。
    /// </summary>
    public bool IsExcretaCompatibleWithOIR;

    /// <summary>
    /// 線源領域の名称。
    /// </summary>
    public string SourceRegion;

    /// <summary>
    /// コンパートメントに対応付けられた線源領域から各標的領域へのS係数(男性)。
    /// </summary>
    public double[] S_CoefficientsM;

    /// <summary>
    /// コンパートメントに対応付けられた線源領域から各標的領域へのS係数(女性)。
    /// </summary>
    public double[] S_CoefficientsF;

    public override string ToString() => $"{Nuclide}/{Name}";
}

public class NuclideData
{
    /// <summary>
    /// 核種のデータを配列から引くためのインデックス。
    /// </summary>
    public int Index;

    /// <summary>
    /// 核種名。
    /// </summary>
    public string Name;

    /// <summary>
    /// 半減期(単位付き)。
    /// </summary>
    public string HalfLife;

    /// <summary>
    /// 崩壊定数λ[/day]。(＝ ln(2) / 半減期[day])
    /// </summary>
    public double Lambda;

    /// <summary>
    /// 娘核種への分岐比(100%＝1.00と置いた比で持つ)。
    /// </summary>
    public (NuclideData Daughter, double Fraction)[] Branches;

    /// <summary>
    /// 子孫核種の場合は<c>true</c>。
    /// </summary>
    public bool IsProgeny { get; set; }

    /// <summary>
    /// S係数データにおける各線源領域の名称。
    /// </summary>
    public string[] SourceRegions;

    /// <summary>
    /// 動態モデルでコンパートメントとして定義されておらず線源領域Otherの一部として取り扱う
    /// 各線源領域の名称。
    /// </summary>
    public string[] OtherSourceRegions;

    /// <summary>
    /// 消化管に対応するコンパートメント群のインデックスと寄与率。
    /// </summary>
    public (int Index, double Rate)[] AtractIndexes = [];

    /// <summary>
    /// 肺に対応するコンパートメント群のインデックスと寄与率。
    /// </summary>
    public (int Index, double Rate)[] LungsIndexes = [];

    /// <summary>
    /// 骨格に対応するコンパートメント群のインデックスと寄与率。
    /// </summary>
    public (int Index, double Rate)[] SkeletonIndexes = [];

    /// <summary>
    /// 肝臓に対応するコンパートメント群のインデックスと寄与率。
    /// </summary>
    public (int Index, double Rate)[] LiverIndexes = [];

    /// <summary>
    /// 甲状腺に対応するコンパートメント群のインデックスと寄与率。
    /// </summary>
    public (int Index, double Rate)[] ThyroidIndexes = [];

    /// <summary>
    /// パラメータ定義。
    /// </summary>
    public Dictionary<string, string> Parameters;

    /// <summary>
    /// 有効なパラメータ名の配列。
    /// </summary>
    public static readonly string[] ParameterNames =
    [
        "ExcludeOtherSourceRegions",
        "IncludeOtherSourceRegions"
    ];

    public override string ToString() => Name;
}

public class InputData
{
    /// <summary>
    /// インプットのタイトル文字列。
    /// </summary>
    public string Title;

    // このコンパートメントモデルが対象とする被ばく評価期間の開始年齢[day]。
    public int StartAge;

    /// <summary>
    /// 全ての核種。
    /// </summary>
    public List<NuclideData> Nuclides = [];

    /// <summary>
    /// SAFデータにおける各線源領域の情報。
    /// </summary>
    public SourceRegionData[] SourceRegions;

    /// <summary>
    /// SAFデータにおける各標的領域の名称。
    /// </summary>
    public string[] TargetRegions;

    /// <summary>
    /// 組織加重係数データにおける各標的領域の係数。
    /// </summary>
    public double[] TargetWeights;

    /// <summary>
    /// 核種毎のS係数データ表(男性)。
    /// </summary>
    public List<Dictionary<string, double[]>> SCoeffTablesM = [];

    /// <summary>
    /// 核種毎のS係数データ表(女性)。
    /// </summary>
    public List<Dictionary<string, double[]>> SCoeffTablesF = [];

    /// <summary>
    /// 全ての臓器。
    /// </summary>
    public List<Organ> Organs = [];

    /// <summary>
    /// パラメータ定義。
    /// </summary>
    public Dictionary<string, string> Parameters;

    /// <summary>
    /// 有効なパラメータ名の配列。
    /// </summary>
    public static readonly string[] ParameterNames =
    [
        "ExcludeOtherSourceRegions",
        "IncludeOtherSourceRegions",
        "OutputDose",
        "OutputDoseRate",
        "OutputRetention",
        "OutputCumulative",
    ];

    public bool TryGetBooleanParameter(string name, bool defaultValue) =>
        Parameters.TryGetValue(name, out var str) && bool.TryParse(str, out var value) ? value : defaultValue;

    /// <summary>
    /// 線量の計算結果をファイルに出力する場合は <see langword="true"/>。
    /// </summary>
    public bool OutputDose { get; set; } = true;

    /// <summary>
    /// 線量率の計算結果をファイルに出力する場合は <see langword="true"/>。
    /// </summary>
    public bool OutputDoseRate { get; set; } = true;

    /// <summary>
    /// 残留放射能の計算結果をファイルに出力する場合は <see langword="true"/>。
    /// </summary>
    public bool OutputRetention { get; set; } = true;

    /// <summary>
    /// 積算放射能の計算結果をファイルに出力する場合は <see langword="true"/>。
    /// </summary>
    public bool OutputCumulative { get; set; } = true;
}

public abstract class InputDataReaderBase : IDisposable
{
    /// <summary>
    /// インプットファイルの読み出し用TextReader。
    /// </summary>
    private readonly StreamReader reader;

    /// <summary>
    /// 子孫核種のインプットを読み飛ばす場合は<c>true</c>。
    /// </summary>
    protected bool CalcProgeny { get; }

    /// <summary>
    /// 行番号(1始まり)。
    /// </summary>
    protected int LineNum { get; private set; }

    /// <summary>
    /// コンストラクタ。
    /// </summary>
    /// <param name="reader">インプットの読み込み元。</param>
    /// <param name="calcProgeny">子孫核種を計算する＝読み込む場合は <see langword="true"/>。</param>
    public InputDataReaderBase(StreamReader reader, bool calcProgeny = true)
    {
        this.reader = reader;
        this.CalcProgeny = calcProgeny;
    }

    public void Dispose() => reader.Dispose();

    /// <summary>
    /// 読み取り位置をファイル先頭に戻す。
    /// </summary>
    protected void ResetPosition()
    {
        reader.BaseStream.Position = 0;
        reader.DiscardBufferedData();
        LineNum = 0;
    }

    /// <summary>
    /// インプットの次行を読み取る。
    /// </summary>
    /// <returns></returns>
    protected virtual string GetNextLine()
    {
    Lagain:
        var line = reader.ReadLine();
        LineNum++;
        if (line is null)
            return null;
        line = line.Trim();

        // 空行を読み飛ばす。
        if (line.Length == 0)
            goto Lagain;

        // コメント行を読み飛ばす。
        if (line.StartsWith("#"))
            goto Lagain;

        // 行末コメントを除去する。
        var trailingComment = line.IndexOf("#");
        if (trailingComment != -1)
            line = line.Substring(0, trailingComment).TrimEnd();
        return line;
    }

    /// <summary>
    /// 組織加重係数データを読み込む。
    /// </summary>
    /// <param name="fileName"></param>
    /// <returns></returns>
    public static (string[] targets, double[] weights) ReadTissueWeights(string fileName)
    {
        var targets = new List<string>();
        var weights = new List<double>();

        var fileLines = File.ReadLines(fileName);
        foreach (var line in fileLines.Skip(1))  // 1行目は読み飛ばす
        {
            var values = line.Split([" "], StringSplitOptions.RemoveEmptyEntries);
            var target = values[0];
            var weight = double.Parse(values[1]);

            targets.Add(target);
            weights.Add(weight);
        }

        return (targets.ToArray(), weights.ToArray());
    }

    /// <summary>
    /// 核種名に合致する正規表現。
    /// 準安定核種について、一般的な表記(m1, m2)とICRP-07データのもの(m, n)の両方を受け付けるようにしている。
    /// </summary>
    protected static readonly Regex patternNuclide = new(@"^[A-Za-z]+-\d+(?:[a-z]|m\d)?$", RegexOptions.Compiled);

    private static readonly Regex patternBar = new("^-+$", RegexOptions.Compiled);

    protected static bool IsBar(string s) => patternBar.IsMatch(s);
}

/// <summary>
/// インプット読み込み中に見つかったエラーの集合を保持する。
/// </summary>
public class InputErrors
{
    private readonly List<(int LineNum, string Message)> errors = [];

    /// <summary>
    /// 現在保持されているエラーの数を取得する。
    /// </summary>
    public int Count => errors.Count;

    public void AddError(string message) => errors.Add((0, message));

    public void AddError(int lineNum, string message) => errors.Add((lineNum, message));

    public void AddErrors(InputErrorsException exception) => errors.AddRange(exception.Errors);

    public bool IfAny(int olderrors = 0) => errors.Count > olderrors;

    public void RaiseIfAny(int olderrors = 0)
    {
        if (IfAny(olderrors))
            throw new InputErrorsException(errors.ToArray());
    }

    public void Clear() => errors.Clear();
}

/// <summary>
/// インプット読み込み処理で見つかったエラーを例外として上位層に送出する。
/// </summary>
public class InputErrorsException : ApplicationException
{
    public InputErrorsException(int lineNum, string message)
        : this([(lineNum, message)])
    {
    }

    public InputErrorsException(IReadOnlyList<(int LineNum, string Message)> errors)
        : base(string.Join("\n", CreateMessageFromErrors(errors)))
    {
        Errors = errors;
    }

    public IReadOnlyList<(int LineNum, string Message)> Errors { get; }

    public IEnumerable<string> ErrorLines => CreateMessageFromErrors(Errors);

    internal static IEnumerable<string> CreateMessageFromErrors(IEnumerable<(int LineNum, string Message)> errors) =>
        errors.Select(x => x.LineNum < 1 ? x.Message : $"Line {x.LineNum}: {x.Message}");
}
