using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace FlexID.Calc
{
    static class SubRoutine
    {
        private static Regex patternPeriod =
            new Regex(@"^ *(?<num>\d+) *(?<unit>days|months|years) *$", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        /// <summary>
        /// 預託期間を日数に換算する。
        /// </summary>
        /// <param name="period"></param>
        /// <returns></returns>
        public static int CommitmentPeriodToDays(string period)
        {
            var m = patternPeriod.Match(period);
            if (m.Success)
            {
                var num = int.Parse(m.Groups["num"].Value);
                var unit = m.Groups["unit"].Value.ToLowerInvariant();
                var days = unit == "days" ? num :
                           unit == "months" ? num * 31 :
                           unit == "years" ? num * 365 :
                           throw Program.Error("Please enter the period ('days', 'months', 'years').");
                return days;
            }
            else
            {
                throw Program.Error("Please enter integer for the Commitment Period.");
            }
        }

        /// <summary>
        /// inputから各臓器に初期値を振り分ける
        /// </summary>
        /// <param name="Act"></param>
        /// <param name="data"></param>
        public static void Init(Activity Act, DataClass data)
        {
            Act.Pre = new OrganActivity[data.Organs.Count];
            Act.Now = new OrganActivity[data.Organs.Count];
            Act.IntakeQuantityPre = new double[data.Organs.Count];
            Act.IntakeQuantityNow = new double[data.Organs.Count];

            Act.rPre = new OrganActivity[data.Organs.Count];
            Act.rNow = new OrganActivity[data.Organs.Count];

            Act.Excreta = new double[data.Organs.Count];
            Act.PreExcreta = new double[data.Organs.Count];

            // 全ての組織における計算結果を初期化する
            foreach (var organ in data.Organs)
            {
                Act.Pre[organ.Index].ini = 0;
                Act.Pre[organ.Index].ave = 0;
                Act.Pre[organ.Index].end = 0;
                Act.Pre[organ.Index].total = 0;

                Act.Now[organ.Index].ini = 0;
                Act.Now[organ.Index].ave = 0;
                Act.Now[organ.Index].end = 0;
                Act.Now[organ.Index].total = 0;
                Act.IntakeQuantityNow[organ.Index] = 0;
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
                        Act.Now[organ.Index].ini = init;
                        Act.Now[organ.Index].ave = init;
                        Act.Now[organ.Index].end = init;
                        Act.Now[organ.Index].total = 0;
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
                // 流入元の生物崩壊・流入割合
                var beforeBio = inflow.Organ.BioDecayCalc;

                // 放射能 = 流入元臓器の放射能 * 流入元臓器の生物学的崩壊定数 * 流入割合
                ini += Act.rNow[inflow.Organ.Index].ini * beforeBio * inflow.Rate;
                ave += Act.rNow[inflow.Organ.Index].ave * beforeBio * inflow.Rate;
                end += Act.rNow[inflow.Organ.Index].end * beforeBio * inflow.Rate;
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
                // 流入元の生物崩壊・流入割合
                var beforeBio = inflow.Organ.BioDecayCalc;

                // 放射能 = 流入元臓器の放射能 * 流入元臓器の生物学的崩壊定数 * 流入割合
                ini += Act.rNow[inflow.Organ.Index].ini * beforeBio * inflow.Rate;
                ave += Act.rNow[inflow.Organ.Index].ave * beforeBio * inflow.Rate;
                end += Act.rNow[inflow.Organ.Index].end * beforeBio * inflow.Rate;
            }
            Act.rNow[organ.Index].ini = ini;
            Act.rNow[organ.Index].ave = ave;
            Act.rNow[organ.Index].end = end;
            Act.rNow[organ.Index].total = 0;
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
            Act.rNow[organ.Index].ini = 0;
            Act.rNow[organ.Index].ave = 0;
            Act.rNow[organ.Index].end = 0;
            Act.rNow[organ.Index].total = 0;
        }

        /// <summary>
        /// 蓄積(OIR)
        /// </summary>
        /// <param name="dT">ΔT[day]</param>
        public static void Accumulation(double dT, Organ organ, Activity Act)
        {
            var ave = 0.0;  // 流入する平均放射能の積算値
            foreach (var inflow in organ.Inflows)
            {
                var inflowOrgan = inflow.Organ;

                var beforeBio = inflowOrgan.BioDecayCalc;   // 流入元の生物学的崩壊定数
                var inflowRate = inflow.Rate;               // 流入割合

                // 親核種からの崩壊の場合、同じコンパートメント内で崩壊するので生物学的崩壊定数の影響を受けない。
                if (inflowOrgan.Nuclide != organ.Nuclide)
                    beforeBio = 1;

                // 平均放射能の積算値 += 流入元のタイムステップ毎の平均放射能 * 流入元の生物学的崩壊定数 * 流入割合
                ave += Act.Now[inflowOrgan.Index].ave * beforeBio * inflowRate;
            }

            // alpha = 核種の崩壊定数 + 当該臓器の生物学的崩壊定数
            var alpha = organ.NuclideDecay + organ.BioDecay;

            Accumulation(alpha, dT, ave, in Act.Pre[organ.Index], ref Act.rNow[organ.Index]);
        }

        /// <summary>
        /// 蓄積(EIR)
        /// </summary>
        /// <param name="dT">ΔT[day]</param>
        public static void Accumulation_EIR(double dT, Organ organLo, Organ organHi, Activity Act, double day, int daysLo, int daysHi)
        {
            // 年齢区間における補間を行う場合はtrue。
            var doInterp = organHi != organLo;

            var ave = 0.0;  // 流入する平均放射能の積算値
            for (int i = 0; i < organLo.Inflows.Count; i++)
            {
                var inflow = organLo.Inflows[i];
                var inflowOrgan = inflow.Organ;

                double beforeBio;           // 流入元の生物学的崩壊定数
                double inflowRate;          // 流入割合
                if (doInterp)
                {
                    var inflowHi = organHi.Inflows[i];
                    var inflowOrganHi = inflowHi.Organ;

                    var beforeBioLo = inflowOrgan.BioDecay;
                    var beforeBioHi = inflowOrganHi.BioDecay;
                    beforeBio = Interpolation(day, beforeBioLo, beforeBioHi, daysLo, daysHi);

                    var inflowRateLo = inflow.Rate;
                    var inflowRateHi = inflowHi.Rate;
                    inflowRate = Interpolation(day, inflowRateLo, inflowRateHi, daysLo, daysHi);
                }
                else
                {
                    beforeBio = inflowOrgan.BioDecay;
                    inflowRate = inflow.Rate;
                }

                // デバッグ用
                //if (inflowOrganLo.Name == "Plasma" && organLo.Name == "ULI")
                //    System.Diagnostics.Debug.Print("{0}  {1,-14:n}  {2,-14:n}  {3}", day, inflowOrganLo.Name, organLo.Name, beforeBio);

                // 親核種からの崩壊の場合、同じコンパートメント内で崩壊するので生物学的崩壊定数の影響を受けない。
                if (inflowOrgan.Nuclide != organLo.Nuclide)
                    beforeBio = 1;

                // 平均放射能の積算値 += 流入元のタイムステップ毎の平均放射能 * 流入元の生物学的崩壊定数 * 流入割合
                ave += Act.Now[inflowOrgan.Index].ave * beforeBio * inflowRate;
            }

            double organBioDecay;
            if (doInterp)
            {
                organBioDecay = Interpolation(day, organLo.BioDecay, organHi.BioDecay, daysLo, daysHi);
            }
            else
            {
                organBioDecay = organLo.BioDecay;
            }

            // alpha = 核種の崩壊定数 + 当該臓器の生物学的崩壊定数
            var alpha = organLo.NuclideDecay + organBioDecay;

            Accumulation(alpha, dT, ave, in Act.Pre[organLo.Index], ref Act.rNow[organLo.Index]);
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

            double rini = pre.end;  // 初期放射能
            double rend;            // 末期放射能
            double rtot;            // 積算放射能
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

            var rave = rtot / dT;   // 平均放射能 = 積算放射能 / Δt

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
