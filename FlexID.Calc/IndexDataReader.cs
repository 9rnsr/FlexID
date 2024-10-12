using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

                while ((line = r.ReadLine()) != null)
                {
                    int icol = 0;
                    string GetColumn(int w) => line.Substring(icol, w);

                    var nuc       /**/= GetColumn(7);
                    var halflife  /**/= GetColumn(8);
                    var units     /**/= GetColumn(2);
                    var decayMode /**/= GetColumn(8);

#if false
                    var pointer1 = int.Parse(GetColumn(7));
                    var pointer2 = int.Parse(GetColumn(7));
                    var pointer3 = int.Parse(GetColumn(7));
                    var pointer4 = int.Parse(GetColumn(6));

                    for (int i = 0; i < 4; i++)
                    {
                        var daughter_i /**/= GetColumn(7);
                        var pointer_i  /**/= GetColumn(7);  // Publ.107ではI6とあるがI7でないと実データと合わない。
                        var branch_i   /**/= double.Parse(GetColumn(11));
                    }

                    var E_alpha    /**/= double.Parse(GetColumn(7));
                    var E_electron /**/= double.Parse(GetColumn(8));
                    var E_photon   /**/= double.Parse(GetColumn(8));

                    var number1 = int.Parse(GetColumn(4));
                    var number2 = int.Parse(GetColumn(4));
                    var number3 = int.Parse(GetColumn(4));
                    var number4 = int.Parse(GetColumn(5));
                    var number5 = int.Parse(GetColumn(4));

                    var amu = double.Parse(GetColumn(11));
                    var airKerma = double.Parse(GetColumn(10));
                    var pointSourceAirKerma = double.Parse(GetColumn(9));
#endif

                    yield return new IndexData
                    {
                        Nuclide = nuc,
                        HalfLife = halflife,
                        HalfLifeUnit = units,
                    };
                }
            }
        }
    }
}
