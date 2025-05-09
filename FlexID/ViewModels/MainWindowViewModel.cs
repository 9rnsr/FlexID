using FlexID.Services;
using Prism.Mvvm;
using Prism.Navigation;
using Prism.Services.Dialogs;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using System;
using System.Reactive.Disposables;
using System.Reflection;
using System.Windows;

namespace FlexID.ViewModels
{
    public class MainWindowViewModel : BindableBase, IDestructible
    {
        private CompositeDisposable Disposables { get; } = new CompositeDisposable();

        public string Title
        {
            get => _title;
            set => SetProperty(ref _title, value);
        }
        private string _title;

        public ReactiveCommand AboutCommand { get; }

        /// <summary>
        /// コンストラクタ。
        /// </summary>
        public MainWindowViewModel(IDialogService dialogService)
        {
            var assembly = Assembly.GetExecutingAssembly();
            var assemblyTitle = Attribute.GetCustomAttributes(assembly, typeof(AssemblyTitleAttribute))[0] as AssemblyTitleAttribute;
            var assemblyInfoVer = Attribute.GetCustomAttributes(assembly, typeof(AssemblyInformationalVersionAttribute))[0] as AssemblyInformationalVersionAttribute;

            var baseVersion = assemblyInfoVer.InformationalVersion.Split('-')[0];
            Title = $"{assemblyTitle.Title} Ver.{baseVersion}";

            AboutCommand = new ReactiveCommand().WithSubscribe(() =>
            {
                dialogService.ShowMessage(
                    "About FlexID",
                    $"FlexID Version {baseVersion}",
                    MessageBoxButton.OK);
            }).AddTo(Disposables);
        }

        public void Destroy() => Disposables.Dispose();
    }

    public class CalcState
    {
        public ReactivePropertySlim<bool> CanExecute { get; } = new ReactivePropertySlim<bool>(true);
    }
}
