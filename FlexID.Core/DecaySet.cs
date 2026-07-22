using System.Diagnostics;

namespace FlexID;

/// <summary>
/// インプットで設定されてる壊変経路の集合から、
/// 暗黙に構成される崩壊系列の集合を計算する。またこれらの中で
/// 暗黙に作成される壊変コンパートメントを取り扱う。
/// </summary>
public class DecaySet
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
    /// 定義済みのコンパートメントをキーとし、当該位置を共有する崩壊系列のリストを値とする辞書。
    /// </summary>
    public Dictionary<Organ, List<DecayChain>> JointChains { get; } = [];

    /// <summary>
    /// 移行速度付きの壊変経路に対して自動作成されたOrganDecayをキーとし、
    /// そこから同じ核種のまま移行した先であるOragnToとその係数Coeffを値とする辞書。
    /// </summary>
    public Dictionary<Organ, (Location Loc, (decimal Coeff, Organ OrganTo) Outflow)> AfterDecayPath { get; } = [];

    /// <summary>
    /// コンストラクタ。
    /// </summary>
    /// <param name="nuclides"></param>
    /// <param name="errors"></param>
    public DecaySet(IReadOnlyList<NuclideData> nuclides, InputErrors errors)
    {
        Nuclides = nuclides;
        Errors = errors;

        nuclideAncestors = [.. Nuclides.Select(root =>
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
        })];

        nuclideProgenies = [.. Nuclides.Select(root =>
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
        })];
    }

    private readonly IReadOnlyList<NuclideData>[] nuclideAncestors;

    private readonly IReadOnlyList<NuclideData>[] nuclideProgenies;

    public IReadOnlyList<NuclideData> GetAncestors(NuclideData nuclide) => nuclideAncestors[nuclide.Index];

    public IReadOnlyList<NuclideData> GetProgenies(NuclideData nuclide) => nuclideProgenies[nuclide.Index];

    /// <summary>
    /// 対象コンパートメントの核種から始まる崩壊系列の情報を保持する。
    /// </summary>
    public class DecayChain
    {
        /// <summary>
        /// コンストラクタ。
        /// </summary>
        /// <param name="nuclideCount">核種の総数。</param>
        public DecayChain(int nuclideCount)
        {
            Compartments = new (Location, Organ?)[nuclideCount];
        }

        /// <summary>
        /// 崩壊系列において、これを構成する核種毎の位置を占めるコンパートメントを保持する。
        /// </summary>
        public (Location Loc, Organ? Organ)[] Compartments;

        /// <summary>
        /// 指定の子孫核種に対応する壊変コンパートメントを取得する。
        /// </summary>
        /// <param name="progeny"></param>
        /// <returns></returns>
        public ref (Location Loc, Organ? Organ) this[NuclideData progeny] => ref Compartments[progeny.Index];

        public bool IsSubsetOf(DecayChain other) =>
            other.Compartments
                 .Zip(this.Compartments)
                 .All(x => x.First.Organ == x.Second.Organ || x.Second.Organ is null);

        public override string ToString()
        {
            return string.Join(" ; ", Compartments.Select(c => c.Organ?.ToString() ?? "null"));
        }
    }

    /// <summary>
    /// 新しい壊変経路を定義する。
    /// </summary>
    /// <param name="loc">位置情報。</param>
    /// <param name="organFrom"></param>
    /// <param name="organTo"></param>
    /// <param name="coeff"></param>
    public void AddDecayPath(Location loc, Organ organFrom, Organ organTo, decimal? coeff)
    {
        var nuclideFrom = organFrom.Nuclide;
        var nuclideTo = organTo.Nuclide;

        // 分岐比が不明な壊変経路は定義できない。
        if (!GetProgenies(nuclideFrom).Contains(nuclideTo))
        {
            Errors.AddError(loc, $"There is no decay path from {nuclideFrom.Name} to {nuclideTo.Name}.");
            return;
        }

        List<DecayChain> GetJointChainsAt(Organ organ)
        {
            if (!JointChains.TryGetValue(organ, out var chains))
                JointChains.Add(organ, chains = []);
            return chains;
        }

        void AddJointChainsAt(Location loc, Organ organ, DecayChain chain)
        {
            chain[organ.Nuclide] = (loc, organ);

            if (!JointChains.TryGetValue(organ, out var chains))
                JointChains.Add(organ, [chain]);
            else if (!chains.Contains(chain))
                chains.Add(chain);
        }

        // organFrom位置を共有する崩壊系列のリストを取得する。
        var chainsFrom = GetJointChainsAt(organFrom);

        // 移行速度付きの壊変経路を処理する場合は、organToを
        // organFromで生成された核種をnuclideTo位置で直接受け止めるための
        // 暗黙生成されるコンパートメントに置き換える。
        if (coeff is not null)
        {
            // 元々のorganToを、崩壊系列のnuclideTo位置から移行係数coeffで流出する経路の移行先として保存する。
            var organOutflowTo = organTo;

            // 新しいorganToとして、organFrom位置を共有する崩壊系列において既にnuclideTo位置を占めるものが
            // 有ればそれを、無ければ新しい壊変コンパートメントを取得する。
            organTo = chainsFrom.FirstOrDefault()?[nuclideTo].Organ
                   ?? CreateDecayCompartment(nuclideTo, organFrom.Func);

            if (organTo.IsDecayCompartment == false)
            {
                // 以前に設定されたorganFromからorganToへ速度なし壊変経路が
                // 今回の設定と衝突するため、これについてエラーを報告する。
                var pre = chainsFrom[0][nuclideTo].Loc;
                Errors.AddError(loc, $"Conflict of decay path definitions found:");
                Errors.AddError(pre, $"    previous, decay to '{organTo}' without coefficient,");
                Errors.AddError(loc, $"    and here, decay to '{organOutflowTo}' with coefficient {coeff.Value}.");
                return;
            }

            var path2 = (Loc: loc, Outflow: (Coeff: coeff.Value, OrganTo: organOutflowTo));
            if (!AfterDecayPath.TryGetValue(organTo, out var path1))
            {
                AfterDecayPath[organTo] = path2;
            }
            else if (path1.Outflow != path2.Outflow)
            {
                // 以前に設定されたorganToからnucideToモデル内へ流出する経路が
                // 今回の設定と衝突するため、これについてエラーを報告する。
                var pre = path1.Loc;
                var outflow1 = path1.Outflow;
                var outflow2 = path2.Outflow;
                Errors.AddError(loc, $"Conflict of decay path definitions found:");
                Errors.AddError(pre, $"    previous, decay to '{outflow1.OrganTo}' with coefficient {outflow1.Coeff},");
                Errors.AddError(loc, $"    and here, decay to '{outflow2.OrganTo}' with coefficient {outflow2.Coeff}.");
                return;
            }
        }

        // organTo位置を共有する崩壊系列のリストを取得する。
        var chainsTo = GetJointChainsAt(organTo);

        var ancestorsOfTo = GetAncestors(nuclideTo);

        if (chainsFrom.Count != 0)
        {
            // chainsFromに含まれる崩壊系列は、nuclideToより子孫の全ての位置について同じ共有状態を持つため
            // nuclideTo位置の整合性確認にはその1つだけを対象とすればよい。
            var chainFrom = chainsFrom[0];

            // organFromを共有している崩壊系列は、全てがorganTo位置も暗黙に共有していることになるため
            // ここでorganTo位置の共有を明示的に設定し、矛盾する場合はエラーを報告する。
            ref var decay = ref chainFrom[nuclideTo];
            if (decay.Organ is null)
            {
                // chainsToも更新される。
                foreach (var chain in chainsFrom)
                    AddJointChainsAt(loc, organTo, chain);
            }
            else if (decay.Organ != organTo)
            {
                ConflictDecayPathsError(decay!, (loc, organTo));
                return;
            }

            // nuclideFromより子孫の核種位置で合流しorganTo位置へ流入する崩壊系列の集合は、
            // organFrom位置を通過していないためchainsFromには含まれない、またその中でも
            // organTo位置をまだ共有していないものはchainsToにも含まれていないため、
            // ここでこれらを収集し、organTo位置の共有を設定する。
            foreach (var ancestor in GetProgenies(nuclideFrom).Intersect(ancestorsOfTo))
            {
                var organAncestor = chainFrom[ancestor].Organ;
                if (organAncestor is null)
                    continue;

                var chainsMore = JointChains[organAncestor].Except(chainsTo).ToList();
                if (chainsMore.Count == 0)
                    continue;

                // chainsMoreに含まれる崩壊系列は、ancestorより子孫の全ての位置について同じ共有状態を持つため
                // nuclideTo位置の整合性確認にはその1つだけを対象とすればよい。
                var chainMore = chainsMore[0];

                // chainsMoreはancestor位置でchainsFromと既に合流しているため、
                // もしnuclideTo位置について共有済み(decay.Organ != null)であるならば
                // すなわちchainsMoreはchainsToに既に含まれているはずであり、
                // これはchainsMoreの取得条件と矛盾するため、
                // 従ってnuclideTo位置については必ず未共有(decay.Organ is null)であることが成り立つ。
                decay = ref chainMore[nuclideTo];
                Debug.Assert(decay.Organ is null);

                // chainsToも更新されうる。
                foreach (var chain in chainsMore)
                    AddJointChainsAt(loc, organTo, chain);
            }
        }

        // 移行速度がない壊変経路では、organTo位置を共有する崩壊系列は、
        // 基本的にはnuclideFrom位置を共有せず、nuclideTo位置で合流するだけの
        // 独立した壊変経路の集合として考える必要がある。
        // 従ってここでは、nuclideToより祖先の位置全てにまだ共有を持たない崩壊系列群を対象に
        // organFrom位置の共有を設定する。
        foreach (var chain in chainsTo.Where(c => ancestorsOfTo.All(n => c[n].Organ is null)))
        {
            // chainsFromも更新される。
            AddJointChainsAt(loc, organFrom, chain);
        }

        if (chainsFrom.Count == 0)
        {
            // 今回設定されているorganFromからorganToへの壊変経路の情報を格納する
            // 既存の崩壊系列が存在しないため、これを行う新しい崩壊系列を作成する。
            var chain = new DecayChain(Nuclides.Count);
            DecayChains.Add(chain);

            AddJointChainsAt(loc, organFrom, chain);
            AddJointChainsAt(loc, organTo, chain);
        }
        else if (chainsTo.Count == 0)
        {
            // organFrom位置を共有する全ての崩壊系列は、
            // nuclideFrom位置より子孫の全ての位置についても既に共有済みであるため
            // ここではこれ以上何もしなくてよい。
            return;
        }

        var chainsMerged = chainsFrom.Concat(chainsTo).Distinct().ToList();
        if (chainsMerged.Count != 0)
        {
            // organToを共有する全ての崩壊系列は、nuclideTo位置より子孫の全ての位置を互いに共有する必要がある。
            foreach (var progeny in GetProgenies(nuclideTo))
            {
                var decay1 = chainsMerged.Select(chain => chain[progeny]).FirstOrDefault(d => d.Organ is not null);

                // progeny位置はchainsMergedのすべての崩壊系列で未設定なので、これ以上は何もしない。
                if (decay1.Organ is null)
                    continue;
                var organProgeny1 = decay1.Organ;

                foreach (var chain in chainsMerged)
                {
                    ref var decay2 = ref chain[progeny];
                    if (decay2.Organ is null)
                    {
                        AddJointChainsAt(decay1.Loc, organProgeny1, chain);
                        continue;
                    }
                    if (decay2.Organ == organProgeny1)
                        continue;
                    var organProgeny2 = decay2.Organ;

                    if (organProgeny1.IsDecayCompartment && organProgeny2.IsDecayCompartment)
                    {
                        var outflow1 = AfterDecayPath[organProgeny1].Outflow;
                        var outflow2 = AfterDecayPath[organProgeny2].Outflow;
                        if (outflow1 == outflow2)
                        {
                            // organProgenyからprogenyの動態モデルに入るための移行先と速度(outflow1)が
                            // decay.Organ からprogenyの動態モデルに入るためのもの(outflow2)と一致する場合は
                            // 後者を前者にマージする。
                            decay2.Organ = organProgeny1;

                            JointChains[organProgeny1].AddRange(JointChains[organProgeny2]);
                            JointChains.Remove(organProgeny2);

                            AfterDecayPath.Remove(organProgeny2);
                            continue;
                        }
                    }

                    ConflictDecayPathsError(decay1!, decay2!);
                    return;
                }
            }
        }

        void ConflictDecayPathsError((Location Loc, Organ Organ) decay1, (Location Loc, Organ Organ) decay2)
        {
            if (decay1.Loc.FilePath == decay2.Loc.FilePath &&
                decay1.Loc.LineNum > decay2.Loc.LineNum)
            {
                (decay1, decay2) = (decay2, decay1);
            }

            var (leading1, leading2) = decay2.Loc == loc
                ? ("previous,", "and here,")
                : ("previous, ", "and there,");

            Errors.AddError(loc, $"Conflict of decay path definitions found:");

            if (decay1.Organ.IsDecayCompartment)
            {
                var outflow1 = AfterDecayPath[decay1.Organ].Outflow;
                Errors.AddError(decay1.Loc, $"    {leading1} decay to '{outflow1.OrganTo}' with coefficient {outflow1.Coeff},");
            }
            else
            {
                Errors.AddError(decay1.Loc, $"    {leading1} decay to '{decay1.Organ}' without coefficient,");
            }

            if (decay2.Organ.IsDecayCompartment)
            {
                var outflow2 = AfterDecayPath[decay2.Organ].Outflow;
                Errors.AddError(decay2.Loc, $"    {leading2} decay to '{outflow2.OrganTo}' with coefficient {outflow2.Coeff}.");
            }
            else
            {
                Errors.AddError(decay2.Loc, $"    {leading2} decay to '{decay2.Organ}' without coefficient.");
            }
        }
    }

    /// <summary>
    /// 核種が異なるコンパートメント間の移行経路を定義する。
    /// </summary>
    /// <param name="compartments"></param>
    public void DefineDecayTransfers(IList<Organ> compartments)
    {
        foreach (var decayChain in DecayChains)
        {
            var froms = decayChain.Compartments.Select(o => o.Organ).OfType<Organ>().ToList();

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

                    // 壊変経路では、親の崩壊定数*親からの分岐比を移行割合として設定する。
                    var inflowRate = organFrom.Nuclide.Lambda * fraction;

                    organTo.Inflows.Add(new Inflow
                    {
                        ID = organFrom.ID,
                        Rate = inflowRate,
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
            // 親核種のコンパートメントに設定されていた線源領域を、
            // 生成核種を受ける壊変コンパートメントの線源領域としても設定する。
            organDecay.SourceRegion = sourceRegion;

            // 生成核種のモデルにおける線源領域Otherの内訳から、
            // 壊変コンパートメントの線源領域を(存在する場合は)取り除く。
            organDecay.Nuclide.OtherSourceRegions.Remove(sourceRegion);
        }
    }
}
