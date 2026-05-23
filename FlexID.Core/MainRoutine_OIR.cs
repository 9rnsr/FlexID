namespace FlexID;

/// <summary>
/// 放射性核種の職業上の摂取(OIR: Occupational Intakes of Radionuclides)における
/// 残留放射能および預託線量の計算。
/// </summary>
public class MainRoutine_OIR
{
    /// <summary>
    /// 出力ディレクトリ。
    /// </summary>
    public required string OutputDirectory { get; init; }

    /// <summary>
    /// 出力ファイル名。
    /// </summary>
    public required string OutputFileName { get; init; }

    /// <summary>
    /// 計算時間メッシュ。
    /// </summary>
    public required TimeMesh ComputeTimeMesh { get; init; }

    /// <summary>
    /// 出力時間メッシュ。
    /// </summary>
    public required TimeMesh OutputTimeMesh { get; init; }

    /// <summary>
    /// 預託期間[sec]。
    /// </summary>
    public required long CommitmentPeriod { get; init; }

    /// <summary>
    /// 計算処理の進捗率を0～100で報告する。
    /// </summary>
    public IProgress<double>? ProgressIndicator { get; init; }

    public void Main(InputData data, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(OutputDirectory))
            throw Program.Error("Output directory is not specified");
        if (string.IsNullOrWhiteSpace(OutputFileName))
            throw Program.Error("Output file name is not specified");

        if (!ComputeTimeMesh.Cover(OutputTimeMesh))
            throw Program.Error("Computational time mesh does not cover all boundaries of output time mesh.");

        Directory.CreateDirectory(OutputDirectory);

        // ログファイルを出力する。
        var logPath = Path.Combine(OutputDirectory, OutputFileName) + ".log";
        using (var logOut = LogOut.Create(data, logPath))
        {
            logOut.WriteOutNuclides();
            logOut.WriteOutCompartments();
            logOut.WriteOutTransfers();

            InputDataReader_OIR.SetSCoefficients(data);
            logOut.WriteOutScoefficients();
        }

        using (var calcOut = new CalcOut(data, OutputDirectory, OutputFileName))
        {
            // OIRでは、集合コンパートメントを処理するための準備を行う。
            calcOut.PrepareCompositeCompartments();

            // コンパートメントの名称をヘッダ―として出力。
            calcOut.ActivityHeader();

            // 標的領域の名称をヘッダーとして出力。
            calcOut.CommitmentHeader();

            MainCalc(data, calcOut, cancellationToken);
        }
    }

    private void MainCalc(InputData data, CalcOut calcOut, CancellationToken cancellationToken)
    {
        const double convergence = 1E-10; // 収束値
        const int iterMax = 1500;  // iterationの最大回数

        // 標的領域の組織加重係数を取得。
        var targetWeights = data.TargetWeights;

        // 計算時間メッシュを準備する。
        var calcTimes = ComputeTimeMesh.Start();
        long calcPreT;
        long calcNowT = calcTimes.Current;
        int calcIter;   // 計算時間メッシュ毎の収束計算回数

        // 出力時間メッシュを準備する。
        var outTimes = OutputTimeMesh.Start();
        long outPreT;
        long outNowT = outTimes.Current;
        int outIter;    // 出力時間メッシュ毎の収束計算回数

        var wholeBodyNow = 0.0; // 今回の出力時間メッシュにおける全身の積算線量。
        var wholeBodyPre = 0.0; // 前回の出力時間メッシュにおける全身の積算線量。
        var resultNowM = new double[43]; // 今回の出力時間メッシュにおける組織毎の計算結果。
        var resultNowF = new double[43]; // 今回の出力時間メッシュにおける組織毎の計算結果。
        var resultPreM = new double[43]; // 前回の出力時間メッシュにおける組織毎の計算結果。
        var resultPreF = new double[43]; // 前回の出力時間メッシュにおける組織毎の計算結果。

        var res = new RetentionSet(data);

        // inputの初期値を各コンパートメントに振り分ける。
        SubRoutine.Init(res, data);

        // 初期配分直後の放射能をファイルに出力する。
        calcOut.ActivityOut(0.0, res, 0);

        var progressValue = 0.0;
        ProgressIndicator?.Report(progressValue);

        // 出力時間メッシュを進める。
        outTimes.MoveNext();
        outPreT = outNowT;
        outNowT = outTimes.Current;
        outIter = 0;

        const long Delta24hourT = 24 * 60 * 60;
        long outLastExcretaT = outPreT; // excコンパートメントで最後に数値を出力した時間メッシュ
        long outBefore24hourT = outNowT - Delta24hourT;

        // 計算時間メッシュを進める。
        while (calcTimes.MoveNext())
        {
            cancellationToken.ThrowIfCancellationRequested();

            calcPreT = calcNowT;
            calcNowT = calcTimes.Current;

            // 預託期間を超える計算は行わない
            if (CommitmentPeriod < calcNowT)
                break;

            var newProgressValue = (int)((double)calcNowT / CommitmentPeriod * 100);
            if (newProgressValue != progressValue)
            {
                progressValue = newProgressValue;
                ProgressIndicator?.Report(progressValue);
            }

            var calcNowDay = TimeMesh.SecondsToDays(calcNowT);

            // ΔT[sec]
            var calcDeltaT = calcNowT - calcPreT;
            var calcDeltaDay = TimeMesh.SecondsToDays(calcDeltaT);

            res.NextCalc(data);

            #region 1つの計算時間メッシュ内で収束計算を繰り返す
            for (calcIter = 1; calcIter <= iterMax; calcIter++)
            {
                foreach (var organ in data.Organs)
                {
                    // 流入がないコンパートメントの計算をスキップする。
                    if (organ.IsZeroInflow)
                        continue;

                    var func = organ.Func; // 臓器機能

                    // 臓器機能ごとに異なる処理をする
                    if (func == OrganFunc.inp) // 入力
                    {
                        SubRoutine.Input(organ, res);
                    }
                    else if (func == OrganFunc.acc) // 蓄積
                    {
                        SubRoutine.Accumulation_OIR(organ, res, calcDeltaDay);
                    }
                    else if (func == OrganFunc.mix) // 混合
                    {
                        SubRoutine.Mix(organ, res);
                    }
                    else if (func == OrganFunc.exc) // 排泄
                    {
                        SubRoutine.Excretion(organ, res, calcDeltaDay);
                    }
                }

                // 前回との差が収束するまで計算を繰り返す
                if (res.NextIter(data, convergence))
                    continue;

                // 出力時間メッシュと終端が一致する計算時間メッシュにおける反復回数を保存する。
                outIter = calcIter;

                // // 出力時間メッシュ内での総反復回数を保存する。
                // outIter += calcIter;
                break;
            }
            #endregion

            res.FinishIter();

            if (calcNowT <= outBefore24hourT)
            {
                foreach (var organ in data.Organs)
                {
                    if (!organ.IsExcretaCompatibleWithOIR)
                        continue;

                    // OIR互換排泄コンパートメントについて、
                    // 残留放射能をカウントすべき24時間より以前の結果を捨てる。
                    res.CalcNow[organ.Index].end = 0;
                }
            }

            // 時間メッシュ毎の原子数を足していく
            foreach (var organ in data.Organs)
            {
                var calcNowTotal = res.CalcNow[organ.Index].total;

                // 今回の出力時間メッシュにおける積算原子数。
                res.OutNow[organ.Index].total += calcNowTotal;

                // 摂取時からの積算原子数。
                res.OutTotalFromIntake[organ.Index] += calcNowTotal;
            }

            foreach (var organ in data.Organs)
            {
                // コンパートメントが線源領域に対応しない場合は何もしない。
                if (organ.SourceRegion is null)
                    continue;

                // コンパートメントの残留放射能がゼロの場合は何もしない。
                var activity = organ.Nuclide.Lambda * res.CalcNow[organ.Index].ave * calcDeltaT;
                if (activity == 0)
                    continue;

                // コンパートメントから各標的領域への預託線量を計算する。
                for (int indexT = 0; indexT < 43; indexT++)
                {
                    // 標的領域の部分的な重量。
                    var targetWeight = targetWeights[indexT];

                    // S係数(男女別)。
                    var scoeffM = organ.S_CoefficientsM[indexT];
                    var scoeffF = organ.S_CoefficientsF[indexT];

                    // 等価線量 = 放射能 * S係数(男女別)
                    var equivalentDoseM = activity * scoeffM;
                    var equivalentDoseF = activity * scoeffF;

                    // 実効線量 = 等価線量(男女平均) * wT
                    var effectiveDose = (equivalentDoseM + equivalentDoseF) / 2 * targetWeight;

                    resultNowM[indexT] += equivalentDoseM;
                    resultNowF[indexT] += equivalentDoseF;
                    wholeBodyNow += effectiveDose;
                }
            }

            if (calcNowT == outNowT)
            {
                var outDeltaT = outNowT - outPreT;

                var outNowDay = TimeMesh.SecondsToDays(outNowT);
                var outPreDay = TimeMesh.SecondsToDays(outPreT);
                var outDeltaDay = TimeMesh.SecondsToDays(outDeltaT);

                if (outDeltaT < Delta24hourT)
                {
                    // 24時間より小さい幅を持つ出力時間メッシュは、それらを合わせた時間幅が
                    // 前回の'exc'コンパートメントに対する数値出力から24-hour経過した時間と一致することをここで確認する。
                    // - 'exc'の残留放射能の出力に寄与しない出力時間メッシュがないことを保証する。
                    // - これを許した場合、excの積算放射能を構成する複数の出力時間メッシュのうち、24-hourを超える古い側の
                    //   メッシュの寄与を後から減算しなければならず、この面倒な処理を避けるという理由もある。
                    // - 本来は計算開始前にTimeMesh入力検証で確認すべきだが、現在はOIRの'exc'に特有の処理であるためここで確認している。
                    if ((outNowT - outLastExcretaT) > Delta24hourT)
                        throw new Exception("sum of out time meshes those are smaller than 24-hour delta should match to 24-hour, "
                                          + "for the 'exc' compartment output compatibility with OIR.");
                }

                // OIR互換排泄コンパートメントについて、前回の数値出力からの
                // 経過時間が24時間に満たない場合は、数値出力を抑制する。
                var maskExcreta = outNowT < outLastExcretaT + Delta24hourT;

                // 出力時間メッシュにおける平均と末期の原子数を計算する。
                foreach (var organ in data.Organs)
                {
                    res.OutNow[organ.Index].ave = res.OutNow[organ.Index].total / outDeltaDay;

                    res.OutNow[organ.Index].end = res.CalcNow[organ.Index].end;
                }

                // 放射能をファイルに出力する。
                calcOut.ActivityOut(outNowDay, res, outIter, maskExcreta);

                // 線量をファイルに出力する。
                calcOut.CommitmentOut(outNowDay, outPreDay, wholeBodyNow, wholeBodyPre, resultNowM, resultPreM, Sex.Male);
                calcOut.CommitmentOut(outNowDay, outPreDay, wholeBodyNow, wholeBodyPre, resultNowF, resultPreF, Sex.Female);

                // これ以上出力時間メッシュが存在しないならば、計算を終了する。
                if (!outTimes.MoveNext())
                    break;

                // 出力時間メッシュを進める。
                outPreT = outNowT;
                outNowT = outTimes.Current;
                outIter = 0;

                if (!maskExcreta)
                {
                    outLastExcretaT = outPreT;

                    foreach (var organ in data.Organs)
                    {
                        if (!organ.IsExcretaCompatibleWithOIR)
                            continue;

                        // OIR互換排泄コンパートメントについて、原子数をゼロクリアする。
                        res.CalcNow[organ.Index].end = 0;
                    }
                }
                outBefore24hourT = outNowT - Delta24hourT;

                res.NextOut(data);

                wholeBodyPre = wholeBodyNow;
                Array.Copy(resultNowM, resultPreM, resultNowM.Length);
                Array.Copy(resultNowF, resultPreF, resultNowF.Length);
            }
        }

        // 計算完了の出力を行う。
        calcOut.FinishOut();

        progressValue = 100;
        ProgressIndicator?.Report(progressValue);
    }
}
