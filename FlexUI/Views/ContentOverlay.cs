using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Imaging;

namespace FlexID.Views;

/// <summary>
/// Content要素の上にモーダルなオーバーレイ要素を表示するコントロール。
/// </summary>
public sealed partial class ContentOverlay : ContentControl
{
    public ContentOverlay()
    {
        DefaultStyleKey = typeof(ContentOverlay);
    }

    private FrameworkElement? PART_Content;
    private FrameworkElement? PART_Overlay;
    private Image? PART_ContentImage;

    protected override void OnApplyTemplate()
    {
        base.OnApplyTemplate();

        PART_Content = GetTemplateChild("PART_Content") as FrameworkElement;
        PART_ContentImage = GetTemplateChild("PART_ContentImage") as Image;
        PART_Overlay = GetTemplateChild("PART_Overlay") as FrameworkElement;
    }

    /// <summary>
    /// オーバーレイの表示状態を取得・設定する。
    /// </summary>
    public bool IsOverlay
    {
        get => (bool)GetValue(IsOverlayProperty);
        set => SetValue(IsOverlayProperty, value);
    }

    public static readonly DependencyProperty IsOverlayProperty =
        DependencyProperty.Register(
            nameof(IsOverlay), typeof(bool), typeof(ContentOverlay),
            new PropertyMetadata(false, SetIsOverlayProperty));

    private static async void SetIsOverlayProperty(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var area = (ContentOverlay)d;

        var newValue = (bool)e.NewValue;
        if (newValue)
        {
            if (area.OverlayContent is null)
                return;

            if (area.PART_Content is null)
            {
                area.PART_ContentImage?.Source = null;
            }
            else
            {
                // 背景となるUI要素のレンダリング結果を画像として取得。
                var bitmap = new RenderTargetBitmap();
                await bitmap.RenderAsync(area.PART_Content);

                area.PART_ContentImage?.Source = bitmap;
                area.PART_ContentImage?.Margin = (area.Content as FrameworkElement)?.Margin ?? default;
            }

            area.PART_Content?.Visibility = Visibility.Collapsed;
            area.PART_Overlay?.Visibility = Visibility.Visible;
        }
        else
        {
            area.PART_ContentImage?.Source = null;

            area.PART_Overlay?.Visibility = Visibility.Collapsed;
            area.PART_Content?.Visibility = Visibility.Visible;
        }
    }

    public Brush BackgroundOverlayBrush
    {
        get => (Brush)GetValue(BackgroundOverlayBrushProperty);
        set => SetValue(BackgroundOverlayBrushProperty, value);
    }

    public static readonly DependencyProperty BackgroundOverlayBrushProperty =
        DependencyProperty.Register(
            nameof(BackgroundOverlayBrush), typeof(Brush), typeof(ContentOverlay),
            new PropertyMetadata(null));

    public Thickness OverlayMargin
    {
        get => (Thickness)GetValue(OverlayMarginProperty);
        set => SetValue(OverlayMarginProperty, value);
    }

    public static readonly DependencyProperty OverlayMarginProperty =
        DependencyProperty.Register(
            nameof(OverlayMargin), typeof(Thickness), typeof(ContentOverlay),
            new PropertyMetadata(new Thickness(20)));

    public UIElement? OverlayContent
    {
        get => (UIElement)GetValue(OverlayContentProperty);
        set => SetValue(OverlayContentProperty, value);
    }

    public static readonly DependencyProperty OverlayContentProperty =
        DependencyProperty.Register(
            nameof(OverlayContent), typeof(UIElement), typeof(ContentOverlay),
            new PropertyMetadata(null));

    public HorizontalAlignment HorizontalOverlayAlignment
    {
        get => (HorizontalAlignment)GetValue(HorizontalOverlayAlignmentProperty);
        set => SetValue(HorizontalOverlayAlignmentProperty, value);
    }

    public static readonly DependencyProperty HorizontalOverlayAlignmentProperty =
        DependencyProperty.Register(
            nameof(HorizontalOverlayAlignment), typeof(HorizontalAlignment), typeof(ContentOverlay),
            new PropertyMetadata(HorizontalAlignment.Stretch));

    public VerticalAlignment VerticalOverlayAlignment
    {
        get => (VerticalAlignment)GetValue(VerticalOverlayAlignmentProperty);
        set => SetValue(VerticalOverlayAlignmentProperty, value);
    }

    public static readonly DependencyProperty VerticalOverlayAlignmentProperty =
        DependencyProperty.Register(
            nameof(VerticalOverlayAlignment), typeof(VerticalAlignment), typeof(ContentOverlay),
            new PropertyMetadata(VerticalAlignment.Stretch));

    public ControlTemplate OverlayContainerTemplate
    {
        get { return (ControlTemplate)GetValue(OverlayContainerTemplateProperty); }
        set { SetValue(OverlayContainerTemplateProperty, value); }
    }

    public static readonly DependencyProperty OverlayContainerTemplateProperty =
        DependencyProperty.Register(nameof(OverlayContainerTemplate), typeof(ControlTemplate), typeof(ContentOverlay), new PropertyMetadata(0));
}
