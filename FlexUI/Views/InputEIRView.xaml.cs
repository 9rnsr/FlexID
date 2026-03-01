using System.ComponentModel;
using System.Windows.Controls;
using CommunityToolkit.Mvvm.DependencyInjection;

namespace FlexID.Views;

/// <summary>
/// Interaction logic for InputEIRView
/// </summary>
public partial class InputEIRView : UserControl
{
    public InputEIRView()
    {
        InitializeComponent();

        if (!DesignerProperties.GetIsInDesignMode(this))
            DataContext = Ioc.Default.GetService<ViewModels.InputEIRViewModel>();
    }
}
