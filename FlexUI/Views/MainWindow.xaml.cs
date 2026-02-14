using FlexID.ViewModels;

namespace FlexID.Views;

public sealed partial class MainWindow
{
    public MainWindow(MainViewModel vm)
    {
        ViewModel = vm;

        InitializeComponent();
    }

    public MainViewModel ViewModel { get; }
}
