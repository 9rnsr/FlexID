using CommunityToolkit.Mvvm.ComponentModel;
using R3;

namespace FlexID.ViewModels;

public class ViewModelBase : ObservableObject, IDisposable
{
    protected readonly CompositeDisposable disposables = [];

    public void Dispose() => disposables.Dispose();
}
