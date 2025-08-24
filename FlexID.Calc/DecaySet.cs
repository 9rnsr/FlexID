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

    public IList<Organ> Compartments { get; }

    private readonly List<(Organ from, Organ to, bool hasCoeff)> decayPaths;
    private readonly Dictionary<Organ, DecayChain> decayChains;
    private readonly Dictionary<NuclideData, NuclideData[]> decayNuclides;

    /// <summary>
    /// コンストラクタ。
    /// </summary>
    /// <param name="nuclides"></param>
    /// <param name="errors"></param>
    /// <param name="compartments"></param>
    public DecaySet(IReadOnlyList<NuclideData> nuclides, InputErrors errors, IList<Organ> compartments)
    {
        Nuclides = nuclides;
        Errors = errors;

        Compartments = compartments;

        decayPaths = [];
        decayChains = [];
        decayNuclides = Nuclides.ToDictionary(n => n, GetDecayNuclides);
    }

    /// <summary>
    /// 対象コンパートメントの核種から始まる崩壊系列の情報を保持する。
    /// </summary>
    internal class DecayChain
    {
        /// <summary>
        /// コンストラクタ。
        /// </summary>
        /// <param name="organRoot"></param>
        /// <param name="decayNuclides">子孫核種の配列。</param>
        public DecayChain(Organ organRoot, NuclideData[] decayNuclides)
        {
            RootCompartment = organRoot;
            DecayNuclides = decayNuclides;
            DecayCompartments = new Organ[decayNuclides.Length];
        }

        public Organ RootCompartment;

        public NuclideData[] DecayNuclides;

        public Organ[] DecayCompartments;

        /// <summary>
        /// 崩壊系列を構成する全ての移行経路を取得する。
        /// </summary>
        /// <returns></returns>
        public IReadOnlyList<(NuclideData Parent, NuclideData Daughter)> GetDecayTransfers()
        {
            var nuclides = DecayNuclides.Prepend(RootCompartment.Nuclide);

            var results = new List<(NuclideData Parent, NuclideData Daughter)>();
            foreach (var parent in nuclides)
            {
                foreach (var daughter in DecayNuclides)
                {
                    if (parent == daughter)
                        continue;
                    if (parent.Branches.Any(b => b.Daughter == daughter))
                        results.Add((parent, daughter));
                }
            }
            return results;
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
        if (!decayNuclides[nuclideFrom].Contains(nuclideTo))
            Errors.AddError(lineNum, $"There is no decay path from {nuclideFrom.Name} to {nuclideTo.Name}.");

        var paths = decayPaths.Where(path => path.from == organFrom);

        // organFromから、同じ子孫核種への2つ以上の壊変経路は定義できない。
        if (paths.Any(path => path.to.Nuclide == nuclideTo))
            Errors.AddError(lineNum, $"Multiple decay paths from {organFrom.Func} '{nuclideFrom.Name}/{organFrom.Name}' to nuclide '{nuclideTo.Name}'.");

        decayPaths.Add((organFrom, organTo, hasCoeff));

        var decayChain = GetDecayChain(organFrom);

        var decayIndex = Array.IndexOf(decayChain.DecayNuclides, nuclideTo);
        if (decayIndex == -1)
        {
            var rootNuc = decayChain.RootCompartment.Nuclide.Name;
            Errors.AddError(lineNum, $"Cannot find progeny nuclide '{nuclideTo.Name}' in decay chain starts from '{rootNuc}'.");
            return null;
        }

        ref var organDecay = ref decayChain.DecayCompartments[decayIndex];
        if (organDecay != null)
        {
            if (hasCoeff)
                Errors.AddError(lineNum, $"Decay compartment is already set.");
            else
                Errors.AddError(lineNum, $"Decay compartment '{organTo.Name}' conflicts with the implicitly deined one.");
            return null;
        }

        if (hasCoeff)
        {
            organDecay = AddDecayCompartment(decayChain.RootCompartment, nuclideTo);

            // 以降の処理を核種が同じコンパートメント間organDecay -> organToの経路設定にすり替える。
            return organDecay;
        }
        else
        {
            // organToを崩壊系列でorganDecayが占める位置に設定する。
            organDecay = organTo;

            // transfersCorrectには核種が異なるコンパートメント間の壊変経路を追加しない。
            return null;
        }
    }

    /// <summary>
    /// 対象核種から始まる崩壊系列を構成する子孫核種を列挙する。
    /// </summary>
    /// <param name="root"></param>
    /// <returns>rootを含まない、子孫核種のみの配列。</returns>
    private NuclideData[] GetDecayNuclides(NuclideData root)
    {
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
        return results.Skip(1).ToArray();
    }

    /// <summary>
    /// organFromから始まる崩壊系列の情報を取得する。
    /// </summary>
    /// <param name="organFrom"></param>
    /// <returns></returns>
    private DecayChain GetDecayChain(Organ organFrom)
    {
        var funcFrom = organFrom.Func;
        if (funcFrom != OrganFunc.acc && funcFrom != OrganFunc.exc)
            return null;
        if (organFrom.IsDecayCompartment)
            return null;

        var progenies = decayNuclides[organFrom.Nuclide];
        if (progenies.Length == 0)
            return null;

        if (!decayChains.TryGetValue(organFrom, out var decayChain))
        {
            decayChain = new DecayChain(organFrom, progenies);
            decayChains[organFrom] = decayChain;
        }
        return decayChain;
    }

    /// <summary>
    /// 核種が異なるコンパートメント間の移行経路を定義する。
    /// </summary>
    public void DefineDecayTransfers()
    {
        foreach (var decayChain in decayChains.Values)
        {
            var organFrom = decayChain.RootCompartment;
            var nuclideFrom = organFrom.Nuclide;
            var decayTransfers = decayChain.GetDecayTransfers();
            var decayCompartments = decayChain.DecayCompartments;

            // 崩壊系列を構成するコンパートメント間の壊変経路を追加定義する。
            // 壊変経路は一本道ではなく、nuclideFromから始まる有効非巡回グラフ(DAG)を構成する点に注意。
            foreach (var path in decayTransfers)
            {
                var from = path.Parent == nuclideFrom ? organFrom
                       : decayCompartments.FirstOrDefault(o => o?.Nuclide == path.Parent);
                var to = decayCompartments.FirstOrDefault(o => o?.Nuclide == path.Daughter);

                // 壊変経路が既に設定されている＝インプットで明示的に定義されている場合は何もしない。
                if (from != null && to != null && to.Inflows.Any(inflow => inflow.Organ == from))
                    continue;

                from ??= AddDecayCompartment(organFrom, path.Parent);
                to ??= AddDecayCompartment(organFrom, path.Daughter);

                var branch = from.Nuclide.Branches.First(b => b.Daughter == to.Nuclide);

                to.Inflows.Add(new Inflow
                {
                    ID = from.ID,

                    // 壊変経路では、親からの分岐比を移行割合としてとする。
                    Rate = branch.Fraction,

                    // 流入経路から流入元臓器の情報を直接引くための参照を設定する。
                    Organ = from,
                });
            }
        }
    }

    /// <summary>
    /// 指定の子孫核種に対応する壊変コンパートメントを追加する。
    /// </summary>
    /// <param name="organFrom"></param>
    /// <param name="progeny"></param>
    /// <returns>追加した壊変コンパートメント</returns>
    private Organ AddDecayCompartment(Organ organFrom, NuclideData progeny)
    {
        var nuclideFrom = organFrom.Nuclide;

        var index = Compartments.Count;
        var organDecay = new Organ
        {
            Nuclide = progeny,
            ID = index + 1,
            Index = index,
            Name = $"Decay-{nuclideFrom.Name}/{organFrom.Name}",
            Func = OrganFunc.acc,
            BioDecay = 1.0,     // accは後で設定する。
            IsDecayCompartment = true,
        };
        Compartments.Add(organDecay);

        var sourceRegion = organFrom.SourceRegion;
        if (sourceRegion != null)
        {
            // 親核種のコンパートメントに設定された線源領域が、
            // 子孫核種の動態モデルおいても明示的に設定されているかどうかを調べる。
            var explicitUsed = Compartments.Where(o => o.Nuclide == progeny)
                                           .Any(o => o.SourceRegion == sourceRegion);

            // 明示的に設定されていない場合は、ambiguous compartmentを'Other'に割り当てる。
            if (!explicitUsed)
                sourceRegion = "Other";

            organDecay.SourceRegion = sourceRegion;
        }

        return organDecay;
    }
}
