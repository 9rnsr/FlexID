using System.Text;
using System.Text.RegularExpressions;
using Sprache;

namespace FlexID;

/// <summary>
/// レポート出力の内容。
/// </summary>
public class ReportData
{
    public ReportData(FileInfo? outputDir, string outputPath, (string Name, string Path)? compare)
    {
        OutputName = Path.GetFileNameWithoutExtension(outputPath);
        OutputNuclide = ""; // 実際に *_Retention.out から読み込むまで不明なので、空文字で初期化
        OutputDosePath = $"{outputPath}_Dose.out";
        OutputRetentionPath = $"{outputPath}_Retention.out";

        if (compare is var (name, path))
        {
            HasExpect = true;
            ExpectName = name;
            ExpectNuclide = name.Split('_')[0];
            ExpectDosePath = Path.Combine(Path.GetDirectoryName(path)!, ExpectNuclide + ".dat");
            ExpectRetentionPath = path;
        }

        ReportPath = (outputDir is null ? outputPath : Path.Combine(outputDir.FullName, OutputName)) + ".xlsx";
    }

    public string OutputName { get; }
    public string OutputNuclide { get; set; }
    public string OutputDosePath { get; }
    public string OutputRetentionPath { get; }

    public bool HasExpect { get; }
    public string? ExpectName { get; }
    public string? ExpectNuclide { get; }
    public string? ExpectDosePath { get; }
    public string? ExpectRetentionPath { get; }

    public string ReportPath { get; }

    public (int AtomicNumber, int MassNumber, string MetaStable) SortKey
    {
        get
        {
            if (ElementTable.TryParseNuclide(OutputNuclide, out var elem, out var massNum, out var meta))
                return (ElementTable.ElementToAtomicNumber(elem), massNum, meta);
            else
                return default;
        }
    }

    /// <summary>
    /// レポート出力処理中に発生したエラーを保持する。
    /// </summary>
    public List<string> Errors { get; } = [];

    public bool HasErrors => Errors.Count != 0;

    // for expect retention file
    public string? RouteOfIntake;
    public string? ChemicalForm;
    public string? ParticleSize;
    public string? NuclideWholeBody;
    public string? NuclideUrine;
    public string? NuclideFaeces;
    public string? NuclideAtract;
    public string? NuclideLungs;
    public string? NuclideSkeleton;
    public string? NuclideLiver;
    public string? NuclideThyroid;

    public Dose? OutputDose;
    public Dose? ExpectDose;

    public IReadOnlyList<Retention>? OutputActs;
    public IReadOnlyList<Retention>? ExpectActs;

    public (double Min, double Max) OutputMinMaxWholeBody /**/= (double.PositiveInfinity, double.NegativeInfinity);
    public (double Min, double Max) OutputMinMaxUrine     /**/= (double.PositiveInfinity, double.NegativeInfinity);
    public (double Min, double Max) OutputMinMaxFaeces    /**/= (double.PositiveInfinity, double.NegativeInfinity);
    public (double Min, double Max) OutputMinMaxAtract    /**/= (double.PositiveInfinity, double.NegativeInfinity);
    public (double Min, double Max) OutputMinMaxLungs     /**/= (double.PositiveInfinity, double.NegativeInfinity);
    public (double Min, double Max) OutputMinMaxSkeleton  /**/= (double.PositiveInfinity, double.NegativeInfinity);
    public (double Min, double Max) OutputMinMaxLiver     /**/= (double.PositiveInfinity, double.NegativeInfinity);
    public (double Min, double Max) OutputMinMaxThyroid   /**/= (double.PositiveInfinity, double.NegativeInfinity);

    // fractions of OutputActs / ExpectActs
    public (double Min, double Max) FractionsWholeBody /**/= (double.PositiveInfinity, double.NegativeInfinity);
    public (double Min, double Max) FractionsUrine     /**/= (double.PositiveInfinity, double.NegativeInfinity);
    public (double Min, double Max) FractionsFaeces    /**/= (double.PositiveInfinity, double.NegativeInfinity);
    public (double Min, double Max) FractionsAtract    /**/= (double.PositiveInfinity, double.NegativeInfinity);
    public (double Min, double Max) FractionsLungs     /**/= (double.PositiveInfinity, double.NegativeInfinity);
    public (double Min, double Max) FractionsSkeleton  /**/= (double.PositiveInfinity, double.NegativeInfinity);
    public (double Min, double Max) FractionsLiver     /**/= (double.PositiveInfinity, double.NegativeInfinity);
    public (double Min, double Max) FractionsThyroid   /**/= (double.PositiveInfinity, double.NegativeInfinity);

    /// <summary>
    /// 線量の取得結果。
    /// </summary>
    public class Dose
    {
        public required double EffectiveDose { get; init; }
        public required double[] EquivalentDosesMale { get; init; }
        public required double[] EquivalentDosesFemale { get; init; }
    }

    /// <summary>
    /// 残留放射能の取得結果。
    /// </summary>
    public struct Retention
    {
        public double StartTime;
        public double EndTime;

        public double? WholeBody;
        public double? Urine;
        public double? Faeces;
        public double? Atract;
        public double? Lungs;
        public double? Skeleton;
        public double? Liver;
        public double? Thyroid;
    }

    /// <summary>
    /// 計算結果と比較対象の結果を取得する。
    /// </summary>
    /// <returns></returns>
    public void LoadResult()
    {
        // 残留放射能について期待値データは子孫核種の数値を提示していることがあり、その場合は
        // 結果値データからも対応する箇所を読み込む必要があるため、ここでは
        // 期待値→結果値の順番で読み込みを行う。
        // また、ExpectRetentionの内容から取得できる情報がExpectDoseの読み込みに必要なため、
        // 残留放射能→線量の順番で読み込みを行う。
        var expectRetentionPath = ExpectRetentionPath;
        var outputRetentionPath = OutputRetentionPath;
        var expectDosePath = ExpectDosePath;
        var outputDosePath = OutputDosePath;

        if (expectRetentionPath is not null)
        {
            if (!File.Exists(expectRetentionPath))
            {
                Errors.Add($"Expect retention file not found: {expectRetentionPath}");
            }
            else
            {
                try { ExpectActs = GetExpectRetentions(this); }
                catch { Errors.Add($"Failed to read expect retention file: {expectRetentionPath}"); }
            }
        }
        if (outputRetentionPath is not null)
        {
            if (!File.Exists(outputRetentionPath))
            {
                Errors.Add($"Result retention file not found: {outputRetentionPath}");
            }
            else
            {
                try { OutputActs = GetOutputRetentions(this); }
                catch { Errors.Add($"Failed to read output retention file: {outputRetentionPath}"); }
            }
        }
        if (expectDosePath is not null && ExpectActs is not null)
        {
            if (!File.Exists(expectDosePath))
            {
                Errors.Add($"Expect dose file not found: {expectDosePath}");
            }
            else
            {
                try { ExpectDose = GetExpectDoses(this); }
                catch { Errors.Add($"Failed to read expect dose file: {expectDosePath}"); }
            }
        }
        if (outputDosePath is not null)
        {
            if (!File.Exists(outputDosePath))
            {
                Errors.Add($"Result dose file not found: {outputDosePath}");
            }
            else
            {
                try { OutputDose = GetOutputDoses(this); }
                catch { Errors.Add($"Failed to read output dose file: {outputDosePath}"); }
            }
        }

        if (OutputActs is not null)
        {
            static void UpdateMinMax(ref (double Min, double Max) minmax, double? output)
            {
                if (output is double outputVal)
                {
                    minmax.Min = Math.Min(minmax.Min, outputVal);
                    minmax.Max = Math.Max(minmax.Max, outputVal);
                }
            }

            foreach (var outputAct in OutputActs)
            {
                UpdateMinMax(ref OutputMinMaxWholeBody, /**/outputAct.WholeBody);
                UpdateMinMax(ref OutputMinMaxUrine,     /**/outputAct.Urine);
                UpdateMinMax(ref OutputMinMaxFaeces,    /**/outputAct.Faeces);
                UpdateMinMax(ref OutputMinMaxAtract,    /**/outputAct.Atract);
                UpdateMinMax(ref OutputMinMaxLungs,     /**/outputAct.Lungs);
                UpdateMinMax(ref OutputMinMaxSkeleton,  /**/outputAct.Skeleton);
                UpdateMinMax(ref OutputMinMaxLiver,     /**/outputAct.Liver);
                UpdateMinMax(ref OutputMinMaxThyroid,   /**/outputAct.Thyroid);
            }
        }

        // 50年の預託期間における、各出力時間メッシュにおける数値の比較。
        // 要約として、期待値に対する下振れ率と上振れ率の最大値を算出する。
        if (OutputActs is not null && ExpectActs is not null)
        {
            static void UpdateMinMax(ref (double Min, double Max) fractions, double? output, double? expect)
            {
                if (output is double outputVal && expect is double expectVal)
                {
                    var frac = outputVal / expectVal;
                    fractions.Min = Math.Min(fractions.Min, frac);
                    fractions.Max = Math.Max(fractions.Max, frac);
                }
            }

            var activities = OutputActs.Zip(ExpectActs, (a, e) => (a, e));
            foreach (var (outputAct, expectAct) in activities)
            {
                UpdateMinMax(ref FractionsWholeBody, /**/outputAct.WholeBody, /**/expectAct.WholeBody);
                UpdateMinMax(ref FractionsUrine,     /**/outputAct.Urine,     /**/expectAct.Urine);
                UpdateMinMax(ref FractionsFaeces,    /**/outputAct.Faeces,    /**/expectAct.Faeces);
                UpdateMinMax(ref FractionsAtract,    /**/outputAct.Atract,    /**/expectAct.Atract);
                UpdateMinMax(ref FractionsLungs,     /**/outputAct.Lungs,     /**/expectAct.Lungs);
                UpdateMinMax(ref FractionsSkeleton,  /**/outputAct.Skeleton,  /**/expectAct.Skeleton);
                UpdateMinMax(ref FractionsLiver,     /**/outputAct.Liver,     /**/expectAct.Liver);
                UpdateMinMax(ref FractionsThyroid,   /**/outputAct.Thyroid,   /**/expectAct.Thyroid);
            }
        }
    }

    /// <summary>
    /// OIR Data Viewerの「Dose per Content & Reference Bioassay Functions」から取得した、
    /// 核種・摂取形態・化学形態・及び粒子径に対応する残留放射能データを保存したファイルから
    /// 全身の残留放射能の数値を取得する。
    /// </summary>
    /// <param name="report"></param>
    /// <returns>時間メッシュ毎の残留放射能データ。</returns>
    /// <exception cref="InvalidDataException"></exception>
    private static List<Retention> GetExpectRetentions(ReportData report)
    {
        var filePath = report.ExpectRetentionPath
            ?? throw new ArgumentNullException("target.ExpectRetentionPath");

        var nuclide = report.ExpectNuclide;

        using var reader = new StreamReader(File.OpenRead(filePath), Encoding.UTF8);
        string ReadLine() => reader.ReadLine() ?? throw new InvalidDataException("Unexpected end of file.");

        string[] columns;

        // ファイルの内容が対象核種のものか確認する。
        columns = ReadLine().Split('\t');
        if (columns[1] != nuclide)
            throw new InvalidDataException();

        // 対象の摂取形態。
        report.RouteOfIntake = ReadLine().Split('\t')[1];

        // 対象の化学形態。
        report.ChemicalForm = ReadLine().Split('\t')[1];

        // 対象の粒子サイズ。
        report.ParticleSize = ReadLine().Split('\t')[1];

        ReadLine();  // (empty line)

        // Bq/Bqで出力された数値データかどうか確認する。
        if (ReadLine() != "Content in an Organ or Excreta Sample per Intake (Reference Bioassay Functions m(t)), Bq per Bq")
            throw new InvalidDataException();

        columns = ReadLine().Split('\t');  // (table header)
        var indexWholeBody /**/= columns.IndexOf(s => s.Contains("Whole Body"));
        var indexUrine     /**/= columns.IndexOf(s => s.Contains("Urine"));
        var indexFaeces    /**/= columns.IndexOf(s => s.Contains("Faeces"));
        var indexAtract    /**/= columns.IndexOf(s => s.Contains("Alimentary Tract"));
        var indexLungs     /**/= columns.IndexOf(s => s.Contains("Lungs"));
        var indexSkeleton  /**/= columns.IndexOf(s => s.Contains("Skeleton"));
        var indexLiver     /**/= columns.IndexOf(s => s.Contains("Liver"));
        var indexThyroid   /**/= columns.IndexOf(s => s.Contains("Thyroid"));

        // 残留放射能データが子孫核種のものである場合、その名前が
        // ヘッダに括弧書きされているためこれを取り出す。
        string? GetRetentionNuclide(int index, string header)
        {
            if (index == -1)
                return null;
            var m = Regex.Match(columns[index], Regex.Escape(header) + @" *\((?<nuc>[^ ]+)\)");
            return m.Success ? m.Groups["nuc"].Value : nuclide;
        }
        report.NuclideWholeBody /**/= GetRetentionNuclide(indexWholeBody, /**/"Whole Body");
        report.NuclideUrine     /**/= GetRetentionNuclide(indexUrine,     /**/"Urine (24-hour sample)");
        report.NuclideFaeces    /**/= GetRetentionNuclide(indexFaeces,    /**/"Faeces (24-hour sample)");
        report.NuclideAtract    /**/= GetRetentionNuclide(indexAtract,    /**/"Alimentary Tract*");
        report.NuclideLungs     /**/= GetRetentionNuclide(indexLungs,     /**/"Lungs*");
        report.NuclideSkeleton  /**/= GetRetentionNuclide(indexSkeleton,  /**/"Skeleton*");
        report.NuclideLiver     /**/= GetRetentionNuclide(indexLiver,     /**/"Liver*");
        report.NuclideThyroid   /**/= GetRetentionNuclide(indexThyroid,   /**/"Thyroid*");

        var retentions = new List<Retention>();
        var startTime = 0.0;

        string? line;
        while ((line = reader.ReadLine()) != null)
        {
            columns = line.Split('\t');

            var endTime   /**/= double.Parse(columns[0]);

            double? GetValue(int index) =>
                index == -1 || columns[index] == "-" ? default(double?) : double.Parse(columns[index]);

            retentions.Add(new Retention
            {
                StartTime /**/= startTime,
                EndTime   /**/= endTime,
                WholeBody /**/= GetValue(indexWholeBody),
                Urine     /**/= GetValue(indexUrine),
                Faeces    /**/= GetValue(indexFaeces),
                Atract    /**/= GetValue(indexAtract),
                Lungs     /**/= GetValue(indexLungs),
                Skeleton  /**/= GetValue(indexSkeleton),
                Liver     /**/= GetValue(indexLiver),
                Thyroid   /**/= GetValue(indexThyroid),
            });

            startTime = endTime;
        }

        return retentions;
    }

    /// <summary>
    /// OIR Data Viewerの「Dose per Intake」＞「Material」から取得した、
    /// ある核種の50年預託実効線量 e(50) [Sv/Bq]データを保存したファイルから、
    /// 指定のmaterialに対応する数値を取得する。
    /// </summary>
    /// <param name="report"></param>
    /// <returns></returns>
    /// <exception cref="InvalidDataException"></exception>
    private static Dose GetExpectDoses(ReportData report)
    {
        var filePath = report.ExpectDosePath
            ?? throw new ArgumentNullException("target.ExpectDosePath");

        var nuclide = report.ExpectNuclide;
        var routeOfIntake = report.RouteOfIntake;

        // 預託線量の期待値を引くための文字列を組み立てる。
        var mat = $"{report.RouteOfIntake}, {report.ChemicalForm}";
        if (report.ParticleSize != "-")
            mat += $", {report.ParticleSize} µm";

        using var reader = new StreamReader(File.OpenRead(filePath), Encoding.UTF8);
        string ReadLine() => reader.ReadLine() ?? throw new InvalidDataException("Unexpected end of file.");

        string[] columns;

        // ファイルの内容が対象核種のものか確認する。
        columns = ReadLine().Split('\t');
        if (columns[1] != nuclide)
            throw new InvalidDataException($"Nuclide '{nuclide}' expected, but {columns[1]}");

        // 数値の単位が[Sv/Bq]であるかを確認する。
        columns = ReadLine().Split('\t');
        if (columns[1] != "Sv per Bq")
            throw new InvalidDataException($"Measurement unit 'Sv per Bq' expected, but '{columns[1]}'.");

        ReadLine();  // (empty line)
        ReadLine();  // (table header)

        string? line;
        while ((line = reader.ReadLine()) != null)
        {
            columns = line.Split('\t'); // Male

            if (columns[0] == mat ||
                columns[0] == routeOfIntake)    // Injectionの場合
            {
                var effectiveDose = double.Parse(columns[1]);

                var equivalentDosesMale = columns.Skip(3).Select(v => double.Parse(v)).ToArray();

                columns = ReadLine().Split('\t'); // Felame
                var equivalentDosesFemale = columns.Skip(3).Select(v => double.Parse(v)).ToArray();

                return new Dose
                {
                    EffectiveDose = effectiveDose,
                    EquivalentDosesMale = equivalentDosesMale,
                    EquivalentDosesFemale = equivalentDosesFemale,
                };
            }

            ReadLine(); // Female
        }

        throw new InvalidDataException($"Missing data for Material '{mat}' and Route of Intake '{routeOfIntake}'.");
    }

    /// <summary>
    /// FlexIDが出力した各出力時間メッシュにおける残留放射能の出力ファイル
    /// *_Retention.outから、Whole Bodyの数値列を読み込む。
    /// </summary>
    /// <param name="report"></param>
    /// <returns>時間メッシュ毎の残留放射能データ。</returns>
    private static List<Retention> GetOutputRetentions(ReportData report)
    {
        using var reader = new OutputDataReader(report.OutputRetentionPath);

        var data = reader.Read();

        // 親核種を取得する。
        report.OutputNuclide = data.Blocks.FirstOrDefault()?.Header ??
            throw new InvalidDataException();

        OutputCompartmentData? GetCompartmentData(string? selectNuclide, string name)
        {
            // 取得対象の核種が未指定の場合は親核種を選択する。
            selectNuclide ??= report.OutputNuclide;

            if (selectNuclide is null)
                return null;

            // 指定核種の計算結果を読み出す。
            var result = data.Blocks.FirstOrDefault(n => n.Header == selectNuclide);
            if (result is null)
                throw new InvalidDataException($"Missing retention data of nuclide '{selectNuclide}'.");

            var index = result.Compartments.IndexOf(c => c.Name == name);
            return index != -1 ? result.Compartments[index] : null;
        }

        var resultWholeBody /**/= GetCompartmentData(report.NuclideWholeBody, /**/"WholeBody");
        var resultUrine     /**/= GetCompartmentData(report.NuclideUrine,     /**/"Urine");
        var resultFaeces    /**/= GetCompartmentData(report.NuclideFaeces,    /**/"Faeces");
        var resultAtract    /**/= GetCompartmentData(report.NuclideAtract,    /**/"AlimentaryTract*");
        var resultLungs     /**/= GetCompartmentData(report.NuclideLungs,     /**/"Lungs*");
        var resultSkeleton  /**/= GetCompartmentData(report.NuclideSkeleton,  /**/"Skeleton*");
        var resultLiver     /**/= GetCompartmentData(report.NuclideLiver,     /**/"Liver*");
        var resultThyroid   /**/= GetCompartmentData(report.NuclideThyroid,   /**/"Thyroid*");

        if (resultWholeBody is null)
            throw new InvalidDataException();

        var retentions = new List<Retention>();

        // 経過時間ゼロでの残留放射能＝初期配分の結果を読み飛ばす。
        for (int istep = 1; istep < data.TimeSteps.Count; istep++)
        {
            double? GetValue(OutputCompartmentData? res) =>
                res?.Values[istep] is double v && !double.IsNaN(v) ? v : default(double?);

            retentions.Add(new Retention
            {
                StartTime /**/= data.TimeSteps[istep - 1],
                EndTime   /**/= data.TimeSteps[istep],
                WholeBody /**/= GetValue(resultWholeBody),
                Urine     /**/= GetValue(resultUrine),
                Faeces    /**/= GetValue(resultFaeces),
                Atract    /**/= GetValue(resultAtract),
                Lungs     /**/= GetValue(resultLungs),
                Skeleton  /**/= GetValue(resultSkeleton),
                Liver     /**/= GetValue(resultLiver),
                Thyroid   /**/= GetValue(resultThyroid),
            });
        }

        return retentions;
    }

    /// <summary>
    /// FlexIDが出力した各出力時間メッシュにおける線量の出力ファイル
    /// *_Dose.outから、預託実効線量と預託等価線量の数値を読み込む。
    /// </summary>
    /// <param name="report"></param>
    /// <returns></returns>
    private static Dose GetOutputDoses(ReportData report)
    {
        // 組織加重係数データを読み込む。
        var (ts, ws) = InputDataReaderBase.ReadTissueWeights(Path.Combine(AppResource.BaseDir, @"lib\OIR\wT.txt"));

        using var reader = new OutputDataReader(report.OutputDosePath);

        var data = reader.Read();
        var resultM = data.Blocks[0];
        var resultF = data.Blocks[1];

        // 列データの最終行＝評価期間の最後における線量値を取得する。
        double GetDose(OutputBlockData result, string targetRegion)
            => result.Compartments.FirstOrDefault(c => c.Name == targetRegion)?.Values.Last()
            ?? throw new InvalidDataException($"Missing '{targetRegion}' data column");

        double GetTissueWeight(string targetRegion)
            => ws[Array.IndexOf(ts, targetRegion)];

        (double Male, double Female) GetResult(string targetRegion, params string[] moreTargetRegions)
        {
            var doseM = GetDose(resultM, targetRegion);
            var doseF = GetDose(resultF, targetRegion);
            if (moreTargetRegions.Length == 0)
                return (doseM, doseF);

            // 複数の標的領域の等価線量を組織加重係数で加重平均したものを返す。
            var wT = GetTissueWeight(targetRegion);
            var sumOfWeightedDoseM = wT * doseM;
            var sumOfWeightedDoseF = wT * doseF;
            var sumOfTissueWeight = wT;
            foreach (var moreTargetRegion in moreTargetRegions)
            {
                wT = GetTissueWeight(moreTargetRegion);
                doseM = GetDose(resultM, moreTargetRegion);
                doseF = GetDose(resultF, moreTargetRegion);
                sumOfWeightedDoseM += wT * doseM;
                sumOfWeightedDoseF += wT * doseF;
                sumOfTissueWeight += wT;
            }
            doseM = sumOfWeightedDoseM / sumOfTissueWeight;
            doseF = sumOfWeightedDoseF / sumOfTissueWeight;
            return (doseM, doseF);
        }

        // Effective Dose
        var resultWholeBody = GetDose(resultM, "WholeBody");

        // Equivalent Dose
        var equivalentDoses = new[]
        {
            // OIR Data Viewerで提示されている預託等価線量の領域名と、
            // それらに対応する標的領域(1つ以上)の名称。
            /* Bone marrow     */ GetResult("R-marrow"),
            /* Colon           */ GetResult("RC-stem", "LC-stem", "RS-stem"),
            /* Lung            */ GetResult("Bronch-bas", "Bronch-sec", "Bchiol-sec", "AI"),
            /* Stomach         */ GetResult("St-stem"),
            /* Breast          */ GetResult("Breast"),
            /* Ovaries         */ GetResult("Ovaries"),
            /* Testes          */ GetResult("Testes"),
            /* Urinary bladder */ GetResult("UB-wall"),
            /* Oesophagus      */ GetResult("Oesophagus"),
            /* Liver           */ GetResult("Liver"),
            /* Thyroid         */ GetResult("Thyroid"),
            /* Bone Surface    */ GetResult("Endost-BS"),
            /* Brain           */ GetResult("Brain"),
            /* Salivary glands */ GetResult("S-glands"),
            /* Skin            */ GetResult("Skin"),
            /* Adrenals        */ GetResult("Adrenals"),
            /* ET of HRTM      */ GetResult("ET1-bas", "ET2-bas"),
            /* Gall bladder    */ GetResult("GB-wall"),
            /* Heart           */ GetResult("Ht-wall"),
            /* Kidneys         */ GetResult("Kidneys"),
            /* Lymphatic nodes */ GetResult("LN-Sys", "LN-Th", "LN-ET"),
            /* Muscle          */ GetResult("Muscle"),
            /* Oral mucosa     */ GetResult("O-mucosa"),
            /* Pancreas        */ GetResult("Pancreas"),
            /* Prostate        */ GetResult("Prostate"),
            /* Small intestine */ GetResult("SI-stem"),
            /* Spleen          */ GetResult("Spleen"),
            /* Thymus          */ GetResult("Thymus"),
            /* Uterus          */ GetResult("Uterus"),
        };

        return new Dose
        {
            EffectiveDose = resultWholeBody,
            EquivalentDosesMale = equivalentDoses.Select(d => d.Male).ToArray(),
            EquivalentDosesFemale = equivalentDoses.Select(d => d.Female).ToArray(),
        };
    }
}
