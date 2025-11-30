using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Microsoft.Xaml.Behaviors;

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
public class PathDropBehavior : Behavior<Control>
{
    public static readonly DependencyProperty AllowDropPathProperty =
        DependencyProperty.Register(
            nameof(AllowDropPath), typeof(AllowDropPath), typeof(PathDropBehavior));

    public static readonly DependencyProperty DropCommandProperty =
        DependencyProperty.Register(
            nameof(DropCommand), typeof(ICommand), typeof(PathDropBehavior));

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
        c.PreviewDragEnter += OnPreviewDragEnter;
        c.PreviewDragOver += OnPreviewDragOver;
        c.PreviewDragLeave += OnPreviewDragLeave;
        c.Drop += OnDrop;
    }

    protected override void OnDetaching()
    {
        var c = AssociatedObject;
        c.PreviewDragEnter -= OnPreviewDragEnter;
        c.PreviewDragOver -= OnPreviewDragOver;
        c.PreviewDragLeave -= OnPreviewDragLeave;
        c.Drop -= OnDrop;
    }

    private void OnPreviewDragEnter(object sender, DragEventArgs e) => CheckDragEventArgs(e);

    private void OnPreviewDragOver(object sender, DragEventArgs e) => CheckDragEventArgs(e);

    private void OnPreviewDragLeave(object sender, DragEventArgs e)
    {
        CheckDragEventArgs(e);
        cacheDataObject = null;
        cacheEffects = DragDropEffects.None;
    }

    private void OnDrop(object sender, DragEventArgs e)
    {
        var paths = e.Data.GetData(DataFormats.FileDrop) as string[];
        if (paths is null)
            return;

        e.Handled = true;

        var command = DropCommand;
        if (command != null)
        {
            paths = FilteringPaths(paths);
            if (paths != null)
            {
                command.Execute(paths);
            }
        }
    }

    private IDataObject cacheDataObject;
    private DragDropEffects cacheEffects;

    private void CheckDragEventArgs(DragEventArgs e)
    {
        if (ReferenceEquals(e.Data, cacheDataObject))
        {
            e.Effects = cacheEffects;
            e.Handled = true;
            return;
        }

        // エクスプローラからファイルやフォルダをドロップすることを許可する
        var paths = e.Data.GetData(DataFormats.FileDrop) as string[];
        if (paths is null)
            return;

        e.Handled = true;
        e.Effects = DragDropEffects.None;

        paths = FilteringPaths(paths);
        if (paths != null)
            e.Effects = DragDropEffects.Copy;

        cacheDataObject = e.Data;
        cacheEffects = e.Effects;
    }

    private string[] FilteringPaths(string[] paths)
    {
        void ResolveLinkPath()
        {
            paths = paths.Select(ShortcutFile.Resolve).ToArray();
        }

        if (paths.Any())
        {
            switch (AllowDropPath)
            {
                case AllowDropPath.None:
                    break;

                case AllowDropPath.SingleAny:
                    if (paths.Length != 1)
                        break;
                    ResolveLinkPath();
                    return paths;

                case AllowDropPath.SingleExists:
                    if (paths.Length != 1)
                        break;
                    ResolveLinkPath();
                    if (File.Exists(paths[0]) || Directory.Exists(paths[0]))
                        return paths;
                    break;

                case AllowDropPath.SingleFile:
                    if (paths.Length != 1)
                        break;
                    ResolveLinkPath();
                    if (File.Exists(paths[0]))
                        return paths;
                    break;

                case AllowDropPath.SingleFolder:
                    if (paths.Length != 1)
                        break;
                    ResolveLinkPath();
                    if (Directory.Exists(paths[0]))
                        return paths;
                    break;

                case AllowDropPath.MultiAny:
                    if (!paths.Any())
                        break;
                    ResolveLinkPath();
                    return paths;

                case AllowDropPath.MultiExists:
                    ResolveLinkPath();
                    if (paths.All(path => File.Exists(paths[0]) || Directory.Exists(paths[0])))
                        return paths;
                    break;

                case AllowDropPath.MultiFiles:
                    ResolveLinkPath();
                    if (paths.All(path => File.Exists(paths[0])))
                        return paths;
                    break;

                case AllowDropPath.MultiFolders:
                    ResolveLinkPath();
                    if (paths.All(path => Directory.Exists(paths[0])))
                        return paths;
                    break;

                default:
                    break;
            }
        }
        return null;
    }
}
