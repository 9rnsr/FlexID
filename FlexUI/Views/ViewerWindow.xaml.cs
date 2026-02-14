using FlexID.ViewModels;
using Microsoft.UI.Input;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Input;
using Windows.System;

namespace FlexID.Views;

public partial class ViewerWindow
{
    public ViewerWindow(ViewerViewModel vm)
    {
        ViewModel = vm;

        InitializeComponent();
    }

    public ViewerViewModel ViewModel { get; }

    private void WindowContent_Loaded(object sender, RoutedEventArgs e)
    {
        // タブオーダーに従って初期フォーカスを設定する。
        var first = FocusManager.FindFirstFocusableElement(Content) as UIElement;
        first?.Focus(FocusState.Programmatic);
    }

    private void Page_PreviewKeyDown(object sender, KeyRoutedEventArgs e)
    {
        void CycleNavigation(bool reverse)
        {
            var count = PivotSegmented.Items.Count;
            var index = PivotSegmented.SelectedIndex;
            if (reverse)
                index = (index + count - 1) % count;
            else
                index = (index + 1) % count;
            PivotSegmented.SelectedIndex = index;
        }

        var ctrlDown = InputKeyboardSource.IsKeyDown(VirtualKey.Control);
        if (ctrlDown)
        {
            // Ctrl + (Shift +) Tab
            if (e.Key == VirtualKey.Tab)
            {
                e.Handled = true;
                var shiftDown = InputKeyboardSource.IsKeyDown(VirtualKey.Shift);
                CycleNavigation(reverse: shiftDown);
                return;
            }

            // Ctrl + PageDown
            if (e.Key == VirtualKey.PageDown)
            {
                e.Handled = true;
                CycleNavigation(reverse: false);
                return;
            }
            // Ctrl + PageUp
            if (e.Key == VirtualKey.PageUp)
            {
                e.Handled = true;
                CycleNavigation(reverse: true);
                return;
            }
        }
    }

    private void OutputFilePathTextBox_KeyDown(object sender, KeyRoutedEventArgs e)
    {
        //if (e.Key == Key.Enter && Keyboard.Modifiers == ModifierKeys.None)
        //{
        //    var binding = ((TextBox)sender).GetBindingExpression(TextBox.TextProperty);
        //    binding?.UpdateSource();
        //}
    }
}
