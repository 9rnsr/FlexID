using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace FlexID.Calc
{
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
    [DebuggerDisplay("{Func} {Name} ({Nuclide})")]
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
        /// excコンパートメントにおいて、OIRにおける24-hour smaple値を模擬した
        /// 残留放射能を出力する場合に <see langword="true"/>。
        /// </summary>
        public bool ExcretaCompatibleWithOIR;

        /// <summary>
        /// 生物学的崩壊定数[/day]。
        /// 蓄積コンパートメントのみで意味を持ち、それ以外では1.0となる。
        /// </summary>
        public double BioDecay;

        /// <summary>
        /// 流入経路。
        /// </summary>
        public List<Inflow> Inflows;

        /// <summary>
        /// コンパートメントへの流入がない場合に<see langword="true"/>。
        /// </summary>
        public bool IsZeroInflow;

        /// <summary>
        /// コンパートメントからの流出が即時に処理される場合に<see langword="true"/>。
        /// </summary>
        public bool IsInstantOutflow => Func == OrganFunc.inp || Func == OrganFunc.mix;

        /// <summary>
        /// 線源領域の名称。
        /// </summary>
        public string SourceRegion;

        /// <summary>
        /// コンパートメントに対応付けられた線源領域から各標的領域へのS係数。
        /// </summary>
        public double[] S_Coefficients;
    }

    [DebuggerDisplay("{Name}")]
    public class NuclideData
    {
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
        /// 親核種からの崩壊割合(100%＝1.00と置いた比で持つ)。
        /// </summary>
        public (string Parent, double Branch)[] DecayRates;

        /// <summary>
        /// 子孫核種の場合は<c>true</c>。
        /// </summary>
        public bool IsProgeny => DecayRates.Length != 0;

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
        public (int Index, double Rate)[] AtractIndexes = Array.Empty<(int, double)>();

        /// <summary>
        /// 肺に対応するコンパートメント群のインデックスと寄与率。
        /// </summary>
        public (int Index, double Rate)[] LungsIndexes = Array.Empty<(int, double)>();

        /// <summary>
        /// 骨格に対応するコンパートメント群のインデックスと寄与率。
        /// </summary>
        public (int Index, double Rate)[] SkeletonIndexes = Array.Empty<(int, double)>();

        /// <summary>
        /// 肝臓に対応するコンパートメント群のインデックスと寄与率。
        /// </summary>
        public (int Index, double Rate)[] LiverIndexes = Array.Empty<(int, double)>();

        /// <summary>
        /// 甲状腺に対応するコンパートメント群のインデックスと寄与率。
        /// </summary>
        public (int Index, double Rate)[] ThyroidIndexes = Array.Empty<(int, double)>();

        /// <summary>
        /// パラメータ定義。
        /// </summary>
        public Dictionary<string, string> Parameters;

        /// <summary>
        /// 有効なパラメータ名の配列。
        /// </summary>
        public static readonly string[] ParameterNames = new[]
        {
            "ExcludeOtherSourceRegions",
            "IncludeOtherSourceRegions"
        };
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
        public List<NuclideData> Nuclides = new List<NuclideData>();

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
        /// 核種毎のS係数データ表。
        /// </summary>
        public List<Dictionary<string, double[]>> SCoeffTables = new List<Dictionary<string, double[]>>();

        /// <summary>
        /// 全ての臓器。
        /// </summary>
        public List<Organ> Organs = new List<Organ>();

        /// <summary>
        /// パラメータ定義。
        /// </summary>
        public Dictionary<string, string> Parameters;

        /// <summary>
        /// 有効なパラメータ名の配列。
        /// </summary>
        public static readonly string[] ParameterNames = new[]
        {
            "ExcludeOtherSourceRegions",
            "IncludeOtherSourceRegions",
            "OutputDose",
            "OutputDoseRate",
            "OutputRetention",
            "OutputCumulative",
        };

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

    /// <summary>
    /// インプットファイルの読み取り処理。
    /// </summary>
    public class InputDataReader : IDisposable
    {
        /// <summary>
        /// インプットファイルの読み出し用TextReader。
        /// </summary>
        private readonly StreamReader reader;

        /// <summary>
        /// 子孫核種のインプットを読み飛ばす場合は<c>true</c>。
        /// </summary>
        private readonly bool calcProgeny;

        /// <summary>
        /// 行番号(1始まり)。
        /// </summary>
        private int lineNum;

        /// <summary>
        /// コンストラクタ。
        /// </summary>
        /// <param name="inputPath">インプットファイルのパス文字列。</param>
        /// <param name="calcProgeny">子孫核種を計算する＝読み込む場合は <see langword="true"/>。</param>
        public InputDataReader(string inputPath, bool calcProgeny = true)
        {
            var stream = new FileStream(inputPath, FileMode.Open, FileAccess.Read, FileShare.Read);
            var reader = new StreamReader(stream);

            this.reader = reader;
            this.calcProgeny = calcProgeny;
        }

        /// <summary>
        /// コンストラクタ。
        /// </summary>
        /// <param name="reader">インプットの読み込み元。</param>
        /// <param name="calcProgeny">子孫核種を計算する＝読み込む場合は <see langword="true"/>。</param>
        public InputDataReader(StreamReader reader, bool calcProgeny = true)
        {
            this.reader = reader;
            this.calcProgeny = calcProgeny;
        }

        public void Dispose() => reader.Dispose();

        private string GetNextLine()
        {
        Lagain:
            var line = reader.ReadLine();
            lineNum++;
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
        /// OIR用のインプットファイルを読み込む。
        /// </summary>
        /// <returns></returns>
        public InputData Read_OIR()
        {
            var evaluator = new InputEvaluator();

            string GetNextLine()
            {
            Lagain:
                var nextLine = this.GetNextLine();

                if (evaluator.TryReadVarDecl(lineNum, nextLine))
                    goto Lagain;

                return nextLine;
            }

            var line = GetNextLine();
            if (line is null)
                throw Program.Error("Reach to EOF while reading input file.");

            bool CheckSectionHeader(string ln) => ln.StartsWith("[");

            ReadOnlySpan<char> GetSectionHeader(string ln)
            {
                if (!CheckSectionHeader(ln))
                    throw Program.Error($"Line {lineNum}: Section header should start with '['");
                if (!ln.EndsWith("]"))
                    throw Program.Error($"Line {lineNum}: Section header should be closed with ']'.");

                return ln.AsSpan(1, ln.Length - 2).Trim();
            }

            var inputTitle = default(string);
            var inputParameters = default(Dictionary<string, string>);
            var nuclideParameters = new Dictionary<string, Dictionary<string, string>>();
            var nuclides = default(List<NuclideData>);
            var nuclideOrgans = new Dictionary<string, List<(int lineNum, Organ)>>();
            var nuclideTransfers = new Dictionary<string,
                    List<(int lineNum, string from, string to, decimal? coeff, bool isRate)>>();
            var SCoeffTables = new List<Dictionary<string, double[]>>();
            int organId = 0;

            while (true)
            {
                var header = GetSectionHeader(line);

                Span<char> buffer = stackalloc char[header.Length];
                for (int i = 0; i < header.Length; i++)
                    buffer[i] = char.ToLowerInvariant(header[i]);

                if (buffer.SequenceEqual("title".AsSpan()))
                {
                    GetTitle(out line);
                }
                else if (buffer.SequenceEqual("parameter".AsSpan()))
                {
                    GetParameters("", out line);
                }
                else if (buffer.EndsWith(":parameter".AsSpan()))
                {
                    var i = header.IndexOf(':');
                    var nuc = header.Slice(0, i).ToString();
                    GetParameters(nuc, out line);
                }
                else if (buffer.SequenceEqual("nuclide".AsSpan()))
                {
                    GetNuclides(out line);
                }
                else if (buffer.EndsWith(":compartment".AsSpan()))
                {
                    var i = header.IndexOf(':');
                    var nuc = header.Slice(0, i).ToString();
                    GetCompartments(nuc, out line);
                }
                else if (buffer.EndsWith(":transfer".AsSpan()))
                {
                    var i = header.IndexOf(':');
                    var nuc = header.Slice(0, i).ToString();
                    GetTransfers(nuc, out line);
                }
                else
                    throw Program.Error($"Line {lineNum}: Unrecognized section $'[{header.ToString()}]'.");

                if (line is null)
                    break;
            }

            // タイトルの定義セクションを読み込む。
            void GetTitle(out string nextLine)
            {
                if (inputTitle != null)
                    throw Program.Error($"Line {lineNum}: Duplicated [title] section.");

                var title = GetNextLine();
                if (title is null)
                    throw Program.Error($"Line {lineNum}: Reach to EOF while reading title section.");
                inputTitle = title;

                nextLine = GetNextLine();
                if (!CheckSectionHeader(nextLine))
                    throw Program.Error($"Line {lineNum}: Unrecognized line in [title] section.");
            }

            void GetParameters(string nuc, out string nextLine)
            {
                Dictionary<string, string> parameters;
                string[] parameterNames;
                if (nuc == "")
                {
                    if (inputParameters != null)
                        throw Program.Error($"Line {lineNum}: Duplicated [parameter] section.");
                    parameters = new Dictionary<string, string>();
                    parameterNames = InputData.ParameterNames;
                    inputParameters = parameters;
                }
                else
                {
                    if (nuclideParameters.ContainsKey(nuc))
                        throw Program.Error($"Line {lineNum}: Duplicated [{nuc}:parameter] section.");
                    parameters = new Dictionary<string, string>();
                    parameterNames = NuclideData.ParameterNames;
                    nuclideParameters.Add(nuc, parameters);
                }


                while (true)
                {
                    nextLine = GetNextLine();
                    if (nextLine is null)
                        break;
                    if (CheckSectionHeader(nextLine))
                        break;

                    var values = nextLine.Split(new string[] { "=" }, 2, StringSplitOptions.RemoveEmptyEntries);
                    if (values.Length != 2)
                        throw Program.Error($"Line {lineNum}: Parameter definition should have 2 values.");

                    var paramName = values[0].Trim();
                    var paramValue = values[1].Trim();

                    if (!parameterNames.Contains(paramName))
                        throw Program.Error($"Line {lineNum}: Unrecognized parameter '{paramName}' definition.");
                    if (parameters.ContainsKey(paramName))
                        throw Program.Error($"Line {lineNum}: Duplicated parameter '{paramName}' definition.");

                    parameters.Add(paramName, paramValue);
                }
            }

            // 核種の定義セクションを読み込む。
            void GetNuclides(out string nextLine)
            {
                if (nuclides != null)
                    throw Program.Error($"Line {lineNum}: Duplicated [nuclide] section.");

                nuclides = new List<NuclideData>();

                // 最初の1行を見て、新旧どちらの型式で入力されているかを判定する。
                var autoMode = default(bool?);

                Dictionary<string, IndexData> indexTable = null;
                var branches = new List<(string Parent, string Daugher, double Branch)>();

                while (true)
                {
                    nextLine = GetNextLine();
                    if (nextLine is null)
                        break;
                    if (CheckSectionHeader(nextLine))
                        break;

                    // 核種の定義行を読み込む。
                    var values = nextLine.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);

                    if (autoMode == true || autoMode == default && values.All(patternNuclide.IsMatch))
                    {
                        autoMode = true;

                        if (indexTable is null)
                            indexTable = IndexDataReader.ReadNDX().ToDictionary(x => x.Nuclide, x => x);

                        foreach (var nuc in values)
                        {
                            if (!patternNuclide.IsMatch(nuc))
                                throw Program.Error($"Line {lineNum}: '{nuc}' is not nuclide name.");

                            var nuclide = new NuclideData { Name = nuc };

                            if (indexTable.TryGetValue(nuc, out var indexData))
                            {
                                nuclide.HalfLife = indexData.HalfLife;
                                nuclide.Lambda = indexData.Lambda;

                                branches.AddRange(indexData.Daughters.Select(d => (nuc, d.Daughter, (double)d.Branch)));
                            }
                            else
                            {
                                // NDXファイルに定義されていないものは安定核種として扱う。
                                nuclide.HalfLife = "---";
                                nuclide.Lambda = 0.0;
                            }

                            nuclides.Add(nuclide);
                        }
                        continue;
                    }
                    else
                        autoMode = false;

                    if (values.Length != 3)
                        throw Program.Error($"Line {lineNum}: Nuclide definition should have 3 values.");

                    if (!double.TryParse(values[1], out var lambda))
                        throw Program.Error($"Line {lineNum}: Cannot get nuclide Lambda.");
                    if (lambda < 0)
                        throw Program.Error($"Line {lineNum}: Nuclide Lambda should be positive.");

                    if (!double.TryParse(values[2], out var decayRate))
                        throw Program.Error($"Line {lineNum}: Cannot get nuclide DecayRate.");
                    if (decayRate < 0)
                        throw Program.Error($"Line {lineNum}: Nuclide DecayRate should be positive.");

                    nuclides.Add(new NuclideData
                    {
                        Name = values[0],
                        Lambda = lambda,
                        DecayRates = nuclides.Count == 0
                            ? Array.Empty<(string Parent, double Branch)>()
                            : new[] { (Parent: nuclides.Last().Name, Branch: decayRate) },
                    });
                }

                if (autoMode == true && nuclides.Any())
                {
                    // 親核種のDecayRatesについては空に設定する。
                    nuclides.First().DecayRates = Array.Empty<(string Parent, double Branch)>();

                    // 親を持つ子孫核種のDecayRatesについて設定する。
                    foreach (var nuclide in nuclides.Skip(1))
                    {
                        var nuc = nuclide.Name;

                        var decayRates = branches.Where(b => b.Daugher == nuc).Select(b => (b.Parent, b.Branch)).ToArray();
                        if (!decayRates.Any())
                            throw Program.Error($"Progeny nuclide '{nuc}' has no decay path from any other nuclides.");

                        nuclide.DecayRates = decayRates;
                    }
                }
            }

            // コンパートメントの定義セクションを読み込む。
            void GetCompartments(string nuc, out string nextLine)
            {
                if (nuclideOrgans.TryGetValue(nuc, out var organs))
                    throw Program.Error($"Line {lineNum}: Duplicated [{nuc}:compartment] section.");

                organs = new List<(int lineNum, Organ)>();
                nuclideOrgans.Add(nuc, organs);

                while (true)
                {
                    nextLine = GetNextLine();
                    if (nextLine is null)
                        break;
                    if (CheckSectionHeader(nextLine))
                        break;

                    var values = nextLine.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);

                    if (values.Length != 3)
                        throw Program.Error($"Line {lineNum}: Compartment definition should have 3 values.");

                    var organFn = values[0];        // コンパートメント機能
                    var organName = values[1];      // コンパートメント名
                    var sourceRegion = values[2];   // コンパートメントに対応する線源領域の名称

                    var organFunc =
                        organFn == "inp" ? OrganFunc.inp :
                        organFn == "acc" ? OrganFunc.acc :
                        organFn == "mix" ? OrganFunc.mix :
                        organFn == "exc" ? OrganFunc.exc :
                        throw Program.Error($"Line {lineNum}: Unrecognized compartment function '{organFn}'.");

                    var organ = new Organ
                    {
                        Nuclide = null,     // 後で設定する。
                        ID = ++organId,
                        Index = -1,         // 後で設定する。
                        Name = organName,
                        Func = organFunc,
                        BioDecay = 1.0,     // accは後で設定する。
                        Inflows = new List<Inflow>(),
                    };

                    // 線源領域の名称については、妥当性を後で確認する。
                    organ.SourceRegion = sourceRegion;

                    organs.Add((lineNum, organ));
                }
            }

            // 移行係数の定義セクションを読み込む。
            void GetTransfers(string nuc, out string nextLine)
            {
                if (nuclideTransfers.TryGetValue(nuc, out var transfers))
                    throw Program.Error($"Line {lineNum}: Duplicated [{nuc}:transfer] section.");

                transfers = new List<(int, string, string, decimal?, bool)>();
                nuclideTransfers.Add(nuc, transfers);

                while (true)
                {
                    nextLine = GetNextLine();
                    if (nextLine is null)
                        break;
                    if (CheckSectionHeader(nextLine))
                        break;

                    var values = nextLine.Split(new string[] { " " }, 3, StringSplitOptions.RemoveEmptyEntries);

                    if (values.Length != 3)
                        throw Program.Error($"Line {lineNum}: Transfer path definition should have 3 values.");

                    var orgamFrom = values[0];
                    var organTo = values[1];
                    var coeffStr = values[2];    // 移行係数、[/d] or [%]

                    var coeff = default(decimal?);
                    var isRate = false;
                    if (!IsBar(coeffStr))
                        (coeff, isRate) = evaluator.ReadCoefficient(lineNum, coeffStr);

                    transfers.Add((lineNum, orgamFrom, organTo, coeff, isRate));
                }
            }

            var data = new InputData();

            if (inputTitle is null)
                throw Program.Error($"Missing [title] section.");
            data.Title = inputTitle;

            data.Parameters = inputParameters ?? new Dictionary<string, string>();

            // 線源領域と標的領域のデータを読み込む。
            var sourceRegions = SAFDataReader.ReadSourceRegions();
            var targetRegions = SAFDataReader.ReadTargetRegions().Select(t => t.Name).ToArray();
            data.SourceRegions = sourceRegions;
            data.TargetRegions = targetRegions;

            // 組織加重係数データを読み込む。
            var (ts, ws) = ReadTissueWeights(Path.Combine("lib", "OIR", "wT.txt"));
            if (!Enumerable.SequenceEqual(data.TargetRegions, ts))
                throw Program.Error($"Found mismatch of target region names on tissue weighting factor data.");
            data.TargetWeights = ws;

            if (nuclides is null)
                throw Program.Error($"Missing [nuclide] section.");
            if (!nuclides.Any())
                throw Program.Error($"None of nuclides defined.");

            Organ input = null;

            // 全ての核種のコンパートメントを定義する。
            foreach (var nuclide in nuclides)
            {
                if (!calcProgeny && nuclide.IsProgeny)
                    continue;

                var nuc = nuclide.Name;
                if (!nuclideOrgans.TryGetValue(nuc, out var organs))
                    throw Program.Error($"Missing [{nuc}:compartment] section.");
                if (!organs.Any())
                    throw Program.Error($"None of compartments defined for nuclide '{nuc}'.");

                if (!nuclideTransfers.TryGetValue(nuc, out var transfers))
                    throw Program.Error($"Missing [{nuc}:transfer] section.");
                if (!transfers.Any())
                    throw Program.Error($"None of transfers defined for nuclide '{nuc}'.");

                nuclide.Parameters = nuclideParameters?.GetValueOrDefault(nuc) ?? new Dictionary<string, string>();

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
                            throw Program.Error($"Line {lineNum}: Cannot define 'inp' compartment which belongs to progeny nuclide.");
                        if (input is null)
                            input = organ;
                        else
                            throw Program.Error($"Line {lineNum}: Duplicated 'inp' compartment.");
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
                            throw Program.Error($"Line {lineNum}: Unknown source region name '{sourceRegion}'.");

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
                            organ.ExcretaCompatibleWithOIR = true;
                        }
                    }
                }

                if (!nuclide.IsProgeny && input is null)
                    throw Program.Error($"Missing 'inp' compartment.");

                nuclide.OtherSourceRegions = otherSourceRegions.ToArray();

                // 核種に対応するS係数データを読み込む。
                var tableSCoeff = ReadSCoeff(data, nuclide);
                data.SCoeffTables.Add(tableSCoeff);

                foreach (var (lineNum, organ) in organs)
                {
                    var sourceRegion = organ.SourceRegion;
                    if (sourceRegion != null)
                    {
                        // コンパートメントの放射能を各標的領域に振り分けるためのS係数データを関連付ける。
                        organ.S_Coefficients = tableSCoeff[sourceRegion];
                    }
                }
            }

            // コンパートメント間の移行経路を定義する。
            foreach (var nuclide in nuclides)
            {
                if (!calcProgeny && nuclide.IsProgeny)
                    continue;

                var nuc = nuclide.Name;
                var transfers = nuclideTransfers[nuc];

                // 移行経路の定義が正しいことの確認と、
                // 各コンパートメントから流出する移行係数の総計を求める。
                var definedTransfers = new HashSet<(string from, string to)>();
                var transfersCorrect = new List<(Organ from, Organ to, decimal coeff)>();
                var sumOfOutflowCoeff = new Dictionary<Organ, decimal>();
                foreach (var (lineNum, from, to, coeff, isRate) in transfers)
                {
                    var fromName = from;
                    var fromNuclide = nuclide;
                    if (from.IndexOf('/') is int i && i != -1)
                    {
                        var fromNuc = from.Substring(0, i);
                        fromName = from.Substring(i + 1);
                        fromNuclide = nuclides.FirstOrDefault(n => n.Name == fromNuc);
                        if (fromNuclide is null)
                            throw Program.Error($"Line {lineNum}: Undefined nuclide '{fromNuc}'.");
                    }

                    var toName = to;
                    var toNuclide = nuclide;
                    if (to.IndexOf('/') is int j && j != -1)
                    {
                        var toNuc = to.Substring(0, j);
                        toName = to.Substring(j + 1);
                        toNuclide = nuclides.FirstOrDefault(n => n.Name == toNuc);
                        if (toNuclide is null)
                            throw Program.Error($"Line {lineNum}: Undefined nuclide '{toNuc}'.");
                    }

                    // 移行先は定義している核種に属するコンパートメントのみとする。
                    if (toNuclide != nuclide)
                        throw Program.Error($"Line {lineNum}: Cannot set transfer path to a compartment which is not belong to '{nuc}'.");

                    var organFrom = data.Organs.FirstOrDefault(o => o.Name == fromName && o.Nuclide == fromNuclide);
                    var organTo = data.Organs.FirstOrDefault(o => o.Name == toName && o.Nuclide == toNuclide);

                    // 移行元と移行先のそれぞれがcompartmentセクションで定義済みかを確認する。
                    if (organFrom is null)
                        throw Program.Error($"Line {lineNum}: Undefined compartment '{fromName}'.");
                    if (organTo is null)
                        throw Program.Error($"Line {lineNum}: Undefined compartment '{toName}'.");

                    // 自分自身への移行経路は定義できない。
                    if (organTo == organFrom)
                        throw Program.Error($"Line {lineNum}: Cannot set transfer path to itself.");

                    // 同じ移行経路が複数回定義されていないことを確認する。
                    if (!definedTransfers.Add((from, to)))
                        throw Program.Error($"Line {lineNum}: Duplicated transfer path from '{fromName}' to '{toName}'.");

                    // 正しくないコンパートメント機能間の移行経路が定義されていないことを確認する。
                    var fromFunc = organFrom.Func;
                    var toFunc = organTo.Func;
                    var isDecayPath = fromNuclide != toNuclide;
                    var hasCoeff = coeff != null;

                    // inpへの流入は定義できない。
                    if (toFunc == OrganFunc.inp)
                        throw Program.Error($"Line {lineNum}: Cannot set input path to inp '{toName}'.");

                    // excからの流出は(娘核種のexcへの壊変経路を除いて)定義できない。
                    if (fromFunc == OrganFunc.exc && !(toFunc == OrganFunc.exc && isDecayPath))
                        throw Program.Error($"Line {lineNum}: Cannot set output path from exc '{fromName}'.");

                    // TODO: mixからmixへの経路は定義できない。
                    //if (fromFunc == OrganFunc.mix && toFunc == OrganFunc.mix)
                    //    throw Program.Error($"Line {lineNum}: Cannot set transfer path from 'mix' to 'mix'.");

                    if (isDecayPath)
                    {
                        // 分岐比が不明な壊変経路は定義できない。
                        if (!organTo.Nuclide.DecayRates.Any(b => b.Parent == fromNuclide.Name))
                            throw Program.Error($"Line {lineNum}: There is no decay path from {fromNuclide.Name} to {toNuclide.Name}.");

                        // inpやmixから娘核種への壊変経路は定義できない。
                        if (organFrom.IsInstantOutflow)
                            throw Program.Error($"Line {lineNum}: Cannot set decay path from {fromFunc} '{fromName}'.");

                        // 親核種からの壊変経路では、係数は指定できない。
                        if (coeff != null)
                            throw Program.Error($"Line {lineNum}: Cannot set transfer coefficient on decay path.");
                    }
                    else
                    {
                        // inpまたはmixからの配分経路では、移行割合の入力を要求する。
                        // なお、ここでは割合値(0.15など)とパーセント値(10.5%など)の両方を受け付ける。
                        if (organFrom.IsInstantOutflow && !hasCoeff)
                            throw Program.Error($"Line {lineNum}: Require fraction of output activity [%] from {fromFunc} '{fromName}'.");

                        // accからの流出経路では、移行速度の入力を要求する。
                        // なお、ここでパーセント値を設定するのは明らかにおかしいので設定エラーとして弾く。
                        if (fromFunc == OrganFunc.acc && (!hasCoeff || isRate))
                            throw Program.Error($"Line {lineNum}: Require transfer rate [/d] from {fromFunc} '{fromName}'.");
                    }
                    if (coeff is decimal coeff_v)
                    {
                        // 移行係数が負の値でないことを確認する。
                        if (coeff_v < 0)
                            throw Program.Error($"Line {lineNum}: Transfer coefficient should be positive.");

                        if (!sumOfOutflowCoeff.TryGetValue(organFrom, out var sum))
                            sum = 0;
                        sumOfOutflowCoeff[organFrom] = sum + coeff_v;
                    }

                    transfersCorrect.Add((organFrom, organTo, coeff ?? 0));
                }

                // あるコンパートメントから同じ核種のまま流出する全ての移行経路について処理する。
                var outflowGroups = transfersCorrect
                    .Where(t => t.from.Nuclide == nuclide).GroupBy(t => t.from);
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
                            throw Program.Error($"Total [%] of transfer paths from '{fromName}' is  not 100%, but {sum * 100:G29}%.");
                    }
                    else
                    {
                        // fromにおける生物学的崩壊定数[/day]を設定する。
                        if (organFrom.Func == OrganFunc.acc)
                            organFrom.BioDecay = (double)sumOfOutflowCoeff[organFrom];
                    }
                }

                // 各コンパートメントへの流入経路と、移行割合を設定する。
                foreach (var (organFrom, organTo, coeff) in transfersCorrect)
                {
                    double inflowRate;
                    if (organFrom.Nuclide != nuclide)
                    {
                        // 親から子への移行経路では、親からの分岐比とする。
                        var parentNuclide = organFrom.Nuclide.Name;
                        var branch = organTo.Nuclide.DecayRates.First(b => b.Parent == parentNuclide).Branch;

                        inflowRate = branch;
                    }
                    else if (organFrom.IsInstantOutflow)
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
            input.IsZeroInflow = true;

            bool CheckOutput(string name) =>
                data.Parameters.TryGetValue(name, out var str) && bool.TryParse(str, out var value) ? value : true;

            // 出力ファイルの設定。
            data.OutputDose = CheckOutput("OutputDose");
            data.OutputDoseRate = CheckOutput("OutputDoseRate");
            data.OutputRetention = CheckOutput("OutputRetention");
            data.OutputCumulative = CheckOutput("OutputCumulative");

            return data;
        }

        /// <summary>
        /// S係数データを読み込む。
        /// </summary>
        /// <param name="data"></param>
        /// <param name="nuclide">対象核種。線源領域の名称が設定される。</param>
        /// <returns>キーが線源領域の名称、値が各標的領域に対する成人男女平均のS係数、となる辞書。</returns>
        private static Dictionary<string, double[]> ReadSCoeff(InputData data, NuclideData nuclide)
        {
            var nuc = nuclide.Name;
            var fileAM = $"{nuc}_AM.txt";
            var fileAF = $"{nuc}_AF.txt";

            using (var readerAM = new StreamReader(Path.Combine("lib", "OIR", "Scoeff", fileAM)))
            using (var readerAF = new StreamReader(Path.Combine("lib", "OIR", "Scoeff", fileAF)))
            {
                var sources = data.SourceRegions.Select(s => s.Name).ToArray();
                var targets = data.TargetRegions;
                var sourcesCount = sources.Length;
                var targetsCount = targets.Length;

                // 1行目から線源領域の名称を配列で取得。
                var sourcesAM = readerAM.ReadLine()?.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries).Skip(1).ToArray();
                var sourcesAF = readerAF.ReadLine()?.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries).Skip(1).ToArray();
                if (sourcesAM is null) throw Program.Error($"Incorrect S-Coefficient file format: {fileAM}");
                if (sourcesAF is null) throw Program.Error($"Incorrect S-Coefficient file format: {fileAF}");
                if (!Enumerable.SequenceEqual(sources, sourcesAM) || !Enumerable.SequenceEqual(sources, sourcesAF))
                    throw Program.Error($"Found mismatch of source region names in S-Coefficient data for nuclide {nuc}.");

                var table = sources.ToDictionary(s => s, s => new double[targetsCount]);

                var columnOther = new double[targetsCount];
                var indexOtherSources = nuclide.OtherSourceRegions.Select(s => Array.IndexOf(sources, s)).ToArray();

                for (int indexT = 0; indexT < targetsCount; indexT++)
                {
                    var columnsAM = readerAM.ReadLine()?.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);
                    var columnsAF = readerAF.ReadLine()?.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);
                    if (columnsAM?.Length != 1 + sourcesCount) throw Program.Error($"Incorrect S-Coefficient file format: {fileAM}");
                    if (columnsAF?.Length != 1 + sourcesCount) throw Program.Error($"Incorrect S-Coefficient file format: {fileAF}");

                    // 各行の1列目から標的領域の名称を取得。
                    var targetAM = columnsAM[0];
                    var targetAF = columnsAF[0];
                    if (targets[indexT] != targetAM || targets[indexT] != targetAF)
                        throw Program.Error($"Found mismatch of target region names in S-Coefficient data for nuclide {nuc}.");

                    // 各線源領域からのS係数を取得する。
                    var valuesAM = columnsAM.Skip(1).Select(v => double.Parse(v)).ToArray();
                    var valuesAF = columnsAF.Skip(1).Select(v => double.Parse(v)).ToArray();

                    for (int indexS = 0; indexS < sourcesCount; indexS++)
                    {
                        var sourceRegion = sources[indexS];
                        var scoeffAM = valuesAM[indexS];
                        var scoeffAF = valuesAF[indexS];

                        // ここでS係数の男女平均を取る。
                        var scoeff = (scoeffAM + scoeffAF) / 2;
                        table[sourceRegion][indexT] = scoeff;
                    }

                    // 線源領域'Other'のS係数を計算する。
                    var massOtherAM = 0.0;
                    var massOtherAF = 0.0;
                    var scoeffOtherAM = 0.0;
                    var scoeffOtherAF = 0.0;
                    foreach (var indexS in indexOtherSources)
                    {
                        var sourceRegion = sources[indexS];
                        var massAM = data.SourceRegions[indexS].MaleMass;
                        var massAF = data.SourceRegions[indexS].FemaleMass;
                        var scoeffAM = valuesAM[indexS];
                        var scoeffAF = valuesAF[indexS];

                        massOtherAM += massAM;
                        massOtherAF += massAF;
                        scoeffOtherAM += massAM * scoeffAM;
                        scoeffOtherAF += massAF * scoeffAF;
                    }
                    if (massOtherAM != 0) scoeffOtherAM /= massOtherAM;
                    if (massOtherAF != 0) scoeffOtherAF /= massOtherAF;

                    // ここでS係数の男女平均を取る。
                    var scoeffOther = (scoeffOtherAM + scoeffOtherAF) / 2;
                    columnOther[indexT] = scoeffOther;
                }

                // S係数データに'Other'列を追加する
                table["Other"] = columnOther;

                // 核種が考慮する線源領域の名称を設定する。
                nuclide.SourceRegions = sources;

                return table;
            }
        }

        /// <summary>
        /// EIR用のインプットファイルを読み込む。
        /// </summary>
        /// <returns></returns>
        public List<InputData> Read_EIR()
        {
            var dataList = new List<InputData>();
            dataList.Add(Read_EIR("Age:3month"));
            dataList.Add(Read_EIR("Age:1year"));
            dataList.Add(Read_EIR("Age:5year"));
            dataList.Add(Read_EIR("Age:10year"));
            dataList.Add(Read_EIR("Age:15year"));
            dataList.Add(Read_EIR("Age:adult"));
            return dataList;
        }

        public InputData Read_EIR(string age)
        {
            // 読み取り位置をファイル先頭に戻す。
            reader.BaseStream.Position = 0;
            reader.DiscardBufferedData();
            lineNum = 0;

            var title = GetNextLine();
            if (title is null)
                throw Program.Error("Reach to EOF while reading input file.");

            var data = new InputData();

            data.Title = title;

            data.StartAge =
                age == "Age:3month" /**/? 100 :
                age == "Age:1year"  /**/? 365 :
                age == "Age:5year"  /**/? 365 * 5 :
                age == "Age:10year" /**/? 365 * 10 :
                age == "Age:15year" /**/? 365 * 15 :
                age == "Age:adult"  /**/? 365 * 25 : // 現在はSrしか計算しないため25歳で決め打ち、今後インプット等で成人の年齢を読み込む必要あり？
                throw new NotSupportedException();

            {
                var isProgeny = false;
            Lcont:
                var firstLine = GetNextLine();
                if (firstLine is null)
                    throw Program.Error("Reach to EOF while reading input file.");

                // 核種のヘッダ行を読み込む。
                var values = firstLine.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);

                var nuclide = new NuclideData
                {
                    Name = values[0],
                    Lambda = double.Parse(values[1]),
                    DecayRates = data.Nuclides.Count == 0
                        ? Array.Empty<(string Parent, double Branch)>()
                        : new[] { (Parent: data.Nuclides.Last().Name, Branch: double.Parse(values[2])) },
                };
                data.Nuclides.Add(nuclide);

                if (!isProgeny)
                {
                    // 組織加重係数データを読み込む。
                    var (ts, ws) = ReadTissueWeights(Path.Combine("lib", "EIR", "wT.txt"));
                    data.TargetRegions = ts;
                    data.TargetWeights = ws;

                    // 親核種の場合、指定年齢に対するインプットが定義された行まで読み飛ばす。
                    while (true)
                    {
                        var ln = GetNextLine();
                        if (ln is null)
                            throw Program.Error("Reach to EOF while reading input file.");
                        if (ln == age)
                            break;
                    }
                }

                // 核種に対応するS係数データを読み込む。
                var tableSCoeff = ReadSee(data, age, nuclide);
                data.SCoeffTables.Add(tableSCoeff);

                // 核種の体内動態モデル構成するコンパートメントの定義行を読み込む。
                while (true)
                {
                    var ln = GetNextLine();
                    if (ln is null)
                        throw Program.Error("Reach to EOF while reading input file.");
                    if (ln == "end" || ln == "next")
                        break;

                    if (ln == "cont")
                    {
                        if (calcProgeny == false)
                            break;

                        isProgeny = true;
                        goto Lcont;
                    }

                    values = ln.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);

                    if (values.Length != 8)
                        throw Program.Error($"Line {lineNum}: First line of compartment definition should have 8 values.");

                    var organId = int.Parse(values[0]);     // 臓器番号
                    var organName = values[1];              // 臓器名
                    var organFn = values[2];                // 臓器機能名称
                    var bioDecay = double.Parse(values[3]); // 生物学的崩壊定数
                    var inflowNum = int.Parse(values[4]);   // 流入臓器数
                    var sourceRegion = values[7];           // 臓器に対応する線源領域の名称

                    var organFunc =
                        organFn == "inp" ? OrganFunc.inp :
                        organFn == "acc" ? OrganFunc.acc :
                        organFn == "mix" ? OrganFunc.mix :
                        organFn == "exc" ? OrganFunc.exc :
                        throw Program.Error($"Line {lineNum}: Unrecognized organ function '{organFn}'.");

                    if (organFunc != OrganFunc.acc)
                        bioDecay = 1.0;

                    var organ = new Organ
                    {
                        Nuclide = nuclide,
                        ID = organId,
                        Index = data.Organs.Count,
                        Name = organName,
                        Func = organFunc,
                        BioDecay = bioDecay,
                        Inflows = new List<Inflow>(inflowNum),
                    };

                    if (!IsBar(sourceRegion))
                    {
                        // コンパートメントに対応する線源領域がS係数データに存在することを確認する。
                        var indexS = Array.IndexOf(nuclide.SourceRegions, sourceRegion);
                        if (indexS == -1)
                            throw Program.Error($"Line {lineNum}: Unknown source region name: '{sourceRegion}'");

                        // コンパートメントの放射能を各標的領域に振り分けるためのS係数データを関連付ける。
                        organ.SourceRegion = sourceRegion;
                        organ.S_Coefficients = tableSCoeff[sourceRegion];
                    }

                    if (organ.Func == OrganFunc.exc)
                    {
                        if (organ.Name == "Urine" || organ.Name == "Faeces")
                        {
                            organ.ExcretaCompatibleWithOIR = true;
                        }
                    }

                    // コンパートメントへの流入経路の記述を読み込む。
                    if (organ.Func == OrganFunc.inp)
                    {
                        if (inflowNum != 0)
                            throw Program.Error($"Line {lineNum}: The number of inflow paths in the Input compartment should be 0.");
                    }
                    else
                    {
                        if (inflowNum <= 0)
                            throw Program.Error($"Line {lineNum}: The number of inflow paths should be >= 1.");

                        for (int i = 0; i < inflowNum; i++)
                        {
                            int inflowID;
                            double inflowRate;
                            if (i == 0)
                            {
                                inflowID = int.Parse(values[5]);
                                inflowRate = double.Parse(values[6]);
                            }
                            else
                            {
                                ln = GetNextLine();
                                if (ln is null)
                                    throw Program.Error("Reach to EOF while reading input file.");
                                values = ln.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);
                                if (values.Length != 2)
                                    throw Program.Error($"Line {lineNum}: Continuous lines of compartment definition should have 2 values.");

                                inflowID = int.Parse(values[0]);
                                inflowRate = double.Parse(values[1]);
                            }

                            organ.Inflows.Add(new Inflow
                            {
                                ID = inflowID,
                                Rate = inflowRate * 0.01,
                            });
                        }
                    }

                    data.Organs.Add(organ);
                }
            }

            foreach (var organ in data.Organs)
            {
                foreach (var inflow in organ.Inflows)
                {
                    if (inflow.ID == 0)
                        continue;

                    // 流入経路から流入元臓器の情報を直接引くための参照を設定する。
                    inflow.Organ = data.Organs.First(o => o.ID == inflow.ID);

                    // 流入割合がマイナスの時の処理は親からの分岐比とする。
                    if (inflow.Rate < 0)
                    {
                        inflow.Rate = organ.Nuclide.DecayRates[0].Branch;
                    }
                }
            }

            // 初期配分を終えた後は流入なし。
            foreach (var input in data.Organs.Where(o => o.Func == OrganFunc.inp))
                input.IsZeroInflow = true;

            return data;
        }

        /// <summary>
        /// SEEデータを読み込む。
        /// </summary>
        /// <param name="data"></param>
        /// <param name="age">被ばく評価期間の開始年齢</param>
        /// <param name="nuclide">対象核種。線源領域の名称が設定される。</param>
        /// <returns>キーが線源領域の名称、値が各標的領域に対する成人男女平均のS係数、となる辞書。</returns>
        private static Dictionary<string, double[]> ReadSee(InputData data, string age, NuclideData nuclide)
        {
            var nuc = nuclide.Name;
            var file = $"{nuc}.txt";

            using (var reader = new StreamReader(Path.Combine("lib", "EIR", "SEE", file)))
            {
                while (reader.ReadLine() != age)
                { }

                // 2行目から線源領域の名称を配列で取得。
                var sources = reader.ReadLine()?.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries).Skip(1).ToArray();
                if (sources is null)
                    throw Program.Error($"Incorrect SEE file format: {file}");
                if (nuclide.SourceRegions != null && !Enumerable.SequenceEqual(nuclide.SourceRegions, sources))
                    throw Program.Error($"Incorrect SEE file format: {file}");
                var sourcesCount = sources.Length;

                var targets = new string[31];
                var table = sources.ToDictionary(s => s, s => new double[31]);
                for (int indexT = 0; indexT < 31; indexT++)
                {
                    var values = reader.ReadLine()?.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);
                    if (values?.Length != 1 + sourcesCount) throw Program.Error($"Incorrect S-Coefficient file format: {file}");

                    // 各行の1列目から標的領域の名称を取得。
                    var target = values[0];
                    targets[indexT] = target;

                    for (int indexS = 0; indexS < sourcesCount; indexS++)
                    {
                        var sourceRegion = sources[indexS];
                        var scoeff = double.Parse(values[1 + indexS]);
                        table[sourceRegion][indexT] = scoeff;
                    }
                }

                // 核種が考慮する線源領域の名称を設定する。
                nuclide.SourceRegions = sources;

                if (!Enumerable.SequenceEqual(data.TargetRegions, targets))
                    throw Program.Error($"Found mismatch of target region names between tissue weighting factor data and S-Coefficient data for nuclide {nuc}.");

                return table;
            }
        }

        /// <summary>
        /// 組織加重係数データを読み込む。
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        private static (string[] targets, double[] weights) ReadTissueWeights(string fileName)
        {
            var targets = new List<string>();
            var weights = new List<double>();

            var fileLines = File.ReadLines(fileName);
            foreach (var line in fileLines.Skip(1))  // 1行目は読み飛ばす
            {
                var values = line.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);
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
        private static readonly Regex patternNuclide = new Regex(@"^[A-Za-z]+-\d+(?:[a-z]|m\d)?$", RegexOptions.Compiled);

        private static readonly Regex patternBar = new Regex("^-+$", RegexOptions.Compiled);

        bool IsBar(string s) => patternBar.IsMatch(s);
    }
}
