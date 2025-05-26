namespace FlexID.Calc;

using System.Globalization;

/// <summary>
/// インデックスデータを表現する。
/// </summary>
public class IndexData
{
    /// <summary>
    /// 親核種。
    /// </summary>
    public string Nuclide { get; set; }

    /// <summary>
    /// 半減期(単位付き)。
    /// </summary>
    public string HalfLife { get; set; }

    /// <summary>
    /// 半減期[day]。
    /// </summary>
    public decimal HalfLifeDay { get; set; }

    /// <summary>
    /// 崩壊定数λ[/day]。(＝ ln(2) / 半減期[day])
    /// </summary>
    public double Lambda => Math.Log(2) / (double)HalfLifeDay;

    public string DecayModes { get; set; }

    public int LocationRAD { get; set; }
    public int LocationBET { get; set; }
    public int LocationACK { get; set; }
    public int LocationNSF { get; set; }

    public IndexDaughterData[] Daughters { get; set; }

    public decimal EnergyAlpha { get; internal set; }
    public decimal EnergyElectron { get; internal set; }
    public decimal EnergyPhoton { get; internal set; }

    public int Number1 { get; internal set; }
    public int Number2 { get; internal set; }
    public int Number3 { get; internal set; }
    public int Number4 { get; internal set; }
    public int Number5 { get; internal set; }

    public decimal AMU { get; internal set; }
    public decimal AirKerma { get; internal set; }
    public decimal PointSourceAirKerma { get; internal set; }
}

/// <summary>
/// インデックスデータの娘核種を表現する。
/// </summary>
public struct IndexDaughterData
{
    /// <summary>
    /// 娘核種。
    /// </summary>
    public string Daughter { get; set; }

    /// <summary>
    /// NDXファイルにおける行位置。
    /// </summary>
    public int LocationNDX { get; set; }

    /// <summary>
    /// 親核種からの分岐比。
    /// </summary>
    public decimal Branch { get; set; }
}

/// <summary>
/// インデックスファイルICRP-07.NDXからデータを読み出す。
/// </summary>
public class IndexDataReader
{
    private const string IndexFilePath = @"lib\ICRP-07.NDX";

    public static IEnumerable<IndexData> ReadNDX()
    {
        using (var r = new StreamReader(IndexFilePath))
        {
            string line;
            line = r.ReadLine();
            //var first = int.Parse(line.Substring(0, 4));
            //var last = int.Parse(line.Substring(4, 8));

            var daughters = new List<IndexDaughterData>();

            while ((line = r.ReadLine()) != null)
            {
                int icol = 0;
                string GetColumn(int w)
                {
                    var res = line.Substring(icol, w);
                    icol += w;
                    return res;
                }

                var nuc        /**/= GetColumn(7);
                var halfLife   /**/= GetColumn(8);
                var units      /**/= GetColumn(2).Trim();
                var decayModes /**/= GetColumn(8);
                //Debug.WriteLine($"{nuc}, {halflife}, {units}, {decayModes}");

                var halfLifeValue = decimal.Parse(halfLife, NumberStyles.Float);
                var halfLifeDay =
                    units == "y" ? halfLifeValue * 365m :
                    units == "d" ? halfLifeValue :
                    units == "h" ? halfLifeValue / 24m :
                    units == "m" ? halfLifeValue / 1440m :
                    units == "s" ? halfLifeValue / 8.64E+04m :
                    units == "ms" ? halfLifeValue / 8.64E+07m :
                    units == "us" ? halfLifeValue / 8.64E+10m :
                    throw new FormatException("Unrecognized unit of Half-Life value: {halfLifeUnit}");

                var pointer1 = int.Parse(GetColumn(7));
                var pointer2 = int.Parse(GetColumn(7));
                var pointer3 = int.Parse(GetColumn(7));
                var pointer4 = int.Parse(GetColumn(6));
                //Debug.WriteLine($"  {pointer1}, {pointer2}, {pointer3}, {pointer4}");

                daughters.Clear();
                for (int i = 0; i < 4; i++)
                {
                    ++icol; // 1文字分の余白がある。
                    var daughter_i /**/= GetColumn(7);
                    var pointer_i  /**/= int.Parse(GetColumn(6));
                    var branch_i   /**/= decimal.Parse(GetColumn(11), NumberStyles.Float);
                    //Debug.WriteLine($"  {daughter_i}, {pointer_i}, {branch_i}");

                    daughters.Add(new IndexDaughterData
                    {
                        Daughter    /**/= daughter_i.Trim(),
                        LocationNDX /**/= 0,//pointer_i,
                        Branch      /**/= branch_i,
                    });
                }

                var E_alpha    /**/= decimal.Parse(GetColumn(7), NumberStyles.Float);
                var E_electron /**/= decimal.Parse(GetColumn(8), NumberStyles.Float);
                var E_photon   /**/= decimal.Parse(GetColumn(8), NumberStyles.Float);
                //Debug.WriteLine($"  {E_alpha}, {E_electron}, {E_photon}");

                var number1 = int.Parse(GetColumn(4));
                var number2 = int.Parse(GetColumn(4));
                var number3 = int.Parse(GetColumn(4));
                var number4 = int.Parse(GetColumn(5));
                var number5 = int.Parse(GetColumn(4));
                //Debug.WriteLine($"  {number1}, {number2}, {number3}, {number4} {number5}");

                var amu = decimal.Parse(GetColumn(11), NumberStyles.Float);
                var airKerma = decimal.Parse(GetColumn(10), NumberStyles.Float);
                var pointSourceAirKerma = decimal.Parse(GetColumn(9), NumberStyles.Float);
                //Debug.WriteLine($"  {amu}, {airKerma}, {pointSourceAirKerma}");

                yield return new IndexData
                {
                    Nuclide = nuc.Trim(),
                    HalfLife = halfLife + units,
                    HalfLifeDay = halfLifeDay,
                    DecayModes = decayModes,

                    LocationRAD = pointer1,
                    LocationBET = pointer2,
                    LocationACK = pointer3,
                    LocationNSF = pointer4,

                    Daughters = daughters.Where(d => d.Daughter != "").ToArray(),

                    EnergyAlpha = E_alpha,
                    EnergyElectron = E_electron,
                    EnergyPhoton = E_photon,

                    Number1 = number1,
                    Number2 = number2,
                    Number3 = number3,
                    Number4 = number4,
                    Number5 = number5,

                    AMU = amu,
                    AirKerma = airKerma,
                    PointSourceAirKerma = pointSourceAirKerma
                };
            }
        }
    }
}
