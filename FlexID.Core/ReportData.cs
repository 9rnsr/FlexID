using System.Text;
using System.Text.RegularExpressions;
using Sprache;

namespace FlexID;

/// <summary>
/// レポート出力の対象。
/// </summary>
public class ReportTarget
{
    public ReportTarget(string outputPath, (string Name, string Path)? compare)
    {
        Name = Path.GetFileNameWithoutExtension(outputPath);
        OutputPath = outputPath;

        ResultRetentionPath = $"{outputPath}_Retention.out";
        ResultDosePath = $"{outputPath}_Dose.out";

        if (compare is var (name, path))
        {
            var expectNuclide = name.Split('_')[0];

            ExpectRetentionPath = path;
            ExpectDosePath = Path.Combine(Path.GetDirectoryName(ExpectRetentionPath)!, expectNuclide + ".dat");
        }
    }

    /// <summary>
    /// レポート出力処理中に発生したエラーを保持する。
    /// </summary>
    public List<string> Errors { get; } = [];

    public string Name { get; }
    public string OutputPath { get; }

    public string Nuclide { get; set; } = "";

    public string ResultRetentionPath { get; }
    public string ResultDosePath { get; }

    public string? ExpectRetentionPath { get; }
    public string? ExpectDosePath { get; }

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
}

/// <summary>
/// レポート出力の内容。
/// </summary>
public class ReportResult(ReportTarget target)
{
    public ReportTarget Target { get; } = target;

    public List<string> Errors => Target.Errors;

    public bool HasErrors => Errors.Count != 0;

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

    public IReadOnlyList<Retention>? ActualActs;
    public IReadOnlyList<Retention>? ExpectActs;

    /// <summary>
    /// 線量の取得結果。
    /// </summary>
    public class Dose
    {
        public required double EffectiveDose { get; init; }
        public required double[] EquivalentDosesMale { get; init; }
        public required double[] EquivalentDosesFemale { get; init; }
    }

    public Dose? ActualDose;
    public Dose? ExpectDose;

    public (double Min, double Max) FractionsWholeBody;
    public (double Min, double Max) FractionsUrine;
    public (double Min, double Max) FractionsFaeces;
    public (double Min, double Max) FractionsAtract;
    public (double Min, double Max) FractionsLungs;
    public (double Min, double Max) FractionsSkeleton;
    public (double Min, double Max) FractionsLiver;
    public (double Min, double Max) FractionsThyroid;

    /// <summary>
    /// 計算悔過kと比較対象の結果を取得する。
    /// </summary>
    /// <returns></returns>
    public void LoadResult()
    {
        var resultRetentionPath = Target.ResultRetentionPath;
        var resultDosePath = Target.ResultDosePath;
        var expectRetentionPath = Target.ExpectRetentionPath;
        var expectDosePath = Target.ExpectDosePath;

        // 残留放射能について期待値データは子孫核種の数値を提示していることがあり、その場合は
        // 結果値データからも対応する箇所を読み込む必要があるため、ここでは期待値→結果値の順番で読み込みを行う。
        if (expectRetentionPath is not null)
        {
            try { ExpectActs = GetExpectRetentions(Target); }
            catch { Errors.Add($"Failed to read expect retention file: {expectRetentionPath}"); }
        }
        if (resultRetentionPath is not null)
        {
            try { ActualActs = GetResultRetentions(Target); }
            catch { Errors.Add($"Failed to read result retention file: {resultRetentionPath}"); }
        }

        // ExpectRetentionが正常に読み込めなかった場合は、
        // ExpectDoseを取得するための情報が不足するため、これをスキップする必要がある。
        if (expectDosePath is not null && ExpectActs is not null)
        {
            try { ExpectDose = GetExpectDoses(Target); }
            catch { Errors.Add($"Failed to read expect dose file: {expectDosePath}"); }
        }
        if (resultDosePath is not null)
        {
            try { ActualDose = GetResultDoses(Target); }
            catch { Errors.Add($"Failed to read result dose file: {resultDosePath}"); }
        }

        // 50年の預託期間における、各出力時間メッシュにおける数値の比較。
        // 要約として、期待値に対する下振れ率と上振れ率の最大値を算出する。
        if (ActualActs is not null && ExpectActs is not null)
        {
            var fractionsWholeBody /**/= (min: double.PositiveInfinity, max: double.NegativeInfinity);
            var fractionsUrine     /**/= (min: double.PositiveInfinity, max: double.NegativeInfinity);
            var fractionsFaeces    /**/= (min: double.PositiveInfinity, max: double.NegativeInfinity);
            var fractionsAtract    /**/= (min: double.PositiveInfinity, max: double.NegativeInfinity);
            var fractionsLungs     /**/= (min: double.PositiveInfinity, max: double.NegativeInfinity);
            var fractionsSkeleton  /**/= (min: double.PositiveInfinity, max: double.NegativeInfinity);
            var fractionsLiver     /**/= (min: double.PositiveInfinity, max: double.NegativeInfinity);
            var fractionsThyroid   /**/= (min: double.PositiveInfinity, max: double.NegativeInfinity);

            var activities = ActualActs.Zip(ExpectActs, (a, e) => (a, e));
            foreach (var (actualAct, expectAct) in activities)
            {
                //Console.WriteLine(
                //    $"{expectAct.EndTime,8}," +
                //    $"{expectAct.WholeBody:0.0E+00}," +
                //    $"{(expectAct.Urine != null ? $"{expectAct.Urine:0.0E+00}" : "-"),-7}," +
                //    $"{(expectAct.Faeces != null ? $"{expectAct.Faeces:0.0E+00}" : "-"),-7}," +
                //    $"{actualAct.WholeBody:0.00000000E+00}," +
                //    $"{(actualAct.Urine != null ? $"{actualAct.Urine:0.00000000E+00}" : "-"),-14}," +
                //    $"{(actualAct.Faeces != null ? $"{actualAct.Faeces:0.00000000E+00}" : "-"),-14}");

                if (expectAct.WholeBody is double expectWholeBody &&
                    actualAct.WholeBody is double actualWholeBody)
                {
                    var fracWholeBody = actualWholeBody / expectWholeBody;
                    fractionsWholeBody = (Math.Min(fractionsWholeBody.min, fracWholeBody),
                                          Math.Max(fractionsWholeBody.max, fracWholeBody));
                }
                if (expectAct.Urine is double expectUrine &&
                    actualAct.Urine is double actualUrine)
                {
                    var fracUrine = actualUrine / expectUrine;
                    fractionsUrine = (Math.Min(fractionsUrine.min, fracUrine),
                                      Math.Max(fractionsUrine.max, fracUrine));
                }
                if (expectAct.Faeces is double expectFaeces &&
                    actualAct.Faeces is double actualFaeces)
                {
                    var fracFaeces = actualFaeces / expectFaeces;
                    fractionsFaeces = (Math.Min(fractionsFaeces.min, fracFaeces),
                                       Math.Max(fractionsFaeces.max, fracFaeces));
                }
                if (expectAct.Atract is double expectAtract &&
                    actualAct.Atract is double actualAtract)
                {
                    var fracAtract = actualAtract / expectAtract;
                    fractionsAtract = (Math.Min(fractionsAtract.min, fracAtract),
                                       Math.Max(fractionsAtract.max, fracAtract));
                }
                if (expectAct.Lungs is double expectLungs &&
                    actualAct.Lungs is double actualLungs)
                {
                    var fracLungs = actualLungs / expectLungs;
                    fractionsLungs = (Math.Min(fractionsLungs.min, fracLungs),
                                      Math.Max(fractionsLungs.max, fracLungs));
                }
                if (expectAct.Skeleton is double expectSkeleton &&
                    actualAct.Skeleton is double actualSkeleton)
                {
                    var fracSkeleton = actualSkeleton / expectSkeleton;
                    fractionsSkeleton = (Math.Min(fractionsSkeleton.min, fracSkeleton),
                                         Math.Max(fractionsSkeleton.max, fracSkeleton));
                }
                if (expectAct.Liver is double expectLiver &&
                    actualAct.Liver is double actualLiver)
                {
                    var fracLiver = actualLiver / expectLiver;
                    fractionsLiver = (Math.Min(fractionsLiver.min, fracLiver),
                                      Math.Max(fractionsLiver.max, fracLiver));
                }
                if (expectAct.Thyroid is double expectThyroid &&
                    actualAct.Thyroid is double actualThyroid)
                {
                    var fracThyroid = actualThyroid / expectThyroid;
                    fractionsThyroid = (Math.Min(fractionsThyroid.min, fracThyroid),
                                        Math.Max(fractionsThyroid.max, fracThyroid));
                }
            }

            FractionsWholeBody /**/= fractionsWholeBody;
            FractionsUrine     /**/= fractionsUrine;
            FractionsFaeces    /**/= fractionsFaeces;
            FractionsAtract    /**/= fractionsAtract;
            FractionsLungs     /**/= fractionsLungs;
            FractionsSkeleton  /**/= fractionsSkeleton;
            FractionsLiver     /**/= fractionsLiver;
            FractionsThyroid   /**/= fractionsThyroid;
        }
    }

    /// <summary>
    /// OIR Data Viewerの「Dose per Content & Reference Bioassay Functions」から取得した、
    /// 核種・摂取形態・化学形態・及び粒子径に対応する残留放射能データを保存したファイルから
    /// 全身の残留放射能の数値を取得する。
    /// </summary>
    /// <param name="target"></param>
    /// <returns>時間メッシュ毎の残留放射能データ。</returns>
    /// <exception cref="InvalidDataException"></exception>
    private static List<Retention> GetExpectRetentions(ReportTarget target)
    {
        var filePath = target.ExpectRetentionPath
            ?? throw new ArgumentNullException("target.ExpectRetentionPath");

        var nuclide = Path.GetFileNameWithoutExtension(filePath).Split('_')[0];
        var retentions = new List<Retention>();

        using var reader = new StreamReader(File.OpenRead(filePath), Encoding.UTF8);
        string ReadLine() => reader.ReadLine() ?? throw new InvalidDataException("Unexpected end of file.");

        string[] columns;

        // ファイルの内容が対象核種のものか確認する。
        columns = ReadLine().Split('\t');
        if (columns[1] != nuclide)
            throw new InvalidDataException();

        // 対象の摂取形態。
        target.RouteOfIntake = ReadLine().Split('\t')[1];

        // 対象の化学形態。
        target.ChemicalForm = ReadLine().Split('\t')[1];

        // 対象の粒子サイズ。
        target.ParticleSize = ReadLine().Split('\t')[1];

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
        target.NuclideWholeBody /**/= GetRetentionNuclide(indexWholeBody, /**/"Whole Body");
        target.NuclideUrine     /**/= GetRetentionNuclide(indexUrine,     /**/"Urine (24-hour sample)");
        target.NuclideFaeces    /**/= GetRetentionNuclide(indexFaeces,    /**/"Faeces (24-hour sample)");
        target.NuclideAtract    /**/= GetRetentionNuclide(indexAtract,    /**/"Alimentary Tract*");
        target.NuclideLungs     /**/= GetRetentionNuclide(indexLungs,     /**/"Lungs*");
        target.NuclideSkeleton  /**/= GetRetentionNuclide(indexSkeleton,  /**/"Skeleton*");
        target.NuclideLiver     /**/= GetRetentionNuclide(indexLiver,     /**/"Liver*");
        target.NuclideThyroid   /**/= GetRetentionNuclide(indexThyroid,   /**/"Thyroid*");

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
    /// <param name="target"></param>
    /// <returns></returns>
    /// <exception cref="InvalidDataException"></exception>
    private static Dose GetExpectDoses(ReportTarget target)
    {
        var filePath = target.ExpectDosePath
            ?? throw new ArgumentNullException("target.ExpectDosePath");

        var nuclide = Path.GetFileNameWithoutExtension(target.ExpectRetentionPath!).Split('_')[0];
        var routeOfIntake = target.RouteOfIntake;

        // 預託線量の期待値を引くための文字列を組み立てる。
        var mat = $"{target.RouteOfIntake}, {target.ChemicalForm}";
        if (target.ParticleSize != "-")
            mat += $", {target.ParticleSize} µm";

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
    /// <param name="target"></param>
    /// <returns>時間メッシュ毎の残留放射能データ。</returns>
    private static List<Retention> GetResultRetentions(ReportTarget target)
    {
        var retentions = new List<Retention>();

        using var reader = new OutputDataReader(target.ResultRetentionPath);

        var data = reader.Read();

        // 親核種を取得する。
        target.Nuclide = data.Blocks.FirstOrDefault()?.Header ??
            throw new InvalidDataException();

        OutputCompartmentData? GetCompartmentData(string? selectNuclide, string name)
        {
            // 取得対象の核種が未指定の場合は親核種を選択する。
            selectNuclide ??= target.Nuclide;

            if (selectNuclide is null)
                return null;

            // 指定核種の計算結果を読み出す。
            var result = data.Blocks.FirstOrDefault(n => n.Header == selectNuclide);
            if (result is null)
                throw new InvalidDataException($"Missing retention data of nuclide '{selectNuclide}'.");

            var index = result.Compartments.IndexOf(c => c.Name == name);
            return index != -1 ? result.Compartments[index] : null;
        }

        var resultWholeBody /**/= GetCompartmentData(target.NuclideWholeBody, /**/"WholeBody");
        var resultUrine     /**/= GetCompartmentData(target.NuclideUrine,     /**/"Urine");
        var resultFaeces    /**/= GetCompartmentData(target.NuclideFaeces,    /**/"Faeces");
        var resultAtract    /**/= GetCompartmentData(target.NuclideAtract,    /**/"AlimentaryTract*");
        var resultLungs     /**/= GetCompartmentData(target.NuclideLungs,     /**/"Lungs*");
        var resultSkeleton  /**/= GetCompartmentData(target.NuclideSkeleton,  /**/"Skeleton*");
        var resultLiver     /**/= GetCompartmentData(target.NuclideLiver,     /**/"Liver*");
        var resultThyroid   /**/= GetCompartmentData(target.NuclideThyroid,   /**/"Thyroid*");

        if (resultWholeBody is null)
            throw new InvalidDataException();

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
    /// <param name="target"></param>
    /// <returns></returns>
    private static Dose GetResultDoses(ReportTarget target)
    {
        // 組織加重係数データを読み込む。
        var (ts, ws) = InputDataReaderBase.ReadTissueWeights(Path.Combine(AppResource.BaseDir, @"lib\OIR\wT.txt"));

        using var reader = new OutputDataReader(target.ResultDosePath);

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
