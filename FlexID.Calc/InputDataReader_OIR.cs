namespace FlexID.Calc;

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

    private readonly InputErrors errors;
    private readonly Dictionary<string, int> compartmentSectionLocs = new();
    private readonly Dictionary<string, int> transferSectionLocs = new();

    private readonly InputEvaluator evaluator;

    private string inputTitle;

    private Dictionary<string, string> inputParameters;

    private Dictionary<string, Dictionary<string, string>> nuclideParameters = new();

    private List<NuclideData> nuclides;

    private Dictionary<string, List<(int lineNum, Organ)>> nuclideOrgans = new();

    private Dictionary<string, List<(int lineNum, string from, string to, decimal? coeff, bool isRate)>> nuclideTransfers = new();

    private List<Dictionary<string, double[]>> SCoeffTables = new();

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
        evaluator = new InputEvaluator(errors);
    }

    protected override string GetNextLine()
    {
    Lagain:
        var nextLine = base.GetNextLine();

        if (evaluator.TryReadVarDecl(LineNum, nextLine))
            goto Lagain;

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

    private string SkipUntilNextSection()
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

    /// <summary>
    /// インプットファイルを読み込む。
    /// </summary>
    /// <returns></returns>
    public InputData Read()
    {
        var line = GetNextLine();
        if (line is null)
            throw Program.Error("Reach to EOF while reading input file.");

        while (true)
        {
            var header = GetSectionHeader(line);

            Span<char> buffer = stackalloc char[header.Length];
            for (int i = 0; i < header.Length; i++)
                buffer[i] = char.ToLowerInvariant(header[i]);

            if (buffer.SequenceEqual("title".AsSpan()))
            {
                line = GetTitle();
            }
            else if (buffer.SequenceEqual("parameter".AsSpan()))
            {
                line = GetParameters("");
            }
            else if (buffer.EndsWith(":parameter".AsSpan()))
            {
                var i = header.IndexOf(':');
                var nuc = header.Slice(0, i).ToString();
                line = GetParameters(nuc);
            }
            else if (buffer.SequenceEqual("nuclide".AsSpan()))
            {
                line = GetNuclides();
            }
            else if (buffer.EndsWith(":compartment".AsSpan()))
            {
                var i = header.IndexOf(':');
                var nuc = header.Slice(0, i).ToString();
                line = GetCompartments(nuc);
            }
            else if (buffer.EndsWith(":transfer".AsSpan()))
            {
                var i = header.IndexOf(':');
                var nuc = header.Slice(0, i).ToString();
                line = GetTransfers(nuc);
            }
            else
            {
                errors.AddError(LineNum, $"Unrecognized section $'[{header.ToString()}]'.");
                line = SkipUntilNextSection();
            }

            if (line is null)
                break;
        }

        // インプットの構文に従って読み取りができていることを確定する。
        // なお、[nuclide]セクションについては実質的な意味解析まで完了している状態となる。
        errors.RaiseIfAny();

        foreach (var (lineNum, nuc) in nuclideOrgans.Keys.Except(nuclides?.Select(n => n.Name) ?? Array.Empty<string>())
            .Select(nuc => (LineNum: compartmentSectionLocs[nuc], nuc)).OrderBy(t => t.LineNum))
        {
            errors.AddError(lineNum, $"Undefined nuclide '{nuc}' is used to define compartments.");
        }

        foreach (var (lineNum, nuc) in nuclideTransfers.Keys.Except(nuclides?.Select(n => n.Name) ?? Array.Empty<string>())
            .Select(nuc => (LineNum: transferSectionLocs[nuc], nuc)).OrderBy(t => t.LineNum))
        {
            errors.AddError(lineNum, $"Undefined nuclide '{nuc}' is used to define transfers.");
        }

        if (inputTitle is null)
            errors.AddError(LineNum, "Missing [title] section.");

        if (nuclides is null)
            errors.AddError(LineNum, "Missing [nuclide] section.");
        else
        {
            var nuclideNames = nuclides.Select(n => n.Name).ToArray();

            foreach (var nuc in nuclideNames.Where(nuc => !nuclideOrgans.ContainsKey(nuc)))
                errors.AddError(LineNum, $"Missing [{nuc}:compartment] section.");

            foreach (var nuc in nuclideNames.Where(nuc => !nuclideTransfers.ContainsKey(nuc)))
                errors.AddError(LineNum, $"Missing [{nuc}:transfer] section.");
        }

        // 核種定義に対して[compartment]と[transfer]セクションに過不足がないことを確定する。
        errors.RaiseIfAny();

        // 線源領域と標的領域のデータを読み込む。
        var sourceRegions = SAFDataReader.ReadSourceRegions();
        var targetRegions = SAFDataReader.ReadTargetRegions().Select(t => t.Name).ToArray();

        // 組織加重係数データを読み込む。
        var (ts, ws) = ReadTissueWeights(Path.Combine("lib", "OIR", "wT.txt"));
        if (!Enumerable.SequenceEqual(targetRegions, ts))
            errors.AddError($"Found mismatch of target region names on tissue weighting factor data.");

        // 外部データの読み込み処理にエラーがないことを確定する。
        errors.RaiseIfAny();

        var data = new InputData();
        data.Title = inputTitle;
        data.Parameters = inputParameters ?? new();
        data.SourceRegions = sourceRegions;
        data.TargetRegions = targetRegions;
        data.TargetWeights = ws;

        // 全てのコンパートメントを定義する。
        DefineCompartments(data);

        // コンパートメント間の移行経路を定義する。
        DefineTransfers(data);

        // 流入を持たないコンパートメントをマークする。
        MarkZeroInflows(data);

        bool CheckOutput(string name) => data.TryGetBooleanParameter(name, true);

        // 出力ファイルの設定。
        data.OutputDose = CheckOutput("OutputDose");
        data.OutputDoseRate = CheckOutput("OutputDoseRate");
        data.OutputRetention = CheckOutput("OutputRetention");
        data.OutputCumulative = CheckOutput("OutputCumulative");

        return data;
    }

    /// <summary>
    /// タイトルの定義セクションを読み込む。
    /// </summary>
    /// <returns>セクションの次行。</returns>
    private string GetTitle()
    {
        if (inputTitle != null)
        {
            errors.AddError(LineNum, "Duplicated [title] section.");
            return SkipUntilNextSection();
        }

        var sectionLineNum = LineNum;
        var title = GetNextLine();
        if (title is null || CheckSectionHeader(title))
        {
            inputTitle = "";
            errors.AddError(sectionLineNum, "Empty [title] section.");
            return title;
        }

        inputTitle = title;

        var line = GetNextLine();
        if (!CheckSectionHeader(line))
        {
            errors.AddError(LineNum, "Unrecognized lines in [title] section.");
            return SkipUntilNextSection();
        }

        return line;
    }

    /// <summary>
    /// パラメーターの定義セクションを読み込む。
    /// </summary>
    /// <param name="nuc"></param>
    /// <returns>セクションの次行。</returns>
    private string GetParameters(string nuc)
    {
        Dictionary<string, string> parameters;
        string[] parameterNames;
        if (nuc == "")
        {
            if (inputParameters != null)
            {
                errors.AddError(LineNum, "Duplicated [parameter] section.");
                return SkipUntilNextSection();
            }
            parameters = new();
            parameterNames = InputData.ParameterNames;
            inputParameters = parameters;
        }
        else
        {
            if (nuclideParameters.ContainsKey(nuc))
            {
                errors.AddError(LineNum, $"Duplicated [{nuc}:parameter] section.");
                return SkipUntilNextSection();
            }
            parameters = new();
            parameterNames = NuclideData.ParameterNames;
            nuclideParameters.Add(nuc, parameters);
        }

        string line;
        while (true)
        {
            line = GetNextLine();
            if (line is null)
                break;
            if (CheckSectionHeader(line))
                break;

            var values = line.Split(new string[] { "=" }, 2, StringSplitOptions.RemoveEmptyEntries);
            if (values.Length != 2)
            {
                errors.AddError(LineNum, "Parameter definition should have 2 values.");
                continue;
            }

            var paramName = values[0].Trim();
            var paramValue = values[1].Trim();

            if (!parameterNames.Contains(paramName))
                errors.AddError(LineNum, $"Unrecognized parameter '{paramName}' definition.");
            else if (parameters.ContainsKey(paramName))
                errors.AddError(LineNum, $"Duplicated parameter '{paramName}' definition.");
            else
                parameters.Add(paramName, paramValue);
        }

        return line;
    }

    /// <summary>
    /// 核種の定義セクションを読み込む。
    /// </summary>
    /// <returns>セクションの次行。</returns>
    private string GetNuclides()
    {
        if (nuclides != null)
        {
            errors.AddError(LineNum, "Duplicated [nuclide] section.");
            return SkipUntilNextSection();
        }

        var olderrors = errors.Count;
        var sectionLineNum = LineNum;

        nuclides = new List<NuclideData>();

        // 最初の1行を見て、新旧どちらの型式で入力されているかを判定する。
        var autoMode = default(bool?);

        Dictionary<string, IndexData> indexTable = null;
        var branchTable = new Dictionary<NuclideData, (string Daughter, decimal Fraction)[]>();
        var nuclideLines = new Dictionary<NuclideData, int>();

        string line;
        while (true)
        {
            line = GetNextLine();
            if (line is null)
                break;
            if (CheckSectionHeader(line))
                break;

            // 核種の定義行を読み込む。
            var values = line.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);

            if (autoMode == true || autoMode == default && values.All(patternNuclide.IsMatch))
            {
                // インデックスファイルから娘核種と分岐比の情報を自動取得する。
                autoMode = true;

                if (indexTable is null)
                    indexTable = IndexDataReader.ReadNDX().ToDictionary(x => x.Nuclide, x => x);

                foreach (var nuc in values)
                {
                    if (!patternNuclide.IsMatch(nuc))
                    {
                        errors.AddError(LineNum, $"'{nuc}' is not nuclide name.");
                        continue;
                    }
                    if (nuclides.Any(n => n.Name == nuc))
                    {
                        errors.AddError(LineNum, $"Duplicated nuclide definition for '{nuc}'.");
                        continue;
                    }

                    indexTable.TryGetValue(nuc, out var indexData);

                    // インデックスファイルに定義されていないものは安定核種として扱う。
                    var halfLife = indexData?.HalfLife;
                    var lambda = indexData?.Lambda;
                    var branches = indexData?.Daughters.Select(d => (d.Daughter, d.Branch)).ToArray()
                                ?? Array.Empty<(string Daughter, decimal Fraction)>();

                    var nuclide = new NuclideData
                    {
                        Name = nuc,
                        HalfLife = halfLife ?? "---",
                        Lambda = lambda ?? 0.0,
                        IsProgeny = nuclides.Count > 0,
                    };
                    nuclides.Add(nuclide);

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
                if (nuclides.Any(n => n.Name == nuc))
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
                    Name = nuc,
                    Lambda = lambda,
                    IsProgeny = nuclides.Count > 0,
                };
                nuclides.Add(nuclide);

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
        if (!nuclides.Any())
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

            NuclideData GetNuclide(string nuc)
                => nuclides.FirstOrDefault(n => n.Name == nuc);

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
        var root = nuclides.FirstOrDefault();
        foreach (var nuclide in nuclides)
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
    /// コンパートメントの定義セクションを読み込む。
    /// </summary>
    /// <param name="nuc"></param>
    /// <returns>セクションの次行。</returns>
    private string GetCompartments(string nuc)
    {
        if (nuclideOrgans.TryGetValue(nuc, out var organs))
        {
            errors.AddError(LineNum, $"Duplicated [{nuc}:compartment] section.");
            return SkipUntilNextSection();
        }

        var olderrors = errors.Count;
        var sectionLineNum = LineNum;
        compartmentSectionLocs[nuc] = sectionLineNum;

        organs = new List<(int lineNum, Organ)>();
        nuclideOrgans.Add(nuc, organs);

        string line;
        while (true)
        {
            line = GetNextLine();
            if (line is null)
                break;
            if (CheckSectionHeader(line))
                break;

            var values = line.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);

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
                case "inp": organFunc = OrganFunc.inp; break;
                case "acc": organFunc = OrganFunc.acc; break;
                case "mix": organFunc = OrganFunc.mix; break;
                case "exc": organFunc = OrganFunc.exc; break;
                default:
                    errors.AddError(LineNum, $"Unrecognized compartment function '{organFn}'.");
                    continue;
            }

            var organ = new Organ
            {
                Nuclide = null,     // 後で設定する。
                ID = organs.Count + 1,
                Index = -1,         // 後で設定する。
                Name = organName,
                Func = organFunc,
                BioDecay = 1.0,     // accは後で設定する。
                Inflows = new List<Inflow>(),
            };

            // 線源領域の名称については、妥当性を後で確認する。
            organ.SourceRegion = sourceRegion;

            organs.Add((LineNum, organ));
        }
        if (errors.IfAny(olderrors))
            return line;

        if (!organs.Any())
            errors.AddError(sectionLineNum, $"Empty [{nuc}:compartment] section.");

        return line;
    }

    /// <summary>
    /// 移行係数の定義セクションを読み込む。
    /// </summary>
    /// <param name="nuc"></param>
    /// <returns>セクションの次行。</returns>
    /// <param name="line"></param>
    private string GetTransfers(string nuc)
    {
        if (nuclideTransfers.TryGetValue(nuc, out var transfers))
        {
            errors.AddError(LineNum, $"Duplicated [{nuc}:transfer] section.");
            return SkipUntilNextSection();
        }

        var olderrors = errors.Count;
        var sectionLineNum = LineNum;
        transferSectionLocs[nuc] = sectionLineNum;

        transfers = new List<(int, string, string, decimal?, bool)>();
        nuclideTransfers.Add(nuc, transfers);

        string line;
        while (true)
        {
            line = GetNextLine();
            if (line is null)
                break;
            if (CheckSectionHeader(line))
                break;

            var values = line.Split(new string[] { " " }, 3, StringSplitOptions.RemoveEmptyEntries);

            if (values.Length != 3)
            {
                errors.AddError(LineNum, "Transfer path definition should have 3 values.");
                continue;
            }

            var orgamFrom = values[0];
            var organTo = values[1];
            var coeffStr = values[2];    // 移行係数、[/d] or [%]

            var coeff = default(decimal?);
            var isRate = false;
            if (!IsBar(coeffStr))
            {
                if (!evaluator.TryReadCoefficient(LineNum, coeffStr, out var res))
                    continue;
                (coeff, isRate) = res;
            }

            transfers.Add((LineNum, orgamFrom, organTo, coeff, isRate));
        }
        if (errors.IfAny(olderrors))
            return line;

        if (!transfers.Any())
            errors.AddError(sectionLineNum, $"Empty [{nuc}:transfer] section.");

        return line;
    }

    /// <summary>
    /// 全てのコンパートメントを定義する。
    /// </summary>
    /// <param name="data"></param>
    private void DefineCompartments(InputData data)
    {
        Organ input = null;

        foreach (var nuclide in nuclides)
        {
            if (!CalcProgeny && nuclide.IsProgeny)
                continue;

            var nuc = nuclide.Name;
            if (!nuclideOrgans.TryGetValue(nuc, out var organs))
                continue;
            var sectionLineNum = compartmentSectionLocs[nuc];

            nuclide.Parameters = nuclideParameters?.GetValueOrDefault(nuc) ?? new();

            data.Nuclides.Add(nuclide);

            // 'Other'は、線源領域「その他の組織」に関連付ける際の名称。
            var validSourceRegions = data.SourceRegions
                .Select(s => s.Name).Append("Other").ToArray();

            var otherSourceRegions = data.SourceRegions
                .Where(s => s.MaleID != 0 || s.FemaleID != 0)
                .Select(s => s.Name).ToList();

            var inpParams = data.Parameters;
            var inpIncludes = (inpParams.GetValueOrDefault("IncludeOtherSourceRegions") ?? "").Split(new[] { " " }, StringSplitOptions.RemoveEmptyEntries);
            var inpExcludes = (inpParams.GetValueOrDefault("ExcludeOtherSourceRegions") ?? "").Split(new[] { " " }, StringSplitOptions.RemoveEmptyEntries);

            var nucParams = nuclide.Parameters;
            var nucExcludes = (nucParams.GetValueOrDefault("ExcludeOtherSourceRegions") ?? "").Split(new[] { " " }, StringSplitOptions.RemoveEmptyEntries);
            var nucIncludes = (nucParams.GetValueOrDefault("IncludeOtherSourceRegions") ?? "").Split(new[] { " " }, StringSplitOptions.RemoveEmptyEntries);

            // Otherについて、以下の優先度で包含・除外指定された線源領域を追加・削除する：
            //  優先度低：インプット全体に共通設定として包含指定されたもの
            //  ↓        インプット全体に共通設定として除外指定されたもの
            //  ↓        特定の核種に対して包含指定されたもの
            //  優先度高：特定の核種に対して除外指定されたもの
            otherSourceRegions.AddRange(inpIncludes.Except(otherSourceRegions).ToArray());
            otherSourceRegions.RemoveAll(reg => inpExcludes.Contains(reg));
            otherSourceRegions.AddRange(nucIncludes.Except(otherSourceRegions).ToArray());
            otherSourceRegions.RemoveAll(reg => nucExcludes.Contains(reg));

            foreach (var (lineNum, organ) in organs)
            {
                if (organ.Func == OrganFunc.inp)
                {
                    if (nuclide.IsProgeny)
                        errors.AddError(lineNum, "Cannot define 'inp' compartment which belongs to progeny nuclide.");
                    else if (input != null)
                        errors.AddError(lineNum, "Duplicated 'inp' compartment.");
                    else
                        input = organ;
                }

                organ.Nuclide = nuclide;
                organ.Index = data.Organs.Count;

                data.Organs.Add(organ);

                var sourceRegion = organ.SourceRegion;
                if (!IsBar(sourceRegion))
                {
                    // コンパートメントに対応する線源領域の名称が有効であることを確認する。
                    var indexS = Array.IndexOf(validSourceRegions, sourceRegion);
                    if (indexS == -1)
                    {
                        errors.AddError(lineNum, $"Unknown source region name '{sourceRegion}'.");
                        continue;
                    }

                    // インプットで明示された線源領域をOtherの内訳から除く。
                    otherSourceRegions.Remove(sourceRegion);
                }
                else
                {
                    organ.SourceRegion = null;
                }

                if (organ.Func == OrganFunc.exc)
                {
                    if (organ.Name == "Urine" || organ.Name == "Faeces")
                    {
                        organ.IsExcretaCompatibleWithOIR = true;
                    }
                }
            }

            if (!nuclide.IsProgeny && input is null)
                errors.AddError(sectionLineNum, "Missing 'inp' compartment.");

            nuclide.OtherSourceRegions = otherSourceRegions.ToArray();
        }

        // コンパートメント定義にエラーがないことを確定する。
        errors.RaiseIfAny();
    }

    /// <summary>
    /// 対象核種から始まる崩壊系列を構成する子孫核種を列挙する。
    /// </summary>
    /// <param name="root"></param>
    /// <returns>rootを含まない、子孫核種のみの配列。</returns>
    private NuclideData[] GetDecayNuclides(NuclideData root)
    {
        var results = new List<NuclideData> { root };

    Lagain:
        var count = results.Count;
        for (int i = 0; i < count; i++)
        {
            // 系列を構成する核種の娘を追加する。
            var nuclide = results[i];
            var daughters = nuclide.Branches.Select(b => b.Daughter);
            results.AddRange(daughters.Where(d => !results.Contains(d)));
        }
        // 系列を構成する核種が増えなくなるまで繰り返す。
        if (results.Count > count)
            goto Lagain;

        // rootを除いた、子孫核種のみの配列を返す。
        return results.Skip(1).ToArray();
    }

    /// <summary>
    /// 対象コンパートメントの核種から始まる崩壊系列の情報を保持する。
    /// </summary>
    private class DecayChain
    {
        /// <summary>
        /// コンストラクタ。
        /// </summary>
        /// <param name="organRoot"></param>
        /// <param name="decayNuclides">子孫核種の配列。</param>
        public DecayChain(Organ organRoot, NuclideData[] decayNuclides)
        {
            RootCompartment = organRoot;
            DecayNuclides = decayNuclides;
            DecayCompartments = new Organ[decayNuclides.Length];
        }

        public Organ RootCompartment;

        public NuclideData[] DecayNuclides;

        public Organ[] DecayCompartments;

        /// <summary>
        /// 崩壊系列を構成する全ての移行経路を取得する。
        /// </summary>
        /// <returns></returns>
        public IReadOnlyList<(NuclideData Parent, NuclideData Daughter)> GetDecayTransfers()
        {
            var nuclides = DecayNuclides.Prepend(RootCompartment.Nuclide);

            var results = new List<(NuclideData Parent, NuclideData Daughter)>();
            foreach (var parent in nuclides)
            {
                foreach (var daughter in DecayNuclides)
                {
                    if (parent == daughter)
                        continue;
                    if (parent.Branches.Any(b => b.Daughter == daughter))
                        results.Add((parent, daughter));
                }
            }
            return results;
        }

        /// <summary>
        /// 指定の子孫核種に対応する壊変コンパートメントを追加する。
        /// </summary>
        /// <param name="data"></param>
        /// <param name="progeny"></param>
        /// <returns>追加した壊変コンパートメント</returns>
        public Organ AddDecayCompartment(InputData data, NuclideData progeny)
        {
            var organFrom = RootCompartment;
            var fromNuclide = organFrom.Nuclide;

            var index = data.Organs.Count;
            var organDecay = new Organ
            {
                Nuclide = progeny,
                ID = index + 1,
                Index = index,
                Name = $"Decay-{fromNuclide.Name}/{organFrom.Name}",
                Func = OrganFunc.acc,
                BioDecay = 1.0,     // accは後で設定する。
                Inflows = new List<Inflow>(),
                IsDecayCompartment = true,
            };
            data.Organs.Add(organDecay);

            var sourceRegion = organFrom.SourceRegion;
            if (sourceRegion != null)
            {
                // 親核種のコンパートメントに設定された線源領域が、
                // 子孫核種の動態モデルおいても明示的に設定されているかどうかを調べる。
                var explicitUsed = data.Organs.Where(o => o.Nuclide == progeny)
                                              .Any(o => o.SourceRegion == sourceRegion);

                // 明示的に設定されていない場合は、ambiguous compartmentを'Other'に割り当てる。
                if (!explicitUsed)
                    sourceRegion = "Other";

                organDecay.SourceRegion = sourceRegion;
            }

            return organDecay;
        }
    }

    /// <summary>
    /// 全ての移行経路を定義する。
    /// </summary>
    /// <param name="data"></param>
    private void DefineTransfers(InputData data)
    {
        var decayPaths = new List<(Organ from, Organ to, bool hasCoeff)>();
        var decayChains = new Dictionary<Organ, DecayChain>();
        var decayNuclides = data.Nuclides.ToDictionary(n => n, n => GetDecayNuclides(n));

        // organFromから始まる崩壊系列の情報を取得する。
        DecayChain GetDecayChain(Organ organFrom)
        {
            var fromFunc = organFrom.Func;
            if (fromFunc != OrganFunc.acc && fromFunc != OrganFunc.exc)
                return null;
            if (organFrom.IsDecayCompartment)
                return null;

            var progenies = decayNuclides[organFrom.Nuclide];
            if (progenies.Length == 0)
                return null;

            if (!decayChains.TryGetValue(organFrom, out var decayChain))
            {
                decayChain = new DecayChain(organFrom, progenies);
                decayChains[organFrom] = decayChain;
            }
            return decayChain;
        }

        foreach (var nuclide in nuclides)
        {
            if (!CalcProgeny && nuclide.IsProgeny)
                continue;

            var nuc = nuclide.Name;
            if (!nuclideTransfers.TryGetValue(nuc, out var transfers))
                continue;

            var olderrorsN = errors.Count;
            var sectionLineNum = transferSectionLocs[nuc];

            // 移行経路の定義が正しいことの確認と、
            // 各コンパートメントから流出する移行係数の総計を求める。
            var definedTransfers = new HashSet<(string from, string to)>();
            var transfersCorrect = new List<(int lineNum, Organ from, Organ to, decimal coeff)>();
            var sumOfOutflowCoeff = new Dictionary<Organ, decimal>();
            foreach (var (lineNum, from, to, coeff, isRate) in transfers)
            {
                var olderrorsT = errors.Count;

                var fromName = from;
                var fromNuclide = nuclide;
                if (from.IndexOf('/') is int i && i != -1)
                {
                    var fromNuc = from.Substring(0, i);
                    fromName = from.Substring(i + 1);
                    fromNuclide = nuclides.FirstOrDefault(n => n.Name == fromNuc);
                    if (fromNuclide is null)
                        errors.AddError(lineNum, $"Undefined nuclide '{fromNuc}'.");
                }

                var toName = to;
                var toNuclide = nuclide;
                if (to.IndexOf('/') is int j && j != -1)
                {
                    var toNuc = to.Substring(0, j);
                    toName = to.Substring(j + 1);
                    toNuclide = nuclides.FirstOrDefault(n => n.Name == toNuc);
                    if (toNuclide is null)
                        errors.AddError(lineNum, $"Undefined nuclide '{toNuc}'.");
                }

                // 移行先は定義している核種に属するコンパートメントのみとする。
                if (toNuclide != null && toNuclide != nuclide)
                    errors.AddError(lineNum, $"Cannot set transfer path to a compartment which is not belong to '{nuc}'.");

                // 以降の処理でfromNuclideとtoNuclideの両方が存在していることを確定する。
                if (errors.IfAny(olderrorsT))
                    continue;

                var organFrom = data.Organs.FirstOrDefault(o => o.Name == fromName && o.Nuclide == fromNuclide);
                var organTo = data.Organs.FirstOrDefault(o => o.Name == toName && o.Nuclide == toNuclide);

                // 移行元と移行先のそれぞれがcompartmentセクションで定義済みかを確認する。
                if (organFrom is null || organTo is null)
                {
                    if (organFrom is null)
                        errors.AddError(lineNum, $"Undefined compartment '{from}'.");
                    if (organTo is null)
                        errors.AddError(lineNum, $"Undefined compartment '{to}'.");
                }
                else if (organTo == organFrom)
                {
                    // 自分自身への移行経路は定義できない。
                    errors.AddError(lineNum, "Cannot set transfer path to itself.");
                }
                else if (!definedTransfers.Add((from, to)))
                {
                    // 同じ移行経路が複数回定義されていないことを確認する。
                    errors.AddError(lineNum, $"Duplicated transfer path from '{from}' to '{to}'.");
                }
                // 以降の処理でorganFromとorganToの両方が存在していることを確定する。
                if (errors.IfAny(olderrorsT))
                    continue;

                // 正しくないコンパートメント機能間の移行経路が定義されていないことを確認する。
                var fromFunc = organFrom.Func;
                var toFunc = organTo.Func;
                var isDecayPath = fromNuclide != toNuclide;
                var hasCoeff = coeff != null;

                // inpへの流入は定義できない。
                if (toFunc == OrganFunc.inp)
                    errors.AddError(lineNum, $"Cannot set input path to inp '{to}'.");

                if (fromFunc == OrganFunc.acc)
                {
                    // accからの流出経路では、移行速度の入力を要求する。
                    // なお、ここでパーセント値を設定するのは明らかにおかしいので設定エラーとして弾く。
                    if (!isDecayPath && !hasCoeff || isRate)
                        errors.AddError(lineNum, $"Require transfer rate [/d] from {fromFunc} '{from}'.");
                }

                if (fromFunc == OrganFunc.exc)
                {
                    // excからの流出は(娘核種のexcへの壊変経路を除いて)定義できない。
                    if (!(toFunc == OrganFunc.exc && isDecayPath))
                        errors.AddError(lineNum, $"Cannot set output path from exc '{from}'.");
                }

                // TODO: mixからmixへの経路は定義できない。
                //if (fromFunc == OrganFunc.mix && toFunc == OrganFunc.mix)
                //    errors.AddError(lineNum, "Cannot set transfer path from 'mix' to 'mix'.");

                if (organFrom.IsInstantOutflow)
                {
                    // inpやmixから娘核種への壊変経路は定義できない。
                    if (isDecayPath)
                        errors.AddError(lineNum, $"Cannot set decay path from {fromFunc} '{from}'.");

                    // inpまたはmixからの同核種での移行経路では、移行割合の入力を要求する。
                    // なお、ここでは割合値(0.15など)とパーセント値(10.5%など)の両方を受け付ける。
                    else if (!hasCoeff)
                        errors.AddError(lineNum, $"Require fraction of output activity [%] from {fromFunc} '{from}'.");
                }

                // 以降の処理で移行経路の設定位置が有効であることを確定する。
                if (errors.IfAny(olderrorsT))
                    continue;

                if (isDecayPath)
                {
                    // 分岐比が不明な壊変経路は定義できない。
                    if (!decayNuclides[fromNuclide].Contains(toNuclide))
                        errors.AddError(lineNum, $"There is no decay path from {fromNuclide.Name} to {toNuclide.Name}.");

                    var paths = decayPaths.Where(path => path.from == organFrom);

                    // organFromから、同じ子孫核種への2つ以上の壊変経路は定義できない。
                    if (paths.Any(path => path.to.Nuclide == toNuclide))
                        errors.AddError(lineNum, $"Multiple decay paths from {fromFunc} '{from}' to nuclide '{toNuclide.Name}'.");

                    decayPaths.Add((organFrom, organTo, hasCoeff));

                    // 次のような壊変経路を、
                    //   Parent/organFrom --(coeff)--> Progeny_i/organTo  ①移行速度あり
                    //   Parent/organFrom -----------> Progeny_i/organTo  ②移行速度なし
                    //
                    // 親核種ParentがコンパートメントorganFromから移動しないまま壊変することで生成された
                    // 子孫核種Progeny_1～Nのそれぞれを受けるorganDecayを、自動的に追加定義する、
                    // あるいはインプットで定義済みのコンパートメントを直接使用することで、
                    // 以下ような経路に構成し直す。
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

                    var decayChain = GetDecayChain(organFrom);

                    var decayIndex = Array.IndexOf(decayChain.DecayNuclides, toNuclide);
                    if (decayIndex == -1)
                    {
                        var rootNuc = decayChain.RootCompartment.Nuclide.Name;
                        errors.AddError(lineNum, $"Cannot find progeny nuclide '{toNuclide.Name}' in decay chain starts from '{rootNuc}'.");
                        continue;
                    }

                    ref var organDecay = ref decayChain.DecayCompartments[decayIndex];
                    if (organDecay != null)
                    {
                        if (hasCoeff)
                            errors.AddError(lineNum, $"Decay compartment is already set.");
                        else
                            errors.AddError(lineNum, $"Decay compartment '{to}' conflicts with the implicitly deined one.");
                        continue;
                    }

                    if (hasCoeff)
                    {
                        organDecay = decayChain.AddDecayCompartment(data, toNuclide);

                        // 以降の処理を核種が同じコンパートメント間organDecay -> organToの経路設定にすり替える。
                        organFrom = organDecay;
                    }
                    else
                    {
                        // organToを崩壊系列でorganDecayが占める位置に設定する。
                        organDecay = organTo;

                        // transfersCorrectには核種が異なるコンパートメント間の壊変経路を追加しない。
                        continue;
                    }
                }

                if (coeff is decimal coeff_v)
                {
                    // 移行係数が負の値でないことを確認する。
                    if (coeff_v < 0)
                    {
                        errors.AddError(lineNum, "Transfer coefficient should be positive.");
                        continue;
                    }

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
                var fromName = organFrom.Name;

                if (organFrom.IsInstantOutflow)
                {
                    // 流出放射能に対する移行割合の合計が1.0 == 100%かどうかを確認する。
                    var sum = sumOfOutflowCoeff[organFrom];
                    if (sum != 1)
                    {
                        var ts = transfersCorrect.Where(t => t.from == organFrom).OrderBy(t => t.lineNum).ToArray();
                        for (int i = 0; i < ts.Length; i++)
                        {
                            var (lineNum, _, _, coeff) = ts[i];
                            if (i == 0)
                                errors.AddError(lineNum, $"Total [%] of transfer paths from '{fromName}' is  not 100%, but {sum * 100:G29}%.");
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

                    // fromからtoへの移行割合 = 移行速度[/d] / fromから流出する移行速度[/d]の総計
                    inflowRate = sum == 0 ? 0.0 : (double)coeff / (double)sum;
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
        foreach (var decayChain in decayChains.Values)
        {
            DefineDecayTransfers(data, decayChain);
        }

        // 移行経路の定義にエラーがないことを確定する。
        errors.RaiseIfAny();
    }

    /// <summary>
    /// 核種が異なるコンパートメント間の移行経路を定義する。
    /// </summary>
    /// <param name="data"></param>
    private void DefineDecayTransfers(InputData data, DecayChain chain)
    {
        var organFrom = chain.RootCompartment;
        var fromNuclide = organFrom.Nuclide;
        var decayTransfers = chain.GetDecayTransfers();
        var decayCompartments = chain.DecayCompartments;

        // 崩壊系列を構成するコンパートメント間の壊変経路を追加定義する。
        // 壊変経路は一本道ではなく、fromNuclideから始まる有効非巡回グラフ(DAG)を構成する点に注意。
        foreach (var path in decayTransfers)
        {
            var from = path.Parent == fromNuclide ? organFrom
                   : decayCompartments.FirstOrDefault(o => o?.Nuclide == path.Parent);
            var to = decayCompartments.FirstOrDefault(o => o?.Nuclide == path.Daughter);

            // 壊変経路が既に設定されている＝インプットで明示的に定義されている場合は何もしない。
            if (from != null && to != null && to.Inflows.Any(inflow => inflow.Organ == from))
                continue;

            if (from is null)
                from = chain.AddDecayCompartment(data, path.Parent);
            if (to is null)
                to = chain.AddDecayCompartment(data, path.Daughter);

            var branch = from.Nuclide.Branches.First(b => b.Daughter == to.Nuclide);

            to.Inflows.Add(new Inflow
            {
                ID = from.ID,

                // 壊変経路では、親からの分岐比を移行割合としてとする。
                Rate = branch.Fraction,

                // 流入経路から流入元臓器の情報を直接引くための参照を設定する。
                Organ = from,
            });
        }
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

        var calcScoeff = new CalcScoeff(safdata);
        calcScoeff.InterpolationMethod = "PCHIP";
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

        // 核種が考慮する線源領域の名称を設定する。
        nuclide.SourceRegions = sources;

        return table;
    }
}
