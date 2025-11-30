using System.Windows;
using FlexID.Viewer.ViewModels;
using FlexID.Viewer.Views;
using Prism.Ioc;

namespace FlexID.Viewer;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App
{
    private string _outPath;

    protected override void OnStartup(StartupEventArgs e)
    {
        // Args ==1     入力GUIからの実行
        // Args !=1(0)  exeファイル直接実行
        if (e.Args.Length == 1)
        {
            var outPath = e.Args[0];
            if (outPath.StartsWith("\"") && outPath.EndsWith("\""))
                outPath = outPath.Substring(1, outPath.Length - 2);
            _outPath = outPath;
        }

        base.OnStartup(e);
    }

    protected override Window CreateShell()
    {
        var window = Container.Resolve<MainWindow>();
        if (_outPath is not null)
        {
            var vm = (MainWindowViewModel)window.DataContext;
            vm.OutputFilePath = _outPath;
        }
        return window;
    }

    protected override void RegisterTypes(IContainerRegistry containerRegistry)
    {
    }
}
