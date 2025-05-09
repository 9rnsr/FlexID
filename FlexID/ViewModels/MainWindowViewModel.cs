using Prism.Mvvm;
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
        public MainWindowViewModel()
        {
            AboutCommand = new ReactiveCommand().WithSubscribe(() =>
            {
                MessageBox.Show("test", "About", MessageBoxButton.OK);
            });
        }
    }

    public class CalcState
    {
        public ReactivePropertySlim<bool> CanExecute { get; } = new ReactivePropertySlim<bool>(true);
    }
}
