using MathNet.Numerics.Interpolation;
using System;
using System.Collections.Generic;

namespace S_Coefficient
{
    public class CalcSfactor
    {
        // 光子と電子のエネルギービン
        private readonly double[] PEenergy = new double[]
        {
            0, 0.001, 0.005, 0.01, 0.015, 0.02, 0.03, 0.04, 0.05, 0.06, 0.08,
            0.1, 0.15, 0.2, 0.3, 0.4, 0.5, 0.6, 0.8, 1, 1.5, 2, 3, 4, 5, 6, 8, 10
        };

        // αのエネルギービン
        private readonly double[] Aenergy = new double[]
        {
            0, 1, 1.5, 2, 2.5, 3, 3.5, 4, 4.5, 5, 5.5, 6, 6.5,
            7, 7.5, 8, 8.5, 9, 9.5, 10, 10.5, 11, 11.5, 12
        };

        // 放射線加重係数
        // 中性子の放射線加重係数は、SAFファイルに記載されている中性子スペクトル平均であるW_Rを使う
        private const double WRalpha = 20.0;
        private const double WRphoton = 1.0;
        private const double WRelectron = 1.0;

        public string InterpolationMethod = "";

        // J/MeVへの変換
        private double ToJoule = 1.602176487E-13;

        /// <summary>
        /// 計算対象となる核種のリスト。
        /// </summary>
        public List<string> Nuclides { get; } = new List<string>();

        /// <summary>
        /// S-factor計算
        /// </summary>
        /// <param name="sex">計算対象の性別</param>
        public (string, string) CalcS(Sex sex)
        {
            // SAFデータ取得
            var safdata = DataReader.ReadSAF(sex);
            if (safdata.Completion == false)
            {
                return ("There are multiple files of the same type.", "Error");
            }

            foreach (var nuclideName in Nuclides)
            {
                var output = new Output();

                output.OutputExcelFilePath = $@"out\Adult{sex}\{nuclideName}_Adult{sex}.xlsx";
                output.OutputTextFilePath = $@"out\Adult{sex}\{nuclideName}_Adult{sex}.txt";

                CalcS(safdata, nuclideName, output);
            }
            return ("Finish.", "S-Coefficient");
        }

        public void CalcS(SAFData safdata, string nuclideName, Output output)
        {
            // 計算結果である、線源領域vs標的領域の組み合わせ毎のS係数値
            var OutTotal = new List<double>();
            var OutP = new List<double>();
            var OutE = new List<double>();
            var OutB = new List<double>();
            var OutA = new List<double>();
            var OutN = new List<double>();

            // 放射線データ取得
            var raddata = DataReader.ReadRAD(nuclideName);

            for (int TScount = 0; TScount < safdata.photon.Count; TScount++)
            {
                var SAFa = safdata.alpha[TScount].Split(new string[] { "<-", " " }, StringSplitOptions.RemoveEmptyEntries);
                var SAFp = safdata.photon[TScount].Split(new string[] { "<-", " " }, StringSplitOptions.RemoveEmptyEntries);
                var SAFe = safdata.electron[TScount].Split(new string[] { "<-", " " }, StringSplitOptions.RemoveEmptyEntries);

                // β線計算の終了判定フラグ
                bool finishBeta = false;

                // 放射線ごとのS係数計算値
                double SfacP = 0;
                double SfacE = 0;
                double SfacB = 0;
                double SfacA = 0;
                double SfacN = 0;

                // doubleに置き換える
                double[] SAFalpha = new double[SAFa.Length - 4];
                double[] SAFphaton = new double[SAFp.Length - 4];
                double[] SAFelectron = new double[SAFe.Length - 4];

                for (int i = 0; i < SAFa.Length - 4; i++)
                    SAFalpha[i] = double.Parse(SAFa[i + 2]);
                for (int i = 0; i < SAFp.Length - 4; i++)
                    SAFphaton[i] = double.Parse(SAFp[i + 2]);
                for (int i = 0; i < SAFe.Length - 4; i++)
                    SAFelectron[i] = double.Parse(SAFe[i + 2]);

                foreach (var rad in raddata)
                {
                    // エネルギー毎のSAF算出
                    string[] Rad = rad.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);

                    string icode = Rad[0];      // 放射線のタイプ
                    string yield = Rad[1];      // 放射線の収率(/nt)
                    string energy = Rad[2];     // 放射線のエネルギー(MeV)
                    string jcode = Rad[3];      // 放射線のタイプ

                    double Yi = double.Parse(yield);
                    double Ei = double.Parse(energy);

                    // X:X線、G:γ線、PG:遅発γ線、DG:即発γ線、AQ:消滅光子
                    if (jcode == "X" || jcode == "G" || jcode == "PG" || jcode == "DG" || jcode == "AQ")
                    {
                        if (InterpolationMethod == "PCHIP")
                        {
                            var pchip = CubicSpline.InterpolatePchip(PEenergy, SAFphaton);
                            SfacP += Yi * Ei * pchip.Interpolate(Ei) * WRphoton * ToJoule;
                        }
                        else if (InterpolationMethod == "線形補間")
                            SfacP += Yi * Ei * CalcSAF(Ei, PEenergy, SAFphaton) * WRphoton * ToJoule;
                    }
                    // IE: 内部転換電子、AE: オージェ電子
                    else if (jcode == "AE" || jcode == "IE")
                    {
                        if (InterpolationMethod == "PCHIP")
                        {
                            var pchip = CubicSpline.InterpolatePchip(PEenergy, SAFelectron);
                            SfacE += Yi * Ei * pchip.Interpolate(Ei) * WRelectron * ToJoule;
                        }
                        else if (InterpolationMethod == "線形補間")
                            SfacE += Yi * Ei * CalcSAF(Ei, PEenergy, SAFelectron) * WRelectron * ToJoule;
                    }
                    // β粒子(電子)、DB: 遅発β
                    else if (jcode == "B-" || jcode == "B+" || jcode == "DB")
                    {
                        if (finishBeta)
                            continue;

                        // RADファイルに定義されているエントリは単に無視し、BETファイルからS係数を計算する
                        SfacB = CalcBeta(nuclideName, SAFelectron) * ToJoule;

                        finishBeta = true;
                    }
                    // α粒子
                    else if (jcode == "A")
                    {
                        if (InterpolationMethod == "PCHIP")
                        {
                            var pchip = CubicSpline.InterpolatePchip(Aenergy, SAFalpha);
                            SfacA += Yi * Ei * pchip.Interpolate(Ei) * WRalpha * ToJoule;
                        }
                        else if (InterpolationMethod == "線形補間")
                            SfacA += Yi * Ei * CalcSAF(Ei, Aenergy, SAFalpha) * WRalpha * ToJoule;
                    }
                    // α反跳核、核分裂片
                    else if (jcode == "AR")
                    {
                        // 2MeVの値を取得
                        SfacA += SAFalpha[3] * Yi * Ei * WRalpha * ToJoule;
                    }
                    else if (jcode == "FF")
                    {
                        SfacN += SAFalpha[3] * Yi * Ei * WRalpha * ToJoule;
                    }
                    // 中性子
                    else if (jcode == "N")
                    {
                        // 放射線データに"N"を持つ＝中性子SAFが定義されていることとイコール
                        int nuclideNo = Array.IndexOf(safdata.neutronNuclideNames, nuclideName);

                        var parts = safdata.neutron[TScount]
                            .Split(new string[] { "<-", " " }, StringSplitOptions.RemoveEmptyEntries);

                        // nuclideNo + 2は、不要な列(線源領域と標的領域の名前の2列)を除去するために必要
                        var SAFn = double.Parse(parts[nuclideNo + 2]);

                        var WRneutron = double.Parse(safdata.neutronRadiationWeights[nuclideNo]);

                        SfacN += CalcNeutron(Yi, Ei, SAFn, WRneutron) * ToJoule;
                    }
                    else
                    {
                        // そのほかのJCODEについては処理しない
                    }
                }

                // 全ての放射線についてのS係数
                double Sfactor = SfacP + SfacE + SfacB + SfacA + SfacN;
                OutTotal.Add(Sfactor);
                OutP.Add(SfacP);
                OutE.Add(SfacE);
                OutB.Add(SfacB);
                OutA.Add(SfacA);
                OutN.Add(SfacN);
            }

            // 核種ごとの計算結果をExcelファイルに出力する
            output.WriteCalcResult(nuclideName, OutTotal, OutP, OutE, OutB, OutA, OutN);
        }

        /// <summary>
        /// 指定エネルギー点におけるSAFを算出する
        /// </summary>
        /// <param name="energy">放射線のエネルギー(MeV)</param>
        /// <param name="energyBin">放射線のエネルギーBinを定義した配列</param>
        /// <param name="SAF">放射線のエネルギーBin毎のSAF値</param>
        /// <returns>SAF値</returns>
        private double CalcSAF(double energy, double[] energyBin, double[] SAF)
        {
            for (int i = 0; i < energyBin.Length; i++)
            {
                if (energy == energyBin[i])
                {
                    // SAFを求めるエネルギー点がエネルギーBin境界上にある場合
                    return SAF[i];
                }
                if (energy < energyBin[i])
                {
                    // SAFを求めるエネルギー点がエネルギーBinの間にある場合
                    var ergBinL = energyBin[i - 1];
                    var ergBinH = energyBin[i];
                    var SAFBinL = SAF[i - 1];
                    var SAFBinH = SAF[i];

                    return SAFBinL + (energy - ergBinL) * (SAFBinH - SAFBinL) / (ergBinH - ergBinL);
                }
            }

            // エネルギービンを超えることはないはず
            throw new Exception("Assert(energyBin.Last < energy)");
        }

        /// <summary>
        /// βスペクトルS-factor計算
        /// </summary>
        /// <param name="nuclideName">計算対象の核種名</param>
        /// <param name="SAF">SAF値の配列</param>
        /// <returns></returns>
        private double CalcBeta(string nuclideName, double[] SAF)
        {
            // βスペクトル取得
            var betdata = DataReader.ReadBET(nuclideName);

            double beta = 0;
            for (int j = 1; j < betdata.Length; j++)
            {
                string[] bet1 = betdata[j].Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);
                string[] bet2 = betdata[j - 1].Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);

                double ergBinH = double.Parse(bet1[0]); // エネルギー幅の上限(MeV)
                double numParH = double.Parse(bet1[1]); // 上限側のエネルギー点における、1壊変・1MeVあたりのβ粒子数
                double ergBinL = double.Parse(bet2[0]); // エネルギー幅の下限(MeV)
                double numParL = double.Parse(bet2[1]); // 下限側のエネルギー点における、1壊変・1MeVあたりのβ粒子数

                double BetaYieldH = ergBinH * numParH;
                double BetaYieldL = ergBinL * numParL;

                if (InterpolationMethod == "PCHIP")
                {
                    var pchip = CubicSpline.InterpolatePchip(PEenergy, SAF);
                    var high = pchip.Interpolate(ergBinH);
                    var low = pchip.Interpolate(ergBinL);
                    var safH = BetaYieldH * high;
                    var safL = BetaYieldL * low;
                    beta += (safH + safL) * (ergBinH - ergBinL) / 2 * WRelectron;
                }
                else if (InterpolationMethod == "線形補間")
                {
                    var high = CalcSAF(ergBinH, PEenergy, SAF);
                    var safH = BetaYieldH * high;
                    var low = CalcSAF(ergBinL, PEenergy, SAF);
                    var safL = BetaYieldL * low;
                    beta += (safH + safL) * (ergBinH - ergBinL) / 2 * WRelectron;
                }
            }
            return beta;
        }

        /// <summary>
        /// 中性子S-factor計算
        /// </summary>
        /// <param name="nuclideNo">中性子SAFを持つ核種群の何番目かどうか</param>
        /// <returns></returns>
        private double CalcNeutron(double yield, double energy, double SAFn, double WRneutron)
        {
            double nS = yield * energy * SAFn * WRneutron;

            return nS;
        }
    }
}
