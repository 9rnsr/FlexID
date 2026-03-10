using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace FlexID.Views;

public partial class ColorScaleBar : UserControl
{
    public ColorScaleBar()
    {
        InitializeComponent();
    }

    public static readonly DependencyProperty ContourMaxProperty =
        DependencyProperty.Register(
            nameof(ContourMax), typeof(double), typeof(ColorScaleBar),
            new PropertyMetadata(0.0));

    public static readonly DependencyProperty ContourMinProperty =
        DependencyProperty.Register(
            nameof(ContourMin), typeof(double), typeof(ColorScaleBar),
            new PropertyMetadata(0.0));

    public static readonly DependencyProperty ContourUnitProperty =
        DependencyProperty.Register(
            nameof(ContourUnit), typeof(string), typeof(ColorScaleBar),
            new PropertyMetadata(""));

    public double ContourMax
    {
        get { return (double)GetValue(ContourMaxProperty); }
        set { SetValue(ContourMaxProperty, value); }
    }

    public double ContourMin
    {
        get { return (double)GetValue(ContourMinProperty); }
        set { SetValue(ContourMinProperty, value); }
    }

    public string ContourUnit
    {
        get { return (string)GetValue(ContourUnitProperty); }
        set { SetValue(ContourUnitProperty, value); }
    }
}
