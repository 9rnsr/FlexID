using FlexID.Services;
using FlexID.ViewModels;
using Microsoft.UI.Input;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using Windows.System;

namespace FlexID.Views;

public sealed partial class MainWindow
{
    public MainWindow(MainViewModel vm, MessageService messageService)
    {
        ViewModel = vm;

        InitializeComponent();

        messageService.Register(NavigationView);

        // NavigationViewItem.IsSelected を設定する方法は問題があるため、
        // 代替として Activated を契機としてコードビハインドから選択を行う。
        Activated += MainWindow_Activated;
    }

    public MainViewModel ViewModel { get; }

    // WinUI では {x:Type} が使えないため、コードビハインドから Type オブジェクトを提供する。
    public Type InputOirPageType => typeof(InputOirPage);
    public Type InputEirPageType => typeof(InputEirPage);
    public Type InputScoeffPageType => typeof(InputScoeffPage);

    private void MainWindow_Activated(object sender, WindowActivatedEventArgs args)
    {
        if (NavigationFrame.Content is null)
        {
            // 左端の項目を選択する。
            CycleNavigation(reverse: false);
        }
    }

    private void NavigationView_PreviewKeyDown(object sender, KeyRoutedEventArgs e)
    {
        static bool IsKeyDown(VirtualKey key)
        {
            // 現在のスレッドにおいて、イベント処理中の入力メッセージに対応するキー状態を取得する。
            var keyState = InputKeyboardSource.GetKeyStateForCurrentThread(key);
            var keyDown = (keyState & Windows.UI.Core.CoreVirtualKeyStates.Down) != 0;
            return keyDown;
        }

        var ctrlDown = IsKeyDown(VirtualKey.Control);

        if (ctrlDown && e.Key == VirtualKey.Tab)
        {
            // Ctrl+(Shift+)Tab項目の切り替え。
            var shiftDown = IsKeyDown(VirtualKey.Shift);
            //Debug.WriteLine($"NavigationView_PreviewKeyDown, Ctrl+Tab, shiftDown = {shiftDown}");
            CycleNavigation(reverse: shiftDown);
            e.Handled = true;
            return;
        }
        if (ctrlDown && e.Key == VirtualKey.PageDown)
        {
            // Ctrl+PageDownによる次の項目への切り替え。
            CycleNavigation(reverse: false);
            e.Handled = true;
            return;
        }
        if (ctrlDown && e.Key == VirtualKey.PageUp)
        {
            // Ctrl+PageUpによる前の項目への切り替え。
            CycleNavigation(reverse: true);
            e.Handled = true;
            return;
        }
    }

    private void NavigationView_SelectionChanged(NavigationView sender, NavigationViewSelectionChangedEventArgs args)
    {
        //Debug.WriteLine($"NavigationView_SelectionChanged, {args.SelectedItemContainer.Content}");

        Type pageType;
        if (args.IsSettingsSelected)
        {
            pageType = typeof(AboutPage);
        }
        else
        {
            // 選択項目が変化した場合に、対応するPageへのナビゲーションを行う。
            // NavigationViewItem.Tag に typeof(対応するPage) が設定されていることを前提としている。
            pageType = (Type)args.SelectedItemContainer.Tag;
        }

        NavigationFrame.NavigateToType(pageType, null, new()
        {
            TransitionInfoOverride = args.RecommendedNavigationTransitionInfo,
            IsNavigationStackEnabled = false,
        });
    }

    private bool _moveFocusIntoFrame = false;

    private void CycleNavigation(bool reverse)
    {
        var count = NavigationView.MenuItems.Count;
        if (count == 0)
            return;

        // 現在のページ内にフォーカスが存在する場合は、ページ切り替え後に
        // 新しいページ内のコントロールへフォーカスを移す。
        _moveFocusIntoFrame = GetMoveFocusIntoFrame();
        //Debug.WriteLine($"+ moveFocusIntoFrame = {_moveFocusIntoFrame}");

        var selectedItem = NavigationView.SelectedItem;
        var index = NavigationView.MenuItems.IndexOf(selectedItem);
        if (index == -1)
            index = reverse ? count - 1 : 0;
        else
            index = (index + (reverse ? count - 1 : 1)) % count;

        var item = NavigationView.MenuItems[index];

        // まず項目コンテナにフォーカスを当ててから、SelectedItem を更新する。
        var itemContainer = NavigationView.ContainerFromMenuItem(item) as NavigationViewItemBase;
        itemContainer?.Focus(FocusState.Programmatic);

        NavigationView.SelectedItem = item;
    }

    private bool GetMoveFocusIntoFrame()
    {
        if (NavigationView.XamlRoot is null)
            return true;

        var currentFocused = FocusManager.GetFocusedElement(NavigationView.XamlRoot) as DependencyObject;
        if (currentFocused is null)
            return false;

        return IsDescendantOf(currentFocused, NavigationFrame);

        static bool IsDescendantOf(DependencyObject node, DependencyObject ancestor)
        {
            while (node is not null)
            {
                if (ReferenceEquals(node, ancestor))
                    return true;

                node = VisualTreeHelper.GetParent(node);
            }
            return false;
        }
    }

    private void NavigationFrame_Navigated(object sender, NavigationEventArgs args)
    {
        if (args.Content is Page page)
        {
            //Debug.WriteLine($"NavigationFrame_Navigated page = {page.GetType().Name}");
            //Debug.WriteLine($"NavigationView.SelectedItem = {((NavigationViewItem)NavigationView.SelectedItem).Content}");

            // 新しいページ内のコントロールへフォーカスを移す処理は Load の完了後に行う。
            if (_moveFocusIntoFrame)
                page.Loaded += PageLoaded;

            _moveFocusIntoFrame = false;

            void PageLoaded(object sender, RoutedEventArgs e)
            {
                page.Loaded -= PageLoaded;

                var pageRoot = (page.Content as DependencyObject) ?? page;
                var pageFirst = FocusManager.FindFirstFocusableElement(pageRoot) as UIElement;
                pageFirst?.Focus(FocusState.Keyboard);
            }
        }
    }
}
