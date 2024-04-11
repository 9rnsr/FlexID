using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace S_Coefficient
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
        public List<string> alpha = new List<string>();
        public List<string> photon = new List<string>();
        public List<string> electron = new List<string>();

        // 中性子はSAFを持つ核種と持たない核種がある
        public string[] neutronNuclideNames;        // 中性子SAFを持つ核種名の配列
        public string[] neutronRadiationWeights;    // 中性子SAFを持つ核種毎の放射線荷重係数(W_R)
        public List<string> neutron = new List<string>();

        public bool Completion = false; // SAFデータの取得が正常に終了したかの判定
    }

    /// <summary>
    /// 各種データファイルからデータを読み出す処理を担うクラス
    /// </summary>
    public class DataReader
    {
        private const string RadFilePath = @"lib\ICRP-07.RAD";
        private const string BetFilePath = @"lib\ICRP-07.BET";

        /// <summary>
        /// 放射線データ取得
        /// </summary>
        /// <param name="nuclideName">取得対象の核種名</param>
        /// <returns>取得した放射線データ</returns>
        public static string[] ReadRAD(string nuclideName)
        {
            using (var r = new StreamReader(RadFilePath))
            {
                string line;        // review:処理対象はRADファイルと判っているので、名前にRADと付ける必要はない
                while ((line = r.ReadLine()) != null)
                {
                    string[] fields = line.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);
                    if (fields[0] != nuclideName)
                        continue;   // review:メインの処理のインデントが深くなるのを避けるため、早期のcontinueを使う

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
                string line;        // review:処理対象はRADファイルと判っているので、名前にBETと付ける必要はない
                while ((line = r.ReadLine()) != null)
                {
                    string[] fields = line.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);
                    if (fields[0] != nuclideName)
                        continue;   // review:メインの処理のインデントが深くなるのを避けるため、早期のcontinueを使う

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
            var data = new SAFData();
            string alphaFilePath;
            string photonFilePath;
            string electronFilePath;
            string neutronFilePath;
            string current = Environment.CurrentDirectory;
            if (sex == Sex.Male)
            {
                var file = Directory.GetFiles(Path.Combine(current, "lib"), @"rcp-am_alpha_????-??-??.SAF").Where(x => x.Contains("rcp-am_alpha_")).ToList();
                if (file.Count == 1)
                    alphaFilePath = file[0];
                else
                    return data;
                file = Directory.GetFiles(Path.Combine(current, "lib"), @"rcp-am_photon_????-??-??.SAF").Where(x => x.Contains("rcp-am_photon_")).ToList();
                if (file.Count == 1)
                    photonFilePath = file[0];
                else
                    return data;
                file = Directory.GetFiles(Path.Combine(current, "lib"), @"rcp-am_electron_????-??-??.SAF").Where(x => x.Contains("rcp-am_electron_")).ToList();
                if (file.Count == 1)
                    electronFilePath = file[0];
                else
                    return data;
                file = Directory.GetFiles(Path.Combine(current, "lib"), @"rcp-am_neutron_????-??-??.SAF").Where(x => x.Contains("rcp-am_neutron_")).ToList();
                if (file.Count == 1)
                    neutronFilePath = file[0];
                else
                    return data;
            }
            else
            {
                var file = Directory.GetFiles(Path.Combine(current, "lib"), @"rcp-af_alpha_????-??-??.SAF").Where(x => x.Contains("rcp-af_alpha_")).ToList();
                if (file.Count == 1)
                    alphaFilePath = file[0];
                else
                    return data;
                file = Directory.GetFiles(Path.Combine(current, "lib"), @"rcp-af_photon_????-??-??.SAF").Where(x => x.Contains("rcp-af_photon_")).ToList();
                if (file.Count == 1)
                    photonFilePath = file[0];
                else
                    return data;
                file = Directory.GetFiles(Path.Combine(current, "lib"), @"rcp-af_electron_????-??-??.SAF").Where(x => x.Contains("rcp-af_electron_")).ToList();
                if (file.Count == 1)
                    electronFilePath = file[0];
                else
                    return data;
                file = Directory.GetFiles(Path.Combine(current, "lib"), @"rcp-af_neutron_????-??-??.SAF").Where(x => x.Contains("rcp-af_neutron_")).ToList();
                if (file.Count == 1)
                    neutronFilePath = file[0];
                else
                    return data;
            }

            // α
            using (var r = new StreamReader(alphaFilePath))
            {
                string line;

                r.ReadLine();
                r.ReadLine();
                r.ReadLine();
                r.ReadLine();
                r.ReadLine();
                while ((line = r.ReadLine()) != null)
                    data.alpha.Add(line);
            }

            // 光子
            using (var r = new StreamReader(photonFilePath))
            {
                string line;

                r.ReadLine();
                r.ReadLine();
                r.ReadLine();
                r.ReadLine();
                r.ReadLine();
                while ((line = r.ReadLine()) != null)
                    data.photon.Add(line);
            }

            // 電子
            using (var r = new StreamReader(electronFilePath))
            {
                string line;

                r.ReadLine();
                r.ReadLine();
                r.ReadLine();
                r.ReadLine();
                r.ReadLine();
                while ((line = r.ReadLine()) != null)
                    data.electron.Add(line);
            }

            // 中性子
            using (var r = new StreamReader(neutronFilePath))
            {
                string line;

                r.ReadLine();
                r.ReadLine();

                line = r.ReadLine();
                data.neutronNuclideNames = line.Split(new string[] { "<-", " " }, StringSplitOptions.RemoveEmptyEntries);
                data.neutronNuclideNames = data.neutronNuclideNames.Skip(1).ToArray();          // 不要な列を除去

                line = r.ReadLine();
                data.neutronRadiationWeights = line.Split(new string[] { "<-", " " }, StringSplitOptions.RemoveEmptyEntries);
                data.neutronRadiationWeights = data.neutronRadiationWeights.Skip(4).ToArray();  // 不要な列を除去

                // SAFを持つ核種の名前と、放射線加重係数の数は必ず一致する
                Debug.Assert(data.neutronNuclideNames.Length == data.neutronRadiationWeights.Length);

                r.ReadLine();

                while ((line = r.ReadLine()) != null)
                    data.neutron.Add(line);
            }
            data.Completion = true;
            return data;
        }
    }
}
