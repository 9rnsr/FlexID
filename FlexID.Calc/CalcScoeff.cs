using MathNet.Numerics.Interpolation;

namespace FlexID.Calc;

public class CalcScoeff
{
    public string InterpolationMethod = "";

    // J/MeVへの変換
    private const double ToJoule = 1.602176634E-13;

    private readonly SAFData safdata;

    // 計算結果である、線源領域vs標的領域の組み合わせ毎のS係数値
    public double[] OutTotal { get; }
    public double[] OutP { get; }
    public double[] OutE { get; }
    public double[] OutB { get; }
    public double[] OutA { get; }
    public double[] OutN { get; }

    /// <summary>
    /// コンストラクタ。
    /// </summary>
    /// <param name="safdata">計算に使用するSAFデータ。</param>
    public CalcScoeff(SAFData safdata)
    {
        this.safdata = safdata;

        // 計算結果である、線源領域vs標的領域の組み合わせ毎のS係数値
        OutTotal = new double[safdata.Count];
        OutP = new double[safdata.Count];
        OutE = new double[safdata.Count];
        OutB = new double[safdata.Count];
        OutA = new double[safdata.Count];
        OutN = new double[safdata.Count];
    }

    /// <summary>
    /// S係数の計算
    /// </summary>
    /// <param name="nuc">対象核種の名称</param>
    public void CalcS(string nuc)
    {
        // 放射線データ取得
        var raddata = SAFDataReader.ReadRAD(nuc);

        // βスペクトル取得
        var betdata = SAFDataReader.ReadBET(nuc);

        var WRalpha = safdata.WRalpha;
        var WRphoton = safdata.WRphoton;
        var WRelectron = safdata.WRelectron;

        var (WRneutron, SAFneutron) =
            safdata.SAFneutron.TryGetValue(nuc, out var n) ? n : (0.0, null);

        for (int TScount = 0; TScount < safdata.Count; TScount++)
        {
            var SAFalpha = safdata.SAFalpha[TScount];
            var SAFphoton = safdata.SAFphoton[TScount];
            var SAFelectron = safdata.SAFelectron[TScount];

            // β線計算の終了判定フラグ
            bool finishBeta = false;

            // 放射線ごとのS係数計算値
            double ScoeffP = 0;
            double ScoeffE = 0;
            double ScoeffB = 0;
            double ScoeffA = 0;
            double ScoeffN = 0;

            // 指定のエネルギー位置におけるSAF値を算出する処理。
            Func<double, double> CalcSAFa;
            Func<double, double> CalcSAFp;
            Func<double, double> CalcSAFe;
            if (InterpolationMethod == "PCHIP")
            {
                var pchipA = CubicSpline.InterpolatePchip(safdata.EnergyA, SAFalpha);
                var pchipP = CubicSpline.InterpolatePchip(safdata.EnergyP, SAFphoton);
                var pchipE = CubicSpline.InterpolatePchip(safdata.EnergyE, SAFelectron);

                CalcSAFa = Ei => pchipA.Interpolate(Ei);
                CalcSAFp = Ei => pchipP.Interpolate(Ei);
                CalcSAFe = Ei => pchipE.Interpolate(Ei);
            }
            else if (InterpolationMethod == "線形補間")
            {
                CalcSAFa = Ei => InterpolateLinearSAF(Ei, safdata.EnergyA, SAFalpha);
                CalcSAFp = Ei => InterpolateLinearSAF(Ei, safdata.EnergyP, SAFphoton);
                CalcSAFe = Ei => InterpolateLinearSAF(Ei, safdata.EnergyE, SAFelectron);
            }
            else
                throw new InvalidOperationException(InterpolationMethod);

            // α反跳核と核分裂片のS係数計算では、α粒子の2MeVにおけるSAFを使用する。
            //
            // ICRP Publ.133 p.73 Para.78
            //   (78) The available kinetic energy of alpha transitions is shared between the alpha
            // particle and the recoiling nucleus.Similarly, the kinetic energy in spontaneous fission
            // is shared between the fission fragments.The yield and kinetic energies of these
            // radiations are included in the decay data tabulations of Publication 107(ICRP,
            // 2008).The range of the alpha recoil nuclei and that of the fission fragments is limited
            // and thus their contribution to absorbed dose in the target tissues is evaluated using
            // the SAF for a 2.0 MeV alpha particle.
            //   (78) アルファ遷移の利用可能な運動エネルギーは、アルファ粒子と反跳核の間で
            // 共有されます。同様に、自発核分裂の運動エネルギーは、核分裂片の間で
            // 共有されます。これらの放射線の収量と運動エネルギーは、Publication 107(ICRP,
            // 2008)の崩壊データ表に記載されています。アルファ反跳核と核分裂片の飛程は限られて
            // いるため、標的組織の吸収線量への寄与は、2.0 MeVアルファ粒子のSAFを使用して
            // 評価されます。
            var SAFa_2MeV = CalcSAFa(2.0);

            foreach (var rad in raddata)
            {
                // エネルギー毎のSAF算出
                string[] Rad = rad.Split([" "], StringSplitOptions.RemoveEmptyEntries);

                string icode = Rad[0];      // 放射線のタイプ
                string yield = Rad[1];      // 放射線の収率(/nt)
                string energy = Rad[2];     // 放射線のエネルギー(MeV)
                string jcode = Rad[3];      // 放射線のタイプ

                double Yi = double.Parse(yield);
                double Ei = double.Parse(energy);

                if (jcode == "X" || jcode == "G" || jcode == "PG" || jcode == "DG" || jcode == "AQ")
                {
                    // X:X線、G:γ線、PG:遅発γ線、DG:即発γ線、AQ:消滅光子
                    ScoeffP += Yi * Ei * CalcSAFp(Ei) * WRphoton * ToJoule;
                }
                else if (jcode == "AE" || jcode == "IE")
                {
                    // IE: 内部転換電子、AE: オージェ電子
                    ScoeffE += Yi * Ei * CalcSAFe(Ei) * WRelectron * ToJoule;
                }
                else if (jcode == "B-" || jcode == "B+" || jcode == "DB")
                {
                    // B-:β粒子(電子)、B+: β+粒子(陽電子)、DB: 遅発β粒子
                    if (finishBeta)
                        continue;

                    // RADファイルに定義されているエントリは単に無視し、BETファイルからS係数を計算する
                    double beta = 0;
                    for (int r = 1; r < betdata.Length; r++)
                    {
                        // エネルギー幅の下限(MeV)
                        // 下限側のエネルギー点における、1壊変・1MeVあたりのβ粒子数(/MeV/nt)
                        var (ebinL, nparL) = betdata[r - 1];

                        // エネルギー幅の上限(MeV)
                        // 上限側のエネルギー点における、1壊変・1MeVあたりのβ粒子数(/MeV/nt)
                        var (ebinH, nparH) = betdata[r - 0];

                        var yieldL = ebinL * nparL; // 下限側のエネルギー点における放射線の収率(/nt)
                        var yieldH = ebinH * nparH; // 上限側のエネルギー点における放射線の収率(/nt)

                        var safL = CalcSAFe(ebinL); // 下限側のエネルギー点におけるS係数
                        var safH = CalcSAFe(ebinH); // 上限側のエネルギー点におけるS係数

                        // 台形積分でエネルギー区間のY * Eを算出する。
                        beta += (yieldL * safL + yieldH * safH) * (ebinH - ebinL) / 2;
                    }
                    ScoeffB = beta * WRelectron * ToJoule;

                    finishBeta = true;
                }
                else if (jcode == "A")
                {
                    // α粒子
                    ScoeffA += Yi * Ei * CalcSAFa(Ei) * WRalpha * ToJoule;
                }
                else if (jcode == "AR")
                {
                    // α反跳核
                    ScoeffA += Yi * Ei * SAFa_2MeV * WRalpha * ToJoule;
                }
                else if (jcode == "FF")
                {
                    // 核分裂片
                    ScoeffN += Yi * Ei * SAFa_2MeV * WRalpha * ToJoule;
                }
                else if (jcode == "N")
                {
                    // 中性子
                    var SAFn = SAFneutron?[TScount] ??
                        throw new InvalidDataException("neutron SAF");

                    ScoeffN += Yi * Ei * SAFn * WRneutron * ToJoule;
                }
                else
                {
                    // そのほかのJCODEについては処理しない
                }
            }

            // 全ての放射線についてのS係数
            var Scoeff = ScoeffP + ScoeffE + ScoeffB + ScoeffA + ScoeffN;
            OutTotal[TScount] = Scoeff;
            OutP[TScount] = ScoeffP;
            OutE[TScount] = ScoeffE;
            OutB[TScount] = ScoeffB;
            OutA[TScount] = ScoeffA;
            OutN[TScount] = ScoeffN;
        }
    }

    /// <summary>
    /// 指定エネルギー点におけるSAFを線形補間で算出する。
    /// </summary>
    /// <param name="energy">放射線のエネルギー(MeV)</param>
    /// <param name="ebins">放射線のエネルギーBinを定義した配列</param>
    /// <param name="SAF">放射線のエネルギーBin毎のSAF値</param>
    /// <returns>SAF値</returns>
    private double InterpolateLinearSAF(double energy, double[] ebins, double[] SAF)
    {
        for (int i = 0; i < ebins.Length; i++)
        {
            if (energy == ebins[i])
            {
                // SAFを求めるエネルギー点がエネルギーBin境界上にある場合
                return SAF[i];
            }
            if (energy < ebins[i])
            {
                // SAFを求めるエネルギー点がエネルギーBinの間にある場合
                var ergBinL = ebins[i - 1];
                var ergBinH = ebins[i - 0];
                var SAFBinL = SAF[i - 1];
                var SAFBinH = SAF[i - 0];

                return SAFBinL + (energy - ergBinL) * (SAFBinH - SAFBinL) / (ergBinH - ergBinL);
            }
        }

        // エネルギービンを超えることはないはず
        throw new Exception("Assert(energyBin.Last < energy)");
    }

    /// <summary>
    /// 計算結果であるS係数データをファイルに書き出す。
    /// </summary>
    /// <param name="filePath"></param>
    public void WriteOutTotalResult(string filePath)
    {
        var lines = GenerateScoeffFileContent(OutTotal);
        File.WriteAllLines(filePath, lines, System.Text.Encoding.UTF8);
    }

    /// <summary>
    /// 計算した光子のS係数データをファイルに書き出す。
    /// </summary>
    /// <param name="filePath"></param>
    public void WriteOutPhotonResult(string filePath)
    {
        var lines = GenerateScoeffFileContent(OutP);
        File.WriteAllLines(filePath, lines, System.Text.Encoding.UTF8);
    }

    /// <summary>
    /// 計算した電子のS係数データをファイルに書き出す。
    /// </summary>
    /// <param name="filePath"></param>
    public void WriteOutElectronaResult(string filePath)
    {
        var lines = GenerateScoeffFileContent(OutE);
        File.WriteAllLines(filePath, lines, System.Text.Encoding.UTF8);
    }

    /// <summary>
    /// 計算したβ粒子のS係数データをファイルに書き出す。
    /// </summary>
    /// <param name="filePath"></param>
    public void WriteOutBetaResult(string filePath)
    {
        var lines = GenerateScoeffFileContent(OutB);
        File.WriteAllLines(filePath, lines, System.Text.Encoding.UTF8);
    }

    /// <summary>
    /// 計算したα粒子のS係数データをファイルに書き出す。
    /// </summary>
    /// <param name="filePath"></param>
    public void WriteOutAlphaResult(string filePath)
    {
        var lines = GenerateScoeffFileContent(OutA);
        File.WriteAllLines(filePath, lines, System.Text.Encoding.UTF8);
    }

    /// <summary>
    /// 計算した中性子のS係数データをファイルに書き出す。
    /// </summary>
    /// <param name="filePath"></param>
    public void WriteOutNeutronResult(string filePath)
    {
        var lines = GenerateScoeffFileContent(OutN);
        File.WriteAllLines(filePath, lines, System.Text.Encoding.UTF8);
    }

    private IEnumerable<string> GenerateScoeffFileContent(double[] outData)
    {
        var sourceRegions = safdata.SourceRegions.Select(s => s.Name).ToArray();
        var targetRegions = safdata.TargetRegions.Select(t => t.Name).ToArray();
        var nS = sourceRegions.Length;
        var nT = targetRegions.Length;

        {
            var line = $"{"  T/S",-10}";
            for (var iS = 0; iS < nS; iS++)
            {
                var sourceRegion = sourceRegions[iS];
                line += $" {sourceRegion,-14}";
            }
            yield return line.TrimEnd();
        }
        for (var iT = 0; iT < nT; iT++)
        {
            var targetRegion = targetRegions[iT];

            var line = $"{targetRegion,-10}";

            for (var iS = 0; iS < nS; iS++)
            {
                line += $" {outData[iT + nT * iS]:0.00000000E+00}";
            }

            yield return line;
        }
    }

    public static IEnumerable<string> GenerateScoeffFileContent(
        Dictionary<string, double[]> outData,
        IReadOnlyList<string> sourceRegions, IReadOnlyList<string> targetRegions)
    {
        var nS = sourceRegions.Count;
        var nT = targetRegions.Count;

        {
            var line = $"{"  T/S",-10}";
            for (var iS = 0; iS < nS; iS++)
            {
                var sourceRegion = sourceRegions[iS];
                line += $" {sourceRegion,-14}";
            }
            yield return line.TrimEnd();
        }
        for (var iT = 0; iT < nT; iT++)
        {
            var targetRegion = targetRegions[iT];

            var line = $"{targetRegion,-10}";

            for (var iS = 0; iS < nS; iS++)
            {
                var sourceRegion = sourceRegions[iS];
                var outDataColumn = outData[sourceRegion];
                line += $" {outDataColumn[iT]:0.00000000E+00}";
            }

            yield return line;
        }
    }

    /// <summary>
    /// 計算したS係数データをIDAC Dose2.1と比較可能な形式でファイルに書き出す。
    /// </summary>
    /// <param name="filePath"></param>
    public void WriteOutIdacDoseCompatibleResult(string filePath, Sex sex)
    {
        var lines = GenerateScoeffFileContent_IdacDoseCompatible(sex);
        File.WriteAllLines(filePath, lines, System.Text.Encoding.UTF8);
    }

    private IEnumerable<string> GenerateScoeffFileContent_IdacDoseCompatible(Sex sex)
    {
        var nS = safdata.SourceRegions.Length;
        var nT = safdata.TargetRegions.Length;

        var sourceRegions = SortSourceRegionsByIdacDoseOrder(safdata.SourceRegions.Select(s => s.Name)).ToArray();
        var targetRegions = SortTargetRegionsByIdacDoseOrder(safdata.TargetRegions.Select(t => t.Name)).ToArray();
        var numS = sourceRegions.Length;
        var numT = targetRegions.Length;

        (string name, double MaleMass, double FemaleMass)[] OtherSourceRegions =
        [
            ("O-mucosa",   3.583E-02, 2.245E-02), ("Teeth-V",    5.000E-02, 4.000E-02), ("Tonsils",    3.000E-03, 3.000E-03),
            ("Oesophag-w", 4.000E-02, 3.500E-02), ("St-wall",    1.500E-01, 1.400E-01), ("SI-wall",    6.500E-01, 6.000E-01),
            ("RC-wall",    1.500E-01, 1.450E-01), ("LC-wall",    1.500E-01, 1.450E-01), ("RS-wall",    7.000E-02, 7.000E-02),
            ("LN-ET",      1.500E-02, 1.200E-02), ("LN-Th",      1.500E-02, 1.200E-02), ("Adrenals",   1.400E-02, 1.300E-02),
            ("C-bone-V",   4.400E+00, 3.200E+00), ("T-bone-V",   1.100E+00, 8.000E-01), ("R-marrow",   1.170E+00, 9.000E-01),
            ("Y-marrow",   2.480E+00, 1.800E+00), ("Brain",      1.450E+00, 1.300E+00), ("Breast",     2.500E-02, 5.000E-01),
            ("Eye-lens",   4.000E-04, 4.000E-04), ("GB-wall",    1.000E-02, 8.000E-03), ("Ht-wall",    3.300E-01, 2.500E-01),
            ("Kidneys",    3.100E-01, 2.750E-01), ("Liver",      1.800E+00, 1.400E+00), ("LN-Sys",     1.484E-01, 1.187E-01),
            ("Ovaries",    0.000E+00, 1.100E-02), ("Pancreas",   1.400E-01, 1.200E-01), ("P-gland",    6.000E-04, 6.000E-04),
            ("Prostate",   1.700E-02, 0.000E+00), ("S-glands",   8.500E-02, 7.000E-02), ("Skin",       3.300E+00, 2.300E+00),
            ("Spleen",     1.500E-01, 1.300E-01), ("Testes",     3.500E-02, 0.000E+00), ("Thymus",     2.500E-02, 2.000E-02),
            ("Thyroid",    2.000E-02, 1.700E-02), ("Ureters",    1.600E-02, 1.500E-02), ("UB-wall",    5.000E-02, 4.000E-02),
            ("Uterus",     0.000E+00, 8.000E-02), ("Adipose",    1.723E+01, 2.141E+01), ("Cartilage",  1.100E+00, 9.000E-01),
            ("Muscle",     2.900E+01, 1.750E+01), ("ET1-wall",   2.923E-03, 2.526E-03), ("ET2-wall",   2.923E-03, 2.526E-03),
            ("Lung-Tis",   5.000E-01, 4.200E-01),
        ];

        string CalcOther(int iT)
        {
            var outP = 0.0;
            var outE = 0.0;
            var outB = 0.0;
            var outA = 0.0;
            var outN = 0.0;
            var outTotal = 0.0;

            var otherMass = 0.0;
            foreach (var o in OtherSourceRegions)
            {
                var name = o.name;
                var mass = sex == Sex.Male ? o.MaleMass : o.FemaleMass;

                var (_, _, iS) = sourceRegions.First(x => x.NameOIR == name);
                var i = iT + nT * iS;

                // Otherの内訳である43個の線源領域のS係数を、線源領域毎の重量で加重平均する
                otherMass += mass;
                outP += mass * OutP[i];
                outE += mass * OutE[i];
                outB += mass * OutB[i];
                outA += mass * OutA[i];
                outN += mass * OutN[i];
                outTotal += OutTotal[i];
            }

            if (otherMass != 0)
            {
                outP /= otherMass;
                outE /= otherMass;
                outB /= otherMass;
                outA /= otherMass;
                outN /= otherMass;
                outTotal /= otherMass;
            }

            string line = "";
            line += $",{outP:0.00000000E+00}";
            line += $",{outE:0.00000000E+00}";
            line += $",{outB:0.00000000E+00}";
            line += $",{outA:0.00000000E+00}";
            line += $",{outN:0.00000000E+00}";
            line += $",{outTotal:0.00000000E+00}";
            return line;
        }

        yield return "Source Regions,,Target Regions,,Photon,Electron,Beta,Alpha,Neutron,Total";
        yield return "IDAC Dose2.1,OIR(FlexID),IDAC Dose2.1,OIR(FlexID)";

        for (var indexS = 0; indexS < numS; indexS++)
        {
            var (sourceIdacDose, sourceOIR, iS) = sourceRegions[indexS];

            var line = $"{sourceIdacDose},{sourceOIR},";

            for (var indexT = 0; indexT < numT; indexT++)
            {
                var (targetIdacDose, targetOIR, iT) = targetRegions[indexT];

                line += $"{targetIdacDose},{targetOIR}";
                if (iS == -1 || iT == -1)
                {
                    if (sourceIdacDose == "\"Other\"" && iT != -1)
                        line += CalcOther(iT);
                    else
                        line += $",,,,,,";
                }
                else
                {
                    var i = iT + nT * iS;
                    line += $",{OutP[i]:0.00000000E+00}";
                    line += $",{OutE[i]:0.00000000E+00}";
                    line += $",{OutB[i]:0.00000000E+00}";
                    line += $",{OutA[i]:0.00000000E+00}";
                    line += $",{OutN[i]:0.00000000E+00}";
                    line += $",{OutTotal[i]:0.00000000E+00}";
                }
                yield return line;

                line = ",,";
            }

            yield return "";
            yield return "";
            yield return "";
        }
    }

    IEnumerable<(string NameIdacDose, string NameOIR, int iS)> SortSourceRegionsByIdacDoseOrder(IEnumerable<string> sourceRegions)
    {
        var regions = sourceRegions.ToArray();

        (string, string, int) Get(string nameIdacDose, string nameOIR = null)
        {
            nameIdacDose = $"\"{nameIdacDose}\"";
            if (nameOIR is null)
                return (nameIdacDose, "", -1);
            else
                return (nameIdacDose, nameOIR, Array.IndexOf(regions, nameOIR));
        }

        yield return Get("Adipose/residual tissue",                      /**/ "Adipose");
        yield return Get("Adrenals",                                     /**/ "Adrenals");
        yield return Get("Alveolar-interstitial",                        /**/ "ALV");
        yield return Get("Blood",                                        /**/ "Blood");
        yield return Get("Brain",                                        /**/ "Brain");
        yield return Get("Breast",                                       /**/ "Breast");
        yield return Get("Bronchi",                                      /**/ "Bronchi");
        yield return Get("Bronchi bound",                                /**/ "Bronchi-b");
        yield return Get("Bronchi sequestered",                          /**/ "Bronchi-q");
        yield return Get("Bronchioles",                                  /**/ "Brchiole");
        yield return Get("Bronchioles bound",                            /**/ "Brchiole-b");
        yield return Get("Bronchioles sequestered",                      /**/ "Brchiole-q");
        yield return Get("Cartilage",                                    /**/ "Cartilage");
        yield return Get("Cortical bone marrow",                         /**/ "C-marrow");
        yield return Get("Cortical bone mineral, surface",               /**/ "C-bone-S");
        yield return Get("Cortical bone mineral, volume",                /**/ "C-bone-V");
        yield return Get("ET1 Surface of anterior nasal passages",       /**/ "ET1-sur");
        yield return Get("ET1 Surface of posterior nasal passages wall", /**/ "ET1-wall");
        yield return Get("ET2 region Bound",                             /**/ "ET2-bnd");
        yield return Get("ET2 region Sequestered",                       /**/ "ET2-seq");
        yield return Get("ET2 region Surface",                           /**/ "ET2-sur");
        yield return Get("ET2 region wall",                              /**/ "ET2-wall");
        yield return Get("Eye lenses",                                   /**/ "Eye-lens");
        yield return Get("Gallbladder contents",                         /**/ "GB-cont");
        yield return Get("Gallbladder wall",                             /**/ "GB-wall");
        yield return Get("Heart wall",                                   /**/ "Ht-wall");
        yield return Get("Kidneys",                                      /**/ "Kidneys");
        yield return Get("Left colon contents",                          /**/ "LC-cont");
        yield return Get("Left colon mucosa",                            /**/ "LC-mucosa");
        yield return Get("Left colon wall",                              /**/ "LC-wall");
        yield return Get("Liver",                                        /**/ "Liver");
        yield return Get("Lung tissue",                                  /**/ "Lung-Tis");
        yield return Get("Lungs",                                        /**/ "Lungs");
        yield return Get("Lymph nodes in ET region",                     /**/ "LN-ET");
        yield return Get("Lymph nodes in sys",                           /**/ "LN-Sys");
        yield return Get("Lymph nodes in thoracic region",               /**/ "LN-Th");
        yield return Get("Lymph nodes total");
        yield return Get("Muscle",                                       /**/ "Muscle");
        yield return Get("Oesophagus fast",                              /**/ "Oesophag-f");
        yield return Get("Oesophagus slow",                              /**/ "Oesophag-s");
        yield return Get("Oesophagus wall",                              /**/ "Oesophag-w");
        yield return Get("Oral cavity",                                  /**/ "O-cavity");
        yield return Get("Oral mucosa",                                  /**/ "O-mucosa");
        yield return Get("Other");
        yield return Get("Ovaries",                                      /**/ "Ovaries");
        yield return Get("Pancreas",                                     /**/ "Pancreas");
        yield return Get("Pituitary gland",                              /**/ "P-gland");
        yield return Get("Prostate",                                     /**/ "Prostate");
        yield return Get("Recto-sigmoid colon content",                  /**/ "RS-cont");
        yield return Get("Recto-sigmoid colon mucosa",                   /**/ "RS-mucosa");
        yield return Get("Recto-sigmoid colon wall",                     /**/ "RS-wall");
        yield return Get("Red (active) bone marrow",                     /**/ "R-marrow");
        yield return Get("Respiratory tract air",                        /**/ "RT-air");
        yield return Get("Right colon contents",                         /**/ "RC-cont");
        yield return Get("Right colon mucosa",                           /**/ "RC-mucosa");
        yield return Get("Right colon wall",                             /**/ "RC-wall");
        yield return Get("Salivary glands",                              /**/ "S-glands");
        yield return Get("Skin",                                         /**/ "Skin");
        yield return Get("Small intestine contents",                     /**/ "SI-cont");
        yield return Get("Small intestine mucosa",                       /**/ "SI-mucosa");
        yield return Get("Small intestine wall",                         /**/ "SI-wall");
        yield return Get("Small intestine villi",                        /**/ "SI-villi");
        yield return Get("Spleen",                                       /**/ "Spleen");
        yield return Get("Stomach contents",                             /**/ "St-cont");
        yield return Get("Stomach mucosa",                               /**/ "St-mucosa");
        yield return Get("Stomach wall",                                 /**/ "St-wall");
        yield return Get("Teeth surface activity",                       /**/ "Teeth-S");
        yield return Get("Teeth volume activity",                        /**/ "Teeth-V");
        yield return Get("Testes",                                       /**/ "Testes");
        yield return Get("Thymus",                                       /**/ "Thymus");
        yield return Get("Thyroid",                                      /**/ "Thyroid");
        yield return Get("Tongue",                                       /**/ "Tongue");
        yield return Get("Tonsils",                                      /**/ "Tonsils");
        yield return Get("Total body");
        yield return Get("Total body tissues excl. content");
        yield return Get("Trabecular bone marrow",                       /**/ "T-marrow");
        yield return Get("Trabecular bone mineral, surface",             /**/ "T-bone-S");
        yield return Get("Trabecular bone mineral, volume",              /**/ "T-bone-V");
        yield return Get("Ureters",                                      /**/ "Ureters");
        yield return Get("Urinary bladder contents",                     /**/ "UB-cont");
        yield return Get("Urinary bladder wall",                         /**/ "UB-wall");
        yield return Get("Uterus/cervix",                                /**/ "Uterus");
        yield return Get("Yellow (inactive) bone marrow",                /**/ "Y-marrow");
    }

    IEnumerable<(string NameIdacDose, string NameOIR, int iS)> SortTargetRegionsByIdacDoseOrder(IEnumerable<string> targetRegions)
    {
        var regions = targetRegions.ToArray();

        (string, string, int) Get(string nameIdacDose, string nameOIR = null)
        {
            nameIdacDose = $"\"{nameIdacDose}\"";
            if (nameOIR is null)
                return (nameIdacDose, "", -1);
            else
                return (nameIdacDose, nameOIR, Array.IndexOf(regions, nameOIR));
        }

        yield return Get("Adipose/residual tissue",        /**/ "Adipose");
        yield return Get("Adrenals",                       /**/ "Adrenals");
        yield return Get("Alveolar-interstitial",          /**/ "AI");
        yield return Get("Brain",                          /**/ "Brain");
        yield return Get("Breast",                         /**/ "Breast");
        yield return Get("Bronchi bound",                  /**/ "Bronch-bas");
        yield return Get("Bronchi sequestered",            /**/ "Bronch-sec");
        yield return Get("Bronchioles",                    /**/ "Bchiol-sec");
        yield return Get("Colon wall");
        yield return Get("Endosteum (bone surface)",       /**/ "Endost-BS");
        yield return Get("ET region");
        yield return Get("ET1 basal cells",                /**/ "ET1-bas");
        yield return Get("ET2 basal cells",                /**/ "ET2-bas");
        yield return Get("Eye lenses",                     /**/ "Eye-lens");
        yield return Get("Gallbladder wall",               /**/ "GB-wall");
        yield return Get("Heart wall",                     /**/ "Ht-wall");
        yield return Get("Kidneys",                        /**/ "Kidneys");
        yield return Get("Left colon wall",                /**/ "LC-stem");
        yield return Get("Liver",                          /**/ "Liver");
        yield return Get("Lung");
        yield return Get("Lymphatic nodes");
        yield return Get("Lymph nodes in ET region",       /**/ "LN-ET");
        yield return Get("Lymph nodes in sys",             /**/ "LN-Sys");
        yield return Get("Lymph nodes in thoracic region", /**/ "LN-Th");
        yield return Get("Muscle",                         /**/ "Muscle");
        yield return Get("Oesophagus",                     /**/ "Oesophagus");
        yield return Get("Oral mucosa",                    /**/ "O-mucosa");
        yield return Get("Ovaries",                        /**/ "Ovaries");
        yield return Get("Pancreas",                       /**/ "Pancreas");
        yield return Get("Pituitary gland",                /**/ "P-gland");
        yield return Get("Prostate",                       /**/ "Prostate");
        yield return Get("Recto-sigmoid colon wall",       /**/ "RS-stem");
        yield return Get("Red (active) bone marrow",       /**/ "R-marrow");
        yield return Get("Right colon wall",               /**/ "RC-stem");
        yield return Get("Salivary glands",                /**/ "S-glands");
        yield return Get("Skin",                           /**/ "Skin");
        yield return Get("Small intestine wall",           /**/ "SI-stem");
        yield return Get("Spleen",                         /**/ "Spleen");
        yield return Get("Stomach wall",                   /**/ "St-stem");
        yield return Get("Testes",                         /**/ "Testes");
        yield return Get("Thymus",                         /**/ "Thymus");
        yield return Get("Thyroid",                        /**/ "Thyroid");
        yield return Get("Tongue",                         /**/ "Tongue");
        yield return Get("Tonsils",                        /**/ "Tonsils");
        yield return Get("Ureters",                        /**/ "Ureters");
        yield return Get("Urinary bladder wall",           /**/ "UB-wall");
        yield return Get("Uterus/cervix",                  /**/ "Uterus");
    }
}
