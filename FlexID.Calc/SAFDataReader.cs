using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace FlexID.Calc
{
    /// <summary>
    /// 性別を表現する列挙型
    /// </summary>
    public enum Sex { Male, Female }

    /// <summary>
    /// SAFデータを表現するクラス
    /// </summary>
    public class SAFData
    {
        public int Count;

        public double[] EnergyA;
        public double[] EnergyP;
        public double[] EnergyE;

        public double[][] SAFalpha;
        public double[][] SAFphoton;
        public double[][] SAFelectron;

        /// <summary>
        /// α粒子の放射線加重係数。
        /// </summary>
        public readonly double WRalpha = 20.0;

        /// <summary>
        /// 光子の放射線加重係数。
        /// </summary>
        public readonly double WRphoton = 1.0;

        /// <summary>
        /// 電子の放射線加重係数。
        /// </summary>
        public readonly double WRelectron = 1.0;

        /// <summary>
        /// 中性子の放射線加重係数。SAFデータが定義された核種名をキーにして、
        /// 当該核種の中性子スペクトル平均であるW_Rと中性子SAFの配列の組を値とした辞書とする。
        /// </summary>
        public Dictionary<string, (double WR, double[] SAFs)> SAFneutron;
    }

    /// <summary>
    /// 各種データファイルからデータを読み出す処理を担うクラス
    /// </summary>
    public class SAFDataReader
    {
        private const string RadFilePath = @"lib\ICRP-07.RAD";
        private const string BetFilePath = @"lib\ICRP-07.BET";

        /// <summary>
        /// 放射線データに定義されている核種を列挙する。
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<string> ReadRadNuclides()
        {
            using (var r = new StreamReader(RadFilePath))
            {
                string line;
                while ((line = r.ReadLine()) != null)
                {
                    string[] fields = line.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);
                    yield return fields[0];

                    var dataCount = int.Parse(fields[2]);
                    for (int dataNo = 0; dataNo < dataCount; dataNo++)
                        r.ReadLine();
                }
            }
        }

        /// <summary>
        /// 放射線データ取得
        /// </summary>
        /// <param name="nuclideName">取得対象の核種名</param>
        /// <returns>取得した放射線データ</returns>
        public static string[] ReadRAD(string nuclideName)
        {
            using (var r = new StreamReader(RadFilePath))
            {
                string line;
                while ((line = r.ReadLine()) != null)
                {
                    string[] fields = line.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);
                    if (fields[0] != nuclideName)
                        continue;

                    var dataCount = int.Parse(fields[2]);
                    var data = new string[dataCount];

                    for (int dataNo = 0; dataNo < dataCount; dataNo++)
                        data[dataNo] = r.ReadLine();

                    return data;
                }
            }

            // 開いたファイルにnuclideNameが見つからなかったなどの問題があった場合はここに来る
            // todo: エラー処理について検討する
            return null;
        }

        /// <summary>
        /// βスペクトル取得
        /// </summary>
        /// <param name="nuclideName">取得対象の核種名</param>
        /// <returns>取得したβスペクトルデータ</returns>
        public static string[] ReadBET(string nuclideName)
        {
            using (var r = new StreamReader(BetFilePath))
            {
                string line;
                while ((line = r.ReadLine()) != null)
                {
                    string[] fields = line.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);
                    if (fields[0] != nuclideName)
                        continue;

                    var dataCount = int.Parse(fields[1]);
                    var data = new string[dataCount];

                    for (int dataNo = 0; dataNo < dataCount; dataNo++)
                        data[dataNo] = r.ReadLine();

                    return data;
                }
            }

            // 開いたファイルにnuclideNameが見つからなかったなどの問題があった場合はここに来る
            // todo: エラー処理について検討する
            return null;
        }

        /// <summary>
        /// SAFデータ取得
        /// </summary>
        /// <param name="sex">取得対象のデータの性別</param>
        /// <returns>取得したSAFデータ</returns>
        public static SAFData ReadSAF(Sex sex)
        {
            var libDir = Path.Combine(Environment.CurrentDirectory, "lib");

            string GetSingleFile(string pattern)
            {
                var files = Directory.GetFiles(libDir, pattern);
                return files.Length == 1 ? files[0] : null;
            }

            var amaf = sex == Sex.Male ? "am" : "af";
            var alphaFilePath    /**/= GetSingleFile($"rcp-{amaf}_alpha_????-??-??.SAF");
            var photonFilePath   /**/= GetSingleFile($"rcp-{amaf}_photon_????-??-??.SAF");
            var electronFilePath /**/= GetSingleFile($"rcp-{amaf}_electron_????-??-??.SAF");
            var neutronFilePath  /**/= GetSingleFile($"rcp-{amaf}_neutron_????-??-??.SAF");

            var data = new SAFData();

            if (alphaFilePath is null || photonFilePath is null ||
                electronFilePath is null || neutronFilePath is null)
            {
                return null;
            }

            // α
            ReadAlphaSAF(data, alphaFilePath, out var nTA, out var nSA);

            // 光子
            ReadPhotonSAF(data, photonFilePath, out var nTP, out var nSP);

            // 電子
            ReadElectronSAF(data, electronFilePath, out var nTE, out var nSE);

            // 中性子
            ReadNeutronSAF(data, neutronFilePath, out var nTN, out var nSN);

            if (new[] { nTA, nTP, nTE, nTN }.Distinct().Count() != 1)
                throw new InvalidDataException("Target tissue counts are different");
            if (new[] { nSA, nSP, nSE, nSN }.Distinct().Count() != 1)
                throw new InvalidDataException("Source region counts are different");

            data.Count = nTA * nSA;

            return data;
        }

        private static (int numT, int numS, double[] Energies) GetHeader(string line)
        {
            var parts = line.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            var numT = int.Parse(parts[0]);
            var numS = int.Parse(parts[1]);
            var energies = new List<double>(parts.Length - 2);
            for (int i = 2; i < parts.Length; i++)
            {
                if (!double.TryParse(parts[i], out var erg))
                {
                    if (energies.Count == 0)
                        continue;
                    else
                        break;
                }
                energies.Add(erg);
            }
            return (numT, numS, energies.ToArray());
        }

        private static void ReadAlphaSAF(SAFData data, string filePath, out int nT, out int nS)
        {
            using (var reader = new StreamReader(filePath))
            {
                string line;

                reader.ReadLine();
                reader.ReadLine();
                reader.ReadLine();

                line = reader.ReadLine();
                (nT, nS, data.EnergyA) = GetHeader(line);

                var nrows = nT * nS;
                var ncols = data.EnergyA.Length;

                data.SAFalpha = new double[nrows][];

                reader.ReadLine();
                for (int r = 0; r < nrows; r++)
                {
                    if ((line = reader.ReadLine()) is null)
                        throw new InvalidDataException(filePath);

                    var parts = line.Split(new string[] { "<-", " " }, StringSplitOptions.RemoveEmptyEntries);
                    if (parts.Length != ncols + 4)
                        throw new InvalidDataException(filePath);

                    var values = parts.Skip(2).Take(ncols).Select(v => double.Parse(v));
                    data.SAFalpha[r] = values.ToArray();
                }
            }
        }

        private static void ReadPhotonSAF(SAFData data, string filePath, out int nT, out int nS)
        {
            using (var reader = new StreamReader(filePath))
            {
                string line;

                reader.ReadLine();
                reader.ReadLine();
                reader.ReadLine();

                line = reader.ReadLine();
                (nT, nS, data.EnergyP) = GetHeader(line);

                var nrows = nT * nS;
                var ncols = data.EnergyP.Length;

                data.SAFphoton = new double[nrows][];

                reader.ReadLine();
                for (int r = 0; r < nrows; r++)
                {
                    if ((line = reader.ReadLine()) is null)
                        throw new InvalidDataException(filePath);

                    var parts = line.Split(new string[] { "<-", " " }, StringSplitOptions.RemoveEmptyEntries);
                    if (parts.Length != ncols + 4)
                        throw new InvalidDataException(filePath);

                    var values = parts.Skip(2).Take(ncols).Select(v => double.Parse(v));
                    data.SAFphoton[r] = values.ToArray();
                }
            }
        }

        private static void ReadElectronSAF(SAFData data, string filePath, out int nT, out int nS)
        {
            using (var reader = new StreamReader(filePath))
            {
                string line;

                reader.ReadLine();
                reader.ReadLine();
                reader.ReadLine();

                line = reader.ReadLine();
                (nT, nS, data.EnergyE) = GetHeader(line);

                var nrows = nT * nS;
                var ncols = data.EnergyE.Length;

                data.SAFelectron = new double[nrows][];

                reader.ReadLine();
                for (int r = 0; r < nrows; r++)
                {
                    if ((line = reader.ReadLine()) is null)
                        throw new InvalidDataException(filePath);

                    var parts = line.Split(new string[] { "<-", " " }, StringSplitOptions.RemoveEmptyEntries);
                    if (parts.Length != ncols + 4)
                        throw new InvalidDataException(filePath);

                    var values = parts.Skip(2).Take(ncols).Select(v => double.Parse(v));
                    data.SAFelectron[r] = values.ToArray();
                }
            }
        }

        private static void ReadNeutronSAF(SAFData data, string filePath, out int nT, out int nS)
        {
            using (var reader = new StreamReader(filePath))
            {
                string line;

                reader.ReadLine();
                reader.ReadLine();

                line = reader.ReadLine();
                var nuclides = line
                    .Split(new string[] { "<-", " " }, StringSplitOptions.RemoveEmptyEntries)
                    .Skip(1).ToArray();     // 不要な列を除去

                line = reader.ReadLine();
                double[] radiationWeights;
                (nT, nS, radiationWeights) = GetHeader(line);

                // SAFを持つ核種の名前と、放射線加重係数の数は必ず一致する
                if (nuclides.Length != radiationWeights.Length)
                    throw new InvalidDataException(filePath);

                var nrows = nT * nS;
                var ncols = nuclides.Length;

                var SAFs = new double[ncols][];
                data.SAFneutron = new Dictionary<string, (double, double[])>();
                for (int c = 0; c < ncols; c++)
                {
                    SAFs[c] = new double[nrows];
                    data.SAFneutron.Add(nuclides[c], (radiationWeights[c], SAFs[c]));
                }

                reader.ReadLine();
                for (int r = 0; r < nrows; r++)
                {
                    if ((line = reader.ReadLine()) is null)
                        throw new InvalidDataException(filePath);

                    var parts = line.Split(new string[] { "<-", " " }, StringSplitOptions.RemoveEmptyEntries);
                    if (parts.Length != ncols + 2)
                        throw new InvalidDataException(filePath);

                    var values = parts.Skip(2).Select(v => double.Parse(v)).ToArray();

                    for (int c = 0; c < ncols; c++)
                    {
                        SAFs[c][r] = values[c];
                    }
                }
            }
        }
    }
}
