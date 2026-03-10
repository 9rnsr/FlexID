namespace FlexID.Models;

/// <summary>
/// 計算対象となるインプットの情報を保持する。
/// </summary>
public class InputTarget
{
    public InputTarget(string filePath, InputData data)
    {
        Name = Path.GetFileNameWithoutExtension(filePath);
        FilePath = filePath;
        Title = data.Title;
        Nuclide = data.Nuclides[0].Name;
        Progenies = [.. data.Nuclides.Skip(1).Select(nuc => nuc.Name)];
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

    /// <summary>
    /// 計算モデルの子孫核種。
    /// </summary>
    public IReadOnlyList<string> Progenies { get; }
}
