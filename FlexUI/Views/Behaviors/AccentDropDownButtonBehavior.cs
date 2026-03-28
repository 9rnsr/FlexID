using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.Xaml.Interactivity;

namespace FlexID.Views.Behaviors;

public sealed class AccentDropDownButtonBehavior : Behavior<DropDownButton>
{
    public bool IsAccent
    {
        get => (bool)GetValue(IsAccentProperty);
        set => SetValue(IsAccentProperty, value);
    }

    public static readonly DependencyProperty IsAccentProperty =
        DependencyProperty.Register(
            nameof(IsAccent), typeof(bool), typeof(AccentDropDownButtonBehavior),
            new PropertyMetadata(false, OnAnyPropertyChanged));

    public Style? AccentStyle
    {
        get => (Style?)GetValue(AccentStyleProperty);
        set => SetValue(AccentStyleProperty, value);
    }

    public static readonly DependencyProperty AccentStyleProperty =
        DependencyProperty.Register(
            nameof(AccentStyle), typeof(Style), typeof(AccentDropDownButtonBehavior),
            new PropertyMetadata(null, OnAnyPropertyChanged));

    public Style? NormalStyle
    {
        get => (Style?)GetValue(NormalStyleProperty);
        set => SetValue(NormalStyleProperty, value);
    }

    public static readonly DependencyProperty NormalStyleProperty =
        DependencyProperty.Register(
            nameof(NormalStyle), typeof(Style), typeof(AccentDropDownButtonBehavior),
            new PropertyMetadata(null, OnAnyPropertyChanged));

    private Style? _originalStyle;

    protected override void OnAttached()
    {
        base.OnAttached();

        if (AssociatedObject is null)
            return;

        _originalStyle = AssociatedObject.Style; // can be null (means default)

        ApplyStyle();
    }

    protected override void OnDetaching()
    {
        // restore to the OriginalStyle
        if (AssociatedObject is not null)
            AssociatedObject.Style = _originalStyle;

        _originalStyle = null;

        base.OnDetaching();
    }

    private static void OnAnyPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var b = (AccentDropDownButtonBehavior)d;
        b.ApplyStyle();
    }

    private void ApplyStyle()
    {
        if (AssociatedObject is null)
            return;

        var normalStyle = NormalStyle ?? _originalStyle;
        var accentStyle = AccentStyle ?? normalStyle;

        AssociatedObject.Style = IsAccent ? accentStyle : normalStyle;
    }
}
