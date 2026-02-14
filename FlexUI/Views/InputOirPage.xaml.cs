using CommunityToolkit.Mvvm.DependencyInjection;
using FlexID.ViewModels;
using Microsoft.UI.Xaml.Controls;

namespace FlexID.Views;

public sealed partial class InputOirPage : Page
{
    public InputOirPage()
    {
        ViewModel = Ioc.Default.GetRequiredService<InputOirViewModel>();

        InitializeComponent();
    }

    public InputOirViewModel ViewModel { get; }
}
