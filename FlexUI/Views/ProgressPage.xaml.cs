using FlexID.ViewModels;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace FlexID.Views;

public sealed partial class ProgressPage : Page
{
    public ProgressPage()
    {
        InitializeComponent();
    }

    public ProgressViewModel? ViewModel
    {
        get => (ProgressViewModel?)GetValue(ProgressViewModelProperty);
        set => SetValue(ProgressViewModelProperty, value);
    }

    public static readonly DependencyProperty ProgressViewModelProperty =
        DependencyProperty.Register(
            nameof(ViewModel), typeof(ProgressViewModel), typeof(ProgressPage),
            new PropertyMetadata(null));

    private void ListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        foreach (var targetVM in e.AddedItems.OfType<ProgressTargetViewModel>())
            targetVM.IsSelected = true;

        foreach (var targetVM in e.RemovedItems.OfType<ProgressTargetViewModel>())
            targetVM.IsSelected = false;
    }
}
