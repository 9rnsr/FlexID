using CommunityToolkit.Mvvm.DependencyInjection;
using FlexID.ViewModels;
using Microsoft.UI.Xaml.Controls;

namespace FlexID.Views;

public sealed partial class InputScoeffPage : Page
{
    public InputScoeffPage()
    {
        ViewModel = Ioc.Default.GetRequiredService<InputScoeffViewModel>();

        InitializeComponent();

        DataContext = ViewModel;
    }

    public InputScoeffViewModel ViewModel { get; }
}
