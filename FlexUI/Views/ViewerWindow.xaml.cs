using FlexID.ViewModels;
using Microsoft.UI.Xaml.Input;

namespace FlexID.Views;

public partial class ViewerWindow
{
    public ViewerWindow(ViewerViewModel vm)
    {
        ViewModel = vm;

        InitializeComponent();
    }

    public ViewerViewModel ViewModel { get; }

#if false
    private void MetroWindow_PreviewKeyDown(object sender, KeyEventArgs e)
    {
        // Ctrl+Tab
        if (e.Key == Key.Tab && (Keyboard.Modifiers & ModifierKeys.Control) != 0)
        {
            var index = MainTabControl.SelectedIndex;
            var count = MainTabControl.Items.Count;

            var shiftPressed = (Keyboard.Modifiers & ~ModifierKeys.Control) == ModifierKeys.Shift;
            index += (shiftPressed ? -1 : +1);
            if (index >= count)
                index = 0;
            else if (index < 0)
                index = count - 1;

            MainTabControl.SelectedIndex = index;

            e.Handled = true;
        }
    }
#endif

    private void OutputFilePathTextBox_KeyDown(object sender, KeyRoutedEventArgs e)
    {
        //if (e.Key == Key.Enter && Keyboard.Modifiers == ModifierKeys.None)
        //{
        //    var binding = ((TextBox)sender).GetBindingExpression(TextBox.TextProperty);
        //    binding?.UpdateSource();
        //}
    }
}
