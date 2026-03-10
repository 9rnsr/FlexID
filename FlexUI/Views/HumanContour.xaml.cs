using FlexID.ViewModels;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace FlexID.Views;

public partial class HumanContour : UserControl
{
    public HumanContour()
    {
        InitializeComponent();
    }

    public static readonly DependencyProperty ViewModelProperty =
        DependencyProperty.Register(
            nameof(ViewModel),
            typeof(ContourViewModel),
            typeof(HumanContourLegend), new PropertyMetadata(default));

    public ContourViewModel ViewModel
    {
        get => (ContourViewModel)GetValue(ViewModelProperty);
        set => SetValue(ViewModelProperty, value);
    }

    public string ForamtContourValue(double value)
    {
        return $"{value:0.0000E+00}";
    }
}
