using MathNet.Numerics.Interpolation;
using System;
using System.Collections.Generic;
using System.IO;

namespace S_Coefficient
{
    public class CalcScoeff
    {
        public string InterpolationMethod = "";

        // J/MeVへの変換
        private double ToJoule = 1.602176487E-13;

        /// <summary>
        /// 計算対象となる核種のリスト。
        /// </summary>
        public List<string> Nuclides { get; } = new List<string>();

        /// <summary>
        /// S係数の計算
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
            var OutTotal = new double[safdata.Count];
            var OutP = new double[safdata.Count];
            var OutE = new double[safdata.Count];
            var OutB = new double[safdata.Count];
            var OutA = new double[safdata.Count];
            var OutN = new double[safdata.Count];

            // 放射線データ取得
            var raddata = DataReader.ReadRAD(nuclideName);

            // βスペクトル取得
            var betdata = DataReader.ReadBET(nuclideName);

            var WRalpha = safdata.WRalpha;
            var WRphoton = safdata.WRphoton;
            var WRelectron = safdata.WRelectron;

            var (WRneutron, SAFneutron) =
                safdata.SAFneutron.TryGetValue(nuclideName, out var n) ? n : (0.0, null);

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
                        ScoeffP += Yi * Ei * CalcSAFp(Ei) * WRphoton * ToJoule;
                    }
                    // IE: 内部転換電子、AE: オージェ電子
                    else if (jcode == "AE" || jcode == "IE")
                    {
                        ScoeffE += Yi * Ei * CalcSAFe(Ei) * WRelectron * ToJoule;
                    }
                    // B-:β粒子(電子)、B+: 陽電子、DB: 遅発β
                    else if (jcode == "B-" || jcode == "B+" || jcode == "DB")
                    {
                        if (finishBeta)
                            continue;

                        // RADファイルに定義されているエントリは単に無視し、BETファイルからS係数を計算する
                        double beta = 0;
                        for (int r = 1; r < betdata.Length; r++)
                        {
                            var betL = betdata[r - 1].Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);
                            var betH = betdata[r - 0].Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);

                            var ebinL = double.Parse(betL[0]); // エネルギー幅の下限(MeV)
                            var ebinH = double.Parse(betH[0]); // エネルギー幅の上限(MeV)
                            var nparL = double.Parse(betL[1]); // 下限側のエネルギー点における、1壊変・1MeVあたりのβ粒子数
                            var nparH = double.Parse(betH[1]); // 上限側のエネルギー点における、1壊変・1MeVあたりのβ粒子数

                            var yieldL = ebinL * nparL;
                            var yieldH = ebinH * nparH;

                            var lo = CalcSAFe(ebinL);
                            var hi = CalcSAFe(ebinH);
                            var safH = yieldH * hi;
                            var safL = yieldL * lo;
                            beta += (safH + safL) * (ebinH - ebinL) / 2 * WRelectron;
                        }
                        ScoeffB = beta * ToJoule;

                        finishBeta = true;
                    }
                    // α粒子
                    else if (jcode == "A")
                    {
                        ScoeffA += Yi * Ei * CalcSAFa(Ei) * WRalpha * ToJoule;
                    }
                    // α反跳核
                    else if (jcode == "AR")
                    {
                        ScoeffA += Yi * Ei * SAFa_2MeV * WRalpha * ToJoule;
                    }
                    // 核分裂片
                    else if (jcode == "FF")
                    {
                        ScoeffN += Yi * Ei * SAFa_2MeV * WRalpha * ToJoule;
                    }
                    // 中性子
                    else if (jcode == "N")
                    {
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

            // 核種ごとの計算結果をExcelファイルに出力する
            output.WriteCalcResult(nuclideName, OutTotal, OutP, OutE, OutB, OutA, OutN);
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
    }
}
