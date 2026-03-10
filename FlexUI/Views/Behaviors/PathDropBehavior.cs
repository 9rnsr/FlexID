using System.Windows.Input;
using Microsoft.UI.Xaml;
using Microsoft.Xaml.Interactivity;
using Windows.ApplicationModel.DataTransfer;
using Windows.Storage;

namespace FlexID.Views.Behaviors;

public enum AllowDropPath
{
    None,
    SingleAny,
    SingleExists,
    SingleFile,
    SingleFolder,
    MultiAny,
    MultiExists,
    MultiFiles,
    MultiFolders,
}

/// <summary>
/// パス文字列に対するドラッグ&ドロップ挙動を定義する。
/// </summary>
public class PathDropBehavior : Behavior<UIElement>
{
    public static readonly DependencyProperty AllowDropPathProperty =
        DependencyProperty.Register(
            nameof(AllowDropPath), typeof(AllowDropPath), typeof(PathDropBehavior),
            new PropertyMetadata(AllowDropPath.None));

    public static readonly DependencyProperty DropCommandProperty =
        DependencyProperty.Register(
            nameof(DropCommand), typeof(ICommand), typeof(PathDropBehavior),
            new PropertyMetadata(null));

    public AllowDropPath AllowDropPath
    {
        get => (AllowDropPath)GetValue(AllowDropPathProperty);
        set => SetValue(AllowDropPathProperty, value);
    }

    public ICommand DropCommand
    {
        get => (ICommand)GetValue(DropCommandProperty);
        set => SetValue(DropCommandProperty, value);
    }

    protected override void OnAttached()
    {
        var c = AssociatedObject;
        c.AllowDrop = true;
        c.DragEnter += OnDragEnter;
        c.DragOver += OnDragOver;
        c.DragLeave += OnDragLeave;
        c.Drop += OnDrop;
    }

    protected override void OnDetaching()
    {
        var c = AssociatedObject;
        c.DragEnter -= OnDragEnter;
        c.DragOver -= OnDragOver;
        c.DragLeave -= OnDragLeave;
        c.Drop -= OnDrop;
    }

    private DataPackageOperation? cacheAcceptedOperation;

    private async void OnDragEnter(object sender, DragEventArgs e) => await CheckDragEventArgs(e);

    private async void OnDragOver(object sender, DragEventArgs e) => await CheckDragEventArgs(e);

    private async Task CheckDragEventArgs(DragEventArgs e)
    {
        e.Handled = true;

        if (cacheAcceptedOperation is null)
        {
            // start cache
            cacheAcceptedOperation = DataPackageOperation.None;

            // エクスプローラからファイルやフォルダをドロップすることを許可する
            var items = await e.DataView.GetStorageItemsAsync();
            if (items is null)
                return;

            e.AcceptedOperation = DataPackageOperation.None;

            items = await FilteringPaths(items);
            if (items != null)
                e.AcceptedOperation = DataPackageOperation.Copy;

            // finish cache with the determined operation
            cacheAcceptedOperation = e.AcceptedOperation;
        }
        else
        {
            // use cache
            e.AcceptedOperation = cacheAcceptedOperation.Value;
        }
    }

    private async void OnDragLeave(object sender, DragEventArgs e)
    {
        await CheckDragEventArgs(e);

        // clear cache
        cacheAcceptedOperation = null;
    }

    private async void OnDrop(object sender, DragEventArgs e)
    {
        e.Handled = true;

        // clear cache
        cacheAcceptedOperation = null;

        var items = await e.DataView.GetStorageItemsAsync();
        if (items is null)
            return;

        var command = DropCommand;
        if (command != null)
        {
            items = await FilteringPaths(items);
            if (items != null)
            {
                var paths = items.Select(item => item.Path).ToArray();
                command.Execute(paths);
            }
        }
    }

    private async ValueTask<IReadOnlyList<IStorageItem?>?> FilteringPaths(IReadOnlyList<IStorageItem> items)
    {
        static async ValueTask<IReadOnlyList<IStorageItem>?> ResolveItems(IReadOnlyList<IStorageItem> items)
        {
            var results = new IStorageItem[items.Count];
            for (int i = 0; i < items.Count; i++)
            {
                var path = ShortcutFile.Resolve(items[i].Path);
                try { results[i] = await StorageFile.GetFileFromPathAsync(path); continue; } catch { }
                try { results[i] = await StorageFolder.GetFolderFromPathAsync(path); continue; } catch { }
                return null;
            }
            return results;
        }

        if (items.Any())
        {
            IReadOnlyList<IStorageItem>? results;
            switch (AllowDropPath)
            {
                case AllowDropPath.None:
                    break;

                case AllowDropPath.SingleAny:
                    if (items.Count != 1)
                        break;
                    return await ResolveItems(items);

                case AllowDropPath.SingleExists:
                    if (items.Count != 1 || (results = await ResolveItems(items)) is null)
                        break;
                    if (!File.Exists(results[0].Path) && !Directory.Exists(results[0].Path))
                        break;
                    return results;

                case AllowDropPath.SingleFile:
                    if (items.Count != 1 || (results = await ResolveItems(items)) is null)
                        break;
                    if (!File.Exists(results[0].Path))
                        break;
                    return results;

                case AllowDropPath.SingleFolder:
                    if (items.Count != 1 || (results = await ResolveItems(items)) is null)
                        break;
                    if (!Directory.Exists(results[0].Path))
                        break;
                    return results;

                case AllowDropPath.MultiAny:
                    if (!items.Any())
                        break;
                    return await ResolveItems(items);

                case AllowDropPath.MultiExists:
                    if ((results = await ResolveItems(items)) is null)
                        break;
                    if (results.Any(item => !File.Exists(item.Path) && !Directory.Exists(item.Path)))
                        break;
                    return results;

                case AllowDropPath.MultiFiles:
                    if ((results = await ResolveItems(items)) is null)
                        break;
                    if (results.Any(item => !File.Exists(item.Path)))
                        break;
                    return results;

                case AllowDropPath.MultiFolders:
                    if ((results = await ResolveItems(items)) is null)
                        break;
                    if (results.Any(item => !Directory.Exists(item.Path)))
                        break;
                    return results;

                default:
                    break;
            }
        }
        return null;
    }
}
