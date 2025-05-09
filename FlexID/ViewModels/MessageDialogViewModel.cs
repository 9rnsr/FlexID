using Prism.Mvvm;
using Prism.Navigation;
using Prism.Services.Dialogs;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using System;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Windows;

namespace FlexID.ViewModels
{
    public class MessageDialogViewModel : BindableBase, IDestructible, IDialogAware
    {
        private CompositeDisposable Disposables { get; } = new CompositeDisposable();

        public string Title
        {
            get => _title;
            set => SetProperty(ref _title, value);
        }
        private string _title;

        public string Message
        {
            get => _message;
            set => SetProperty(ref _message, value);
        }
        private string _message;

        public MessageBoxButton Buttons
        {
            get => _buttons;
            set => SetProperty(ref _buttons, value);
        }
        private MessageBoxButton _buttons;

        public MessageBoxResult DefaultResult
        {
            get => _defaultResult;
            set => SetProperty(ref _defaultResult, value);
        }
        private MessageBoxResult _defaultResult;

        public event Action<IDialogResult> RequestClose;

        public ReactiveCommandSlim OKCommand { get; }

        public ReactiveCommandSlim YesCommand { get; }

        public ReactiveCommandSlim NoCommand { get; }

        public ReactiveCommandSlim CancelCommand { get; }

        /// <summary>
        /// コンストラクタ。
        /// </summary>
        public MessageDialogViewModel()
        {
            var buttons = this.ObserveProperty(vm => vm.Buttons).ToReadOnlyReactivePropertySlim();

            OKCommand = buttons
                .Select(v => v == MessageBoxButton.OK || v == MessageBoxButton.OKCancel)
                .ToReactiveCommandSlim().WithSubscribe(() => RequestClose?.Invoke(new DialogResult(ButtonResult.OK)))
                .AddTo(Disposables);

            YesCommand = buttons
                .Select(v => v == MessageBoxButton.YesNo || v == MessageBoxButton.YesNoCancel)
                .ToReactiveCommandSlim().WithSubscribe(() => RequestClose?.Invoke(new DialogResult(ButtonResult.Yes)))
                .AddTo(Disposables);

            NoCommand = buttons
                .Select(v => v == MessageBoxButton.YesNo || v == MessageBoxButton.YesNoCancel)
                .ToReactiveCommandSlim().WithSubscribe(() => RequestClose?.Invoke(new DialogResult(ButtonResult.No)))
                .AddTo(Disposables);

            CancelCommand = buttons
                .Select(v => v == MessageBoxButton.OKCancel || v == MessageBoxButton.YesNoCancel)
                .ToReactiveCommandSlim().WithSubscribe(() => RequestClose?.Invoke(new DialogResult(ButtonResult.Cancel)))
                .AddTo(Disposables);
        }

        public void Destroy() => Disposables.Dispose();

        public void OnDialogOpened(IDialogParameters parameters)
        {
            Title = parameters.GetValue<string>("Title");
            Message = parameters.GetValue<string>("Message");
            Buttons = parameters.GetValue<MessageBoxButton>("Buttons");
            DefaultResult = parameters.GetValue<MessageBoxResult>("DefaultResult");

            // DefaultResultの設定値がButtonsと整合しない場合をチェックする。
            switch (Buttons)
            {
                case MessageBoxButton.OK:
                    if (DefaultResult != MessageBoxResult.OK)
                        DefaultResult = MessageBoxResult.None;
                    break;

                case MessageBoxButton.OKCancel:
                    if (DefaultResult != MessageBoxResult.OK && DefaultResult != MessageBoxResult.Cancel)
                        DefaultResult = MessageBoxResult.None;
                    break;

                case MessageBoxButton.YesNoCancel:
                    if (DefaultResult != MessageBoxResult.Yes && DefaultResult != MessageBoxResult.No && DefaultResult != MessageBoxResult.Cancel)
                        DefaultResult = MessageBoxResult.None;
                    break;

                case MessageBoxButton.YesNo:
                    if (DefaultResult != MessageBoxResult.Yes && DefaultResult != MessageBoxResult.No)
                        DefaultResult = MessageBoxResult.None;
                    break;
            }

            // DefaultResultがない場合に既定値を設定する。
            if (DefaultResult == MessageBoxResult.None)
            {
                if (Buttons == MessageBoxButton.OK || Buttons == MessageBoxButton.OKCancel)
                    DefaultResult = MessageBoxResult.OK;

                if (Buttons == MessageBoxButton.YesNo || Buttons == MessageBoxButton.YesNoCancel)
                    DefaultResult = MessageBoxResult.Yes;
            }
        }

        public bool CanCloseDialog() => true;

        public void OnDialogClosed() { }
    }
}
