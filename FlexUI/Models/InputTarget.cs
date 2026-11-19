namespace FlexID.Models;

/// <summary>
/// 計算対象となるインプットの情報を保持する。
/// </summary>
public class InputTarget : IComparable<InputTarget>
{
    public InputTarget(string filePath, InputData data)
    {
        Name = Path.GetFileNameWithoutExtension(filePath);
        FilePath = filePath;
        Title = data.Title;
        Nuclide = data.Nuclides[0].Name;
        Progenies = (string[])[.. data.Nuclides.Skip(1).Select(nuc => nuc.Name)];

        if (!ElementTable.TryParseNuclide(Nuclide, out var elem, out var massNum, out var metaStable))
            throw new InvalidOperationException();

        AtomicNumber = ElementTable.ElementToAtomicNumber(elem);
        MassNumber = massNum;
        MetaStable = metaStable;
    }

    /// <summary>
    /// インプットの名前。
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// インプットのファイルパス。
    /// </summary>
    public string FilePath { get; }

    /// <summary>
    /// 計算モデルのタイトル(被ばく経路, 化学形態, etc.)。
    /// </summary>
    public string Title { get; }

    /// <summary>
    /// 計算モデルの親核種。
    /// </summary>
    public string Nuclide { get; }

    private readonly int AtomicNumber;
    private readonly int MassNumber;
    private readonly string MetaStable;

    /// <summary>
    /// 計算モデルの子孫核種。
    /// </summary>
    public IReadOnlyList<string> Progenies { get; }

    /// <summary>
    /// 原子番号、質量数、順安定状態、名前による比較を行う。
    /// </summary>
    /// <param name="other">比較対象。</param>
    /// <returns></returns>
    public int CompareTo(InputTarget? other)
    {
        if (other is null) return 1;

        int cmp;
        if ((cmp = this.AtomicNumber - other.AtomicNumber) != 0) return cmp;
        if ((cmp = this.MassNumber - other.MassNumber) != 0) return cmp;
        if ((cmp = string.Compare(this.MetaStable, other.MetaStable)) != 0) return cmp;

        return string.Compare(this.Name, other.Name);
    }
}
