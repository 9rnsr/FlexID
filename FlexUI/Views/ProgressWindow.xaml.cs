using FlexID.ViewModels;

namespace FlexID.Views;

public sealed partial class ProgressWindow
{
    public ProgressWindow(ProgressViewModel vm)
    {
        ViewModel = vm;

        InitializeComponent();
    }

    public ProgressViewModel ViewModel { get; }
}
