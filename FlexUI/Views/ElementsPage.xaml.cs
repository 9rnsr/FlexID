using FlexID.ViewModels;
using Microsoft.UI.Xaml;

namespace FlexID.Views;

public sealed partial class ElementsPage
{
    public ElementsPage()
    {
        InitializeComponent();
    }

    public static readonly DependencyProperty ViewModelProperty =
        DependencyProperty.Register(
            nameof(ViewModel), typeof(ElementsViewModel), typeof(ElementsPage),
            new PropertyMetadata(null));

    public ElementsViewModel ViewModel
    {
        get => (ElementsViewModel)GetValue(ViewModelProperty);
        set => SetValue(ViewModelProperty, value);
    }
}
