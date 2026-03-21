using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;

namespace FlexID;

public partial class ElementTable
{
    static readonly IReadOnlyList<string> Lanthanoid =
    [
        "La", "Ce", "Pr", "Nd", "Pm", "Sm", "Eu", "Gd", "Tb", "Dy", "Ho", "Er", "Tm", "Yb", "Lu",
    ];
    static readonly IReadOnlyList<string> Actinoid =
    [
        "Ac", "Th", "Pa", "U",  "Np", "Pu", "Am", "Cm", "Bk", "Cf", "Es", "Fm", "Md", "No", "Lr",
    ];
    public static IReadOnlyList<string> Names { get; } =
    [
        "H",                                                                                                          "He",
        "Li", "Be",                                                                     "B",  "C",  "N",  "O",  "F",  "Ne",
        "Na", "Mg",                                                                     "Al", "Si", "P",  "S",  "Cl", "Ar",
        "K",  "Ca", "Sc",         "Ti", "V",  "Cr", "Mn", "Fe", "Co", "Ni", "Cu", "Zn", "Ga", "Ge", "As", "Se", "Br", "Kr",
        "Rb", "Sr", "Y",          "Zr", "Nb", "Mo", "Tc", "Ru", "Rh", "Pd", "Ag", "Cd", "In", "Sn", "Sb", "Te", "I",  "Xe",
        "Cs", "Ba", ..Lanthanoid, "Hf", "Ta", "W",  "Re", "Os", "Ir", "Pt", "Au", "Hg", "Tl", "Pb", "Bi", "Po", "At", "Rn",
        "Fr", "Ra", ..Actinoid,   "Rf", "Db", "Sg", "Bh", "Hs", "Mt", "Ds", "Rg", "Cn", "Nh", "Fl", "Mc", "Lv", "Ts", "Og",
    ];

    public static int ElementToAtomicNumber(string element)
    {
        int index = Names.IndexOf(element);
        if (index >= 0)
            return index + 1;
        throw new ArgumentException($"Invalid element: {element}", nameof(element));
    }

    public static string AtomicNumberToElement(int atomicNumber)
    {
        if (1 <= atomicNumber && atomicNumber <= Names.Count)
            return Names[atomicNumber - 1];
        throw new ArgumentOutOfRangeException(nameof(atomicNumber));
    }

    /// <summary>
    /// 核種名に合致する正規表現。
    /// 準安定核種について、一般的な表記(m1, m2)とICRP-07データのもの(m, n)の両方を受け付けるようにしている。
    /// </summary>
    [GeneratedRegex(@"^(?<elem>[A-Za-z]+)-(?<num>\d+)(?<meta>[a-z]|m\d)?$")]
    public static partial Regex PatternNuclide { get; }

    public static bool TryParseNuclide(
        [NotNullWhen(true)] string? input,
        [NotNullWhen(true)] out string? element, out int massNumber, [NotNullWhen(true)] out string? metaStable)
    {
        if (input is not null)
        {
            var m = PatternNuclide.Match(input);
            if (m.Success &&
                m.Groups["elem"].Value is string elem && Names.IndexOf(elem) != -1)
            {
                element = elem;
                massNumber = int.Parse(m.Groups["num"].Value);
                metaStable = m.Groups["meta"].Value;
                return true;
            }
        }

        element = null;
        massNumber = 0;
        metaStable = null;
        return false;
    }
}
