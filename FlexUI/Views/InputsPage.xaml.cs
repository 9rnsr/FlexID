using System.Windows.Input;
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

        SearchFilter.Updated += (_, _) => Targets?.AttachFilter(SearchFilter);
    }

    public static readonly DependencyProperty IsBuiltinProperty =
        DependencyProperty.Register(
            nameof(IsBuiltin), typeof(bool), typeof(InputsPage),
            new PropertyMetadata(true));

    public bool IsBuiltin
    {
        get => (bool)GetValue(IsBuiltinProperty);
        set => SetValue(IsBuiltinProperty, value);
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

    public SearchFilterViewModel SearchFilter { get; } = new();

    public ElementsViewModel ElementsTable { get; } = new();

    private void ElementsFlyout_Closed(object sender, object e)
    {
        var elements = ElementsTable.CheckedElements
            .Select(elementVM => elementVM.Element);

        SearchFilter.TargetElements.Clear();
        SearchFilter.TargetElements.AddRange(elements);
    }

    public static readonly DependencyProperty AddCommandProperty =
        DependencyProperty.Register(
            nameof(AddCommand), typeof(ICommand), typeof(InputsPage),
            new PropertyMetadata(null));

    public ICommand AddCommand
    {
        get => (ICommand)GetValue(AddCommandProperty);
        set => SetValue(AddCommandProperty, value);
    }

    public static readonly DependencyProperty RemoveCommandProperty =
        DependencyProperty.Register(
            nameof(RemoveCommand), typeof(ICommand), typeof(InputsPage),
            new PropertyMetadata(null));

    public ICommand RemoveCommand
    {
        get => (ICommand)GetValue(RemoveCommandProperty);
        set => SetValue(RemoveCommandProperty, value);
    }

    private void RemoveButton_Click(object sender, RoutedEventArgs e)
    {
        var selectedItems = InputTargets.SelectedItems;
        if (selectedItems.Count == 0)
            return;

        RemoveCommand?.Execute(selectedItems.ToArray());
    }

    private void ListView_RemoveItems(object sender, RoutedEventArgs e)
    {
        if (IsBuiltin)
            return;
        var selectedItems = InputTargets.SelectedItems;
        if (selectedItems.Count == 0)
            return;

        RemoveCommand?.Execute(selectedItems.ToArray());
    }

    public string SwitchInputsButtonText(bool isBuiltin) =>
        isBuiltin ? "External inputs ..." : "Built-in inputs ...";

    private void SwitchInputsButton_Click(object sender, RoutedEventArgs e)
    {
        IsBuiltin = !IsBuiltin;
    }
}
