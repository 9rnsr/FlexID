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

        DataContext = ViewModel;
    }

    public InputEirViewModel ViewModel { get; }
}
