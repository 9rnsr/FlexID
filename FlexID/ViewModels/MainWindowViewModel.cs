using FlexID.Services;
using Prism.Mvvm;
using Prism.Services.Dialogs;
using Reactive.Bindings;
using System.Windows;

namespace FlexID.ViewModels
{
    public class MainWindowViewModel : BindableBase
    {
        public ReactiveCommand AboutCommand { get; }

        /// <summary>
        /// コンストラクタ。
        /// </summary>
        public MainWindowViewModel(IDialogService dialogService)
        {
            AboutCommand = new ReactiveCommand().WithSubscribe(() =>
            {
                dialogService.ShowMessage("About FlexID", "FlexID Version x.yy.zz", MessageBoxButton.OK);
            });
        }
    }

    public class CalcState
    {
        public ReactivePropertySlim<bool> CanExecute { get; } = new ReactivePropertySlim<bool>(true);
    }
}
