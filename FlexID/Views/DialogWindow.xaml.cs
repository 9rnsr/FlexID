using Prism.Services.Dialogs;
using System.Windows;

namespace FlexID.Views
{
    /// <summary>
    /// Interaction logic for DialogWindow.xaml
    /// </summary>
    public partial class DialogWindow : IDialogWindow
    {
        public DialogWindow()
        {
            this.WindowStartupLocation = WindowStartupLocation.CenterOwner;

            InitializeComponent();
        }

        public IDialogResult Result { get; set; }
    }
}
