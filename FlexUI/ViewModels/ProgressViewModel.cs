using CommunityToolkit.Mvvm.ComponentModel;

namespace FlexID.ViewModels;

public partial class ProgressViewModel : ObservableObject
{
    /// <summary>
    /// コンストラクタ。
    /// </summary>
    public ProgressViewModel()
    {
    }

    [ObservableProperty]
    public partial string Title { get; set; } = "Progress";
}
