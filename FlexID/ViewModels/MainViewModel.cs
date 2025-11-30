using CommunityToolkit.Mvvm.Messaging.Messages;

namespace FlexID.ViewModels;

public class MainViewModel
{
    /// <summary>
    /// コンストラクタ。
    /// </summary>
    public MainViewModel()
    {
    }
}

public class BusyState(bool value) : ValueChangedMessage<bool>(value);
