using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace FlexID.Calc
{
    static class SubRoutine
    {
        /// <summary>
        /// inputから各臓器に初期値を振り分ける
        /// </summary>
        /// <param name="Act"></param>
        /// <param name="data"></param>
        public static void Init(Activity Act, DataClass data)
        {
            Act.CalcPre = new OrganActivity[data.Organs.Count];
            Act.CalcNow = new OrganActivity[data.Organs.Count];

            Act.IterPre = new OrganActivity[data.Organs.Count];
            Act.IterNow = new OrganActivity[data.Organs.Count];

            Act.OutTotalNow = new double[data.Organs.Count];

            Act.IntakeQuantityPre = new double[data.Organs.Count];
            Act.IntakeQuantityNow = new double[data.Organs.Count];

            Act.Excreta = new double[data.Organs.Count];
            Act.PreExcreta = new double[data.Organs.Count];

            // 全ての組織における計算結果を初期化する
            foreach (var organ in data.Organs)
            {
                Act.CalcPre[organ.Index].ini = 0;
                Act.CalcPre[organ.Index].ave = 0;
                Act.CalcPre[organ.Index].end = 0;
                Act.CalcPre[organ.Index].total = 0;

                Act.CalcNow[organ.Index].ini = 0;
                Act.CalcNow[organ.Index].ave = 0;
                Act.CalcNow[organ.Index].end = 0;
                Act.CalcNow[organ.Index].total = 0;

                Act.OutTotalNow[organ.Index] = 0;

                Act.IntakeQuantityNow[organ.Index] = 0;

                Act.Excreta[organ.Index] = 0;
            }

            foreach (var organ in data.Organs)
            {
                var nuclide = organ.Nuclide;

                foreach (var inflow in organ.Inflows)
                {
                    // 流入元の臓器機能が"入力"でないものをはじく
                    if (inflow.Organ.Func != OrganFunc.inp)
                        continue;

                    // inputから初期値を受け取る臓器、対象組織が対象核種の組織か確認
                    if (inflow.Organ.Nuclide == nuclide)
                    {
                        var nucDecay = nuclide.Ramd;

                        var init = inflow.Rate / nucDecay;
                        Act.CalcNow[organ.Index].ini = init;
                        Act.CalcNow[organ.Index].ave = init;
                        Act.CalcNow[organ.Index].end = init;
                        Act.CalcNow[organ.Index].total = 0;
                        Act.IntakeQuantityNow[organ.Index] = 0;
                        break;
                    }
                }
            }
        }

        /// <summary>
        /// 排泄物
        /// </summary>
        public static void Excretion(Organ organ, Activity Act, double dt)
        {
            double ini = 0;
            double ave = 0;
            double end = 0;

            foreach (var inflow in organ.Inflows)
            {
                // 流入元の生物学的崩壊定数[/day]
                var beforeBio = inflow.Organ.BioDecayCalc;

                // 放射能[Bq/day] = 流入元の放射能[Bq/day] * 流入元の生物学的崩壊定数[/day] * 流入割合[-]
                ini += Act.IterNow[inflow.Organ.Index].ini * beforeBio * inflow.Rate;
                ave += Act.IterNow[inflow.Organ.Index].ave * beforeBio * inflow.Rate;
                end += Act.IterNow[inflow.Organ.Index].end * beforeBio * inflow.Rate;
            }
            organ.BioDecayCalc = 1;

            Act.PreExcreta[organ.Index] = dt * ave;
        }

        /// <summary>
        /// 混合
        /// </summary>
        public static void Mix(Organ organ, Activity Act)
        {
            double ini = 0;
            double ave = 0;
            double end = 0;

            foreach (var inflow in organ.Inflows)
            {
                // 流入元の生物学的崩壊定数[/day]
                var beforeBio = inflow.Organ.BioDecayCalc;

                // 放射能[Bq/day] = 流入元の放射能[Bq/day] * 流入元の生物学的崩壊定数[/day] * 流入割合[-]
                ini += Act.IterNow[inflow.Organ.Index].ini * beforeBio * inflow.Rate;
                ave += Act.IterNow[inflow.Organ.Index].ave * beforeBio * inflow.Rate;
                end += Act.IterNow[inflow.Organ.Index].end * beforeBio * inflow.Rate;
            }
            Act.IterNow[organ.Index].ini = ini;
            Act.IterNow[organ.Index].ave = ave;
            Act.IterNow[organ.Index].end = end;
            Act.IterNow[organ.Index].total = 0;
            organ.BioDecayCalc = 1;
        }

        /// <summary>
        /// 入力
        /// </summary>
        public static void Input(Organ organ, Activity Act)
        {
            // 初期振り分けはしたので0を設定するだけ？
            // todo: 機能3の区画にID=0以外の区画からの流入がある場合はその前提が崩れるため、
            // 入力ファイルを読み取った時点でそのような経路をエラーとする必要がある
            Act.IterNow[organ.Index].ini = 0;
            Act.IterNow[organ.Index].ave = 0;
            Act.IterNow[organ.Index].end = 0;
            Act.IterNow[organ.Index].total = 0;
        }

        /// <summary>
        /// 蓄積(OIR)
        /// </summary>
        /// <param name="dT">ΔT[day]</param>
        public static void Accumulation_OIR(double dT, Organ organ, Activity Act)
        {
            // alpha = 核種の崩壊定数[/day] + 当該臓器の生物学的崩壊定数[/day]
            var alpha = organ.NuclideDecay + organ.BioDecay;

            // 流入する平均放射能[Bq/day]
            var ave = 0.0;

            #region 流入臓器の数ループ
            foreach (var inflow in organ.Inflows)
            {
                var inflowOrgan = inflow.Organ;

                // 流入元の生物学的崩壊定数[/day]
                var beforeBio = inflowOrgan.BioDecayCalc;

                // 親核種からの崩壊の場合、同じ臓器内で崩壊するので生物学的崩壊定数の影響を受けない
                if (inflowOrgan.Nuclide != organ.Nuclide)
                    beforeBio = 1;

                // 放射能[Bq/day] = 流入元の放射能[Bq/day] * 流入元の生物学的崩壊定数[/day] * 流入割合[-]
                ave += Act.CalcNow[inflowOrgan.Index].ave * beforeBio * inflow.Rate;
            }
            #endregion

            Accumulation(alpha, dT, ave, in Act.CalcPre[organ.Index], ref Act.IterNow[organ.Index]);
        }

        /// <summary>
        /// 蓄積(EIR)
        /// </summary>
        /// <param name="dT">ΔT[day]</param>
        public static void Accumulation_EIR(double dT, Organ organLo, Organ organHi, Activity Act, double day, int daysLo, int daysHi)
        {
            // 当該臓器の生物学的崩壊定数[/day]
            var bioDecay = organLo.BioDecay;

            // 流入する平均放射能[Bq/day]
            var ave = 0.0;

            #region 流入臓器の数ループ
            for (int i = 0; i < organLo.Inflows.Count; i++)
            {
                // 丸め誤差が出るので、Roundするか否か
                var inflowLo = Math.Round(organLo.Inflows[i].Organ.BioDecay * organLo.Inflows[i].Rate, 6);
                var inflowHi = Math.Round(organHi.Inflows[i].Organ.BioDecay * organHi.Inflows[i].Rate, 6);

                // 流入元の生物学的崩壊定数[/day] * 流入割合[-]
                double beforeBio;
                if (day <= MainRoutine_EIR.AgeAdult)
                {
                    if (organLo.Name == "Plasma" && organLo.Inflows[i].Organ.Name == "SI")
                    {
                        beforeBio = organLo.Inflows[i].Organ.BioDecay * organLo.Inflows[i].Rate;
                        bioDecay = Interpolation(day, organLo.BioDecay, organHi.BioDecay, daysLo, daysHi);
                    }
                    else if (organLo.Name == "SI")
                    {
                        beforeBio = Interpolation(day, inflowLo, inflowHi, daysLo, daysHi);
                        bioDecay = organLo.BioDecay;
                    }
                    else
                    {
                        beforeBio = Interpolation(day, inflowLo, inflowHi, daysLo, daysHi);
                        bioDecay = Interpolation(day, organLo.BioDecay, organHi.BioDecay, daysLo, daysHi);
                    }
                }
                else
                {
                    beforeBio = organLo.Inflows[i].Organ.BioDecay * organLo.Inflows[i].Rate;
                    bioDecay = organLo.BioDecay;
                }

                // デバッグ用
                //if (organLo.Inflows[i].Organ.Name == "Plasma" & organLo.Name == "ULI")
                //    System.Diagnostics.Debug.Print("{0}  {1,-14:n}  {2,-14:n}  {3}", day.ToString(), organLo.Inflows[i].Organ.Name, organLo.Name, beforeBio.ToString());

                // 親核種からの崩壊の場合、同じ臓器内で崩壊するので生物学的崩壊定数の影響を受けない
                if (organLo.Inflows[i].Organ.Nuclide != organLo.Nuclide)
                    beforeBio = 1 * organLo.Inflows[i].Rate;

                // 放射能[Bq/day] = 流入元臓器の放射能[Bq/day] * 流入元臓器の生物学的崩壊定数 * 流入割合
                ave += Act.CalcNow[organLo.Inflows[i].Organ.Index].ave * beforeBio;
            }
            #endregion

            // alpha = 核種の崩壊定数[/day] + 当該臓器の生物学的崩壊定数[/day]
            var alpha = organLo.NuclideDecay + bioDecay;

            Accumulation(alpha, dT, ave, in Act.CalcPre[organLo.Index], ref Act.IterNow[organLo.Index]);
        }

        /// <summary>
        /// コンパートメントの蓄積計算。
        /// </summary>
        /// <param name="alpha">崩壊定数[/day]。</param>
        /// <param name="dT">時間メッシュの幅[day]</param>
        /// <param name="ave">流入する平均放射能[Bq/day]</param>
        /// <param name="pre">前回時間メッシュの計算結果。</param>
        /// <param name="now">今回時間メッシュの計算結果。</param>
        private static void Accumulation(double alpha, double dT, double ave, in OrganActivity pre, ref OrganActivity now)
        {
            var alpha_dT = alpha * dT;

            double rini = pre.end;  // 初期放射能[Bq/day]
            double rend;            // 末期放射能[Bq/day]
            double rtot;            // 積算放射能[Bq]
            if (alpha_dT <= 1E-9)
            {
                rend = ave * alpha_dT / alpha + rini * Math.Exp(-alpha_dT);
                rtot = ave * (dT - alpha_dT / alpha) / alpha + rini / alpha * alpha_dT;
            }
            else
            {
                if (alpha_dT >= 100)
                    alpha_dT = 100;

                rend = ave * (1 - Math.Exp(-alpha_dT)) / alpha + rini * Math.Exp(-alpha_dT);
                rtot = ave * (dT - (1 - Math.Exp(-alpha_dT)) / alpha) / alpha + rini / alpha * (1 - Math.Exp(-alpha_dT));
            }

            var rave = rtot / dT;   // 平均放射能[Bq/day] = 積算放射能[Bq] / Δt[day]

            // 計算値が1E-60以下の場合は0とする
            if (rave <= 1E-60)
                rave = 0;
            if (rend <= 1E-60)
                rend = 0;
            if (rtot <= 1E-60)
                rtot = 0;

            now.ini = rini;
            now.ave = rave;
            now.end = rend;
            now.total = rtot;
        }

        public static double Interpolation(double day, double valueLo, double valueHi, int daysLo, int daysHi)
        {
            double value;
            value = valueLo + (day - daysLo) * (valueHi - valueLo) / (daysHi - daysLo);
            return value;
        }
    }
}
