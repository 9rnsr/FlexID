namespace FlexID.Viewer.Views.Behaviors;

using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Microsoft.Xaml.Behaviors;

/// <summary>
/// パス文字列に対する貼り付け挙動を定義する。
/// </summary>
public class PathPasteBehavior : Behavior<TextBox>
{
    protected override void OnAttached()
    {
        var tb = AssociatedObject;
        CommandManager.AddPreviewCanExecuteHandler(tb, TextBox_PreviewCanExecute);
        CommandManager.AddPreviewExecutedHandler(tb, TextBox_PreviewExecuted);
    }

    protected override void OnDetaching()
    {
        var tb = AssociatedObject;
        CommandManager.RemovePreviewCanExecuteHandler(tb, TextBox_PreviewCanExecute);
        CommandManager.RemovePreviewExecutedHandler(tb, TextBox_PreviewExecuted);
    }

    private void TextBox_PreviewCanExecute(object sender, CanExecuteRoutedEventArgs e)
    {
        if (e.Command == ApplicationCommands.Paste)
        {
            var dataObj = Clipboard.GetDataObject();
            var paths = (string[])dataObj.GetData(DataFormats.FileDrop);
            if (paths is null)
                return;

            var tb = (TextBox)sender;
            if (!tb.AcceptsReturn && paths.Length != 1)
                return;

            e.CanExecute = true;
            e.Handled = true;
        }
    }

    private void TextBox_PreviewExecuted(object sender, ExecutedRoutedEventArgs e)
    {
        if (e.Command == ApplicationCommands.Paste)
        {
            var dataObj = Clipboard.GetDataObject();
            var paths = (string[])dataObj.GetData(DataFormats.FileDrop);
            if (paths is null || paths.Length != 1)
                return;

            paths = paths.Select(ShortcutFile.Resolve).ToArray();

            var tb = (TextBox)sender;
            var path = string.Join("\n", paths);    // 複数ファイルは改行で区切る。

            var prevSelectionStart = tb.IsSelectionActive ? tb.SelectionStart : tb.CaretIndex;

            // SelectedText経由で選択範囲を置き換えることで、Undoも効く。
            tb.SelectedText = path;

            tb.CaretIndex = prevSelectionStart + path.Length;
        }
    }
}
