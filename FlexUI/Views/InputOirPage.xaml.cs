using CommunityToolkit.Mvvm.DependencyInjection;
using FlexID.ViewModels;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace FlexID.Views;

public sealed partial class InputOirPage : Page
{
    public InputOirPage()
    {
        ViewModel = Ioc.Default.GetRequiredService<InputOirViewModel>();

        InitializeComponent();

        var progressVM = ViewModel.ProgressViewModel;
        progressVM.ShowAction = () => ProgressOverlay.IsOverlay = true;
        progressVM.CloseAction = () => ProgressOverlay.IsOverlay = false;
    }

    public InputOirViewModel ViewModel { get; }

    public string AbortCloseButtonText(bool isBusy) => isBusy ? "Abort" : "Close";

    public bool CanCloseProgress(bool isBusy) => !isBusy;

    public Visibility VisibilityOpenSummary(bool isBusy) => isBusy ? Visibility.Collapsed : Visibility.Visible;
}
