namespace FlexID;

static class SubRoutine
{
    /// <summary>
    /// inputから各臓器に初期値を振り分ける
    /// </summary>
    /// <param name="act"></param>
    /// <param name="data"></param>
    public static void Init(Activity act, InputData data)
    {
        foreach (var organ in data.Organs)
        {
            var nuclide = organ.Nuclide;

            // organが、inputからの流入経路を持ち、かつinputと同じ対象核種に属するかを確認する。
            var inflowFromInp = organ.Inflows.FirstOrDefault(i => i.Organ.Func == OrganFunc.inp &&
                                                                  i.Organ.Nuclide == nuclide);
            if (inflowFromInp != null)
            {
                // 親核種の1[Bq]相当の原子数を各コンパートメントに初期配分する。
                // 親核種が安定核種(λ=0)の場合は、原子数1個を初期配分する。
                var init = nuclide.Lambda != 0 ? 1.0 / nuclide.Lambda : 1.0;

                var rate = inflowFromInp.Rate;
                act.CalcNow[organ.Index].ini = init * rate;
                act.CalcNow[organ.Index].ave = init * rate;
                act.CalcNow[organ.Index].end = init * rate;
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
    /// <param name="Act">計算結果</param>
    /// <param name="dT">ΔT[day]</param>
    public static void Excretion(Organ organ, Activity Act, double dT)
    {
        // 1日あたりの平均流入量[atoms/day]
        var ave = 0.0;

        foreach (var inflow in organ.Inflows)
        {
            var inflowOrgan = inflow.Organ;
            var rate = inflow.Rate;

            // 平均流入量[atoms/day] = 流入元の平均量[atoms/day] * 流入割合[-]
            ave += Act.IterPre[inflowOrgan.Index].ave * rate;
        }

        // 蓄積した核種の壊変による減衰を適用する。
        var alpha = organ.NuclideDecay;

        Accumulation(alpha, dT, ave, in Act.CalcPre[organ.Index], ref Act.IterNow[organ.Index]);
    }

    /// <summary>
    /// 混合
    /// </summary>
    /// <param name="organ">対象コンパートメント</param>
    /// <param name="Act">計算結果</param>
    public static void Mix(Organ organ, Activity Act)
    {
        double ini = 0;
        double ave = 0;
        double end = 0;

        foreach (var inflow in organ.Inflows)
        {
            var inflowOrgan = inflow.Organ;
            var rate = inflow.Rate;

            // 平均流入量[atoms/day] = 流入元の平均量[atoms/day] * 流入割合[-]
            ini += Act.IterPre[inflowOrgan.Index].ini * rate;
            ave += Act.IterPre[inflowOrgan.Index].ave * rate;
            end += Act.IterPre[inflowOrgan.Index].end * rate;
        }
        Act.IterNow[organ.Index].ini = ini;
        Act.IterNow[organ.Index].ave = ave;
        Act.IterNow[organ.Index].end = end;

        // 混合コンパートメントでは、流入は全て接続先へ流出するため、
        // 計算時間メッシュ期間における積算量をゼロと計算する。
        Act.IterNow[organ.Index].total = 0;
    }

    /// <summary>
    /// 入力
    /// </summary>
    /// <param name="organ">対象コンパートメント</param>
    /// <param name="Act">計算結果</param>
    public static void Input(Organ organ, Activity Act)
    {
        // 初期振り分けはしたので0を設定するだけ。
        Act.IterNow[organ.Index].ini = 0;
        Act.IterNow[organ.Index].ave = 0;
        Act.IterNow[organ.Index].end = 0;

        // 入力コンパートメントでは、全ての流入は初期配分によって接続先へ流出するため、
        // 計算時間メッシュ期間における積算量をゼロと計算する。
        Act.IterNow[organ.Index].total = 0;
    }

    /// <summary>
    /// 蓄積(OIR)
    /// </summary>
    /// <param name="organ">対象コンパートメント</param>
    /// <param name="Act">計算結果</param>
    /// <param name="dT">ΔT[day]</param>
    public static void Accumulation_OIR(Organ organ, Activity Act, double dT)
    {
        // 1日あたりの平均流入量[atoms/day]
        var ave = 0.0;

        foreach (var inflow in organ.Inflows)
        {
            var inflowOrgan = inflow.Organ;
            var rate = inflow.Rate;

            // 平均流入量[atoms/day] = 流入元の平均量[atoms/day] * 流入割合[-]
            ave += Act.IterPre[inflowOrgan.Index].ave * rate;
        }

        // alpha = 核種の崩壊定数[/day] + 当該臓器の生物学的崩壊定数[/day]
        var alpha = organ.NuclideDecay + organ.BioDecay;

        Accumulation(alpha, dT, ave, in Act.CalcPre[organ.Index], ref Act.IterNow[organ.Index]);
    }

    /// <summary>
    /// 蓄積(EIR)
    /// </summary>
    /// <param name="organLo">補間期間下限側の対象コンパートメント</param>
    /// <param name="organHi">補間期間上限側の対象コンパートメント</param>
    /// <param name="Act">計算結果</param>
    /// <param name="dT">ΔT[day]</param>
    /// <param name="ageDay">評価時刻における年齢[day]</param>
    /// <param name="daysLo">補間期間下限側の年齢[day]</param>
    /// <param name="daysHi">補間期間上限側の年齢[day]</param>
    public static void Accumulation_EIR(Organ organLo, Organ organHi, Activity Act, double dT, double ageDay, int daysLo, int daysHi)
    {
        // 当該臓器の生物学的崩壊定数[/day]
        var bioDecay = organLo.BioDecay;

        // 1日あたりの平均流入量[atoms/day]
        var ave = 0.0;

        for (int i = 0; i < organLo.Inflows.Count; i++)
        {
            // 丸め誤差が出るので、Roundするか否か
            var inflowLo = Math.Round(organLo.Inflows[i].Rate, 6);
            var inflowHi = Math.Round(organHi.Inflows[i].Rate, 6);

            double rate;
            if (ageDay <= MainRoutine_EIR.AgeAdult)
            {
                if (organLo.Name == "Plasma" && organLo.Inflows[i].Organ.Name == "SI")
                {
                    rate = organLo.Inflows[i].Rate;
                    bioDecay = Interpolation(ageDay, organLo.BioDecay, organHi.BioDecay, daysLo, daysHi);
                }
                else if (organLo.Name == "SI")
                {
                    rate = Interpolation(ageDay, inflowLo, inflowHi, daysLo, daysHi);
                    bioDecay = organLo.BioDecay;
                }
                else
                {
                    rate = Interpolation(ageDay, inflowLo, inflowHi, daysLo, daysHi);
                    bioDecay = Interpolation(ageDay, organLo.BioDecay, organHi.BioDecay, daysLo, daysHi);
                }
            }
            else
            {
                rate = organLo.Inflows[i].Rate;
                bioDecay = organLo.BioDecay;
            }

            // デバッグ用
            //if (organLo.Inflows[i].Organ.Name == "Plasma" & organLo.Name == "ULI")
            //    System.Diagnostics.Debug.Print("{0}  {1,-14:n}  {2,-14:n}  {3}", day.ToString(), organLo.Inflows[i].Organ.Name, organLo.Name, beforeBio.ToString());

            // 親核種からの崩壊の場合、同じ臓器内で崩壊するので生物学的崩壊定数の影響を受けない
            if (organLo.Inflows[i].Organ.Nuclide != organLo.Nuclide)
                rate = organLo.Inflows[i].Rate;

            // 平均流入量[atoms/day] = 流入元の平均量[atoms/day] * 流入割合[-]
            ave += Act.IterPre[organLo.Inflows[i].Organ.Index].ave * rate;
        }

        // alpha = 核種の崩壊定数[/day] + 当該臓器の生物学的崩壊定数[/day]
        var alpha = organLo.NuclideDecay + bioDecay;

        Accumulation(alpha, dT, ave, in Act.CalcPre[organLo.Index], ref Act.IterNow[organLo.Index]);
    }

    /// <summary>
    /// コンパートメントの蓄積計算。
    /// </summary>
    /// <param name="alpha">崩壊定数[/day]。</param>
    /// <param name="dT">時間メッシュの幅[day]</param>
    /// <param name="ave">1日あたりの平均流入量[atoms/day]</param>
    /// <param name="pre">前回時間メッシュの計算結果。</param>
    /// <param name="now">今回時間メッシュの計算結果。</param>
    private static void Accumulation(double alpha, double dT, double ave, in OrganActivity pre, ref OrganActivity now)
    {
        var alpha_dT = alpha * dT;

        double rini = pre.end;  // 初期原子数[atoms]
        double rend;            // 末期原子数[atoms]
        double rtot;            // ΔTあたりの積算原子数[atoms]
        if (alpha == 0)
        {
            rend = rini + ave * dT;
            rtot = (rini + ave) * dT;
        }
        else if (alpha_dT <= 1E-9)
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

        var rave = rtot / dT;   // 1日あたりの平均原子数[atoms/day] = ΔTあたりの積算原子数[atoms] / ΔT[day]

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
