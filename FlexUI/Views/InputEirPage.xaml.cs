using CommunityToolkit.Mvvm.DependencyInjection;
using FlexID.ViewModels;
using Microsoft.UI.Xaml.Controls;

namespace FlexID.Views;

public sealed partial class InputEirPage : Page
{
    public InputEirPage()
    {
        ViewModel = Ioc.Default.GetRequiredService<InputEirViewModel>();

        InitializeComponent();

        var progressVM = ViewModel.ProgressViewModel;
        progressVM.ShowAction = () => ProgressOverlay.IsOverlay = true;
        progressVM.CloseAction = () => ProgressOverlay.IsOverlay = false;
    }

    public InputEirViewModel ViewModel { get; }

    public string AbortCloseButtonText(bool isBusy) => isBusy ? "Abort" : "Close";

    public bool CanCloseProgress(bool isBusy) => !isBusy;
}
