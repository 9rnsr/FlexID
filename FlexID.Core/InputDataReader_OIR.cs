using System.Diagnostics;
using System.Text;

namespace FlexID;

/// <summary>
/// OIR用インプットファイルの読み取り処理。
/// </summary>
public class InputDataReader_OIR : InputDataReaderBase
{
    private static readonly SAFData safdataAM;
    private static readonly SAFData safdataAF;

    static InputDataReader_OIR()
    {
        safdataAM = SAFDataReader.ReadSAF(Sex.Male);
        safdataAF = SAFDataReader.ReadSAF(Sex.Female);
    }

    /// <summary>
    /// $if ～ $else ～ $endif指令の挙動を制御する状態フラグ。
    /// </summary>
    enum ConditionDirectiveState
    {
        Disabled = -1,          /// <summary>セクション内では$if ～ $endif指令の使用を禁止する。</summary>
        OutsideBlock = 0,       /// <summary>セクション内で現在行は$if ～ $endif指令の外にある。</summary>
        InsideIfBlock = 1,      /// <summary>セクション内で現在行は$ifブロックの中にある。</summary>
        InsideElseBlock = 2,    /// <summary>セクション内で現在行は$elseブロックの中にある。</summary>
    }
    private ConditionDirectiveState conditionDirectiveState = ConditionDirectiveState.Disabled;

    private bool InsideConditionDirective => conditionDirectiveState is ConditionDirectiveState.InsideIfBlock
                                                                     or ConditionDirectiveState.InsideElseBlock;

    /// <summary>
    /// $if ～ $endif指令の内部において、現在行が親核種のみを対象とする場合に<see langword="true"/>となる。
    /// </summary>
    private bool insideParentBlock;

    private bool? IsParentOnly => InsideConditionDirective ? insideParentBlock : null;

    private readonly InputErrors errors;
    private readonly Dictionary<string, int> compartmentSectionLocs = [];
    private readonly Dictionary<string, int> transferSectionLocs = [];

    private readonly Dictionary<NuclideData, InputEvaluator> inputEvaluators = [];

    private bool inputTitleRead;
    private string inputTitle = "";

    private bool inputNuclidesRead;
    private readonly List<NuclideData> inputNuclides = [];

    private bool inputParametersRead;
    private readonly List<InputParameter> inputParameters = [];

    private readonly Dictionary<string, List<InputOrgan>> inputOrgans = [];

    private bool inputIntakesRead;
    private int intakeSectionLoc;
    private readonly List<InputIntake> inputIntakes = [];

    private readonly Dictionary<string, List<InputTransfer>> inputTransfers = [];

    /// <summary>
    /// コンストラクタ。
    /// </summary>
    /// <param name="inputPath">インプットファイルのパス文字列。</param>
    /// <param name="calcProgeny">子孫核種を計算する＝読み込む場合は <see langword="true"/>。</param>
    public InputDataReader_OIR(string inputPath, bool calcProgeny = true)
        : this(new StreamReader(File.OpenRead(inputPath)), calcProgeny)
    {
    }

    /// <summary>
    /// コンストラクタ。
    /// </summary>
    /// <param name="reader">インプットの読み込み元。</param>
    /// <param name="calcProgeny">子孫核種を計算する＝読み込む場合は <see langword="true"/>。</param>
    public InputDataReader_OIR(StreamReader reader, bool calcProgeny = true)
        : base(reader, calcProgeny)
    {
        errors = new InputErrors();
    }

    protected override string? GetNextLine()
    {
        Span<Range> parts = stackalloc Range[3];

    Lagain:
        var nextLine = base.GetNextLine();

        if (nextLine is not null && nextLine.StartsWith("$"))
        {
            var count = nextLine.SplitAny(parts, [' ', '\t', '\f', '\v'], StringSplitOptions.RemoveEmptyEntries);
            if (nextLine.AsSpan()[parts[0]].SequenceEqual("$if"))
            {
                if (conditionDirectiveState == ConditionDirectiveState.Disabled)
                    goto LerrorDirective;

                if (conditionDirectiveState == ConditionDirectiveState.OutsideBlock && count == 2)
                {
                    if (nextLine.AsSpan()[parts[1]].Equals("parent", StringComparison.OrdinalIgnoreCase))
                    {
                        conditionDirectiveState = ConditionDirectiveState.InsideIfBlock;
                        insideParentBlock = true;
                        goto Lagain;
                    }
                    else if (nextLine.AsSpan()[parts[1]].Equals("progeny", StringComparison.OrdinalIgnoreCase))
                    {
                        conditionDirectiveState = ConditionDirectiveState.InsideIfBlock;
                        insideParentBlock = false;
                        goto Lagain;
                    }
                }
            }
            else if (nextLine.AsSpan()[parts[0]].SequenceEqual("$else"))
            {
                if (conditionDirectiveState == ConditionDirectiveState.Disabled)
                    goto LerrorDirective;

                if (conditionDirectiveState == ConditionDirectiveState.InsideIfBlock && count == 1)
                {
                    conditionDirectiveState = ConditionDirectiveState.InsideElseBlock;
                    insideParentBlock = !insideParentBlock;
                    goto Lagain;
                }
            }
            else if (nextLine.AsSpan()[parts[0]].SequenceEqual("$endif"))
            {
                if (conditionDirectiveState == ConditionDirectiveState.Disabled)
                    goto LerrorDirective;

                if (InsideConditionDirective)
                {
                    conditionDirectiveState = ConditionDirectiveState.OutsideBlock;
                    goto Lagain;
                }
            }
            else
            {
                goto Lcont;
            }

        LerrorDirective:
            errors.AddError(LineNum, $"Unrecognized directive line: '{nextLine}'.");
            goto Lagain;
        }

    Lcont:
        return nextLine;
    }

    private static bool CheckSectionHeader(string ln) => ln.StartsWith("[");

    private ReadOnlySpan<char> GetSectionHeader(string ln)
    {
        var olderrors = errors.Count;

        if (!CheckSectionHeader(ln))
            errors.AddError(LineNum, "Section header should start with '['");
        if (!ln.EndsWith("]"))
            errors.AddError(LineNum, "Section header should be closed with ']'.");

        // セクションヘッダが見つからないとそれ以上のエラーを報告することは
        // 難しいため、ここでインプット読み取りを終える。
        errors.RaiseIfAny(olderrors);

        return ln.AsSpan(1, ln.Length - 2).Trim();
    }

    private string? SkipUntilNextSection()
    {
        while (true)
        {
            var line = base.GetNextLine();
            if (line is null)
                return null;
            if (CheckSectionHeader(line))
                return line;
        }
    }

    public InputData ReadRough()
    {
        GetInput(isRough: true);

        var data = new InputData();
        data.Title = inputTitle;
        data.Nuclides.AddRange(inputNuclides);

        return data;
    }

    /// <summary>
    /// インプットファイルを読み込む。
    /// </summary>
    /// <returns></returns>
    public InputData Read()
    {
        GetInput(isRough: false);

        if (!inputIntakesRead)
            errors.AddError(LineNum, "Missing [intake] section.");

        foreach (var (lineNum, nuc) in inputOrgans.Keys.Except(inputNuclides.Select(n => n.Name) ?? [])
            .Select(nuc => (LineNum: compartmentSectionLocs[nuc], nuc)).OrderBy(t => t.LineNum))
        {
            errors.AddError(lineNum, $"Undefined nuclide '{nuc}' is used to define compartments.");
        }

        foreach (var (lineNum, nuc) in inputTransfers.Keys.Except(inputNuclides.Select(n => n.Name) ?? [])
            .Select(nuc => (LineNum: transferSectionLocs[nuc], nuc)).OrderBy(t => t.LineNum))
        {
            errors.AddError(lineNum, $"Undefined nuclide '{nuc}' is used to define transfers.");
        }

        {
            var nuclideNames = inputNuclides.Select(n => n.Name).ToArray();

            foreach (var nuc in nuclideNames.Where(nuc => !inputOrgans.ContainsKey(nuc)))
                errors.AddError(LineNum, $"Missing [{nuc}:compartment] section.");

            foreach (var nuc in nuclideNames.Where(nuc => !inputTransfers.ContainsKey(nuc)))
                errors.AddError(LineNum, $"Missing [{nuc}:transfer] section.");
        }

        // 核種定義に対して[compartment]と[transfer]セクションに過不足がないことを確定する。
        errors.RaiseIfAny();

        // 線源領域と標的領域のデータを読み込む。
        var sourceRegions = SAFDataReader.ReadSourceRegions();
        var targetRegions = SAFDataReader.ReadTargetRegions().Select(t => t.Name).ToArray();

        // 組織加重係数データを読み込む。
        var (ts, ws) = ReadTissueWeights(Path.Combine(AppResource.BaseDir, @"lib\OIR\wT.txt"));
        if (!targetRegions.SequenceEqual(ts))
            errors.AddError("Found mismatch of target region names on tissue weighting factor data.");

        // 外部データの読み込み処理にエラーがないことを確定する。
        errors.RaiseIfAny();

        var data = new InputData();
        data.Title = inputTitle;
        data.SourceRegions = sourceRegions;
        data.TargetRegions = targetRegions;
        data.TargetWeights = ws;

        var expander = new InputNuclideExpander(inputNuclides, errors);

        DefineParameters(data, expander);

        // 全てのコンパートメントを定義する。
        DefineCompartments(data);

        // コンパートメント間の移行経路を定義する。
        DefineIntakes(data);
        DefineTransfers(data);

        // 流入を持たないコンパートメントをマークする。
        MarkZeroInflows(data);

        return data;
    }

    private void GetInput(bool isRough)
    {
        var line = GetNextLine();
        if (line is null)
            throw Program.Error("Reach to EOF while reading input file.");

        while (true)
        {
            if (InsideConditionDirective)
            {
                errors.AddError(LineNum, "if directive block should be closed till the end of the section.");
                conditionDirectiveState = ConditionDirectiveState.OutsideBlock;
            }

            var header = GetSectionHeader(line);

            if (Ascii.EqualsIgnoreCase(header, "title"))
            {
                conditionDirectiveState = ConditionDirectiveState.Disabled;
                line = GetTitle();
            }
            else if (Ascii.EqualsIgnoreCase(header, "nuclide"))
            {
                conditionDirectiveState = ConditionDirectiveState.Disabled;
                line = GetNuclides();
            }
            else if (isRough)
            {
                if (inputTitleRead && inputNuclidesRead)
                    break;

                conditionDirectiveState = ConditionDirectiveState.OutsideBlock;
                line = SkipUntilNextSection();
            }
            else if (Ascii.EqualsIgnoreCase(header, "parameter"))
            {
                conditionDirectiveState = ConditionDirectiveState.OutsideBlock;
                line = GetParameters();
            }
            else if (header.EndsWith(":compartment", StringComparison.OrdinalIgnoreCase))
            {
                conditionDirectiveState = ConditionDirectiveState.Disabled;
                var i = header.IndexOf(':');
                var nuc = header.Slice(0, i).ToString();
                line = GetCompartments(nuc);
            }
            else if (Ascii.EqualsIgnoreCase(header, "intake"))
            {
                conditionDirectiveState = ConditionDirectiveState.Disabled;
                line = GetIntakes();
            }
            else if (header.EndsWith(":transfer", StringComparison.OrdinalIgnoreCase))
            {
                conditionDirectiveState = ConditionDirectiveState.Disabled;
                var i = header.IndexOf(':');
                var nuc = header.Slice(0, i).ToString();
                line = GetTransfers(nuc);
            }
            else
            {
                errors.AddError(LineNum, $"Unrecognized section $'[{header.ToString()}]'.");
                conditionDirectiveState = ConditionDirectiveState.OutsideBlock;
                line = SkipUntilNextSection();
            }

            if (line is null)
                break;
        }

        if (!inputTitleRead)
            errors.AddError(LineNum, "Missing [title] section.");
        if (!inputNuclidesRead)
            errors.AddError(LineNum, "Missing [nuclide] section.");

        // インプットの構文に従って読み取りができていることを確定する。
        // なお、[nuclide]セクションについては実質的な意味解析まで完了している状態となる。
        errors.RaiseIfAny();
    }

    /// <summary>
    /// タイトルの定義セクションを読み込む。
    /// </summary>
    /// <returns>セクションの次行。</returns>
    private string? GetTitle()
    {
        if (inputTitleRead)
        {
            errors.AddError(LineNum, "Duplicated [title] section.");
            return SkipUntilNextSection();
        }
        inputTitleRead = true;

        var sectionLineNum = LineNum;
        var title = GetNextLine();
        if (title is null || CheckSectionHeader(title))
        {
            errors.AddError(sectionLineNum, "Empty [title] section.");
            return title;
        }

        inputTitle = title;

        var line = GetNextLine();
        if (line is not null && !CheckSectionHeader(line))
        {
            errors.AddError(LineNum, "Unrecognized lines in [title] section.");
            return SkipUntilNextSection();
        }

        return line;
    }

    /// <summary>
    /// 核種の定義セクションを読み込む。
    /// </summary>
    /// <returns>セクションの次行。</returns>
    private string? GetNuclides()
    {
        if (inputNuclidesRead)
        {
            errors.AddError(LineNum, "Duplicated [nuclide] section.");
            return SkipUntilNextSection();
        }
        inputNuclidesRead = true;

        var olderrors = errors.Count;
        var sectionLineNum = LineNum;

        // 最初の1行を見て、新旧どちらの型式で入力されているかを判定する。
        var autoMode = default(bool?);

        Dictionary<string, IndexData>? indexTable = null;
        var branchTable = new Dictionary<NuclideData, (string Daughter, decimal Fraction)[]>();
        var nuclideLines = new Dictionary<NuclideData, int>();

        string? line;
        while (true)
        {
            line = GetNextLine();
            if (line is null)
                break;
            if (CheckSectionHeader(line))
                break;

            // 核種の定義行を読み込む。
            var values = line.Split(StringSplitOptions.RemoveEmptyEntries);

            if (autoMode == true || autoMode == default && values.All(ElementTable.PatternNuclide.IsMatch))
            {
                // インデックスファイルから娘核種と分岐比の情報を自動取得する。
                autoMode = true;

                if (indexTable is null)
                    indexTable = IndexDataReader.ReadNDX().ToDictionary(x => x.Nuclide, x => x);

                foreach (var nuc in values)
                {
                    if (!ElementTable.PatternNuclide.IsMatch(nuc))
                    {
                        errors.AddError(LineNum, $"'{nuc}' is not nuclide name.");
                        continue;
                    }
                    if (inputNuclides.Any(n => n.Name == nuc))
                    {
                        errors.AddError(LineNum, $"Duplicated nuclide definition for '{nuc}'.");
                        continue;
                    }

                    indexTable.TryGetValue(nuc, out var indexData);

                    // インデックスファイルに定義されていないものは安定核種として扱う。
                    var halfLife = indexData?.HalfLife;
                    var lambda = indexData?.Lambda;
                    var branches = indexData?.Daughters.Select(d => (d.Daughter, d.Branch)).ToArray() ?? [];

                    var nuclide = new NuclideData
                    {
                        Index = inputNuclides.Count,
                        Name = nuc,
                        HalfLife = halfLife ?? "---",
                        Lambda = lambda ?? 0.0,
                        IsProgeny = inputNuclides.Count > 0,
                    };
                    inputNuclides.Add(nuclide);

                    branchTable[nuclide] = branches;
                    nuclideLines[nuclide] = LineNum;
                }
            }
            else
            {
                // 崩壊定数、娘核種、および分岐比を直接指定する。
                autoMode = false;

                if (values.Length < 2)
                {
                    errors.AddError(LineNum, "Nuclide definition should have at least 2 values.");
                    continue;
                }

                var nuc = values[0];
                if (inputNuclides.Any(n => n.Name == nuc))
                {
                    errors.AddError(LineNum, $"Duplicated nuclide definition for '{nuc}'.");
                    continue;
                }

                if (!double.TryParse(values[1], out var lambda))
                {
                    errors.AddError(LineNum, "Cannot get nuclide Lambda.");
                    continue;
                }
                if (lambda < 0)
                {
                    errors.AddError(LineNum, "Nuclide Lambda should be positive.");
                    continue;
                }

                var nuclide = new NuclideData
                {
                    Index = inputNuclides.Count,
                    Name = nuc,
                    Lambda = lambda,
                    IsProgeny = inputNuclides.Count > 0,
                };
                inputNuclides.Add(nuclide);

                var branches = new (string Daughter, decimal Fraction)[values.Length - 2];
                for (int i = 0; i < branches.Length; i++)
                {
                    var part = values[i + 2];

                    var iSep = part.IndexOf('/');
                    if (iSep == -1)
                    {
                        errors.AddError(LineNum, "Daughter name and branching fraction should be separated with '/'.");
                        continue;
                    }
                    if (iSep == 0)
                    {
                        errors.AddError(LineNum, "Daughter name should not be empty.");
                        continue;
                    }

                    var daughter = part.Substring(0, iSep);
                    var frac = part.Substring(iSep + 1);

                    if (!decimal.TryParse(frac, out var fraction))
                    {
                        errors.AddError(LineNum, "Cannot get branching fraction.");
                        continue;
                    }
                    if (fraction < 0)
                    {
                        errors.AddError(LineNum, "Branching fraction should be positive.");
                        continue;
                    }

                    branches[i] = (daughter, fraction);
                }
                branchTable[nuclide] = branches;
                nuclideLines[nuclide] = LineNum;
            }
        }
        if (errors.IfAny(olderrors))
            return line;
        if (!inputNuclides.Any())
        {
            errors.AddError(sectionLineNum, "Empty [nuclide] section.");
            return line;
        }

        // 壊変する娘核種とその分岐比を設定する。
        foreach (var entry in branchTable)
        {
            var nuclide = entry.Key;
            var lineNum = nuclideLines[nuclide];
            var branches = entry.Value;

            NuclideData? GetNuclide(string nuc)
                => inputNuclides.FirstOrDefault(n => n.Name == nuc);

            if (autoMode == true)
            {
                // インデックスファイルには定義されているが、インプットでは
                // 計算対象として指定されていない娘核種への分岐を除去する。
                nuclide.Branches = branches
                    .Select(b => (Daughter: GetNuclide(b.Daughter), (double)b.Fraction))
                    .Where(b => b.Daughter != null).ToArray();
            }
            else
            {
                // 全ての娘核種の名前が[nuclide]セクションで定義されていることを確認する。
                nuclide.Branches = branches.Select(b =>
                {
                    var daughter = GetNuclide(b.Daughter);
                    if (daughter is null)
                        errors.AddError(lineNum, $"Nuclide '{nuclide.Name}' defines a branch to undefined daughter '{b.Daughter}'.");
                    return (Daughter: daughter, (double)b.Fraction);
                }).Where(b => b.Daughter != null).ToArray();
            }
        }

        // 崩壊系列が適切であることを確認する。
        var root = inputNuclides.FirstOrDefault();
        foreach (var nuclide in inputNuclides)
        {
            var tested = new List<NuclideData>();

            CycleCheck(nuclide);

            void CycleCheck(NuclideData target)
            {
                var lineNum = nuclideLines[target];

                foreach (var (daughter, _) in target.Branches)
                {
                    // 自分自身へ壊変する経路が存在しないことを確認する。
                    if (daughter == target)
                    {
                        errors.AddError(lineNum, $"Nuclide '{target.Name}' has a decay path to itself.");
                        continue;
                    }

                    // 親核種に壊変する核種が存在しないことを確認する。
                    if (daughter == root)
                    {
                        errors.AddError(lineNum, $"Nuclide '{target.Name}' has a decay path to parent nuclide '{root.Name}'.");
                        continue;
                    }

                    // 循環する壊変経路がないことを確認する。
                    if (daughter == nuclide)
                    {
                        errors.AddError(nuclideLines[nuclide], $"Nuclide '{nuclide.Name}'  has a cyclic decay path starting from '{target.Name}'.");
                        continue;
                    }

                    if (tested.Contains(daughter))
                        continue;
                    tested.Add(daughter);

                    CycleCheck(daughter);
                }
            }
        }

        return line;
    }

    /// <summary>
    /// インプットから読み取ったパラメータ/変数定義の情報を保持する。
    /// </summary>
    /// <param name="LineNum">行番号。</param>
    /// <param name="ParentOnly">親核種に対してのみ定義される場合は<see langword="true"/>。</param>
    /// <param name="IsVarDecl">変数定義の場合は<see langword="true"/>。</param>
    /// <param name="Nuclide">核種名。</param>
    /// <param name="Name">パラメータ/変数名。</param>
    /// <param name="Value">パラメータ/変数値。</param>
    private record class InputParameter(int LineNum, bool? ParentOnly, bool IsVarDecl, string Nuclide, string Name, string Value);

    /// <summary>
    /// パラメーターの定義セクションを読み込む。
    /// </summary>
    /// <returns>セクションの次行。</returns>
    private string? GetParameters()
    {
        if (inputParametersRead)
        {
            errors.AddError(LineNum, "Duplicated [parameter] section.");
            return SkipUntilNextSection();
        }
        inputParametersRead = true;

        string? line;
        while (true)
        {
            line = GetNextLine();
            if (line is null)
                break;
            if (CheckSectionHeader(line))
                break;

            var values = line.Split(["="], 2, StringSplitOptions.RemoveEmptyEntries);
            if (values.Length != 2)
            {
                errors.AddError(LineNum, "Parameter definition should have 2 values.");
                continue;
            }

            var name = values[0].Trim();
            var value = values[1].Trim();
            var parentOnly = IsParentOnly;

            var isVarDecl = name.StartsWith('$');
            if (isVarDecl)
                name = name[1..];
            else if (InsideConditionDirective)
            {
                var directive = conditionDirectiveState is ConditionDirectiveState.InsideIfBlock ? "$if" : "$else";
                errors.AddError(LineNum, $"Parameter definition cannot be placed in {directive} directive block.");
                continue;
            }

            var nuc = "";
            if (name.IndexOf('/') is int j && j != -1)
            {
                nuc = name[..j];
                name = name[(j + 1)..];
            }

            inputParameters.Add(new InputParameter(LineNum, parentOnly, isVarDecl, nuc, name, value));
        }

        return line;
    }

    /// <summary>
    /// コンパートメントの定義セクションを読み込む。
    /// </summary>
    /// <param name="nuc"></param>
    /// <returns>セクションの次行。</returns>
    private string? GetCompartments(string nuc)
    {
        if (inputOrgans.TryGetValue(nuc, out var organs))
        {
            errors.AddError(LineNum, $"Duplicated [{nuc}:compartment] section.");
            return SkipUntilNextSection();
        }
        inputOrgans.Add(nuc, organs = []);

        var olderrors = errors.Count;
        var sectionLineNum = LineNum;
        compartmentSectionLocs[nuc] = sectionLineNum;

        string? line;
        while (true)
        {
            line = GetNextLine();
            if (line is null)
                break;
            if (CheckSectionHeader(line))
                break;

            var values = line.Split(StringSplitOptions.RemoveEmptyEntries);

            if (values.Length != 3)
            {
                errors.AddError(LineNum, "Compartment definition should have 3 values.");
                continue;
            }

            var organFn = values[0];        // コンパートメント機能
            var organName = values[1];      // コンパートメント名
            var sourceRegion = values[2];   // コンパートメントに対応する線源領域の名称

            OrganFunc organFunc;
            switch (organFn)
            {
                case "acc": organFunc = OrganFunc.acc; break;
                case "mix": organFunc = OrganFunc.mix; break;
                case "exc": organFunc = OrganFunc.exc; break;
                default:
                    errors.AddError(LineNum, $"Unrecognized compartment function '{organFn}'.");
                    continue;
            }

            organs.Add(new InputOrgan(LineNum, organFunc, organName, IsBar(sourceRegion) ? null : sourceRegion));
        }
        if (errors.IfAny(olderrors))
            return line;

        if (!organs.Any())
            errors.AddError(sectionLineNum, $"Empty [{nuc}:compartment] section.");

        return line;
    }

    private string? GetIntakes()
    {
        if (inputIntakesRead)
        {
            errors.AddError(LineNum, $"Duplicated [intake] section.");
            return SkipUntilNextSection();
        }
        inputIntakesRead = true;

        var olderrors = errors.Count;
        var sectionLineNum = LineNum;
        intakeSectionLoc = sectionLineNum;

        string? line;
        while (true)
        {
            line = GetNextLine();
            if (line is null)
                break;
            if (CheckSectionHeader(line))
                break;

            var values = line.Split(2, StringSplitOptions.RemoveEmptyEntries);
            if (values.Length != 2)
            {
                errors.AddError(LineNum, "Intake path definition should have 2 values.");
                continue;
            }

            var organTo = values[0];
            var coeff = values[1];      // 移行割合、[%]

            inputIntakes.Add(new InputIntake(LineNum, organTo, IsBar(coeff) ? null : coeff));
        }
        if (errors.IfAny(olderrors))
            return line;

        if (!inputIntakes.Any())
            errors.AddError(sectionLineNum, $"Empty [intake] section.");

        return line;
    }

    /// <summary>
    /// 移行係数の定義セクションを読み込む。
    /// </summary>
    /// <param name="nuc"></param>
    /// <returns>セクションの次行。</returns>
    /// <param name="line"></param>
    private string? GetTransfers(string nuc)
    {
        if (inputTransfers.TryGetValue(nuc, out var transfers))
        {
            errors.AddError(LineNum, $"Duplicated [{nuc}:transfer] section.");
            return SkipUntilNextSection();
        }
        inputTransfers.Add(nuc, transfers = []);

        var olderrors = errors.Count;
        var sectionLineNum = LineNum;
        transferSectionLocs[nuc] = sectionLineNum;

        string? line;
        while (true)
        {
            line = GetNextLine();
            if (line is null)
                break;
            if (CheckSectionHeader(line))
                break;

            var values = line.Split(3, StringSplitOptions.RemoveEmptyEntries);

            if (values.Length != 3)
            {
                errors.AddError(LineNum, "Transfer path definition should have 3 values.");
                continue;
            }

            var orgamFrom = values[0];
            var organTo = values[1];
            var coeff = values[2];      // 移行係数、[/d] or [%]

            transfers.Add(new InputTransfer(LineNum, orgamFrom, organTo, IsBar(coeff) ? null : coeff));
        }
        if (errors.IfAny(olderrors))
            return line;

        //if (!transfers.Any())
        //    errors.AddError(sectionLineNum, $"Empty [{nuc}:transfer] section.");

        return line;
    }

    /// <summary>
    /// パラメータの定義セクションを読み込む。
    /// </summary>
    /// <param name="data"></param>
    /// <param name="expander"></param>
    private void DefineParameters(InputData data, InputNuclideExpander expander)
    {
        foreach (var nuclide in inputNuclides)
            inputEvaluators.Add(nuclide, new InputEvaluator(nuclide.Name, errors));

        var globalParams = new Dictionary<string, string>();
        var nuclideParams = inputNuclides.ToDictionary(n => n, _ => new Dictionary<string, string>());

        IReadOnlyList<NuclideData> parentNuclide = [inputNuclides[0]];

        foreach (var (lineNum, parentOnly, isVarDecl, nuc, name, value) in inputParameters)
        {
            var expandNuclides = nuc == "" ? inputNuclides : expander.ExpandNuclides(lineNum, nuc);

            if (parentOnly == true)
                expandNuclides = [.. expandNuclides.Intersect(parentNuclide)];
            else if (parentOnly == false)
                expandNuclides = [.. expandNuclides.Except(parentNuclide)];

            if (isVarDecl)
            {
                foreach (var nuclide in expandNuclides)
                {
                    inputEvaluators[nuclide].TryReadVarDecl(lineNum, name, value);
                }
            }
            else if (nuc == "")
            {
                if (!InputData.ParameterNames.Contains(name))
                {
                    errors.AddError(lineNum, $"Unrecognized parameter '{name}' definition.");
                    continue;
                }

                if (globalParams.ContainsKey(name))
                    errors.AddError(lineNum, $"Duplicated parameter '{name}' definition.");
                else
                    globalParams.Add(name, value);
            }
            else
            {
                if (!NuclideData.ParameterNames.Contains(name))
                {
                    errors.AddError(lineNum, $"Unrecognized parameter '{name}' definition.");
                    continue;
                }

                foreach (var nuclide in expandNuclides)
                {
                    var parameters = nuclideParams[nuclide];

                    if (parameters.ContainsKey(name))
                        errors.AddError(lineNum, $"Duplicated parameter '{name}' definition.");
                    else
                        parameters.Add(name, value);
                }
            }
        }

        // インプット全体に対するパラメータを設定する。
        data.Parameters = globalParams;

        foreach (var (nuclide, parameters) in nuclideParams)
        {
            // 核種を指定した設定がないパラメータについて、インプット全体に対するパラメータ設定を継承する。
            foreach (var (paramName, paramValue) in globalParams.Where(kv =>
                NuclideData.ParameterNames.Contains(kv.Key) && !parameters.ContainsKey(kv.Key)))
            {
                parameters.Add(paramName, paramValue);
            }

            // 核種に対するパラメータを設定する。
            nuclide.Parameters = parameters;
        }
    }

    /// <summary>
    /// インプットから読み取ったコンパートメント定義の情報を保持する。
    /// </summary>
    /// <param name="LineNum">行番号。</param>
    /// <param name="Func">コンパートメント機能。</param>
    /// <param name="Name">コンパートメント名。</param>
    /// <param name="SourceRegion">線源領域。</param>
    private record class InputOrgan(int LineNum, OrganFunc Func, string Name, string? SourceRegion);

    /// <summary>
    /// 全てのコンパートメントを定義する。
    /// </summary>
    /// <param name="data"></param>
    private void DefineCompartments(InputData data)
    {
        // 'Other'は、線源領域「その他の組織」に関連付ける際の名称。
        var validSourceRegions = data.SourceRegions
            .Select(s => s.Name).Append("Other").ToArray();

        var otherSourceRegionsBase = data.SourceRegions
            .Where(s => s.MaleID != 0 || s.FemaleID != 0)
            .Select(s => s.Name).ToList();

        foreach (var nuclide in inputNuclides)
        {
            if (!CalcProgeny && nuclide.IsProgeny)
                continue;

            var nuc = nuclide.Name;
            if (!inputOrgans.TryGetValue(nuc, out var organs))
                continue;
            var sectionLineNum = compartmentSectionLocs[nuc];

            data.Nuclides.Add(nuclide);

            var otherSourceRegions = new List<string>(otherSourceRegionsBase);
            var otherCompartments = new List<Organ>();
            var anyCTmarrow = false;
            var anyRYmarrow = false;
            bool? otherContainsMineralBone;

            // Otherから無機質骨の体積組織(C-bone-VとT-bone-V)への分配について制御する。
            var paramOtherContainsMineralBone =
                (nuclide.Parameters.GetValueOrDefault("OtherContainsMineralBone") ??
                    data.Parameters.GetValueOrDefault("OtherContainsMineralBone") ?? "auto").Trim();
            if (bool.TryParse(paramOtherContainsMineralBone, out var v))
            {
                otherContainsMineralBone = v;
            }
            else
            {
                if (!paramOtherContainsMineralBone.Equals("auto", StringComparison.OrdinalIgnoreCase))
                    errors.AddError($"unrecognized OtherContainsMineralBone parameter: '{paramOtherContainsMineralBone}'");
                otherContainsMineralBone = null;
            }

            if (!nuclide.IsProgeny)
            {
                var input = new Organ
                {
                    Nuclide /**/= nuclide,
                    ID      /**/= data.Organs.Count + 1,
                    Index   /**/= data.Organs.Count,
                    Name    /**/= "input",
                    Func    /**/= OrganFunc.inp,
                };
                data.Organs.Add(input);
            }

            foreach (var (lineNum, organFunc, organName, sourceRegion) in organs)
            {
                var organ = new Organ
                {
                    Nuclide  /**/= nuclide,
                    ID       /**/= data.Organs.Count + 1,
                    Index    /**/= data.Organs.Count,
                    Name     /**/= organName,
                    Func     /**/= organFunc,
                };

                if (organFunc == OrganFunc.mix)
                    organ.BioDecay = 1.0;

                data.Organs.Add(organ);

                // 線源領域の名称について、妥当性を確認する。
                if (sourceRegion is not null)
                {
                    // コンパートメントに対応する線源領域の名称が有効であることを確認する。
                    var indexS = Array.IndexOf(validSourceRegions, sourceRegion);
                    if (indexS == -1)
                    {
                        errors.AddError(lineNum, $"Unknown source region name '{sourceRegion}'.");
                        continue;
                    }
                    if (nuclide.IsStable)
                    {
                        errors.AddError(lineNum, $"Cannot specify source region for stable nuclide '{nuclide.Name}'.");
                        continue;
                    }

                    organ.SourceRegion = sourceRegion;

                    // インプットで明示された線源領域をOtherの内訳から除く。
                    otherSourceRegions.Remove(sourceRegion);

                    if (sourceRegion == "C-marrow" || sourceRegion == "T-marrow")
                        anyCTmarrow = true;
                    if (sourceRegion == "R-marrow" || sourceRegion == "Y-marrow")
                        anyRYmarrow = true;

                    if (sourceRegion == "Other")
                        otherCompartments.Add(organ);
                }

                if (organ.Func == OrganFunc.exc)
                {
                    if (organ.Name == "Urine" || organ.Name == "Faeces")
                    {
                        organ.IsExcretaCompatibleWithOIR = true;
                    }
                }
            }

            if (otherContainsMineralBone is null)
            {
                // 自動判定を行う場合、線源領域Otherを設定した全てのコンパートメントについて
                // 名称から"ST"＝Soft Tissue, 軟組織であると示されている場合に、骨体積への分配を抑制する。
                var allSoftTissue = otherCompartments.All(o => o.Name.StartsWith("ST"));
                if (allSoftTissue)
                    otherContainsMineralBone = false;
                else
                {
                    otherContainsMineralBone = true;

                    // STと非STが混合している状態について、警告を提供した方がよい…
                    // if (otherCompartments.Any(o => o.Name.StartsWith("ST")))
                    //     ;
                }
            }
            if (otherContainsMineralBone == false)
            {
                // sregions_2016-08-12.NDXでID=1となっている
                // 皮質骨体積と梁骨体積をOtherを構成する線源領域から除く。
                otherSourceRegions.Remove("C-bone-V");
                otherSourceRegions.Remove("T-bone-V");
            }

            if (anyCTmarrow)
            {
                // C/T-marrowがコンパートメントとして明示されている場合は、
                // OtherからR/Y-marrowへの分配を行わないようにする。
                otherSourceRegions.Remove("R-marrow");
                otherSourceRegions.Remove("Y-marrow");

                // C/T-marrowとR/Y-marrowの組み合わせが両方同時に使用されている場合はエラーとする。
                if (anyRYmarrow)
                    errors.AddError(sectionLineNum, "Both of C/T-marrow and R/Y-marrow source region pairs are used.");
            }

            nuclide.OtherSourceRegions = otherSourceRegions;
        }

        // コンパートメント定義にエラーがないことを確定する。
        errors.RaiseIfAny();
    }

    /// <summary>
    /// インプットから読み取った摂取経路の情報を保持する。
    /// </summary>
    /// <param name="LineNum">行番号。</param>
    /// <param name="To">初期配分先コンパートメントの名前。</param>
    /// <param name="Coeff">初期配分割合。</param>
    private record class InputIntake(int LineNum, string To, string? Coeff);

    /// <summary>
    /// 全ての摂取経路を定義する。
    /// </summary>
    /// <param name="data"></param>
    private void DefineIntakes(InputData data)
    {
        var organFrom = data.Organs.First(); // inp
        var nuclideFrom = organFrom.Nuclide;
        var nuclideTo = inputNuclides.First();

        var olderrorsN = errors.Count;
        var sectionLineNum = intakeSectionLoc;

        var evaluatorTo = inputEvaluators[nuclideTo];

        // 移行経路の定義が正しいことの確認と、
        // 各コンパートメントから流出する移行係数の総計を求める。
        var definedTransfers = new HashSet<Organ>();
        var transfersCorrect = new List<(int lineNum, Organ from, Organ to, decimal coeff)>();
        decimal sum = 0;
        foreach (var intake in inputIntakes)
        {
            var olderrorsT = errors.Count;
            var lineNum = intake.LineNum;

            var nameTo = intake.To;
            var organTo = data.Organs.FirstOrDefault(o => o.Name == nameTo && o.Nuclide == nuclideTo);

            // 移行先がcompartmentセクションで定義済みかを確認する。
            if (organTo is null)
            {
                errors.AddError(lineNum, $"Undefined compartment '{nuclideFrom.Name}/{nameTo}'.");
            }
            else if (!definedTransfers.Add(organTo))
            {
                // 同じ移行経路が複数回定義されていないことを確認する。
                errors.AddError(lineNum, $"Duplicated intake path to '{nameTo}'.");
            }
            // 以降の処理でorganToが存在していることを確定する。
            if (errors.IfAny(olderrorsT))
                continue;

            // 移行割合の入力を要求する。
            // なお、ここでは割合値(0.15など)とパーセント値(10.5%など)の両方を受け付ける。
            decimal coeff;
            if (intake.Coeff is not null)
            {
                if (!evaluatorTo.TryReadCoefficient(LineNum, intake.Coeff, out var res))
                    continue;
                (coeff, _) = res;

                // 移行係数が負の値でないことを確認する。
                if (coeff < 0)
                {
                    errors.AddError(lineNum, "Transfer coefficient should be positive.");
                    continue;
                }

                sum += coeff;
            }
            else
            {
                errors.AddError(lineNum, $"Require fraction of intake activity [%].");
                continue;
            }

            transfersCorrect.Add((lineNum, organFrom, organTo!, coeff));
        }

        if (errors.IfAny(olderrorsN))
            return;

        // 流出放射能に対する移行割合の合計が1.0 == 100%かどうかを確認する。
        if (sum != 1)
        {
            errors.AddError(sectionLineNum, $"Total [%] of intake paths is not 100%, but {sum * 100:G29}%.");
            var ts = transfersCorrect.Where(t => t.from == organFrom).OrderBy(t => t.lineNum).ToArray();
            for (int i = 0; i < ts.Length; i++)
            {
                var (lineNum, _, _, coeff) = ts[i];
                errors.AddError(lineNum, $"    = {coeff * 100:G29}%");
            }
        }

        // 核種nuclideの動態モデルに入る移行経路と係数にエラーがないことを確定する。
        if (errors.IfAny(olderrorsN))
            return;

        // 核種が同じコンパートメントへの流入経路と、移行割合を設定する。
        foreach (var (_, _, organTo, coeff) in transfersCorrect)
        {
            // fromからtoへの移行割合 = 移行割合[%]
            var inflowRate = (double)coeff;

            organTo.Inflows.Add(new Inflow
            {
                ID = organFrom.ID,
                Rate = inflowRate,

                // 流入経路から流入元臓器の情報を直接引くための参照を設定する。
                Organ = organFrom,
            });
        }
    }

    /// <summary>
    /// インプットから読み取った移行経路定義の情報を保持する。
    /// </summary>
    /// <param name="LineNum">行番号。</param>
    /// <param name="From">移行元コンパートメント。</param>
    /// <param name="To">移行先コンパートメント。</param>
    /// <param name="Coeff">移行係数。</param>
    private record class InputTransfer(int LineNum, string From, string To, string? Coeff);

    private static bool IsDecayPath(Organ from, Organ to) => from.Nuclide != to.Nuclide;

    private class TransferParser(
        IReadOnlyList<NuclideData> nuclides,
        IReadOnlyList<Organ> organs,
        Dictionary<NuclideData, InputEvaluator> evaluators,
        InputErrors errors)
    {
        private readonly HashSet<(Organ from, Organ to)> definedTransfers = [];

        public bool TryParse(
            NuclideData nuclide, InputTransfer transfer,
            out (Organ OrganFrom, Organ OrganTo, decimal? Coeff) result)
        {
            var evaluator = evaluators[nuclide];
            var lineNum = transfer.LineNum;

            result = default;

            var olderrorsT = errors.Count;

            // 移行元コンパートメントを読み取る。
            var nuclideFrom = nuclide;
            var nameFrom = transfer.From;
            if (nameFrom.IndexOf('/') is int i && i != -1)
            {
                var nucFrom = nameFrom[..i];
                nameFrom = nameFrom[(i + 1)..];
                nuclideFrom = nuclides.FirstOrDefault(n => n.Name == nucFrom);
                if (nuclideFrom is null)
                    errors.AddError(lineNum, $"Undefined nuclide '{nucFrom}'.");
            }

            // 移行先コンパートメントを読み取る。
            var nuclideTo = nuclide;
            var nameTo = transfer.To;
            if (nameTo.IndexOf('/') is int j && j != -1)
            {
                var nucTo = nameTo[..j];
                nameTo = nameTo[(j + 1)..];
                nuclideTo = nuclides.FirstOrDefault(n => n.Name == nucTo);
                if (nuclideTo is null)
                {
                    errors.AddError(lineNum, $"Undefined nuclide '{nucTo}'.");
                }
                else if (nuclideTo != nuclide)
                {
                    // 移行先は定義している核種に属するコンパートメントのみとする。
                    errors.AddError(lineNum, $"Cannot set transfer path to a compartment which is not belong to '{nuclide.Name}'.");
                }
            }

            // 以降の処理でnuclideFromとnuclideToの両方が存在していることを確定する。
            if (errors.IfAny(olderrorsT))
                return false;
            Debug.Assert(nuclideFrom is not null);
            Debug.Assert(nuclideTo is not null);

            var organFrom = organs.FirstOrDefault(o => o.Name == nameFrom && o.Nuclide == nuclideFrom);
            var organTo = organs.FirstOrDefault(o => o.Name == nameTo && o.Nuclide == nuclideTo);

            // 移行元と移行先のそれぞれがcompartmentセクションで定義済みかを確認する。
            if (organFrom is null || organTo is null)
            {
                if (organFrom is null)
                    errors.AddError(lineNum, $"Undefined compartment '{nuclideFrom.Name}/{nameFrom}'.");
                if (organTo is null)
                    errors.AddError(lineNum, $"Undefined compartment '{nuclideTo.Name}/{nameTo}'.");
            }
            else if (organTo == organFrom)
            {
                // 自分自身への移行経路は定義できない。
                errors.AddError(lineNum, "Cannot set transfer path to itself.");
            }
            else if (!definedTransfers.Add((organFrom, organTo)))
            {
                // 同じ移行経路が複数回定義されていないことを確認する。
                errors.AddError(lineNum, $"Duplicated transfer path from '{organFrom}' to '{organTo}'.");
            }

            // 以降の処理でorganFromとorganToの両方が存在していることを確定する。
            if (errors.IfAny(olderrorsT))
                return false;
            Debug.Assert(organFrom is not null);
            Debug.Assert(organTo is not null);

            var coeff = default(decimal?);
            var isFrac = false;
            var hasCoeff = false;
            if (transfer.Coeff is not null)
            {
                if (!evaluator.TryReadCoefficient(lineNum, transfer.Coeff, out var res))
                    return false;
                (coeff, isFrac) = res;
                hasCoeff = true;

                // 移行係数が負の値でないことを確認する。
                if (coeff < 0)
                    errors.AddError(lineNum, "Transfer coefficient should be positive.");
            }

            // 設定された移行経路が有効かどうかを確認する。
            var funcFrom = organFrom.Func;
            var funcTo = organTo.Func;
            if (IsDecayPath(organFrom, organTo))
            {
                if (funcFrom == OrganFunc.acc)
                {
                    // accからの壊変経路では、係数なし、または移行速度の設定を要求する。
                    // パーセント値(移行割合)の設定はエラーとする。
                    if (isFrac)
                        errors.AddError(lineNum, $"Require transfer rate [/d] from {funcFrom} '{organFrom}'.");

                    // accからの壊変経路はaccにしか移行できない。
                    // ただし、excへの係数付きの壊変経路は、実際には途中に壊変コンパートメントとしてのaccが生成されるため許可する。
                    if (funcTo != OrganFunc.acc && !(funcTo == OrganFunc.exc && hasCoeff))
                        errors.AddError(lineNum, $"Cannot set decay path from {funcFrom} '{organFrom}' to non-acc '{organTo.Name}'.");
                }

                // excからの壊変経路では、子孫核種のexcへの移行のみ許可する。
                if (funcFrom == OrganFunc.exc && funcTo != OrganFunc.exc)
                    errors.AddError(lineNum, $"Cannot set decay path from {funcFrom} '{organFrom}' to non-exc '{organTo.Name}'.");

                // mixからの壊変経路は定義できない。
                if (funcFrom == OrganFunc.mix)
                    errors.AddError(lineNum, $"Cannot set decay path from {funcFrom} '{organFrom}'.");
            }
            else
            {
                // accからの流出経路では、移行速度の入力を要求する。
                // 係数なし、またはパーセント値(移行割合)の設定はエラーとする。
                if (funcFrom == OrganFunc.acc && (!hasCoeff || isFrac))
                    errors.AddError(lineNum, $"Require transfer rate [/d] from {funcFrom} '{organFrom.Name}'.");

                // excからの流出は定義できない。
                if (funcFrom == OrganFunc.exc)
                    errors.AddError(lineNum, $"Cannot set output path from exc '{organFrom.Name}'.");

                // TODO: mixからmixへの経路は定義できない。
                //if (funcFrom == OrganFunc.mix && funcTo == OrganFunc.mix)
                //    errors.AddError(lineNum, "Cannot set transfer path from 'mix' to 'mix'.");

                // mixからの同核種での移行経路では、移行割合の入力を要求する。
                // なお、ここでは割合値(0.15など)とパーセント値(10.5%など)の両方を受け付ける。
                if (funcFrom == OrganFunc.mix && !hasCoeff)
                    errors.AddError(lineNum, $"Require fraction of output activity [%] from {funcFrom} '{organFrom.Name}'.");
            }

            // 以降の処理で移行経路の設定位置が有効であることを確定する。
            if (errors.IfAny(olderrorsT))
                return false;

            result = (organFrom, organTo, coeff);

            return true;
        }
    }

    /// <summary>
    /// 全ての移行経路を定義する。
    /// </summary>
    /// <param name="data"></param>
    private void DefineTransfers(InputData data)
    {
        var decaySet = new DecaySet(data.Nuclides, errors);
        var parser = new TransferParser(inputNuclides, data.Organs, inputEvaluators, errors);

        foreach (var nuclide in inputNuclides)
        {
            if (!CalcProgeny && nuclide.IsProgeny)
                continue;

            var nuc = nuclide.Name;
            if (!inputTransfers.TryGetValue(nuc, out var transfers))
                continue;

            var olderrorsN = errors.Count;
            var sectionLineNum = transferSectionLocs[nuc];

            var evaluator = inputEvaluators[nuclide];

            // 移行経路の定義が正しいことの確認と、
            // 各コンパートメントから流出する移行係数の総計を求める。
            var transfersCorrect = new List<(int lineNum, Organ from, Organ to, decimal coeff)>();
            var sumOfOutflowCoeff = new Dictionary<Organ, decimal>();
            foreach (var transfer in transfers)
            {
                if (!parser.TryParse(nuclide, transfer, out var result))
                    continue;

                var lineNum = transfer.LineNum;
                var organFrom = result.OrganFrom;
                var organTo = result.OrganTo;
                var coeff = result.Coeff;

                if (IsDecayPath(organFrom, organTo))
                {
                    // 次のような壊変経路を、
                    //   Parent/organFrom --(coeff)--> Progeny_i/organTo  ①移行速度あり
                    //   Parent/organFrom -----------> Progeny_i/organTo  ②移行速度なし
                    //
                    // 親核種ParentがコンパートメントorganFromから移動しないまま壊変することで生成された
                    // 子孫核種Progeny_1～Nのそれぞれを受けるorganDecayを、自動的に追加定義する、
                    // あるいはインプットで定義済みのコンパートメントを直接使用することで、
                    // 以下のような経路に構成し直す。
                    //   Parent/organFrom
                    //      ↓
                    //   Progeny_1/organDecay
                    //   ： ：
                    //   ↓ ↓
                    //   ↓ Progeny_i/organDecay --(coeff)--> Progeny_i/organTo  ①移行速度あり
                    //   ↓ Progeny_i/organDecay == organTo                      ②移行速度なし
                    //   ↓ ：
                    //   ↓ ↓
                    //   Progeny_N/organDecay

                    var organDecay = decaySet.AddDecayPath(lineNum, organFrom, organTo, coeff);
                    if (organDecay is null)
                        continue;

                    // 以降の処理を核種が同じコンパートメント間organDecay -> organToの経路設定にすり替える。
                    organFrom = organDecay;
                }

                if (coeff is decimal coeff_v)
                {
                    if (!sumOfOutflowCoeff.TryGetValue(organFrom, out var sum))
                        sum = 0;
                    sumOfOutflowCoeff[organFrom] = sum + coeff_v;
                }

                transfersCorrect.Add((lineNum, organFrom, organTo, coeff ?? 0));
            }

            // 核種nuclideの動態モデルに入る移行経路にエラーがないことを確定する。
            if (errors.IfAny(olderrorsN))
                continue;

            // あるコンパートメントから同じ核種のまま流出する全ての移行経路について処理する。
            var outflowGroups = transfersCorrect.GroupBy(t => t.from);
            foreach (var outflows in outflowGroups)
            {
                if (!outflows.Any())
                    continue;
                var organFrom = outflows.Key;
                var nameFrom = organFrom.Name;

                if (organFrom.IsInstantOutflow)
                {
                    // 流出路毎の移行割合を合計したものが1.0 == 100%かどうかを確認する。
                    var sum = sumOfOutflowCoeff[organFrom];
                    if (sum != 1)
                    {
                        var ts = transfersCorrect.Where(t => t.from == organFrom).OrderBy(t => t.lineNum).ToArray();
                        for (int i = 0; i < ts.Length; i++)
                        {
                            var (lineNum, _, _, coeff) = ts[i];
                            if (i == 0)
                                errors.AddError(lineNum, $"Total [%] of transfer paths from '{nameFrom}' is not 100%, but {sum * 100:G29}%.");
                            errors.AddError(lineNum, $"    = {coeff * 100:G29}%");
                        }
                    }
                }
                else
                {
                    // fromにおける生物学的崩壊定数[/day]を設定する。
                    if (organFrom.Func == OrganFunc.acc)
                        organFrom.BioDecay = (double)sumOfOutflowCoeff[organFrom];
                }

                // GetDecayChain(organFrom);
            }

            // 核種nuclideの動態モデルに入る移行経路と係数にエラーがないことを確定する。
            if (errors.IfAny(olderrorsN))
                continue;

            // 核種が同じコンパートメントへの流入経路と、移行割合を設定する。
            foreach (var (_, organFrom, organTo, coeff) in transfersCorrect)
            {
                double inflowRate;
                if (organFrom.IsInstantOutflow)
                {
                    // fromからtoへの移行割合 = 移行割合[%]
                    inflowRate = (double)coeff;
                }
                else
                {
                    var sum = sumOfOutflowCoeff[organFrom];

                    // fromからtoへの移行割合 = 移行速度[/d]
                    inflowRate = (double)coeff;
                }

                organTo.Inflows.Add(new Inflow
                {
                    ID = organFrom.ID,
                    Rate = inflowRate,

                    // 流入経路から流入元臓器の情報を直接引くための参照を設定する。
                    Organ = organFrom,
                });
            }
        }

        // 全ての核種について陽に設定された移行経路と係数にエラーがないことを確定する。
        errors.RaiseIfAny();

        // 核種が異なるコンパートメントへの流入経路と、移行割合を設定する。
        decaySet.DefineDecayTransfers(data.Organs);

        // 移行経路の定義にエラーがないことを確定する。
        errors.RaiseIfAny();

        // 壊変経路の充足を確認する。
        VerifyDecayPaths(data);
    }

    /// <summary>
    /// 崩壊系列に沿って全ての壊変経路が定義されていることを確認する。
    /// </summary>
    /// <param name="data"></param>
    private void VerifyDecayPaths(InputData data)
    {
        var decays = data.Organs.ToDictionary(o => o, o => new List<NuclideData>());
        foreach (var organTo in data.Organs)
        {
            foreach (var organFrom in organTo.Inflows
                            .Select(i => i.Organ).Where(o => o.Nuclide != organTo.Nuclide))
            {
                decays[organFrom].Add(organTo.Nuclide);
            }
        }

        foreach (var nuclide in data.Nuclides)
        {
            if (!CalcProgeny && nuclide.IsProgeny)
                continue;

            foreach (var organFrom in data.Organs.Where(o => o.Nuclide == nuclide))
            {
                if (organFrom.Func == OrganFunc.inp || organFrom.Func == OrganFunc.mix)
                    return;

                // 排泄後の残留放射能については、厳密に経路を定義しなくてもよいことにする。
                if (organFrom.Func == OrganFunc.exc)
                    return;

                var decayNuclides = decays[organFrom];

                var daughters = nuclide.Branches.Select(b => b.Daughter);
                foreach (var daughter in daughters)
                {
                    if (!decayNuclides.Contains(daughter))
                        errors.AddError($"Missing decay path from '{organFrom}' to daughter '{daughter}'");
                }
            }
        }

        errors.RaiseIfAny();
    }

    /// <summary>
    /// 流入を持たないコンパートメントをマークする。
    /// </summary>
    /// <param name="data"></param>
    private void MarkZeroInflows(InputData data)
    {
        while (true)
        {
            var modified = false;

            foreach (var organ in data.Organs)
            {
                if (organ.Func == OrganFunc.inp)
                    continue;   // 初期配分による流入があるため。
                if (organ.IsZeroInflow)
                    continue;   // 既に流入なしと判定済み。

                // コンパートメントが流入を持たない場合。
                if (organ.Inflows.All(i => i.Rate == 0 || i.Organ.IsZeroInflow))
                {
                    organ.IsZeroInflow = true;
                    modified = true;
                }
            }

            // 流入なしマーク状態が変化しなくなるまで繰り返す。
            if (!modified)
                break;
        }

        // 初期配分を終えた後は流入なし。
        var input = data.Organs.First(o => o.Func == OrganFunc.inp);
        input.IsZeroInflow = true;
    }

    /// <summary>
    /// S係数データをインプットに設定する。
    /// </summary>
    /// <param name="data"></param>
    public static void SetSCoefficients(InputData data)
    {
        foreach (var nuclide in data.Nuclides)
        {
            if (nuclide.IsStable)
            {
                data.SCoeffTablesM.Add([]);
                data.SCoeffTablesF.Add([]);
                continue;
            }

            // 核種に対応するS係数データを読み込む。
            var tableSCoeffM = ReadSCoeff(data, nuclide, Sex.Male);
            var tableSCoeffF = ReadSCoeff(data, nuclide, Sex.Female);
            data.SCoeffTablesM.Add(tableSCoeffM);
            data.SCoeffTablesF.Add(tableSCoeffF);

            foreach (var organ in data.Organs.Where(o => o.Nuclide == nuclide))
            {
                var sourceRegion = organ.SourceRegion;
                if (sourceRegion is null)
                    continue;

                // コンパートメントの放射能を各標的領域に振り分けるためのS係数データを関連付ける。
                organ.S_CoefficientsM = tableSCoeffM[sourceRegion];
                organ.S_CoefficientsF = tableSCoeffF[sourceRegion];
            }
        }
    }

    /// <summary>
    /// S係数データを読み込む。
    /// </summary>
    /// <param name="data"></param>
    /// <param name="nuclide">対象核種。線源領域の名称が設定される。</param>
    /// <param name="sex">S係数を計算する性別。</param>
    /// <returns>キーが線源領域の名称、値が各標的領域に対する成人のS係数、となる辞書。</returns>
    private static Dictionary<string, double[]> ReadSCoeff(InputData data, NuclideData nuclide, Sex sex)
    {
        var safdata = sex == Sex.Male ? safdataAM : safdataAF;
        if (safdata is null)
            throw Program.Error($"Cannot read SAF data.");

        var nuc = nuclide.Name;

        var calcScoeff = new CalcScoeff(safdata, InterpolationMethod.PCHIP);
        calcScoeff.CalcS(nuc);

        var sources = data.SourceRegions.Select(s => s.Name).ToArray();
        var targets = data.TargetRegions;
        var sourcesCount = sources.Length;
        var targetsCount = targets.Length;

        var table = sources.ToDictionary(s => s, s => new double[targetsCount]);

        var columnOther = new double[targetsCount];
        var indexOtherSources = nuclide.OtherSourceRegions.Select(s => Array.IndexOf(sources, s)).ToArray();

        for (int indexT = 0; indexT < targetsCount; indexT++)
        {
            double GetValue(int indexS) => calcScoeff.OutTotal[indexT + targetsCount * indexS];

            for (int indexS = 0; indexS < sourcesCount; indexS++)
            {
                var sourceRegion = sources[indexS];
                var scoeff = GetValue(indexS);

                table[sourceRegion][indexT] = scoeff;
            }

            // 線源領域'Other'のS係数を計算する。
            var massOther = 0.0;
            var scoeffOther = 0.0;
            foreach (var indexS in indexOtherSources)
            {
                var sourceRegion = data.SourceRegions[indexS];
                var mass = sourceRegion.GetMass(sex);
                var scoeff = GetValue(indexS);

                massOther += mass;
                scoeffOther += mass * scoeff;
            }
            if (massOther != 0) scoeffOther /= massOther;

            columnOther[indexT] = scoeffOther;
        }

        // S係数データに'Other'列を追加する
        table["Other"] = columnOther;

        return table;
    }
}
