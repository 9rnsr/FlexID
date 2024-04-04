using System;

namespace FlexID.Calc
{
    class SubRoutine
    {
        /// <summary>
        /// inputから各臓器に初期値を振り分ける
        /// </summary>
        /// <param name="Act"></param>
        /// <param name="data"></param>
        public void Init(Activity Act, DataClass data)
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
                        var nucDecay = data.Ramd[nuclide];

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
        public void Excretion(Organ organ, Activity Act, double dt)
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
        public void Mix(Organ organ, Activity Act)
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
        public void Input(Organ organ, Activity Act)
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
        public void Accumulation(double dT, Organ organ, Activity Act, double d)
        {
            // alpha = 核種の崩壊定数 + 当該臓器の生物学的崩壊定数
            var alpha = organ.NuclideDecay + organ.BioDecay;

            #region 流入臓器の数ループ
            double ave = 0.0;
            foreach (var inflow in organ.Inflows)
            {
                var inflowOrgan = inflow.Organ;

                // 流入元生物学的崩壊定数
                var beforeBio = inflowOrgan.BioDecayCalc;

                // 親核種からの崩壊の場合、同じ臓器内で崩壊するので生物学的崩壊定数の影響を受けない
                if (inflowOrgan.Nuclide != organ.Nuclide)
                    beforeBio = 1;

                // 平均放射能の積算値 += 流入元臓器のタイムステップ毎の平均放射能 * 流入元臓器の生物学的崩壊定数 * 流入割合
                ave += Act.Now[inflowOrgan.Index].ave * beforeBio * inflow.Rate;
            }
            #endregion

            var alpha_dT = alpha * dT;

            double rini = Act.Pre[organ.Index].end;
            double rend;
            double rtotal;
            if (alpha_dT <= 1E-9)
            {
                rend = ave * alpha_dT / alpha + rini * Math.Exp(-alpha_dT);
                rtotal = ave * (dT - alpha_dT / alpha) / alpha + rini / alpha * (alpha_dT);
            }
            else
            {
                if (alpha_dT >= 100)
                    alpha_dT = 100;

                rend = ave * (1 - Math.Exp(-alpha_dT)) / alpha + rini * Math.Exp(-alpha_dT);
                rtotal = ave * (dT - (1 - Math.Exp(-alpha_dT)) / alpha) / alpha + rini / alpha * (1 - Math.Exp(-alpha_dT));
            }

            Act.rNow[organ.Index].ini = rini;
            Act.rNow[organ.Index].ave = rtotal / dT;    // 当該臓器の平均放射能 = 当該臓器の積算放射能 / Δt
            Act.rNow[organ.Index].end = rend;
            Act.rNow[organ.Index].total = rtotal;

            // 計算値が1E-60以下の場合は0とする
            if (Act.rNow[organ.Index].ave <= 1e-60)
                Act.rNow[organ.Index].ave = 0;
            if (Act.rNow[organ.Index].end <= 1e-60)
                Act.rNow[organ.Index].end = 0;
            if (Act.rNow[organ.Index].total <= 1e-60)
                Act.rNow[organ.Index].total = 0;
        }

        /// <summary>
        /// 蓄積(EIR)
        /// </summary>
        /// <param name="dT">ΔT[day]</param>
        public void Accumulate_EIR(double dT, Organ organLow, Organ organHigh, Activity Act, double day, int LowDays, int HighDays)
        {
            // alpha = 核種の崩壊定数 + 当該臓器の生物学的崩壊定数
            double alpha;
            alpha = organLow.BioDecay;
            #region 流入臓器の数ループ
            double ave = 0.0;
            for (int i = 0; i < organLow.Inflows.Count; i++)
            {
                // 丸め誤差が出るので、Roundするか否か
                var InflowLow = Math.Round(organLow.Inflows[i].Organ.BioDecay * organLow.Inflows[i].Rate, 6);
                var InflowHigh = Math.Round(organHigh.Inflows[i].Organ.BioDecay * organHigh.Inflows[i].Rate, 6);

                double beforeBio;
                // 流入元生物学的崩壊定数
                if (day <= MainRoutine.adult)
                {
                    if (organLow.Name == "Plasma" && organLow.Inflows[i].Organ.Name == "SI")
                    {
                        beforeBio = organLow.Inflows[i].Organ.BioDecay * organLow.Inflows[i].Rate;
                        alpha = MainRoutine.Interpolation(day, organLow.BioDecay, organHigh.BioDecay, LowDays, HighDays);
                    }
                    else if (organLow.Name == "SI")
                    {
                        beforeBio = MainRoutine.Interpolation(day, InflowLow, InflowHigh, LowDays, HighDays);
                        alpha = organLow.BioDecay;
                    }
                    else
                    {
                        beforeBio = MainRoutine.Interpolation(day, InflowLow, InflowHigh, LowDays, HighDays);
                        alpha = MainRoutine.Interpolation(day, organLow.BioDecay, organHigh.BioDecay, LowDays, HighDays);
                    }
                }
                else
                {
                    beforeBio = organLow.Inflows[i].Organ.BioDecay * organLow.Inflows[i].Rate;
                    alpha = organLow.BioDecay;
                }

                // デバッグ用
                //if (organLow.Inflows[i].Organ.Name == "Plasma" & organLow.Name == "ULI")
                //    System.Diagnostics.Debug.Print("{0}  {1,-14:n}  {2,-14:n}  {3}", day.ToString(), organLow.Inflows[i].Organ.Name, organLow.Name, beforeBio.ToString());

                // 親核種からの崩壊の場合、同じ臓器内で崩壊するので生物学的崩壊定数の影響を受けない
                if (organLow.Inflows[i].Organ.Nuclide != organLow.Nuclide)
                    beforeBio = 1 * organLow.Inflows[i].Rate;

                // 平均放射能の積算値 += 流入元臓器のタイムステップ毎の平均放射能 * 流入
                ave += Act.Now[organLow.Inflows[i].Organ.Index].ave * beforeBio;
            }
            #endregion

            alpha += organLow.NuclideDecay;

            var alpha_dT = alpha * dT;

            double rini = Act.Pre[organLow.Index].end;
            double rend;
            double rtotal;
            if (alpha_dT <= 1E-9)
            {
                rend = ave * (alpha_dT) / alpha + rini * Math.Exp(-alpha_dT);
                rtotal = ave * (dT - alpha_dT / alpha) / alpha + rini / alpha * (alpha_dT);
            }
            else
            {
                if (alpha_dT >= 100)
                    alpha_dT = 100;

                rend = ave * (1 - Math.Exp(-alpha_dT)) / alpha + rini * Math.Exp(-alpha_dT);
                rtotal = ave * (dT - (1 - Math.Exp(-alpha_dT)) / alpha) / alpha + rini / alpha * (1 - Math.Exp(-alpha_dT));
            }

            Act.rNow[organLow.Index].ini = rini;
            Act.rNow[organLow.Index].ave = rtotal / dT;    // 当該臓器の平均放射能 = 当該臓器の積算放射能 / Δt
            Act.rNow[organLow.Index].end = rend;
            Act.rNow[organLow.Index].total = rtotal;

            // 計算値が1E-60以下の場合は0とする
            if (Act.rNow[organLow.Index].ave <= 1e-60)
                Act.rNow[organLow.Index].ave = 0;
            if (Act.rNow[organLow.Index].end <= 1e-60)
                Act.rNow[organLow.Index].end = 0;
            if (Act.rNow[organLow.Index].total <= 1e-60)
                Act.rNow[organLow.Index].total = 0;
        }
    }
}
