namespace FlexID.Calc;

static class SubRoutine
{
    /// <summary>
    /// inputから各臓器に初期値を振り分ける
    /// </summary>
    /// <param name="act"></param>
    /// <param name="data"></param>
    public static void Init(Activity act, InputData data)
    {
        act.CalcPre = new OrganActivity[data.Organs.Count];
        act.CalcNow = new OrganActivity[data.Organs.Count];

        act.IterPre = new OrganActivity[data.Organs.Count];
        act.IterNow = new OrganActivity[data.Organs.Count];

        act.OutNow = new OrganActivity[data.Organs.Count];
        act.OutTotalFromIntake = new double[data.Organs.Count];

        // 全ての組織における計算結果を初期化する
        foreach (var organ in data.Organs)
        {
            act.CalcPre[organ.Index].ini = 0;
            act.CalcPre[organ.Index].ave = 0;
            act.CalcPre[organ.Index].end = 0;
            act.CalcPre[organ.Index].total = 0;

            act.CalcNow[organ.Index].ini = 0;
            act.CalcNow[organ.Index].ave = 0;
            act.CalcNow[organ.Index].end = 0;
            act.CalcNow[organ.Index].total = 0;

            act.OutNow[organ.Index].ini = 0;
            act.OutNow[organ.Index].ave = 0;
            act.OutNow[organ.Index].end = 0;
            act.OutNow[organ.Index].total = 0;

            act.OutTotalFromIntake[organ.Index] = 0;
        }

        foreach (var organ in data.Organs)
        {
            var nuclide = organ.Nuclide;

            // organが、inputからの流入経路を持ち、かつinputと同じ対象核種に属するかを確認する。
            var inflowFromInp = organ.Inflows.FirstOrDefault(i => i.Organ.Func == OrganFunc.inp &&
                                                                  i.Organ.Nuclide == nuclide);
            if (inflowFromInp != null)
            {
                var init = inflowFromInp.Rate;
                act.CalcNow[organ.Index].ini = init;
                act.CalcNow[organ.Index].ave = init;
                act.CalcNow[organ.Index].end = init;
                act.CalcNow[organ.Index].total = 0;

                // 初期配分結果の出力に使用される。
                act.OutNow[organ.Index] = act.CalcNow[organ.Index];
            }
        }
    }

    /// <summary>
    /// 排泄
    /// </summary>
    /// <param name="organ">対象コンパートメント</param>
    /// <param name="act">計算結果</param>
    /// <param name="dT">ΔT[day]</param>
    public static void Excretion(Organ organ, Activity act, double dT)
    {
        double ini = 0;
        double ave = 0;
        double end = 0;

        // 排泄コンパートメントでは、計算時間メッシュ期間において
        // 他のコンパートメントからの流入量のみに着目する。
        // このため、以下の2つの経路についての計算を行わない。
        // - 親核種の崩壊による子核種への移行(親exc側からの流出、及び子exc側への流入)
        // - 蓄積した核種の崩壊による流出(減衰)
        foreach (var inflow in organ.Inflows)
        {
            var inflowOrgan = inflow.Organ;

            // 流入元の生物学的崩壊定数[/day]
            var beforeBio = inflowOrgan.BioDecay;

            // 親核種からの崩壊の場合、同じ臓器内で崩壊するので生物学的崩壊定数の影響を受けない
            if (inflowOrgan.Nuclide != organ.Nuclide)
                beforeBio = organ.NuclideDecay; // TODO: なぜ娘核種のλを乗算するのか？

            // 放射能[Bq/day] = 流入元の放射能[Bq/day] * 流入元の生物学的崩壊定数[/day] * 流入割合[-]
            ini += act.IterPre[inflowOrgan.Index].ini * beforeBio * inflow.Rate;
            ave += act.IterPre[inflowOrgan.Index].ave * beforeBio * inflow.Rate;
            end += act.IterPre[inflowOrgan.Index].end * beforeBio * inflow.Rate;
        }

        // 蓄積した核種の壊変による減衰を適用する。
        var alpha = organ.NuclideDecay;
        Accumulation(alpha, dT, ave, in act.CalcPre[organ.Index], ref act.IterNow[organ.Index]);
    }

    /// <summary>
    /// 混合
    /// </summary>
    /// <param name="organ">対象コンパートメント</param>
    /// <param name="act">計算結果</param>
    public static void Mix(Organ organ, Activity act)
    {
        double ini = 0;
        double ave = 0;
        double end = 0;

        foreach (var inflow in organ.Inflows)
        {
            // 流入元の生物学的崩壊定数[/day]
            var beforeBio = inflow.Organ.BioDecay;

            // 放射能[Bq/day] = 流入元の放射能[Bq/day] * 流入元の生物学的崩壊定数[/day] * 流入割合[-]
            ini += act.IterPre[inflow.Organ.Index].ini * beforeBio * inflow.Rate;
            ave += act.IterPre[inflow.Organ.Index].ave * beforeBio * inflow.Rate;
            end += act.IterPre[inflow.Organ.Index].end * beforeBio * inflow.Rate;
        }
        act.IterNow[organ.Index].ini = ini;
        act.IterNow[organ.Index].ave = ave;
        act.IterNow[organ.Index].end = end;

        // 混合コンパートメントでは、流入放射能は全て接続先へ流出するため、
        // 計算時間メッシュ期間における積算放射能をゼロと計算する。
        act.IterNow[organ.Index].total = 0;
    }

    /// <summary>
    /// 入力
    /// </summary>
    /// <param name="organ">対象コンパートメント</param>
    /// <param name="act">計算結果</param>
    public static void Input(Organ organ, Activity act)
    {
        // 初期振り分けはしたので0を設定するだけ。
        act.IterNow[organ.Index].ini = 0;
        act.IterNow[organ.Index].ave = 0;
        act.IterNow[organ.Index].end = 0;

        // 入力コンパートメントでは、全ての放射能は初期配分によって接続先へ流出するため、
        // 計算時間メッシュ期間における積算放射能をゼロと計算する。
        act.IterNow[organ.Index].total = 0;
    }

    /// <summary>
    /// 蓄積(OIR)
    /// </summary>
    /// <param name="organ">対象コンパートメント</param>
    /// <param name="act">計算結果</param>
    /// <param name="dT">ΔT[day]</param>
    public static void Accumulation_OIR(Organ organ, Activity act, double dT)
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
            var beforeBio = inflowOrgan.BioDecay;

            // 親核種からの崩壊の場合、同じ臓器内で崩壊するので生物学的崩壊定数の影響を受けない
            if (inflowOrgan.Nuclide != organ.Nuclide)
                beforeBio = organ.NuclideDecay; // TODO: なぜ娘核種のλを乗算するのか？

            // 放射能[Bq/day] = 流入元の放射能[Bq/day] * 流入元の生物学的崩壊定数[/day] * 流入割合[-]
            ave += act.IterPre[inflowOrgan.Index].ave * beforeBio * inflow.Rate;
        }
        #endregion

        Accumulation(alpha, dT, ave, in act.CalcPre[organ.Index], ref act.IterNow[organ.Index]);
    }

    /// <summary>
    /// 蓄積(EIR)
    /// </summary>
    /// <param name="organLo">補間期間下限側の対象コンパートメント</param>
    /// <param name="organHi">補間期間上限側の対象コンパートメント</param>
    /// <param name="act">計算結果</param>
    /// <param name="dT">ΔT[day]</param>
    /// <param name="ageDay">評価時刻における年齢[day]</param>
    /// <param name="daysLo">補間期間下限側の年齢[day]</param>
    /// <param name="daysHi">補間期間上限側の年齢[day]</param>
    public static void Accumulation_EIR(Organ organLo, Organ organHi, Activity act, double dT, double ageDay, int daysLo, int daysHi)
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
            if (ageDay <= MainRoutine_EIR.AgeAdult)
            {
                if (organLo.Name == "Plasma" && organLo.Inflows[i].Organ.Name == "SI")
                {
                    beforeBio = organLo.Inflows[i].Organ.BioDecay * organLo.Inflows[i].Rate;
                    bioDecay = Interpolation(ageDay, organLo.BioDecay, organHi.BioDecay, daysLo, daysHi);
                }
                else if (organLo.Name == "SI")
                {
                    beforeBio = Interpolation(ageDay, inflowLo, inflowHi, daysLo, daysHi);
                    bioDecay = organLo.BioDecay;
                }
                else
                {
                    beforeBio = Interpolation(ageDay, inflowLo, inflowHi, daysLo, daysHi);
                    bioDecay = Interpolation(ageDay, organLo.BioDecay, organHi.BioDecay, daysLo, daysHi);
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
                beforeBio = organLo.NuclideDecay * organLo.Inflows[i].Rate; // TODO: なぜ娘核種のλを乗算するのか？

            // 放射能[Bq/day] = 流入元臓器の放射能[Bq/day] * 流入元臓器の生物学的崩壊定数 * 流入割合
            ave += act.IterPre[organLo.Inflows[i].Organ.Index].ave * beforeBio;
        }
        #endregion

        // alpha = 核種の崩壊定数[/day] + 当該臓器の生物学的崩壊定数[/day]
        var alpha = organLo.NuclideDecay + bioDecay;

        Accumulation(alpha, dT, ave, in act.CalcPre[organLo.Index], ref act.IterNow[organLo.Index]);
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
