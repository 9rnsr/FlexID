using FlexID.Models;
using FlexID.ViewModels;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace FlexID.Views;

public sealed partial class InputsPage : Page
{
    public InputsPage()
    {
        InitializeComponent();
    }

    public static readonly DependencyProperty TargetsProperty =
        DependencyProperty.Register(
            nameof(Targets), typeof(CheckableItemsView<InputTarget, InputTargetViewModel>), typeof(InputsPage),
            new PropertyMetadata(null));

    public CheckableItemsView<InputTarget, InputTargetViewModel>? Targets
    {
        get => (CheckableItemsView<InputTarget, InputTargetViewModel>?)GetValue(TargetsProperty);
        set => SetValue(TargetsProperty, value);
    }

    public ElementsViewModel ElementsTable { get; } = new();

    private void ElementsFlyout_Closed(object sender, object e)
    {
        var elements = ElementsTable.CheckedElements
            .Select(elementVM => elementVM.Element);

        // Update search filter...
    }
}
