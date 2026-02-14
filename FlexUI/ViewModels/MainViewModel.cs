using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging.Messages;

namespace FlexID.ViewModels;

public partial class MainViewModel : ObservableObject
{
    /// <summary>
    /// コンストラクタ。
    /// </summary>
    public MainViewModel()
    {
    }

    [ObservableProperty]
    public partial string Title { get; set; } = "FlexID";
}

public class BusyState(bool value) : ValueChangedMessage<bool>(value);
