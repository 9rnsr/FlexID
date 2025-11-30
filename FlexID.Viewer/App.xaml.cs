using System.Collections.ObjectModel;
using System.Windows;
using CommunityToolkit.Mvvm.DependencyInjection;
using FlexID.Viewer.ViewModels;
using FlexID.Viewer.Views;
using Microsoft.Extensions.DependencyInjection;

namespace FlexID.Viewer;

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
            .BuildServiceProvider());
    }

    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        var mainWindow = Ioc.Default.GetRequiredService<MainWindow>();

        // Args == 1     入力GUIからの実行
        // Args != 1(0)  exeファイル直接実行
        if (e.Args.Length == 1)
        {
            var outPath = e.Args[0];
            if (outPath.StartsWith("\"") && outPath.EndsWith("\""))
                outPath = outPath.Substring(1, outPath.Length - 2);

            var vm = (MainViewModel)mainWindow.DataContext;
            vm.OutputFilePath = outPath;
        }

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
