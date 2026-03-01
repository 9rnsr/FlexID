using System.Windows;
using CommunityToolkit.Mvvm.DependencyInjection;
using FlexID.ViewModels;

namespace FlexID.Views;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    public MainWindow()
    {
        SizeToContent = SizeToContent.Height;

        InitializeComponent();

        DataContext = Ioc.Default.GetService<MainViewModel>();

        ContentRendered += MainWindow_ContentRendered;

        void MainWindow_ContentRendered(object sender, System.EventArgs e)
        {
            ContentRendered -= MainWindow_ContentRendered;
            SizeToContent = SizeToContent.Manual;
        }
    }
}
