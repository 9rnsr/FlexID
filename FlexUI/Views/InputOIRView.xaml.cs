using System.ComponentModel;
using System.Windows.Controls;
using CommunityToolkit.Mvvm.DependencyInjection;

namespace FlexID.Views;

/// <summary>
/// Interaction logic for InputOIRView
/// </summary>
public partial class InputOIRView : UserControl
{
    public InputOIRView()
    {
        InitializeComponent();

        if (!DesignerProperties.GetIsInDesignMode(this))
            DataContext = Ioc.Default.GetService<ViewModels.InputOIRViewModel>();
    }
}
