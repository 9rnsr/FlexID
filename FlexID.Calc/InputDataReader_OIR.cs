using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace FlexID.Calc
{
    /// <summary>
    /// OIR用インプットファイルの読み取り処理。
    /// </summary>
    public class InputDataReader_OIR : InputDataReaderBase
    {
        private readonly InputEvaluator evaluator = new InputEvaluator();

        private string inputTitle;

        private Dictionary<string, string> inputParameters;

        private Dictionary<string, Dictionary<string, string>> nuclideParameters = new Dictionary<string, Dictionary<string, string>>();

        private List<NuclideData> nuclides;

        private Dictionary<string, List<(int lineNum, Organ)>> nuclideOrgans = new Dictionary<string, List<(int lineNum, Organ)>>();

        private Dictionary<string, List<(int lineNum, string from, string to, decimal? coeff, bool isRate)>> nuclideTransfers =
            new Dictionary<string, List<(int lineNum, string from, string to, decimal? coeff, bool isRate)>>();

        private List<Dictionary<string, double[]>> SCoeffTables = new List<Dictionary<string, double[]>>();

        /// <summary>
        /// コンストラクタ。
        /// </summary>
        /// <param name="inputPath">インプットファイルのパス文字列。</param>
        /// <param name="calcProgeny">子孫核種を計算する＝読み込む場合は <see langword="true"/>。</param>
        public InputDataReader_OIR(string inputPath, bool calcProgeny = true)
            : base(new StreamReader(new FileStream(inputPath, FileMode.Open, FileAccess.Read, FileShare.Read)), calcProgeny)
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
            if (!CheckSectionHeader(ln))
                throw Program.Error($"Line {LineNum}: Section header should start with '['");
            if (!ln.EndsWith("]"))
                throw Program.Error($"Line {LineNum}: Section header should be closed with ']'.");

            return ln.AsSpan(1, ln.Length - 2).Trim();
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
                    throw Program.Error($"Line {LineNum}: Unrecognized section $'[{header.ToString()}]'.");

                if (line is null)
                    break;
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

            // 全てのコンパートメントを定義する。
            DefineCompartments(data);

            // コンパートメント間の移行経路を定義する。
            DefineTransfers(data);

            // 流入を持たないコンパートメントをマークする。
            MarkZeroInflows(data);

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
        /// タイトルの定義セクションを読み込む。
        /// </summary>
        /// <returns>セクションの次行。</returns>
        private string GetTitle()
        {
            if (inputTitle != null)
                throw Program.Error($"Line {LineNum}: Duplicated [title] section.");

            var title = GetNextLine();
            if (title is null)
                throw Program.Error($"Line {LineNum}: Reach to EOF while reading title section.");
            inputTitle = title;

            var line = GetNextLine();
            if (!CheckSectionHeader(line))
                throw Program.Error($"Line {LineNum}: Unrecognized line in [title] section.");

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
                    throw Program.Error($"Line {LineNum}: Duplicated [parameter] section.");
                parameters = new Dictionary<string, string>();
                parameterNames = InputData.ParameterNames;
                inputParameters = parameters;
            }
            else
            {
                if (nuclideParameters.ContainsKey(nuc))
                    throw Program.Error($"Line {LineNum}: Duplicated [{nuc}:parameter] section.");
                parameters = new Dictionary<string, string>();
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
                    throw Program.Error($"Line {LineNum}: Parameter definition should have 2 values.");

                var paramName = values[0].Trim();
                var paramValue = values[1].Trim();

                if (!parameterNames.Contains(paramName))
                    throw Program.Error($"Line {LineNum}: Unrecognized parameter '{paramName}' definition.");
                if (parameters.ContainsKey(paramName))
                    throw Program.Error($"Line {LineNum}: Duplicated parameter '{paramName}' definition.");

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
                throw Program.Error($"Line {LineNum}: Duplicated [nuclide] section.");

            nuclides = new List<NuclideData>();

            // 最初の1行を見て、新旧どちらの型式で入力されているかを判定する。
            var autoMode = default(bool?);

            Dictionary<string, IndexData> indexTable = null;
            var branches = new List<(string Parent, string Daugher, double Branch)>();

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
                    autoMode = true;

                    if (indexTable is null)
                        indexTable = IndexDataReader.ReadNDX().ToDictionary(x => x.Nuclide, x => x);

                    foreach (var nuc in values)
                    {
                        if (!patternNuclide.IsMatch(nuc))
                            throw Program.Error($"Line {LineNum}: '{nuc}' is not nuclide name.");

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
                    throw Program.Error($"Line {LineNum}: Nuclide definition should have 3 values.");

                if (!double.TryParse(values[1], out var lambda))
                    throw Program.Error($"Line {LineNum}: Cannot get nuclide Lambda.");
                if (lambda < 0)
                    throw Program.Error($"Line {LineNum}: Nuclide Lambda should be positive.");

                if (!double.TryParse(values[2], out var decayRate))
                    throw Program.Error($"Line {LineNum}: Cannot get nuclide DecayRate.");
                if (decayRate < 0)
                    throw Program.Error($"Line {LineNum}: Nuclide DecayRate should be positive.");

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
                throw Program.Error($"Line {LineNum}: Duplicated [{nuc}:compartment] section.");

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
                    throw Program.Error($"Line {LineNum}: Compartment definition should have 3 values.");

                var organFn = values[0];        // コンパートメント機能
                var organName = values[1];      // コンパートメント名
                var sourceRegion = values[2];   // コンパートメントに対応する線源領域の名称

                var organFunc =
                    organFn == "inp" ? OrganFunc.inp :
                    organFn == "acc" ? OrganFunc.acc :
                    organFn == "mix" ? OrganFunc.mix :
                    organFn == "exc" ? OrganFunc.exc :
                    throw Program.Error($"Line {LineNum}: Unrecognized compartment function '{organFn}'.");

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
                throw Program.Error($"Line {LineNum}: Duplicated [{nuc}:transfer] section.");

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
                    throw Program.Error($"Line {LineNum}: Transfer path definition should have 3 values.");

                var orgamFrom = values[0];
                var organTo = values[1];
                var coeffStr = values[2];    // 移行係数、[/d] or [%]

                var coeff = default(decimal?);
                var isRate = false;
                if (!IsBar(coeffStr))
                    (coeff, isRate) = evaluator.ReadCoefficient(LineNum, coeffStr);

                transfers.Add((LineNum, orgamFrom, organTo, coeff, isRate));
            }

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
        }

        private NuclideData[] GetParents(NuclideData nuclide)
        {
            return nuclide.DecayRates.Select(b => nuclides.First(n => n.Name == b.Parent)).ToArray();
        }

        /// <summary>
        /// 対象核種から始まる崩壊系列を構成する核種を列挙する。
        /// </summary>
        /// <param name="root"></param>
        /// <returns>要素[0] == rootのリスト。</returns>
        private IReadOnlyList<NuclideData> GetDecayNuclides(NuclideData root)
        {
            var results = new List<NuclideData> { root };

        Lagain:
            var count = results.Count;
            foreach (var nuclide in nuclides)
            {
                // 系列を構成する核種の娘がnuclideである場合に、これを追加する。
                if (!results.Contains(nuclide) &&
                    GetParents(nuclide).Any(parent => results.Contains(parent)))
                {
                    results.Add(nuclide);
                }
            }
            // 系列を構成する核種が増えなくなるまで繰り返す。
            if (results.Count > count)
                goto Lagain;

            return results;
        }

        /// <summary>
        /// 対象核種から始まる崩壊系列の経路を列挙する。
        /// </summary>
        /// <param name="targets">崩壊系列を構成する核種のリスト。</param>
        /// <returns></returns>
        private IReadOnlyList<(NuclideData Parent, NuclideData Daughter)> GetDecayTransfers(IReadOnlyList<NuclideData> targets)
        {
            var results = new List<(NuclideData Parent, NuclideData Daughter)>();

            foreach (var parent in targets)
            {
                foreach (var daughter in targets)
                {
                    if (parent == daughter)
                        continue;
                    if (GetParents(daughter).Contains(parent))
                    {
                        results.Add((parent, daughter));
                    }
                }
            }

            return results;
        }

        /// <summary>
        /// 対象コンパートメントで生成された子孫核種を受けるコンパートメント群を追加定義する。
        /// </summary>
        /// <param name="data"></param>
        /// <param name="organFrom"></param>
        /// <returns></returns>
        private IReadOnlyList<Organ> DefineDecayCompartments(InputData data, Organ organFrom)
        {
            var fromNuclide = organFrom.Nuclide;

            // 崩壊系列を構成する核種と移行経路を取得する。
            var decayNuclides = GetDecayNuclides(fromNuclide);
            var decayTransfers = GetDecayTransfers(decayNuclides);

            var decayCompartments = new List<Organ>();

            // 崩壊系列を構成するコンパートメント群を追加定義する。
            foreach (var progeny in decayNuclides.Skip(1))
            {
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
                decayCompartments.Add(organDecay);

                var sourceRegion = organFrom.SourceRegion;
                if (sourceRegion != null)
                {
                    // 親核種のコンパートメントと同じ名前の線源領域を娘核種でも設定する。
                    organDecay.SourceRegion = sourceRegion;

                    // 核種に対応するS係数データを取得する。
                    var tableSCoeff = data.SCoeffTables[nuclides.IndexOf(progeny)];

                    // コンパートメントの放射能を各標的領域に振り分けるためのS係数データを関連付ける。
                    organDecay.S_Coefficients = tableSCoeff[sourceRegion];
                }
            }

            // 崩壊系列を構成するコンパートメント間の壊変経路を追加定義する。
            // 壊変経路は一本道ではなく、fromNuclideから始まる有効非巡回グラフ(DAG)を構成する点に注意。
            foreach (var path in decayTransfers)
            {
                var from = path.Parent == fromNuclide ? organFrom
                       : decayCompartments.First(o => o.Nuclide == path.Parent);
                var to = decayCompartments.First(o => o.Nuclide == path.Daughter);

                var parentNuclide = from.Nuclide.Name;
                var inflowRate = to.Nuclide.DecayRates.First(b => b.Parent == parentNuclide).Branch;

                to.Inflows.Add(new Inflow
                {
                    ID = from.ID,
                    Rate = inflowRate,

                    // 流入経路から流入元臓器の情報を直接引くための参照を設定する。
                    Organ = from,
                });
            }

            return decayCompartments;
        }

        /// <summary>
        /// 全ての移行経路を定義する。
        /// </summary>
        /// <param name="data"></param>
        private void DefineTransfers(InputData data)
        {
            var decayPaths = new List<(Organ from, Organ to, bool hasCoeff)>();
            var decayChains = new Dictionary<Organ, IReadOnlyList<Organ>>();

            foreach (var nuclide in nuclides)
            {
                if (!CalcProgeny && nuclide.IsProgeny)
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
                        throw Program.Error($"Line {lineNum}: Undefined compartment '{from}'.");
                    if (organTo is null)
                        throw Program.Error($"Line {lineNum}: Undefined compartment '{to}'.");

                    // 自分自身への移行経路は定義できない。
                    if (organTo == organFrom)
                        throw Program.Error($"Line {lineNum}: Cannot set transfer path to itself.");

                    // 同じ移行経路が複数回定義されていないことを確認する。
                    if (!definedTransfers.Add((from, to)))
                        throw Program.Error($"Line {lineNum}: Duplicated transfer path from '{from}' to '{to}'.");

                    // 正しくないコンパートメント機能間の移行経路が定義されていないことを確認する。
                    var fromFunc = organFrom.Func;
                    var toFunc = organTo.Func;
                    var isDecayPath = fromNuclide != toNuclide;
                    var hasCoeff = coeff != null;

                    // inpへの流入は定義できない。
                    if (toFunc == OrganFunc.inp)
                        throw Program.Error($"Line {lineNum}: Cannot set input path to inp '{to}'.");

                    if (fromFunc == OrganFunc.acc)
                    {
                        // accからの流出経路では、移行速度の入力を要求する。
                        // なお、ここでパーセント値を設定するのは明らかにおかしいので設定エラーとして弾く。
                        if (!isDecayPath && !hasCoeff || isRate)
                            throw Program.Error($"Line {lineNum}: Require transfer rate [/d] from {fromFunc} '{from}'.");
                    }

                    if (fromFunc == OrganFunc.exc)
                    {
                        // excからの流出は(娘核種のexcへの壊変経路を除いて)定義できない。
                        if (!(toFunc == OrganFunc.exc && isDecayPath))
                            throw Program.Error($"Line {lineNum}: Cannot set output path from exc '{from}'.");
                    }

                    // TODO: mixからmixへの経路は定義できない。
                    //if (fromFunc == OrganFunc.mix && toFunc == OrganFunc.mix)
                    //    throw Program.Error($"Line {lineNum}: Cannot set transfer path from 'mix' to 'mix'.");

                    if (organFrom.IsInstantOutflow)
                    {
                        // inpやmixから娘核種への壊変経路は定義できない。
                        if (isDecayPath)
                            throw Program.Error($"Line {lineNum}: Cannot set decay path from {fromFunc} '{from}'.");

                        // inpまたはmixからの同核種での移行経路では、移行割合の入力を要求する。
                        // なお、ここでは割合値(0.15など)とパーセント値(10.5%など)の両方を受け付ける。
                        else if (!hasCoeff)
                            throw Program.Error($"Line {lineNum}: Require fraction of output activity [%] from {fromFunc} '{from}'.");
                    }

                    if (isDecayPath)
                    {
                        // 分岐比が不明な壊変経路は定義できない。
                        if (!toNuclide.DecayRates.Any(b => b.Parent == fromNuclide.Name))
                            throw Program.Error($"Line {lineNum}: There is no decay path from {fromNuclide.Name} to {toNuclide.Name}.");

                        var paths = decayPaths.Where(path => path.from == organFrom);

                        // organFromから、同じ子孫核種への2つ以上の壊変経路は定義できない。
                        if (paths.Any(path => path.to.Nuclide == toNuclide))
                            throw Program.Error($"Line {lineNum}: Multiple decay paths from {fromFunc} '{from}' to nuclide '{toNuclide.Name}'.");

                        // organFromから、移行係数の設定有無が異なる壊変経路は同時に定義できない。
                        if (paths.Any(path => path.hasCoeff != hasCoeff))
                            throw Program.Error($"Line {lineNum}: Conflict decay paths from {fromFunc} '{from}' with inconsistent coefficient setting.");

                        decayPaths.Add((organFrom, organTo, hasCoeff));
                    }

                    if (coeff is decimal coeff_v)
                    {
                        // 移行係数が負の値でないことを確認する。
                        if (coeff_v < 0)
                            throw Program.Error($"Line {lineNum}: Transfer coefficient should be positive.");

                        if (isDecayPath)
                        {
                            // 移行係数を伴う次のような壊変経路を、
                            //   Parent/organFrom --(coeff)--> Progeny_i/organTo
                            //
                            // 親核種Parentの子孫核種Progeny_1～Nのそれぞれを受ける
                            // organDecayを追加することで、以下ような経路に構成し直す。
                            //   Parent/organFrom
                            //      ↓
                            //   Progeny_1/organDecay
                            //   ： ：
                            //   ↓ ↓
                            //   ↓ Progeny_i/organDecay --(coeff)--> Progeny_i/organTo
                            //   ↓ ：
                            //   ↓ ↓
                            //   Progeny_N/organDecay

                            // organFromから始まる壊変経路を構成するコンパートメント群を取得する。
                            if (!decayChains.TryGetValue(organFrom, out var decayCompartments))
                                decayCompartments = DefineDecayCompartments(data, organFrom);
                            decayChains[organFrom] = decayCompartments;

                            // 以降の処理をProgeny_iにおけるorganDecay -> organToの経路設定にすり替える。
                            var organDecay = decayCompartments.First(o => o.Nuclide == nuclide);
                            organFrom = organDecay;
                        }

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
    }
}
