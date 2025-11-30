using CommunityToolkit.Mvvm.Messaging.Messages;

namespace FlexID.ViewModels;

public class MainWindowViewModel
{
    /// <summary>
    /// コンストラクタ。
    /// </summary>
    public MainWindowViewModel()
    {
    }
}

public class BusyState(bool value) : ValueChangedMessage<bool>(value);
