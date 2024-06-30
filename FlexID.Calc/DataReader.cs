using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

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
        /// </summary>
        public double BioDecay;

        /// <summary>
        /// 生物学的崩壊定数[/day](計算用)。
        /// </summary>
        public double BioDecayCalc;

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

        private DataClass() { }

        /// <summary>
        /// OIR用のインプットファイルを読み込む。
        /// </summary>
        /// <param name="inputPath">インプットファイルのパス文字列。</param>
        /// <param name="calcProgeny">子孫核種を計算する＝読み込む場合は<c>true</c>。</param>
        /// <returns></returns>
        public static DataClass Read(string inputPath, bool calcProgeny)
        {
            var lines = File.ReadLines(inputPath).ToList();

            var data = new DataClass();
            {
                int num = 0;

                string GetNextLine()
                {
                Lagain:
                    if (num == lines.Count)
                        throw Program.Error("Reach to EOF while reading input file.");
                    var ln = lines[num++].Trim();

                    // 空行を読み飛ばす。
                    if (ln.Length == 0)
                        goto Lagain;

                    // コメント行を読み飛ばす。
                    if (ln.StartsWith("#"))
                        goto Lagain;

                    // 行末コメントを除去する。
                    var trailingComment = ln.IndexOf("#");
                    if (trailingComment != -1)
                        ln = ln.Substring(0, trailingComment).TrimEnd();
                    return ln;
                }

                var isProgeny = false;
            Lcont:
                // 核種のヘッダ行を読み込む。
                var values = GetNextLine().Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);

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
                    if (ln == "end")
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
                        throw Program.Error($"Line {num}: First line of compartment definition should have 8 values.");

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
                        throw Program.Error($"Line {num}: Unrecognized organ function '{organFn}'.");

                    var organ = new Organ
                    {
                        Nuclide = nuclide,
                        ID = organId,
                        Index = data.Organs.Count,
                        Name = organName,
                        Func = organFunc,
                        BioDecay = bioDecay,
                        BioDecayCalc = bioDecay,
                        Inflows = new List<Inflow>(inflowNum),
                    };

                    if (sourceRegion != "-")
                    {
                        // コンパートメントに対応する線源領域がS係数データに存在することを確認する。
                        var indexS = Array.IndexOf(nuclide.SourceRegions, sourceRegion);
                        if (indexS == -1)
                            throw Program.Error($"Line {num}: Unknown source region name: '{sourceRegion}'");

                        // コンパートメントの放射能を各標的領域に振り分けるためのS係数データを関連付ける。
                        organ.SourceRegion = sourceRegion;
                        organ.S_Coefficients = tableSCoeff[sourceRegion];
                    }

                    // コンパートメントへの流入経路の記述を読み込む。
                    if (organ.Func == OrganFunc.inp)
                    {
                        if (inflowNum != 0)
                            throw Program.Error($"Line {num}: The number of inflow paths in the Input compartment should be 0.");
                    }
                    else
                    {
                        if (inflowNum <= 0)
                            throw Program.Error($"Line {num}: The number of inflow paths should be >= 1.");

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
                                values = GetNextLine().Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);
                                if (values.Length != 2)
                                    throw Program.Error($"Line {num}: Continuous lines of compartment definition should have 2 values.");

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
        /// <param name="inputPath">インプットファイルのパス文字列。</param>
        /// <param name="calcProgeny">子孫核種を計算する＝読み込む場合は<c>true</c>。</param>
        /// <returns></returns>
        public static List<DataClass> Read_EIR(string inputPath, bool calcProgeny)
        {
            var lines = File.ReadLines(inputPath).ToList();

            var dataList = new List<DataClass>();
            dataList.Add(Read_EIR(lines, calcProgeny, "Age:3month"));
            dataList.Add(Read_EIR(lines, calcProgeny, "Age:1year"));
            dataList.Add(Read_EIR(lines, calcProgeny, "Age:5year"));
            dataList.Add(Read_EIR(lines, calcProgeny, "Age:10year"));
            dataList.Add(Read_EIR(lines, calcProgeny, "Age:15year"));
            dataList.Add(Read_EIR(lines, calcProgeny, "Age:adult"));
            return dataList;
        }

        public static DataClass Read_EIR(List<string> lines, bool calcProgeny, string age)
        {
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
                int num = 0;

                string GetNextLine()
                {
                Lagain:
                    if (num == lines.Count)
                        throw Program.Error("Reach to EOF while reading input file.");
                    var ln = lines[num++].Trim();

                    // 空行を読み飛ばす。
                    if (ln.Length == 0)
                        goto Lagain;

                    // コメント行を読み飛ばす。
                    if (ln.StartsWith("#"))
                        goto Lagain;

                    // 行末コメントを除去する。
                    var trailingComment = ln.IndexOf("#");
                    if (trailingComment != -1)
                        ln = ln.Substring(0, trailingComment).TrimEnd();
                    return ln;
                }

                var isProgeny = false;
            Lcont:
                // 核種のヘッダ行を読み込む。
                var values = GetNextLine().Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);

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
                    while (GetNextLine() != age)
                    { }
                }

                // 核種に対応するS係数データを読み込む。
                var tableSCoeff = ReadSee(data, age, nuclide);
                data.SCoeffTables.Add(tableSCoeff);

                // 核種の体内動態モデル構成するコンパートメントの定義行を読み込む。
                while (true)
                {
                    var ln = GetNextLine();
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
                        throw Program.Error($"Line {num}: First line of compartment definition should have 8 values.");

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
                        throw Program.Error($"Line {num}: Unrecognized organ function '{organFn}'.");

                    var organ = new Organ
                    {
                        Nuclide = nuclide,
                        ID = organId,
                        Index = data.Organs.Count,
                        Name = organName,
                        Func = organFunc,
                        BioDecay = bioDecay,
                        BioDecayCalc = bioDecay,
                        Inflows = new List<Inflow>(inflowNum),
                    };

                    if (sourceRegion != "-")
                    {
                        // コンパートメントに対応する線源領域がS係数データに存在することを確認する。
                        var indexS = Array.IndexOf(nuclide.SourceRegions, sourceRegion);
                        if (indexS == -1)
                            throw Program.Error($"Line {num}: Unknown source region name: '{sourceRegion}'");

                        // コンパートメントの放射能を各標的領域に振り分けるためのS係数データを関連付ける。
                        organ.SourceRegion = sourceRegion;
                        organ.S_Coefficients = tableSCoeff[sourceRegion];
                    }

                    // コンパートメントへの流入経路の記述を読み込む。
                    if (organ.Func == OrganFunc.inp)
                    {
                        if (inflowNum != 0)
                            throw Program.Error($"Line {num}: The number of inflow paths in the Input compartment should be 0.");
                    }
                    else
                    {
                        if (inflowNum <= 0)
                            throw Program.Error($"Line {num}: The number of inflow paths should be >= 1.");

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
                                values = GetNextLine().Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);
                                if (values.Length != 2)
                                    throw Program.Error($"Line {num}: Continuous lines of compartment definition should have 2 values.");

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
    }
}
