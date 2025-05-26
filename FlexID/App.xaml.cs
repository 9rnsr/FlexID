namespace FlexID;

using System.Windows;
using FlexID.ViewModels;
using FlexID.Views;
using Prism.Ioc;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App
{
    protected override void RegisterTypes(IContainerRegistry containerRegistry)
    {
        containerRegistry.RegisterSingleton<CalcState>();
        containerRegistry.RegisterForNavigation<InputOIRView>();
        containerRegistry.RegisterForNavigation<InputEIRView>();
    }

    protected override Window CreateShell()
    {
        return Container.Resolve<MainWindow>();
    }
}
