using System.Windows.Controls;
using System.Windows.Input;

namespace FlexID.Viewer.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        public MainWindow()
        {
            InitializeComponent();
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
}
