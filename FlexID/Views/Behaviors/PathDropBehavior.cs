using Microsoft.Xaml.Behaviors;
using System.Windows;
using System.Windows.Controls;

namespace FlexID.Views.Behaviors
{
    /// <summary>
    /// 関連付けられたTextBoxに、ファイルのドロップ操作を付加する。
    /// </summary>
    public class PathDropBehavior : Behavior<TextBox>
    {
        protected override void OnAttached()
        {
            var c = AssociatedObject;
            c.PreviewDragOver += TextBox_DragOver;
            c.PreviewDrop += TextBox_Drop;

            c.AllowDrop = true;
        }

        protected override void OnDetaching()
        {
            var c = AssociatedObject;
            c.PreviewDragOver -= TextBox_DragOver;
            c.PreviewDrop -= TextBox_Drop;
        }

        private void TextBox_DragOver(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
                e.Effects = DragDropEffects.Copy;
            else
                e.Effects = DragDropEffects.None;
            e.Handled = true;
        }

        private void TextBox_Drop(object sender, DragEventArgs e)
        {
            var files = e.Data.GetData(DataFormats.FileDrop, false) as string[];
            if (files != null)
                AssociatedObject.Text = files[0];
        }
    }
}
