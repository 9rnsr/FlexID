using System.Diagnostics;

namespace FlexID.Calc;

/// <summary>
/// インプットで設定されてる壊変経路の集合から、
/// 暗黙に構成される崩壊系列の集合を計算する。またこれらの中で
/// 暗黙に作成される壊変コンパートメントを取り扱う。
/// </summary>
internal class DecaySet
{
    /// <summary>
    /// 処理対象となる核種のリスト。
    /// </summary>
    public IReadOnlyList<NuclideData> Nuclides { get; }

    /// <summary>
    /// 壊変経路設定時に検出したエラーの格納先。
    /// </summary>
    public InputErrors Errors { get; }

    /// <summary>
    /// 作成した崩壊系列のリスト。
    /// </summary>
    public List<DecayChain> DecayChains { get; } = [];

    /// <summary>
    /// 壊変経路のために作成したコンパートメントのリスト。
    /// </summary>
    public List<Organ> DecayCompartments { get; } = [];

    /// <summary>
    /// コンパートメントをキーとし、当該位置を共有する崩壊系列のリストを値とする辞書。
    /// </summary>
    public Dictionary<Organ, List<DecayChain>> JointChains { get; } = [];

    /// <summary>
    /// 移行速度付きの壊変経路が移行した先(organDecay)をキーとし、そこから同じ核種のまま移行した先とその係数を値とする辞書。
    /// </summary>
    private Dictionary<Organ, (Organ OrganTo, decimal? Coeff)> AfterDecayPath { get; } = [];

    /// <summary>
    /// コンストラクタ。
    /// </summary>
    /// <param name="nuclides"></param>
    /// <param name="errors"></param>
    public DecaySet(IReadOnlyList<NuclideData> nuclides, InputErrors errors)
    {
        Nuclides = nuclides;
        Errors = errors;

        nuclideAncestors = Nuclides.Select(root =>
        {
            // 対象核種を末端とする崩壊系列を構成する祖先核種を列挙する。
            var results = new List<NuclideData> { root };

        Lagain:
            var count = results.Count;
            foreach (var nuclide in Nuclides.Except(results))
            {
                // 系列を構成する核種の親を追加する。
                var daughters = nuclide.Branches.Select(b => b.Daughter);
                if (daughters.Any(d => results.Contains(d)))
                    results.Add(nuclide);
            }
            // 系列を構成する核種が増えなくなるまで繰り返す。
            if (results.Count > count)
                goto Lagain;

            // rootを除いた、祖先核種のみの配列を返す。
            results.RemoveAt(0);
            return (IReadOnlyList<NuclideData>)results;
        }).ToArray();

        nuclideProgenies = Nuclides.Select(root =>
        {
            // 対象核種を始点とする崩壊系列を構成する子孫核種を列挙する。
            var results = new List<NuclideData> { root };

        Lagain:
            var count = results.Count;
            for (int i = 0; i < count; i++)
            {
                // 系列を構成する核種の娘を追加する。
                var nuclide = results[i];
                var daughters = nuclide.Branches.Select(b => b.Daughter);
                results.AddRange(daughters.Where(d => !results.Contains(d)));
            }
            // 系列を構成する核種が増えなくなるまで繰り返す。
            if (results.Count > count)
                goto Lagain;

            // rootを除いた、子孫核種のみの配列を返す。
            results.RemoveAt(0);
            return (IReadOnlyList<NuclideData>)results;
        }).ToArray();
    }

    private readonly IReadOnlyList<NuclideData>[] nuclideAncestors;

    private readonly IReadOnlyList<NuclideData>[] nuclideProgenies;

    private IReadOnlyList<NuclideData> GetAncestors(NuclideData nuclide) => nuclideAncestors[nuclide.Index];

    private IReadOnlyList<NuclideData> GetProgenies(NuclideData nuclide) => nuclideProgenies[nuclide.Index];

    /// <summary>
    /// 対象コンパートメントの核種から始まる崩壊系列の情報を保持する。
    /// </summary>
    internal class DecayChain
    {
        /// <summary>
        /// コンストラクタ。
        /// </summary>
        /// <param name="nuclideCount">核種の総数。</param>
        public DecayChain(int nuclideCount)
        {
            Compartments = new (int, Organ)[nuclideCount];
        }

        /// <summary>
        /// 崩壊系列において、これを構成する核種毎の位置を占めるコンパートメントを保持する。
        /// </summary>
        public (int LineNum, Organ Organ)[] Compartments;

        /// <summary>
        /// 指定の子孫核種に対応する壊変コンパートメントを取得する。
        /// </summary>
        /// <param name="progeny"></param>
        /// <returns></returns>
        public ref (int LineNum, Organ Organ) this[NuclideData progeny] => ref Compartments[progeny.Index];

        public bool IsSubsetOf(DecayChain other)
        {
            return other.Compartments.Zip(this.Compartments)
                        .All(x => x.Item1.Organ == x.Item2.Organ || x.Item2.Organ is null);
        }

        public override string ToString()
        {
            return string.Join(" ; ", Compartments.Select(c => c.Organ?.ToString() ?? "null"));
        }
    }

    /// <summary>
    /// 新しい壊変経路を定義する。
    /// </summary>
    /// <param name="lineNum"></param>
    /// <param name="organFrom"></param>
    /// <param name="organTo"></param>
    /// <param name="coeff"></param>
    /// <remarks>organFrom + organTo + coeffの組み合わせは高々1回だけ与えられることを前提としている。</remarks>
    /// <returns></returns>
    public Organ AddDecayPath(int lineNum, Organ organFrom, Organ organTo, decimal? coeff)
    {
        var nuclideFrom = organFrom.Nuclide;
        var nuclideTo = organTo.Nuclide;
        var hasCoeff = coeff is not null;

        // 分岐比が不明な壊変経路は定義できない。
        if (!GetProgenies(nuclideFrom).Contains(nuclideTo))
        {
            Errors.AddError(lineNum, $"There is no decay path from {nuclideFrom.Name} to {nuclideTo.Name}.");
            return null;
        }

        // organFrom位置を共有する崩壊系列の1つをtargetChainとして取得する。
        DecayChain targetChain;
        if (JointChains.TryGetValue(organFrom, out var chainsFrom))
        {
            targetChain = chainsFrom[0];
        }
        else if (!hasCoeff &&
            JointChains.TryGetValue(organTo, out chainsFrom) && chainsFrom.Count == 1 &&
            GetAncestors(nuclideTo).All(ancestor => chainsFrom[0][ancestor].Organ is null))
        {
            // nuclideTo位置を通るただ1つの崩壊系列が、nuclideToの祖先核種全てについてコンパートメント未設定の場合に、
            // これをorganFrom位置も通る崩壊系列として扱い、これに対してorganFromからorganToへの経路を「接ぎ木」する。
            targetChain = chainsFrom[0];
            targetChain[nuclideFrom] = (lineNum, organFrom);

            chainsFrom = [targetChain];
            JointChains.Add(organFrom, chainsFrom);
        }
        else
        {
            targetChain = new DecayChain(Nuclides.Count);
            DecayChains.Add(targetChain);

            ref var decayFrom = ref targetChain[nuclideFrom];
            decayFrom = (lineNum, organFrom);

            chainsFrom = [targetChain];
            JointChains.Add(organFrom, chainsFrom);
        }

        // organFromから壊変によって生成された子孫核種を
        // nuclideTo位置で受けるためのコンパートメントorganDecayを取得する。
        Organ organDecay;
        {
            var decayTo = targetChain[nuclideTo];
            bool consistentPath;
            if (hasCoeff)
            {
                organDecay = decayTo.Organ;

                if (decayTo.Organ is null)
                {
                    organDecay = CreateDecayCompartment(nuclideTo, organFrom.Func);
                    AfterDecayPath.Add(organDecay, (organTo, coeff));
                    consistentPath = true;
                }
                else
                {
                    // 係数付きの壊変経路は、nuclideTo位置を高々一度だけ占めることができる。
                    // 言い換えると、以前の経路設定でnuclideTo位置を(係数あり・なしにかかわらず)
                    // 既に占めている場合はエラーにする。
                    #region 詳細な説明

                    // 係数なしの壊変経路の場合は、同じ崩壊系列を辿る場合でもfromが違えば経路設定が可能となっている。
                    // 具体例を示すと：
                    //   N1/C1 -> N2/C2 -> N3/C3 の崩壊系列に対して以下の3つの経路設定を行う場合に、
                    //   - N1/C1 -> N2/C2
                    //   - N1/C1 -> N3/C3 (経路A)
                    //   - N2/C2 -> N3/C3 (経路B)
                    //   これらをどのような順序で設定しても経路Aと経路Bが衝突なく受け付けられる。
                    //   このとき、設定順序によってはDecayChainが2個作成され得るが、その場合でも設定順序に関係なく
                    //   2つのDecayChainのN1,N2,N3位置のそれぞれはC1,C2,C3の3つのコンパートメントによって共有され、
                    //   従って最終的にただ1つの崩壊系列を構成するためである。
                    //
                    // しかし係数付きの壊変経路では、organDecayを経路設定時に明示できないため、もしここで禁止している
                    // 経路設定を受け付けると、設定順序に依存する挙動が生じる。
                    // 具体例を示すと：
                    //   N1/C1 -> N2/C2 -> N3/C3 の崩壊系列に対して以下の3つの経路設定を行う場合に、
                    //   - N1/C1 -> N2/C2
                    //   - N1/C1 -> N3/C3 + 移行速度 (経路X)
                    //   - N2/C2 -> N3/C3 + 移行速度 (経路Y)
                    //
                    //     設定順序A：
                    //     (1) N1/C1 -> N2/C2
                    //     (2) N1/C1 -> N3/C3 + 移行速度 (経路X)
                    //     (3) N2/C2 -> N3/C3 + 移行速度 (経路Y)
                    //     …(2)の時点で DecayChain1 { C1, C2, organDecay } だけが作成される
                    //     …(3)の時点で設定される経路Yは、この1つだけ存在するDecayChain1の一部と判断できるため、
                    //       経路Yの設定は経路Xと衝突せず受け付けられる。
                    //     
                    //     設定順序B：
                    //     (1) N1/C1 -> N3/C3 + 移行速度 (経路X)
                    //     (2) N2/C2 -> N3/C3 + 移行速度 (経路Y)
                    //     (3) N1/C1 -> N2/C2
                    //     …(1)の時点で DecayChain1 { C1, null, organDecay1 } が作成される。
                    //     …(2)の時点で DecayChain2 { null, C2, organDecay2 } が作成される。これは経路Yの設定が
                    //       N3位置についてorganDecay1を明示できず、従ってDecayChain1とこれを共有するかどうかを
                    //       この時点では判断できないためである。
                    //       ※ 例えば、もしこの後に N1/C1 -> N4/C41 と N2/C2 -> N4/C42 という2つの経路設定が行われる場合、
                    //          明らかにDecayChain1とDecayChain2は独立した2つの崩壊系列を構成することになる。
                    //     …(3)の経路設定によって、DecayChain1とDecayChain2の2つがN1,N2位置についてそれぞれC1,C2を共有する形が作られるが、
                    //       N3位置についてはorganDecay1とorganDecay2という異なるコンパートメントが設定済みであるため、
                    //       2つの崩壊系列の分岐が検出されてエラーが報告される。
                    //
                    // このように設定順序に依存してエラーの有無が変わることを避けるため、
                    // 係数付きの壊変経路では、経路Yを設定しようとした時点でエラーを報告する仕様としている。

                    #endregion
                    consistentPath = false;
                }
            }
            else
            {
                organDecay = organTo;

                if (decayTo.Organ is null)
                {
                    consistentPath = true;
                }
                else
                {
                    // 以前に設定された係数なしの壊変経路と、organDecay(== organTo)が整合している。
                    consistentPath = decayTo.Organ == organDecay;
                }
            }

            if (!consistentPath)
            {
                // 今回と前回の矛盾する2つの設定経路について、
                // nuclideFrom位置からnuclideTo位置までを表示する。
                var (prevTo, prevCoeff) = AfterDecayPath.GetValueOrDefault(decayTo.Organ, (decayTo.Organ, null));
                var prev = $"'{prevTo.ToString()}'";
                if (prevCoeff is decimal v) prev += $" (with coeff. {v})";

                var here = $"'{organTo.ToString()}'";
                if (hasCoeff) here += $" (with coeff. {coeff.Value})";

                Errors.AddError(lineNum, "Decay paths conflict each other:");
                Errors.AddError($"    the previous: '{organFrom}' --> {prev} at Line {decayTo.LineNum}");
                Errors.AddError($"    and here    : '{organFrom}' --> {here}");
                return null;
            }
        }

        // nuclideTo位置に新たに合流する崩壊系列の集合を取得する。
        var chainsInflow = CalcChainsInflow(targetChain, nuclideTo);

        // organDecay位置を共有する崩壊系列のリストを取得する。
        if (!JointChains.TryGetValue(organDecay, out var chainsTo))
        {
            chainsTo = chainsInflow;

            // chainsToのnuclideTo位置にorganDecayを設定する。
            foreach (var chain in chainsTo)
            {
                Debug.Assert(chain[nuclideTo].Organ is null);
                chain[nuclideTo] = (lineNum, organDecay);
            }

            // organDecay位置を共有する崩壊系列群を設定する。
            JointChains.Add(organDecay, chainsTo);
        }
        else
        {
            // chainsToに含まれる、言い換えるとnuclideTo位置を既に
            // organDecayで占めている崩壊系列をchainsInflowから除外する。
            // その結果、chainsToへ追加されるべき崩壊系列が0個となる場合は何もしない。
            chainsInflow.RemoveAll(chain => chainsTo.Contains(chain));
            if (chainsInflow.Count == 0)
                goto Lend;

            // ここからの処理で、
            // organDecay位置を既に占めている崩壊系列群chainsToに
            // organDecay位置へ新たに合流する崩壊系列群chainsInflowを追加する。

            // chainsInflowのnuclideTo位置にorganDecayを設定する。
            foreach (var chain in chainsInflow)
            {
                Debug.Assert(chain[nuclideTo].Organ is null);
                chain[nuclideTo] = (lineNum, organDecay);
            }

            // chainsToとchainsInflowの間に、nuclideToより子孫核種の位置で矛盾がないことを確認する。
            // 比較処理のために、2つの集合からそれぞれ1つずつ代表となる崩壊系列を取得する。
            var chainTo = chainsTo[0];
            var chainInflow = chainsInflow[0];
            foreach (var progeny in GetProgenies(nuclideTo))
            {
                var decayTo = chainTo[progeny];
                var decayInflow = chainInflow[progeny];

                // 全ての崩壊系列がprogeny位置をまだ共有していない場合は何もしない。
                if (decayInflow.Organ is null && decayTo.Organ is null)
                    continue;

                if (decayTo.Organ is null)
                {
                    foreach (var chain in chainsTo)
                        chain[progeny] = decayInflow;
                }
                else if (decayInflow.Organ is null)
                {
                    foreach (var chain in chainsInflow)
                        chain[progeny] = decayTo;
                }
                else if (decayTo.Organ != decayInflow.Organ)
                {
                    // nuclideTo位置より子孫のコンパートメントが全て共有されるべきにもかかわらず
                    // nuclideToの子孫核種progenyの位置で分岐が生じてしまう2つの壊変経路について、
                    // nuclideTo位置からprogeny位置までを表示する。
                    var organStart = chainTo[nuclideTo].Organ;

                    var (prevTo, prevCoeff) = AfterDecayPath.GetValueOrDefault(decayTo.Organ, (decayTo.Organ, null));
                    var prev = $"'{prevTo.ToString()}'";
                    if (prevCoeff.HasValue) prev += $" (with coeff. {prevCoeff.Value})";

                    var (hereTo, hereCoeff) = AfterDecayPath.GetValueOrDefault(decayInflow.Organ, (decayInflow.Organ, null));
                    var here = $"'{hereTo.ToString()}'";
                    if (hereCoeff.HasValue) here += $" (with coeff. {hereCoeff.Value})";

                    Errors.AddError(lineNum, "Decay paths conflict each other:");
                    Errors.AddError($"    the previous: '{organStart}' --> {prev} at Line {decayTo.LineNum}");
                    Errors.AddError($"    and another : '{organStart}' --> {here} at Line {decayInflow.LineNum}");
                    return null;
                }

                // progeny位置を共有する崩壊系列にchainsInflowを追加する。
                var chainsProgeny = JointChains[decayTo.Organ];
                foreach (var chain in chainsInflow)
                {
                    if (!chainsProgeny.Contains(chain))
                        chainsProgeny.Add(chain);
                }
            }

            chainsTo.AddRange(chainsInflow);
        }

    Lend:
        // chainsToの全ての崩壊系列は、nuclideTo位置でorganDecayコンパートメントを共有している。
        Debug.Assert(chainsTo.All(chain => chain[nuclideTo].Organ == organDecay));

        return hasCoeff ? organDecay : null;
    }

    /// <summary>
    /// targetChain上でnuclideTo位置に流入している崩壊系列の集合を計算する。
    /// </summary>
    /// <param name="targetChain"></param>
    /// <param name="nuclideTo"></param>
    /// <returns>結果を新しいリストに格納して返す。結果には少なくともtargetChainが含まれる。</returns>
    private List<DecayChain> CalcChainsInflow(DecayChain targetChain, NuclideData nuclideTo)
    {
        List<DecayChain> results = [targetChain];

        // targetChain上で、nuclideToの祖先核種の位置を確認し、コンパートメントが設定済みの箇所については
        // それらの位置を共有する崩壊系列を全て取得しそれらの集合を返す。
        foreach (var nuclide in GetAncestors(nuclideTo))
        {
            if (targetChain[nuclide].Organ is not Organ organ)
                continue;
            foreach (var chain in JointChains[organ])
            {
                if (!results.Contains(chain))
                    results.Add(chain);
            }
        }

        return results;
    }

    /// <summary>
    /// 核種が異なるコンパートメント間の移行経路を定義する。
    /// </summary>
    /// <param name="compartments"></param>
    public void DefineDecayTransfers(IList<Organ> compartments)
    {
        foreach (var decayChain in DecayChains)
        {
            var froms = decayChain.Compartments.Select(o => o.Organ).Where(o => o != null).ToList();

            // デバッグ用
            var decayCompartmentName = $"<decay_from_{froms[0].Nuclide.Name}/{froms[0].Name}>";

            // 崩壊系列を構成するコンパートメント間の壊変経路を追加定義する。
            // 壊変経路は一本道ではなく、親核種から始まる有効非巡回グラフ(DAG)を構成する点に注意。
            while (froms.Count != 0)
            {
                var organFrom = froms[0];

                foreach (var (daughter, fraction) in organFrom.Nuclide.Branches)
                {
                    ref var decayTo = ref decayChain[daughter];
                    var organTo = decayTo.Organ;

                    if (organTo is null)
                    {
                        organTo = CreateDecayCompartment(daughter, organFrom.Func);
                        decayTo.Organ = organTo;
                        froms.Add(organTo);
                    }
                    else if (organTo.Inflows.Any(inflow => inflow.Organ == organFrom))
                    {
                        // 壊変経路が既に設定されている場合は何もしない。
                        continue;
                    }

                    organTo.Inflows.Add(new Inflow
                    {
                        ID = organFrom.ID,
                        Rate = fraction,    // 壊変経路では、親からの分岐比を移行割合としてとする。
                        Organ = organFrom,  // 流入経路から流入元臓器の情報を直接引くための参照を設定する。
                    });

                    if (organTo.IsDecayCompartment)
                    {
                        organTo.Name = decayCompartmentName;

                        SetSourceRegion(compartments, organFrom, organTo);
                    }
                }

                froms.RemoveAt(0);
            }
        }

        var index = compartments.Count;
        foreach (var organDecay in DecayCompartments)
        {
            organDecay.ID = index + 1;
            organDecay.Index = index;
            compartments.Add(organDecay);
            index++;
        }
    }

    /// <summary>
    /// 指定の子孫核種に対応する壊変コンパートメントを作成する。
    /// </summary>
    /// <param name="nuclide"></param>
    /// <param name="func"></param>
    /// <returns>作成した壊変コンパートメント。</returns>
    private Organ CreateDecayCompartment(NuclideData nuclide, OrganFunc func)
    {
        var organDecay = new Organ
        {
            Nuclide = nuclide,
            ID = 0,         // 後で設定する。
            Index = -1,     // 後で設定する。
            Name = "",      // 後で設定する。
            Func = func,
            BioDecay = 1.0, // accは後で設定する。
            IsDecayCompartment = true,
        };
        DecayCompartments.Add(organDecay);

        return organDecay;
    }

    /// <summary>
    /// 壊変コンパートメントの線源領域を、親コンパートメントの線源領域から
    /// 導出して設定する。
    /// </summary>
    /// <param name="compartments"></param>
    /// <param name="organFrom"></param>
    /// <param name="organDecay"></param>
    private void SetSourceRegion(IList<Organ> compartments, Organ organFrom, Organ organDecay)
    {
        Debug.Assert(organDecay.IsDecayCompartment);

        var sourceRegion = organFrom.SourceRegion;
        if (sourceRegion != null)
        {
            var daughter = organDecay.Nuclide;

            // 親核種のコンパートメントに設定された線源領域が、
            // 子孫核種の動態モデルおいても明示的に設定されているかどうかを調べる。
            var explicitUsed = compartments.Where(o => o.Nuclide == daughter)
                                           .Any(o => o.SourceRegion == sourceRegion);

            // 明示的に設定されていない場合は、ambiguous compartmentを'Other'に割り当てる。
            if (!explicitUsed)
                sourceRegion = "Other";

            organDecay.SourceRegion = sourceRegion;
        }
    }
}
