using System.Collections.ObjectModel;
using System.Diagnostics;
using CommunityToolkit.Mvvm.DependencyInjection;
using FlexID.ViewModels;
using FlexID.Views;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using R3;

namespace FlexID;

public partial class App
{
    public App()
    {
        InitializeComponent();

        WinUI3ProviderInitializer.SetDefaultObservableSystem(ex => Trace.WriteLine(ex.ToString()));
    }

    public new static App Current => (App)Application.Current;

    private Window? _window;

    public Window? Window => _window;

    public AppWindow? AppWindow => _window?.AppWindow;

    /// <summary>
    /// Invoked when the application is launched.
    /// </summary>
    /// <param name="args">Details about the launch request and process.</param>
    protected override void OnLaunched(LaunchActivatedEventArgs args)
    {
        Ioc.Default.ConfigureServices(
            new ServiceCollection()
            .AddTransient<MainWindow>()
            .AddTransient<MainViewModel>()
            .AddTransient<InputOirViewModel>()
            .AddTransient<InputEirViewModel>()
            .AddTransient<InputScoeffViewModel>()
            .AddTransient<ViewerWindow>()
            .AddTransient<ViewerViewModel>()
            .BuildServiceProvider());

        var arguments = Environment.GetCommandLineArgs();

        if (arguments.Length == 2)
        {
            var outPath = arguments[1];
            if (outPath.Length >= 2 && outPath.StartsWith("\"") && outPath.EndsWith("\""))
                outPath = outPath[1..^1];

            var viewerWindow = Ioc.Default.GetRequiredService<ViewerWindow>();
            var vm = viewerWindow.ViewModel;
            vm.OutputFilePath = outPath;

            _window = viewerWindow;
        }
        else
        {
            var mainWindow = Ioc.Default.GetRequiredService<MainWindow>();
            _window = mainWindow;
        }
        _window.Activate();
    }
}

public static class CollectionExtensions
{
    public static void AddRange<T>(this ObservableCollection<T> collection, IEnumerable<T> items)
    {
        foreach (var item in items)
        {
            collection.Add(item);
        }
    }

    public static void Replace<T>(this ObservableCollection<T> collection, IEnumerable<T> items)
    {
        collection.Clear();
        foreach (var item in items)
        {
            collection.Add(item);
        }
    }
}
