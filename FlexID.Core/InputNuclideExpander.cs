using Sprache;
using static Sprache.Parse;

namespace FlexID;

public class InputNuclideExpander
{
    private readonly IReadOnlyList<NuclideData> nuclides;

    private readonly InputErrors errors;

    /// <summary>
    /// コンストラクタ。
    /// </summary>
    /// <param name="nuclides">核種パターン展開の候補となる核種データのリスト。</param>
    /// <param name="errors">展開処理中に発生したエラー情報の格納先。</param>
    public InputNuclideExpander(IReadOnlyList<NuclideData> nuclides, InputErrors errors)
    {
        this.nuclides = nuclides;
        this.errors = errors;
    }

    /// <summary>
    /// 核種パターンを核種データのリストに展開する。
    /// </summary>
    /// <param name="loc">位置情報。</param>
    /// <param name="nuclidePattern">核種パターン。</param>
    /// <returns>展開結果。</returns>
    public IReadOnlyList<NuclideData> ExpandNuclides(Location loc, string nuclidePattern)
    {
        List<NuclideData> results = [];

        if (nuclidePattern.StartsWith("{") && nuclidePattern.EndsWith("}"))
        {
            var patternList = nuclidePattern[1..^1].Trim();
            if (patternList == "*")
                return nuclides;

            var inverse = patternList.StartsWith('^');
            patternList = patternList[(inverse ? 1 : 0)..];

            foreach (var pattern in patternList.Split(',').Select(x => x.Trim()))
            {
                if (pattern.Contains('-'))
                {
                    var nuclide = nuclides.FirstOrDefault(n => n.Name == pattern);
                    if (nuclide is null)
                        errors.AddError(loc, $"Undefined nuclide '{pattern}'.");
                    else
                        results.Add(nuclide);
                }
                else
                {
                    var anyFound = false;
                    foreach (var nuclide in nuclides)
                    {
                        var elem = nuclide.Name.AsSpan()[..nuclide.Name.IndexOf('-')];
                        if (pattern.Equals(elem, StringComparison.Ordinal))
                        {
                            results.Add(nuclide);
                            anyFound = true;
                        }
                    }
                    if (!anyFound)
                    {
#if false
                        errors.AddError(loc, $"Unrecognized element name: '{pattern}'.");
#else
                        // [nuclide]セクションに定義されていない元素でも、元素名としては正しいならエラーにはしない。
                        if (!ElementTable.Names.Contains(pattern))
                            errors.AddError(loc, $"Unrecognized element name: '{pattern}'.");
#endif
                        continue;
                    }
                }
            }

            if (inverse)
                results = [.. nuclides.Except(results)];

#if false
            if (results.Count == 0)
                errors.AddError(loc, $"No nuclides match the pattern: '{nuclidePattern}'.");
#endif
        }
        else
        {
            var nuclide = nuclides.FirstOrDefault(n => n.Name == nuclidePattern);
            if (nuclide is null)
                errors.AddError(loc, $"Undefined nuclide '{nuclidePattern}'.");
            else
                results.Add(nuclide);
        }

        return results;
    }
}
