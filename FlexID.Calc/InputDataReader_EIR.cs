namespace FlexID.Calc;

/// <summary>
/// EIR用インプットファイルの読み取り処理。
/// </summary>
public class InputDataReader_EIR : InputDataReaderBase
{
    /// <summary>
    /// コンストラクタ。
    /// </summary>
    /// <param name="inputPath">インプットファイルのパス文字列。</param>
    /// <param name="calcProgeny">子孫核種を計算する＝読み込む場合は <see langword="true"/>。</param>
    public InputDataReader_EIR(string inputPath, bool calcProgeny = true)
        : base(new StreamReader(new FileStream(inputPath, FileMode.Open, FileAccess.Read, FileShare.Read)), calcProgeny)
    {
    }

    /// <summary>
    /// コンストラクタ。
    /// </summary>
    /// <param name="reader">インプットの読み込み元。</param>
    /// <param name="calcProgeny">子孫核種を計算する＝読み込む場合は <see langword="true"/>。</param>
    public InputDataReader_EIR(StreamReader reader, bool calcProgeny = true)
        : base(reader, calcProgeny)
    {
    }

    /// <summary>
    /// インプットファイルを読み込む。
    /// </summary>
    /// <returns></returns>
    public List<InputData> Read()
    {
        var dataList = new List<InputData>();
        dataList.Add(Read("Age:3month"));
        dataList.Add(Read("Age:1year"));
        dataList.Add(Read("Age:5year"));
        dataList.Add(Read("Age:10year"));
        dataList.Add(Read("Age:15year"));
        dataList.Add(Read("Age:adult"));
        return dataList;
    }

    private InputData Read(string age)
    {
        // 読み取り位置をファイル先頭に戻す。
        ResetPosition();

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

        var branchTable = new List<double>();

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
                IsProgeny = isProgeny,
            };
            data.Nuclides.Add(nuclide);

            branchTable.Add(double.Parse(values[2]));

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
            data.SCoeffTablesM.Add(tableSCoeff);

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
                    if (CalcProgeny == false)
                        break;

                    isProgeny = true;
                    goto Lcont;
                }

                values = ln.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);

                if (values.Length != 8)
                    throw Program.Error($"Line {LineNum}: First line of compartment definition should have 8 values.");

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
                    throw Program.Error($"Line {LineNum}: Unrecognized organ function '{organFn}'.");

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
                        throw Program.Error($"Line {LineNum}: Unknown source region name: '{sourceRegion}'");

                    // コンパートメントの放射能を各標的領域に振り分けるためのS係数データを関連付ける。
                    organ.SourceRegion = sourceRegion;
                    organ.S_CoefficientsM = tableSCoeff[sourceRegion];
                }

                if (organ.Func == OrganFunc.exc)
                {
                    if (organ.Name == "Urine" || organ.Name == "Faeces")
                    {
                        organ.IsExcretaCompatibleWithOIR = true;
                    }
                }

                // コンパートメントへの流入経路の記述を読み込む。
                if (organ.Func == OrganFunc.inp)
                {
                    if (inflowNum != 0)
                        throw Program.Error($"Line {LineNum}: The number of inflow paths in the Input compartment should be 0.");
                }
                else
                {
                    if (inflowNum <= 0)
                        throw Program.Error($"Line {LineNum}: The number of inflow paths should be >= 1.");

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
                                throw Program.Error($"Line {LineNum}: Continuous lines of compartment definition should have 2 values.");

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

        // 娘核種への分岐比を設定する。
        for (int i = 0; i < data.Nuclides.Count; i++)
        {
            var nuclide = data.Nuclides[i];
            if (i + 1 < data.Nuclides.Count)
            {
                var fraction = branchTable[i + 1];
                var daughter = data.Nuclides[i + 1];
                nuclide.Branches = new[] { (daughter, fraction) };
            }
            else
            {
                nuclide.Branches = Array.Empty<(NuclideData, double)>();
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
                    var fromNuclide = inflow.Organ.Nuclide;
                    var toNuclide = organ.Nuclide;
                    var branch = fromNuclide.Branches.FirstOrDefault(b => b.Daughter == toNuclide);
                    if (branch == default)
                        throw Program.Error($": There is no decay path from {fromNuclide.Name} to {toNuclide.Name}.");
                    inflow.Rate = branch.Fraction;
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
    /// <returns>キーが線源領域の名称、値が各標的領域に対するS係数、となる辞書。</returns>
    private static Dictionary<string, double[]> ReadSee(InputData data, string age, NuclideData nuclide)
    {
        var nuc = nuclide.Name;
        var file = $"{nuc}.txt";

        using var reader = new StreamReader(Path.Combine("lib", "EIR", "SEE", file));

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
