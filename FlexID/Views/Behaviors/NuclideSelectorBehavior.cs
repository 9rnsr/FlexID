namespace FlexID.Views.Behaviors;

using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;
using Microsoft.Xaml.Behaviors;

/// <summary>
/// 核種選択用ListViewの挙動を定義する。
/// </summary>
public class NuclideSelectorBehavior : Behavior<ListView>
{
    public NuclideSelectorBehavior()
    {
        resetNuclideInputTimer = new DispatcherTimer();
        resetNuclideInputTimer.Interval = new TimeSpan(0, 0, 1);
        resetNuclideInputTimer.Tick += (o, e) => StopResetTimeout();
    }

    // 文字入力でマッチする核種名に選択項目を移動する機能。
    private readonly DispatcherTimer resetNuclideInputTimer;
    private string searchPattern = string.Empty;
    private bool avoidResetTimeout = false;

    protected override void OnAttached()
    {
        var lv = AssociatedObject;
        lv.SelectionChanged += ListView_SelectionChanged;
        lv.PreviewKeyDown += ListView_PreviewKeyDown;
        lv.PreviewTextInput += ListView_PreviewTextInput;
    }

    protected override void OnDetaching()
    {
        var lv = AssociatedObject;
        lv.PreviewKeyDown -= ListView_PreviewKeyDown;
        lv.PreviewTextInput -= ListView_PreviewTextInput;
        lv.SelectionChanged -= ListView_SelectionChanged;
    }

    private void StartResetTimeout()
    {
        resetNuclideInputTimer.Stop();
        resetNuclideInputTimer.Start();
    }

    private void StopResetTimeout()
    {
        resetNuclideInputTimer.Stop();
        searchPattern = string.Empty;
        //System.Diagnostics.Debug.WriteLine($"--> reset");
    }

    private void SelectAndFocusItem(int index)
    {
        var lv = AssociatedObject;
        if (lv.Items.Count == 0)
            return;

        var item = lv.Items[index];
        lv.ScrollIntoView(item);
        lv.SelectedItem = item;

        var lvitem = lv.ItemContainerGenerator.ContainerFromIndex(index) as ListViewItem;
        lvitem.Focus();
    }

    private void ListView_PreviewKeyDown(object sender, KeyEventArgs e)
    {
        if (e.Key == Key.Space)
        {
            // 選択項目のチェック状態を切り替える。
            e.Handled = true;

            var lv = AssociatedObject;
            var value = lv.SelectedValue;
            if (value is null)
                return;

            // 現在フォーカスがある項目のチェック状態を基準にする。
            var currentChecked = ((ViewModels.NuclideItem)value).IsChecked;
            foreach (ViewModels.NuclideItem nucItem in lv.SelectedItems)
            {
                nucItem.IsChecked = !currentChecked;
            }
        }
        if (e.Key == Key.Home)
        {
            // フォーカス位置を最初の項目に移動する。
            e.Handled = true;

            var lv = AssociatedObject;
            SelectAndFocusItem(index: 0);
            return;
        }
        if (e.Key == Key.End)
        {
            // フォーカス位置を最後の項目に移動する。
            e.Handled = true;

            var lv = AssociatedObject;
            SelectAndFocusItem(index: lv.Items.Count - 1);
            return;
        }
    }

    private void ListView_PreviewTextInput(object sender, TextCompositionEventArgs e)
    {
        if (searchPattern is null)
        {
            e.Handled = true;
            StartResetTimeout();
            return;
        }

        // 核種名の先頭から数文字をキー入力した際に対象を選択する。
        var startChar = char.ToUpperInvariant(searchPattern.Length == 0 ? e.Text[0] : searchPattern[0]);
        if ('A' <= startChar && startChar <= 'Z')
        {
            e.Handled = true;

            resetNuclideInputTimer.Stop();
            //System.Diagnostics.Debug.WriteLine($"pattern = '{searchPattern}{e.Text}'  <-- '{searchPattern}' + '{e.Text}'");
            searchPattern += e.Text;

            var lv = AssociatedObject;
            var count = lv.Items.Count;
            var current = lv.SelectedIndex;
            if (current < 0)
                current = 0;
            for (int i = 0; i < count; i++)
            {
                // 現在の選択項目から検索を開始する。
                var index = (current + i) % count;

                var item = (ViewModels.NuclideItem)lv.Items[index];
                if (!item.Nuclide.StartsWith(searchPattern, StringComparison.OrdinalIgnoreCase))
                    continue;

                avoidResetTimeout = true;
                SelectAndFocusItem(index);
                StartResetTimeout();
                return;
            }

            // これ以上のキー入力は決してマッチしない状態であることを記憶する。
            searchPattern = null;
            StartResetTimeout();
            return;
        }
    }

    private void ListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (!avoidResetTimeout)
        {
            // 矢印キーなどによる項目移動は、核種名入力の状態をリセットする。
            StopResetTimeout();
        }
        avoidResetTimeout = false;
    }
}
