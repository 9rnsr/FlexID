using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
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
    public class Organ
    {
        /// <summary>
        /// 臓器が対象とする核種。
        /// </summary>
        public NuclideData Nuclide;

        /// <summary>
        /// 崩壊定数[/day]。(＝ ln(2) / 半減期[day])
        /// </summary>
        public double NuclideDecay => Nuclide.Ramd;

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
        public List<Inflow> Inflows;

        /// <summary>
        /// 線源領域の名称。
        /// </summary>
        public string SourceRegion;

        /// <summary>
        /// コンパートメントに対応付けられた線源領域から各標的領域へのS係数。
        /// </summary>
        public double[] S_Coefficients;
    }

    public class NuclideData
    {
        /// <summary>
        /// 核種の一覧。
        /// </summary>
        public string Nuclide;

        /// <summary>
        /// 被ばく経路。
        /// </summary>
        public string IntakeRoute;

        /// <summary>
        /// 崩壊定数[/day]。(＝ ln(2) / 半減期[day])
        /// </summary>
        public double Ramd;

        /// <summary>
        /// 親核種からの崩壊割合(100%＝1.00と置いた比で持つ)。
        /// </summary>
        public double DecayRate;

        /// <summary>
        /// 子孫核種の場合は<c>true</c>。
        /// </summary>
        public bool IsProgeny;

        /// <summary>
        /// S係数データにおける各線源領域の名称。
        /// </summary>
        public string[] SourceRegions;
    }

    public class DataClass
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
        /// 組織加重係数データにおける各標的領域の名称。
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
    }

    /// <summary>
    /// インプットファイルの読み取り処理。
    /// </summary>
    public class DataReader : IDisposable
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
        /// <param name="calcProgeny">子孫核種を計算する＝読み込む場合は<c>true</c>。</param>
        public DataReader(string inputPath, bool calcProgeny)
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
        /// <param name="calcProgeny">子孫核種を計算する＝読み込む場合は<c>true</c>。</param>
        public DataReader(StreamReader reader, bool calcProgeny)
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
        public DataClass Read_OIR()
        {
            var firstLine = GetNextLine();
            if (firstLine is null)
                throw Program.Error("Reach to EOF while reading input file.");
            if (firstLine.StartsWith("["))
                return Read_OIR_New(firstLine);

            var data = new DataClass();
            {
                var isProgeny = false;
            Lcont:
                // 核種のヘッダ行を読み込む。
                var values = firstLine.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);

                var nuclide = new NuclideData
                {
                    Nuclide = values[0],
                    IntakeRoute = values[1],
                    Ramd = double.Parse(values[2]),
                    DecayRate = double.Parse(values[3]),
                    IsProgeny = isProgeny,
                };
                data.Nuclides.Add(nuclide);

                if (!isProgeny)
                {
                    // 組織加重係数データを読み込む。
                    var (ts, ws) = ReadTissueWeights(Path.Combine("lib", "OIR", "wT.txt"));
                    data.TargetRegions = ts;
                    data.TargetWeights = ws;
                }

                // 核種に対応するS係数データを読み込む。
                var tableSCoeff = ReadSCoeff(data, nuclide);
                data.SCoeffTables.Add(tableSCoeff);

                // 核種の体内動態モデル構成するコンパートメントの定義行を読み込む。
                while (true)
                {
                    var ln = GetNextLine();
                    if (ln is null)
                        throw Program.Error("Reach to EOF while reading input file.");
                    if (ln == "end")
                        break;

                    if (ln == "cont")
                    {
                        if (calcProgeny == false)
                            break;

                        isProgeny = true;
                        firstLine = GetNextLine();
                        if (firstLine is null)
                            throw Program.Error("Reach to EOF while reading input file.");
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

                    // 流入割合がマイナスの時の処理は親からの分岐比*親の崩壊定数とする。
                    if (inflow.Rate < 0)
                    {
                        var nucDecay = inflow.Organ.NuclideDecay;
                        inflow.Rate = organ.Nuclide.DecayRate * nucDecay;
                    }
                }
            }

            return data;
        }

        /// <summary>
        /// 新しい形式でOIR用のインプットファイルを読み込む。
        /// </summary>
        /// <param name="line"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        private DataClass Read_OIR_New(string line)
        {
            bool CheckSectionHeader(string ln) => ln.StartsWith("[");

            ReadOnlySpan<char> GetSectionHeader(string ln)
            {
                if (!CheckSectionHeader(ln))
                    throw new NotSupportedException("unreachable");
                if (!ln.EndsWith("]"))
                    throw Program.Error($"Line {lineNum}: Section header is not closed with ']'.");

                return ln.AsSpan(1, ln.Length - 2).Trim();
            }

            var inputTitle = default(string);
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

            // 核種の定義セクションを読み込む。
            void GetNuclides(out string nextLine)
            {
                if (nuclides != null)
                    throw Program.Error($"Line {lineNum}: Duplicated [nuclide] section.");

                nuclides = new List<NuclideData>();

                // 1番目が親核種、2番目以降が子孫核種になる。
                var isProgeny = false;

                while (true)
                {
                    nextLine = GetNextLine();
                    if (nextLine is null)
                        break;
                    if (CheckSectionHeader(nextLine))
                        break;

                    // 核種の定義行を読み込む。
                    var values = nextLine.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);

                    if (values.Length != 4)
                        throw Program.Error($"Line {lineNum}: Nuclide definition should have 4 values.");

                    if (!double.TryParse(values[2], out var ramd))
                        throw Program.Error($"Line {lineNum}: Cannot get nuclide Ramd.");
                    if (ramd < 0)
                        throw Program.Error($"Line {lineNum}: Nuclide Ramd should be positive.");

                    if (!double.TryParse(values[3], out var decayRate))
                        throw Program.Error($"Line {lineNum}: Cannot get nuclide DecayRate.");
                    if (decayRate < 0)
                        throw Program.Error($"Line {lineNum}: Nuclide DecayRate should be positive.");

                    var nuclide = new NuclideData
                    {
                        Nuclide = values[0],
                        IntakeRoute = values[1],
                        Ramd = ramd,
                        DecayRate = decayRate,
                        IsProgeny = isProgeny,
                    };
                    nuclides.Add(nuclide);

                    isProgeny = true;
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

                    var values = nextLine.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);

                    if (values.Length != 3)
                        throw Program.Error($"Line {lineNum}: Transfer path definition should have 3 values.");

                    var orgamFrom = values[0];
                    var organTo = values[1];
                    var coeffStr = values[2];    // 移行係数、[/d] or [%]

                    decimal? coeff = null;
                    var isRate = false;
                    if (!IsBar(coeffStr))
                    {
                        if (coeffStr.EndsWith("%"))
                        {
                            isRate = true;
                            coeffStr = coeffStr.Substring(0, coeffStr.Length - 1);
                        }
                        if (decimal.TryParse(coeffStr, NumberStyles.Float, null, out var v))
                            coeff = v;
                        else
                            throw Program.Error($"Line {lineNum}: Transfer coefficient should be a number or '---', not '{values[2]}'.");
                    }

                    transfers.Add((lineNum, orgamFrom, organTo, coeff, isRate));
                }
            }

            var data = new DataClass();

            if (inputTitle is null)
                throw Program.Error($"Missing [title] section.");
            data.Title = inputTitle;

            // 組織加重係数データを読み込む。
            var (ts, ws) = ReadTissueWeights(Path.Combine("lib", "OIR", "wT.txt"));
            data.TargetRegions = ts;
            data.TargetWeights = ws;

            if (nuclides is null)
                throw Program.Error($"Missing [nuclide] section.");
            if (!nuclides.Any())
                throw Program.Error($"None of nuclides defined.");

            // 全ての核種のコンパートメントを定義する。
            foreach (var nuclide in nuclides)
            {
                if (!calcProgeny && nuclide.IsProgeny)
                    continue;

                var nuc = nuclide.Nuclide;
                if (!nuclideOrgans.TryGetValue(nuc, out var organs))
                    throw Program.Error($"Missing [{nuc}:compartment] section.");
                if (!organs.Any())
                    throw Program.Error($"None of compartments defined for nuclide '{nuc}'.");

                if (!nuclideTransfers.TryGetValue(nuc, out var transfers))
                    throw Program.Error($"Missing [{nuc}:transfer] section.");
                if (!transfers.Any())
                    throw Program.Error($"None of transfers defined for nuclide '{nuc}'.");

                data.Nuclides.Add(nuclide);

                // 核種に対応するS係数データを読み込む。
                var tableSCoeff = ReadSCoeff(data, nuclide);
                data.SCoeffTables.Add(tableSCoeff);

                Organ input = null;

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
                        // コンパートメントに対応する線源領域がS係数データに存在することを確認する。
                        var indexS = Array.IndexOf(nuclide.SourceRegions, sourceRegion);
                        if (indexS == -1)
                            throw Program.Error($"Line {lineNum}: Unknown source region name '{sourceRegion}'.");

                        // コンパートメントの放射能を各標的領域に振り分けるためのS係数データを関連付ける。
                        organ.S_Coefficients = tableSCoeff[sourceRegion];
                    }
                    else
                    {
                        organ.SourceRegion = null;
                    }
                }

                if (!nuclide.IsProgeny && input is null)
                    throw Program.Error($"Missing 'inp' compartment.");
            }

            // コンパートメント間の移行経路を定義する。
            foreach (var nuclide in nuclides)
            {
                if (!calcProgeny && nuclide.IsProgeny)
                    continue;

                var nuc = nuclide.Nuclide;
                var transfers = nuclideTransfers[nuc];

                // 移行経路の定義が正しいことの確認と、
                // 各コンパートメントから流出する移行係数の総計を求める。
                var definedTransfers = new HashSet<(string from, string to)>();
                var transfersCorrect = new List<(Organ from, Organ to, decimal coeff, bool isRate)>();
                var sumOfOutflowCoeff = new Dictionary<Organ, decimal>();
                foreach (var (lineNum, from, to, coeff, isRate) in transfers)
                {
                    var fromName = from;
                    var fromNuclide = nuclide;
                    if (from.IndexOf('/') is int i && i != -1)
                    {
                        var fromNuc = from.Substring(0, i);
                        fromName = from.Substring(i + 1);
                        fromNuclide = nuclides.FirstOrDefault(n => n.Nuclide == fromNuc);
                        if (fromNuclide is null)
                            throw Program.Error($"Line {lineNum}: Undefined nuclide '{fromNuc}'.");
                    }

                    var toName = to;
                    var toNuclide = nuclide;
                    if (to.IndexOf('/') is int j && j != -1)
                    {
                        var toNuc = to.Substring(0, j);
                        toName = to.Substring(j + 1);
                        toNuclide = nuclides.FirstOrDefault(n => n.Nuclide == toNuc);
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
                    var isCoeff = coeff != null && !isRate;

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
                        // inpやmixから娘核種への壊変経路は定義できない。
                        if (fromFunc == OrganFunc.inp || fromFunc == OrganFunc.mix)
                            throw Program.Error($"Line {lineNum}: Cannot set decay path from {fromFunc} '{fromName}'.");

                        // 親核種からの壊変経路では、係数は指定できない。
                        if (coeff != null)
                            throw Program.Error($"Line {lineNum}: Cannot set transfer coefficient on decay path.");
                    }
                    else
                    {
                        // inpまたはmixからの配分経路では、[%]入力を要求する。
                        if ((fromFunc == OrganFunc.inp || fromFunc == OrganFunc.mix) && !isRate)
                            throw Program.Error($"Line {lineNum}: Require fraction of output activity [%] from {fromFunc} '{fromName}'.");

                        // accからの流出経路では、[/d]入力を要求する。
                        if (fromFunc == OrganFunc.acc && !isCoeff)
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

                    transfersCorrect.Add((organFrom, organTo, coeff ?? 0, isRate));
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
                    var isRate = outflows.First().isRate;

                    // inp,mixからは移行割合[%]で、accからは移行速度[/d]で流出することを確認済み。
                    Debug.Assert(outflows.All(t => t.isRate == isRate));

                    if (isRate)
                    {
                        // 流出放射能に対する移行割合[%]の合計が100%かどうかを確認する。
                        var sum = sumOfOutflowCoeff[organFrom];
                        if (sum != 100)
                            throw Program.Error($"Total [%] of transfer paths from '{fromName}' is  not 100%, but {sum:G29}%.");
                    }
                    else
                    {
                        // fromにおける生物学的崩壊定数[/day]を設定する。
                        if (organFrom.Func == OrganFunc.acc)
                            organFrom.BioDecay = (double)sumOfOutflowCoeff[organFrom];
                    }
                }

                // 各コンパートメントへの流入経路と、移行割合を設定する。
                foreach (var (organFrom, organTo, coeff, isRate) in transfersCorrect)
                {
                    double inflowRate;
                    if (organFrom.Nuclide != nuclide)
                    {
                        // 親から子への移行経路では、親からの分岐比*親の崩壊定数とする。
                        inflowRate = organTo.Nuclide.DecayRate * organFrom.NuclideDecay;
                    }
                    else if (isRate)
                    {
                        // fromからtoへの移行割合 = 移行係数[%] / 100
                        inflowRate = (double)(coeff / 100);
                    }
                    else
                    {
                        var sum = sumOfOutflowCoeff[organFrom];

                        // fromからtoへの移行割合 = 移行係数[/d] / fromから流出する移行係数[/d]の総計
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

                // TODO; inpを除く、流入がないコンパートメントがある場合はこれをエラーにする。
                // TODO; excを除く、流出がないコンパートメントがある場合はこれをエラーにする。
            }

            return data;
        }

        /// <summary>
        /// S係数データを読み込む。
        /// </summary>
        /// <param name="nuclide">対象核種。線源領域の名称が設定される。</param>
        /// <returns>キーが線源領域の名称、値が各標的領域に対する成人男女平均のS係数、となる辞書。</returns>
        private static Dictionary<string, double[]> ReadSCoeff(DataClass data, NuclideData nuclide)
        {
            var nuc = nuclide.Nuclide;
            var prg = nuclide.IsProgeny ? "prg_" : "";

            var fileAM = $"{nuc}_AM_{prg}S-Coefficient.txt";
            var fileAF = $"{nuc}_AF_{prg}S-Coefficient.txt";
            using (var readerAM = new StreamReader(Path.Combine("lib", "OIR", fileAM)))
            using (var readerAF = new StreamReader(Path.Combine("lib", "OIR", fileAF)))
            {
                // 1行目から線源領域の名称を配列で取得。
                var sourcesAM = readerAM.ReadLine()?.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries).Skip(1).ToArray();
                var sourcesAF = readerAF.ReadLine()?.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries).Skip(1).ToArray();
                if (sourcesAM is null) throw Program.Error($"Incorrect S-Coefficient file format: {fileAM}");
                if (sourcesAF is null) throw Program.Error($"Incorrect S-Coefficient file format: {fileAF}");

                if (!Enumerable.SequenceEqual(sourcesAM, sourcesAF))
                    throw Program.Error($"Found mismatch of source region names in S-Coefficient data for nuclide {nuc}.");
                var sources = sourcesAM;
                var sourcesCount = sources.Length;

                var targets = new string[43];

                var table = sources.ToDictionary(s => s, s => new double[43]);

                for (int indexT = 0; indexT < 43; indexT++)
                {
                    var valuesAM = readerAM.ReadLine()?.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);
                    var valuesAF = readerAF.ReadLine()?.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);
                    if (valuesAM?.Length != 1 + sourcesCount) throw Program.Error($"Incorrect S-Coefficient file format: {fileAM}");
                    if (valuesAF?.Length != 1 + sourcesCount) throw Program.Error($"Incorrect S-Coefficient file format: {fileAF}");

                    // 各行の1列目から標的領域の名称を取得。
                    var targetAM = valuesAM[0];
                    var targetAF = valuesAF[0];
                    if (targetAM != targetAF)
                        throw Program.Error($"Found mismatch of target region names in S-Coefficient data for nuclide {nuc}.");
                    targets[indexT] = targetAM;

                    for (int indexS = 0; indexS < sourcesCount; indexS++)
                    {
                        var sourceRegion = sources[indexS];

                        // ここでS係数の男女平均を取る。
                        var scoeffAM = double.Parse(valuesAM[1 + indexS]);
                        var scoeffAF = double.Parse(valuesAF[1 + indexS]);
                        var scoeff = (scoeffAM + scoeffAF) / 2;

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
        /// EIR用のインプットファイルを読み込む。
        /// </summary>
        /// <returns></returns>
        public List<DataClass> Read_EIR()
        {
            var dataList = new List<DataClass>();
            dataList.Add(Read_EIR("Age:3month"));
            dataList.Add(Read_EIR("Age:1year"));
            dataList.Add(Read_EIR("Age:5year"));
            dataList.Add(Read_EIR("Age:10year"));
            dataList.Add(Read_EIR("Age:15year"));
            dataList.Add(Read_EIR("Age:adult"));
            return dataList;
        }

        public DataClass Read_EIR(string age)
        {
            // 読み取り位置をファイル先頭に戻す。
            reader.BaseStream.Position = 0;
            reader.DiscardBufferedData();
            lineNum = 0;

            var firstLine = GetNextLine();
            if (firstLine is null)
                throw Program.Error("Reach to EOF while reading input file.");

            var data = new DataClass();

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
                // 核種のヘッダ行を読み込む。
                var values = firstLine.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);

                var nuclide = new NuclideData
                {
                    Nuclide = values[0],
                    IntakeRoute = values[1],
                    Ramd = double.Parse(values[2]),
                    DecayRate = double.Parse(values[3]),
                    IsProgeny = isProgeny,
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
                        firstLine = GetNextLine();
                        if (firstLine is null)
                            throw Program.Error("Reach to EOF while reading input file.");
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

                    // 流入割合がマイナスの時の処理は親からの分岐比*親の崩壊定数とする。
                    if (inflow.Rate < 0)
                    {
                        var nucDecay = inflow.Organ.NuclideDecay;
                        inflow.Rate = organ.Nuclide.DecayRate * nucDecay;
                    }
                }
            }

            return data;
        }

        /// <summary>
        /// SEEデータを読み込む。
        /// </summary>
        /// <param name="age">被ばく評価期間の開始年齢</param>
        /// <param name="nuclide">対象核種。線源領域の名称が設定される。</param>
        /// <returns>キーが線源領域の名称、値が各標的領域に対する成人男女平均のS係数、となる辞書。</returns>
        private static Dictionary<string, double[]> ReadSee(DataClass data, string age, NuclideData nuclide)
        {
            var nuc = nuclide.Nuclide;
            var file = $"{nuc}SEE.txt";

            using (var reader = new StreamReader(Path.Combine("lib", "EIR", file)))
            {
                while (reader.ReadLine() != age)
                { }

                // 2行目から線源領域の名称を配列で取得。
                var sources = reader.ReadLine()?.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries).Skip(1).ToArray();
                if (sources is null)
                    throw Program.Error($"Incorrect S-Coefficient file format: {file}");
                if (nuclide.SourceRegions != null && !Enumerable.SequenceEqual(nuclide.SourceRegions, sources))
                    throw Program.Error($"Incorrect S-Coefficient file format: {file}");
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

        private static readonly Regex patternBar = new Regex("^-+$", RegexOptions.Compiled);

        bool IsBar(string s) => patternBar.IsMatch(s);
    }
}
