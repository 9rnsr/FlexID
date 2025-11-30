using System.ComponentModel;
using System.Windows.Controls;
using CommunityToolkit.Mvvm.DependencyInjection;

namespace FlexID.Views;

/// <summary>
/// Interaction logic for ScoeffCalcView
/// </summary>
public partial class ScoeffCalcView : UserControl
{
    public ScoeffCalcView()
    {
        InitializeComponent();

        if (!DesignerProperties.GetIsInDesignMode(this))
            DataContext = Ioc.Default.GetService<ViewModels.ScoeffCalcViewModel>();
    }
}
