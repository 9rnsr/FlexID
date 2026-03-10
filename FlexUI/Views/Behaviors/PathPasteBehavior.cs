using Microsoft.UI.Xaml.Controls;
using Microsoft.Xaml.Interactivity;
using Windows.ApplicationModel.DataTransfer;

namespace FlexID.Views.Behaviors;

/// <summary>
/// パス文字列に対する貼り付け挙動を定義する。
/// </summary>
public class PathPasteBehavior : Behavior<TextBox>
{
    protected override void OnAttached()
    {
        var tb = AssociatedObject;
        tb.Paste += TextBox_Paste;
    }

    protected override void OnDetaching()
    {
        var tb = AssociatedObject;
        tb.Paste -= TextBox_Paste;
    }

    private async void TextBox_Paste(object sender, TextControlPasteEventArgs args)
    {
        var view = Clipboard.GetContent();
        if (!view.Contains(StandardDataFormats.StorageItems))
            return;

        var items = await view.GetStorageItemsAsync();

        var tb = (TextBox)sender;
        if (!tb.AcceptsReturn && items.Count != 1)
            return;

        args.Handled = true;

        var paths = items.Select(item => ShortcutFile.Resolve(item.Path)).ToArray();
        var path = string.Join("\n", paths);    // 複数ファイルは改行で区切る。

        // SelectedText経由で選択範囲を置き換えることで、Undoも効く。
        tb.SelectedText = path;
    }
}
