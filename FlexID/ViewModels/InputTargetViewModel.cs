using FlexID.Models;

namespace FlexID.ViewModels;

public class InputTargetViewModel
{
    /// <summary>
    /// コンストラクタ
    /// </summary>
    /// <param name="target"></param>
    public InputTargetViewModel(InputTarget target)
    {
        InputTarget = target;
    }

    public InputTarget InputTarget { get; }

    public string Title => InputTarget.Title;

    public string Nuclide => InputTarget.Nuclide;

    /// <summary>
    /// 子孫核種の計算モデルを持っている場合は<c>true</c>。
    /// </summary>
    public bool HasProgeny => InputTarget.Progenies.Count > 1;
}
