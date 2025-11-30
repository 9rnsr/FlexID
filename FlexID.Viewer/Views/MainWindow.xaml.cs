using System.Windows.Controls;
using System.Windows.Input;
using CommunityToolkit.Mvvm.DependencyInjection;

namespace FlexID.Viewer.Views;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow
{
    public MainWindow()
    {
        InitializeComponent();

        DataContext = Ioc.Default.GetService<ViewModels.MainViewModel>();
    }

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

    private void OutputFilePathTextBox_KeyDown(object sender, KeyEventArgs e)
    {
        if (e.Key == Key.Enter && Keyboard.Modifiers == ModifierKeys.None)
        {
            var binding = ((TextBox)sender).GetBindingExpression(TextBox.TextProperty);
            binding?.UpdateSource();
        }
    }
}
