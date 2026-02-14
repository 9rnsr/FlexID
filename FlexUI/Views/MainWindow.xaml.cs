using FlexID.ViewModels;

namespace FlexID.Views;

public sealed partial class MainWindow
{
    public MainWindow(MainViewModel vm)
    {
        ViewModel = vm;
        //SizeToContent = SizeToContent.Height;

        InitializeComponent();

        //DataContext = ViewModel;

        //ContentRendered += MainWindow_ContentRendered;

        //void MainWindow_ContentRendered(object sender, System.EventArgs e)
        //{
        //    ContentRendered -= MainWindow_ContentRendered;
        //    SizeToContent = SizeToContent.Manual;
        //}
    }

    public MainViewModel ViewModel { get; }
}
