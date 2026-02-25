using CommunityToolkit.WinUI;
using FlexID.Models;
using Microsoft.UI.Input;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Input;
using Microsoft.Xaml.Interactivity;
using Windows.Foundation;
using Windows.System;

namespace FlexID.Views.Behaviors;

/// <summary>
/// 処理対象項目選択用ListViewの挙動を定義する。
/// </summary>
public partial class CheckSearchItemsBehavior : Behavior<ListViewBase>
{
    public CheckSearchItemsBehavior()
    {
        _resetNuclideInputTimer = new DispatcherTimer();
        _resetNuclideInputTimer.Interval = new TimeSpan(0, 0, 1);
        _resetNuclideInputTimer.Tick += (o, e) => StopResetTimeout();
    }

    public event TypedEventHandler<object, RoutedEventArgs>? DeleteItems;

    // 文字入力でマッチする核種名に選択項目を移動する機能。
    private readonly DispatcherTimer _resetNuclideInputTimer;
    private string? _searchPattern = string.Empty;
    private bool _avoidResetTimeout = false;

    #region Behaviorの初期化と後始末のタイミング捕捉

    // WinUI3の以下のような挙動に対応するためタイミング捕捉のための処理を行っている。
    // (1) Pageがキャッシュされている場合
    //     - Loadedイベント後に再度のLoaded, OnAttached, Unloadedが発生することがあるため、これを
    //       _initialized == nullとして記憶し「fakeな」イベントとして無視する処理を行う。
    //       またこの場合は正しい後始末のタイミングでOnDetachingが呼ばれないため、代わりに
    //       Unloadedイベントで後始末を行うようにする。
    //     - PageのキャッシュによってOnAttached時点でGridViewが既にIsLoaded == trueの場合は、
    //       Loadedイベントは発生せず、しかしOnDetachingの後にUnloadedイベントが発生する。
    //       このケースでは(1)とは逆に、OnDetachingで後始末を実施する必要がある。
    //     - 上記の問題のためAssociatedObject is nullとなる場所では、senderを代わりに使うハックが必要となる。
    // (2) Pageのキャッシュがない場合
    //     - こちらでもイベントが混じるように見えるが、この場合は
    //       異なるBehaviorインスタンスが生成されていることが理由であるため害はない。
    private bool? _initialized = false;

    partial void TraceLifetime(string message);
#if false//true
    partial void TraceLifetime(string message) => System.Diagnostics.Debug.WriteLine($"[{RuntimeHelpers.GetHashCode(this):X16}] {message}");
#endif

    protected override void OnAttached()
    {
        base.OnAttached();

        TraceLifetime("OnAttach");
        var lv = AssociatedObject;
        if (!lv.IsLoaded)
        {
            lv.Loaded += ListViewBase_Loaded;
            lv.Unloaded += ListViewBase_Unloaded;
        }
        else
        {
            Initialize(lv);
        }
    }

    private void ListViewBase_Loaded(object sender, RoutedEventArgs e)
    {
        if (_initialized == false)
        {
            TraceLifetime("Loaded");
            var lv = AssociatedObject ?? (ListViewBase)sender;
            Initialize(lv);
        }
        else if (_initialized == true)
        {
            TraceLifetime("Loaded (fake)");
            _initialized = null; // switch into 'undeterministic' state
        }
    }

    protected override void OnDetaching()
    {
        base.OnDetaching();

        if (_initialized is null)
        {
            TraceLifetime("OnDetaching (fake)");
            return;
        }
        else if (_initialized == true)
        {
            TraceLifetime("OnDetach");
            var lv = AssociatedObject;
            Uninitialize(lv);
        }
    }

    private void ListViewBase_Unloaded(object sender, RoutedEventArgs e)
    {
        if (_initialized is null)
        {
            TraceLifetime("Unloaded (fake)");
            _initialized = true; // back to 'initialized' state.
        }
        else if (_initialized == true)
        {
            TraceLifetime("Unloaded");
            var lv = AssociatedObject ?? (ListViewBase)sender;
            lv.Loaded -= ListViewBase_Loaded;
            lv.Unloaded -= ListViewBase_Unloaded;

            Uninitialize(lv);
        }
    }

    #endregion

    private void Initialize(ListViewBase lv)
    {
        TraceLifetime(" --> Initialized");

        lv.AddHandler(UIElement.PreviewKeyDownEvent,
            new KeyEventHandler(ListViewBase_PreviewKeyDown), handledEventsToo: false);

        lv.CharacterReceived += ListViewBase_CharacterReceived;
        lv.SelectionChanged += ListViewBase_SelectionChanged;

        _initialized = true;
    }

    private void Uninitialize(ListViewBase lv)
    {
        TraceLifetime(" --> Uninitialized");

        lv.RemoveHandler(UIElement.PreviewKeyDownEvent,
            new KeyEventHandler(ListViewBase_PreviewKeyDown));

        lv.CharacterReceived -= ListViewBase_CharacterReceived;
        lv.SelectionChanged -= ListViewBase_SelectionChanged;

        _initialized = false;
    }

    private async void ListViewBase_PreviewKeyDown(object sender, KeyRoutedEventArgs e)
    {
        static bool IsKeyDown(VirtualKey key)
        {
            // 現在のスレッドにおいて、イベント処理中の入力メッセージに対応するキー状態を取得する。
            var keyState = InputKeyboardSource.GetKeyStateForCurrentThread(key);
            var keyDown = (keyState & Windows.UI.Core.CoreVirtualKeyStates.Down) != 0;
            return keyDown;
        }

        //Debug.WriteLine("PreviewKeyDown");
        if (e.Key == VirtualKey.Space)
        {
            var ctrlDown = IsKeyDown(VirtualKey.Control);
            if (ctrlDown)
                return;

            // 選択項目のチェック状態を切り替える。
            var lv = AssociatedObject ?? (ListViewBase)sender;

            // キーボードフォーカスがSelectorItem配下にある場合のみを処理する。
            var focused =
                (FocusManager.GetFocusedElement(lv.XamlRoot) as DependencyObject)
                ?.FindAscendantOrSelf<FrameworkElement>(e => e is SelectorItem || ReferenceEquals(e, lv));
            if (focused is not SelectorItem)
                return;

            var item = lv.SelectedItem as ICheckableItem;
            if (item is null)
                return;

            // 現在フォーカスがある項目のチェック状態を基準にする。
            var currentChecked = item.IsChecked;
            foreach (ICheckableItem itemVM in lv.SelectedItems)
            {
                itemVM.IsChecked = !currentChecked;
            }

            e.Handled = true;
            return;
        }
        if (e.Key == VirtualKey.Delete)
        {
            var deleteItems = DeleteItems;
            if (deleteItems is null)
                return;

            // 選択項目の削除を実施する。
            var lv = AssociatedObject ?? (ListViewBase)sender;

            var selectedItems = lv.SelectedItems;
            if (selectedItems.Count == 0)
                return;

            // キーボードフォーカスがSelectorItem配下にある場合のみを処理する。
            var focused =
                (FocusManager.GetFocusedElement(lv.XamlRoot) as DependencyObject)
                ?.FindAscendantOrSelf<FrameworkElement>(e => e is SelectorItem || ReferenceEquals(e, lv));
            if (focused is not SelectorItem)
                return;

            deleteItems?.Invoke(this, new RoutedEventArgs());

            e.Handled = true;
            return;
        }
    }

    private void StartResetTimeout()
    {
        _resetNuclideInputTimer.Stop();
        _resetNuclideInputTimer.Start();
    }

    private void StopResetTimeout()
    {
        _resetNuclideInputTimer.Stop();
        _searchPattern = string.Empty;
        //Debug.WriteLine($"--> reset");
    }

    public static async Task<SelectorItem?> ScrollIntoViewAndWaitForItemContainerAsync(ListViewBase lv, object item)
    {
        // 現時点で取れるなら即座に返す。
        if (lv.ContainerFromItem(item) is SelectorItem already)
            return already;

        var tcs = new TaskCompletionSource<SelectorItem?>(
            TaskCreationOptions.RunContinuationsAsynchronously);

        // ContainerContentChangingはItemsPanelがItemsStackPanel/ItemsWrapGridのときのみ発火する。
        // See also: https://learn.microsoft.com/ja-jp/windows/windows-app-sdk/api/winrt/microsoft.ui.xaml.controls.listviewbase.containercontentchanging#remarks
        void Handler(ListViewBase sender, ContainerContentChangingEventArgs args)
        {
            // 対象itemのコンテナが実体化したタイミングを捕まえる。
            // ただし、当該コンテナが再利用キューに入った≒表示領域外へ出た場合は除外する。
            if (ReferenceEquals(args.Item, item) &&
                !args.InRecycleQueue)
            {
                tcs.TrySetResult(args.ItemContainer);
            }
        }

        lv.ContainerContentChanging += Handler;
        try
        {
            // ItemsSource 変更直後などは UpdateLayout が必要な場合がある。
            // See also: https://learn.microsoft.com/ja-jp/windows/windows-app-sdk/api/winrt/microsoft.ui.xaml.controls.listviewbase.scrollintoview
            lv.UpdateLayout();

            // スクロールを要求。
            lv.ScrollIntoView(item);

            // イベントが先に走る/走らない差を吸収するため、もう一度だけ即時確認する。
            if (lv.ContainerFromItem(item) is SelectorItem afterScroll)
                return afterScroll;

            CancellationToken cancellationToken = default;
            using (cancellationToken.Register(() => tcs.TrySetCanceled(cancellationToken)))
            {
                return await tcs.Task;
            }
        }
        finally
        {
            lv.ContainerContentChanging -= Handler;
        }
    }

    private async void ListViewBase_CharacterReceived(UIElement sender, CharacterReceivedRoutedEventArgs args)
    {
        if (_searchPattern is null)
        {
            args.Handled = true;
            StartResetTimeout();
            return;
        }

        // 核種名の先頭から数文字をキー入力した際に対象を選択する。
        var startChar = char.ToUpperInvariant(_searchPattern.Length == 0 ? args.Character : _searchPattern[0]);
        if ('A' <= startChar && startChar <= 'Z')
        {
            args.Handled = true;

            _resetNuclideInputTimer.Stop();
            //Debug.WriteLine($"pattern = '{_searchPattern}{args.Character}'  <-- '{_searchPattern}' + '{args.Character}'");
            _searchPattern += args.Character;

            var lv = AssociatedObject ?? (ListViewBase)sender;
            var count = lv.Items.Count;
            var current = lv.SelectedIndex;
            if (current < 0)
                current = 0;
            for (int i = 0; i < count; i++)
            {
                // 現在の選択項目から検索を開始する。
                var index = (current + i) % count;

                var item = lv.Items[index] as ICheckableItem;
                if (item is null)
                    continue;
                if (!item.ItemText.StartsWith(_searchPattern, StringComparison.OrdinalIgnoreCase))
                    continue;

                _avoidResetTimeout = true;

                lv.SelectedItem = item;
                var itemContainer = await ScrollIntoViewAndWaitForItemContainerAsync(lv, item);
                itemContainer?.Focus(FocusState.Keyboard);

                StartResetTimeout();
                return;
            }

            // これ以上のキー入力は決してマッチしない状態であることを記憶する。
            _searchPattern = null;
            StartResetTimeout();
            return;
        }
    }

    private void ListViewBase_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        //Debug.WriteLine("SelectionChanged");
        //foreach (ICheckableViewModel itemVM in AssociatedObject.SelectedItems)
        //    Debug.WriteLine($"  {itemVM.ItemText}");

        if (!_avoidResetTimeout)
        {
            // 矢印キーなどによる項目移動は、核種名入力の状態をリセットする。
            StopResetTimeout();
        }
        _avoidResetTimeout = false;
    }
}
