using System.Collections.ObjectModel;
using System.Windows;
using CommunityToolkit.Mvvm.DependencyInjection;
using FlexID.ViewModels;
using FlexID.Views;
using Microsoft.Extensions.DependencyInjection;

namespace FlexID;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App
{
    public App()
    {
        Ioc.Default.ConfigureServices(
            new ServiceCollection()
            .AddTransient<MainWindow>()
            .AddTransient<MainViewModel>()
            .AddTransient<InputOIRView>()
            .AddTransient<InputOIRViewModel>()
            .AddTransient<InputEIRView>()
            .AddTransient<InputEIRViewModel>()
            .AddTransient<ScoeffCalcView>()
            .AddTransient<ScoeffCalcViewModel>()
            .BuildServiceProvider());
    }

    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        var mainWindow = Ioc.Default.GetRequiredService<MainWindow>();

        mainWindow.Show();
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
