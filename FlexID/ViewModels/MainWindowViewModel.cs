using Prism.Mvvm;
using Reactive.Bindings;

namespace FlexID.ViewModels;

public class MainWindowViewModel : BindableBase
{
    /// <summary>
    /// コンストラクタ。
    /// </summary>
    public MainWindowViewModel()
    {
    }
}

public class CalcState
{
    public ReactivePropertySlim<bool> CanExecute { get; } = new(true);
}
