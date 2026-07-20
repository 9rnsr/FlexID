using System.Diagnostics;
using System.Text.RegularExpressions;

namespace FlexID;

#nullable disable

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
    /// 流入元からの移行割合[-]
    /// ・核種が変わらない場合は、移行速度[/day]
    /// ・核種が壊変する場合は、親核種の崩壊定数[/day] * 娘核種の分岐比[-]
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
    /// 蓄積コンパートメントでは0以上の値を取る。
    /// 混合コンパートメントでは計算の都合上1.0を設定する。
    /// その他のコンパートメントでは使用されない(0.0が入る)。
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
    /// 子孫核種の場合は<see langword="true"/>。
    /// </summary>
    public bool IsProgeny { get; set; }

    /// <summary>
    /// 安定核種の場合は<see langword="true"/>。
    /// </summary>
    public bool IsStable => Lambda == 0;

    /// <summary>
    /// 動態モデルでコンパートメントとして定義されておらず線源領域Otherの一部として取り扱う
    /// 各線源領域の名称。
    /// </summary>
    public List<string> OtherSourceRegions;

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
        "OtherContainsMineralBone"
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
    public IReadOnlyList<double> TargetWeights;

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
        "OtherContainsMineralBone",
        "PrintCompartments",
        "PrintTransfers",
        "PrintScoefficients",
        "OutputDose",
        "OutputDoseRate",
        "OutputRetention",
        "OutputCumulative",
        "OutputAtoms",
    ];

    internal bool TryGetBooleanParameter(string name, bool defaultValue) =>
        Parameters.TryGetValue(name, out var str) && bool.TryParse(str, out var value) ? value : defaultValue;

    public bool PrintCompartments
    {
        get => TryGetBooleanParameter("PrintCompartments", false);
        set => Parameters["PrintCompartments"] = value.ToString();
    }

    public bool PrintTransfers
    {
        get => TryGetBooleanParameter("PrintTransfers", false);
        set => Parameters["PrintTransfers"] = value.ToString();
    }

    public bool PrintScoefficients
    {
        get => TryGetBooleanParameter("PrintScoefficients", false);
        set => Parameters["PrintScoefficients"] = value.ToString();
    }

    /// <summary>
    /// 線量の計算結果をファイルに出力する場合は <see langword="true"/>。
    /// </summary>
    public bool OutputDose
    {
        get => TryGetBooleanParameter("OutputDose", true);
        set => Parameters["OutputDose"] = value.ToString();
    }

    /// <summary>
    /// 線量率の計算結果をファイルに出力する場合は <see langword="true"/>。
    /// </summary>
    public bool OutputDoseRate
    {
        get => TryGetBooleanParameter("OutputDoseRate", true);
        set => Parameters["OutputDoseRate"] = value.ToString();
    }

    /// <summary>
    /// 残留放射能の計算結果をファイルに出力する場合は <see langword="true"/>。
    /// </summary>
    public bool OutputRetention
    {
        get => TryGetBooleanParameter("OutputRetention", true);
        set => Parameters["OutputRetention"] = value.ToString();
    }

    /// <summary>
    /// 積算放射能の計算結果をファイルに出力する場合は <see langword="true"/>。
    /// </summary>
    public bool OutputCumulative
    {
        get => TryGetBooleanParameter("OutputCumulative", true);
        set => Parameters["OutputCumulative"] = value.ToString();
    }

    /// <summary>
    /// 計算結果として原子数をファイルに出力する場合は <see langword="true"/>。
    /// </summary>  
    public bool OutputAtoms
    {
        get => TryGetBooleanParameter("OutputAtoms", false);
        set => Parameters["OutputAtoms"] = value.ToString();
    }

    private static readonly Regex patternBar = new("^-+$", RegexOptions.Compiled);

    public static bool IsBar(string s) => patternBar.IsMatch(s);
}

#nullable restore

/// <summary>
/// 組織加重係数データを保持する。
/// </summary>
public class TissueWeightData
{
    public IReadOnlyList<string> TargetRegions { get; }
    public IReadOnlyList<double> TargetWeights { get; }

    private TissueWeightData(IReadOnlyList<string> ts, IReadOnlyList<double> ws)
    {
        TargetRegions = ts;
        TargetWeights = ws;
    }

    /// <summary>
    /// 組織加重係数データを読み込む。
    /// </summary>
    /// <param name="fileName"></param>
    /// <returns></returns>
    public static TissueWeightData Read(string fileName)
    {
        var targets = new List<string>();
        var weights = new List<double>();

        var fileLines = File.ReadLines(fileName);
        foreach (var line in fileLines.Skip(1))  // 1行目は読み飛ばす
        {
            var values = line.Split(StringSplitOptions.RemoveEmptyEntries);
            var target = values[0];
            var weight = double.Parse(values[1]);

            targets.Add(target);
            weights.Add(weight);
        }

        return new TissueWeightData(targets, weights);
    }
}

public record struct Location
{
    /// <summary>
    /// ファイルパス。
    /// </summary>
    public string? FilePath;

    /// <summary>
    /// 行番号(1始まり)。
    /// </summary>
    public int LineNum;

    public override string ToString()
    {
        var location = FilePath ?? "";
        if (LineNum > 0)
            location += $"({LineNum})";
        if (location.Length == 0)
            return "Error";
        return location;
    }
}

/// <summary>
/// インプット読み込み中に見つかったエラーの集合を保持する。
/// </summary>
public class InputErrors
{
    private readonly List<(Location Loc, string Message)> errors = [];

    /// <summary>
    /// 現在保持されているエラーの数を取得する。
    /// </summary>
    public int Count => errors.Count;

    public void AddError(Location loc, string message) => errors.Add((loc, message));

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
    public InputErrorsException(Location loc, string message)
        : this([(loc, message)])
    {
    }

    public InputErrorsException(IReadOnlyList<(Location Loc, string Message)> errors)
        : base(string.Join("\n", CreateMessageFromErrors(errors)))
    {
        Errors = errors;
    }

    public IReadOnlyList<(Location Loc, string Message)> Errors { get; }

    public IEnumerable<string> ErrorLines => CreateMessageFromErrors(Errors);

    internal static IEnumerable<string> CreateMessageFromErrors(IEnumerable<(Location Loc, string Message)> errors) =>
        errors.Select(x => $"{x.Loc}: {x.Message}");
}
