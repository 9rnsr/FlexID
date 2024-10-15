using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace FlexID.Calc
{
    /// <summary>
    /// インデックスデータを表現するクラス
    /// </summary>
    public class IndexData
    {
        public string Nuclide { get; set; }

        public string HalfLife { get; set; }
        public string HalfLifeUnit { get; set; }

        private double HalfLifeDay => double.Parse(HalfLife) * (
            HalfLifeUnit == "y" ? 365.0 :
            HalfLifeUnit == "d" ? 1.0 :
            HalfLifeUnit == "m" ? 1.0 / (24.0 * 60.0) :
            HalfLifeUnit == "s" ? 1.0 / (24.0 * 60.0 * 60.0) :
            HalfLifeUnit == "ms" ? 1.0 / (24.0 * 60.0 * 60.0 * 1000.0) :
            HalfLifeUnit == "us" ? 1.0 / (24.0 * 60.0 * 60.0 * 1000.0 * 1000.0) :
            throw new NotSupportedException());

        public double Lambda => Math.Log(2) / HalfLifeDay;

        public string DecayModes { get; set; }

        public int LocationRAD { get; set; }
        public int LocationBET { get; set; }
        public int LocationACK { get; set; }
        public int LocationNSF { get; set; }

        public IndexDaughterData[] Daughters { get; set; }

        public double EnergyAlpha { get; internal set; }
        public double EnergyElectron { get; internal set; }
        public double EnergyPhoton { get; internal set; }

        public int Number1 { get; internal set; }
        public int Number2 { get; internal set; }
        public int Number3 { get; internal set; }
        public int Number4 { get; internal set; }
        public int Number5 { get; internal set; }

        public double AMU { get; internal set; }
        public double AirKerma { get; internal set; }
        public double PointSourceAirKerma { get; internal set; }
    }

    public struct IndexDaughterData
    {
        public string Daughter { get; set; }

        public int LocationNDX { get; set; }

        public double Branch { get; set; }
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
                    var halflife   /**/= GetColumn(8);
                    var units      /**/= GetColumn(2);
                    var decayModes /**/= GetColumn(8);
                    //Debug.WriteLine($"{nuc}, {halflife}, {units}, {decayModes}");

#if true//false
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
                        var branch_i   /**/= double.Parse(GetColumn(11));
                        //Debug.WriteLine($"  {daughter_i}, {pointer_i}, {branch_i}");

                        daughters.Add(new IndexDaughterData
                        {
                            Daughter    /**/= daughter_i.Trim(),
                            LocationNDX /**/= 0,//pointer_i,
                            Branch      /**/= branch_i,
                        });
                    }

                    var E_alpha    /**/= double.Parse(GetColumn(7));
                    var E_electron /**/= double.Parse(GetColumn(8));
                    var E_photon   /**/= double.Parse(GetColumn(8));
                    //Debug.WriteLine($"  {E_alpha}, {E_electron}, {E_photon}");

                    var number1 = int.Parse(GetColumn(4));
                    var number2 = int.Parse(GetColumn(4));
                    var number3 = int.Parse(GetColumn(4));
                    var number4 = int.Parse(GetColumn(5));
                    var number5 = int.Parse(GetColumn(4));
                    //Debug.WriteLine($"  {number1}, {number2}, {number3}, {number4} {number5}");

                    var amu = double.Parse(GetColumn(11));
                    var airKerma = double.Parse(GetColumn(10));
                    var pointSourceAirKerma = double.Parse(GetColumn(9));
                    //Debug.WriteLine($"  {amu}, {airKerma}, {pointSourceAirKerma}");
#endif

                    yield return new IndexData
                    {
                        Nuclide = nuc.Trim(),
                        HalfLife = halflife,
                        HalfLifeUnit = units.Trim(),
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
}
