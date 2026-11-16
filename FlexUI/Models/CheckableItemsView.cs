using System.Collections.Specialized;
using System.ComponentModel;
using CommunityToolkit.Mvvm.ComponentModel;
using ObservableCollections;
using R3;

namespace FlexID.Models;

public interface ICheckableItem : INotifyPropertyChanged
{
    public bool IsChecked { get; set; }

    public string ItemText { get; }
}

public partial class CheckableItemsView<T, TView> : ObservableObject, IDisposable
    where TView : ICheckableItem
{
    public CheckableItemsView(Func<T, TView> createView)
    {
        View = Items.CreateView(item =>
        {
            var itemView = createView(item);

            itemView.ObservePropertyChanged(v => v.IsChecked, pushCurrentValueOnSubscribe: false)
                    .Subscribe(value => OnIsCheckedChanged(value))
                    .AddTo(disposables);

            return itemView;
        }).AddTo(disposables);

        FilteredItems = View
            .ToNotifyCollectionChanged(SynchronizationContextCollectionEventDispatcher.Current)
            .AddTo(disposables);

        View.ViewChanged += OnViewChanged;

        _countChecked = 0;
        _countUnchecked = FilteredItems.Count;
        _isCheckedAll = false;
    }

    protected readonly CompositeDisposable disposables = [];

    public void Dispose() => disposables.Dispose();

    private ObservableList<T> Items { get; } = [];

    private ISynchronizedView<T, TView> View { get; }

    public NotifyCollectionChangedSynchronizedViewList<TView> FilteredItems { get; }

    public void AttachFilter(ISynchronizedViewFilter<T, TView>? filter)
    {
        if (filter is null)
            View.ResetFilter();
        else
            View.AttachFilter(filter);
    }

    private int _countChecked;
    private int _countUnchecked;
    private bool? _isCheckedAll;
    private bool _isCheckedAllChanging;

    [ObservableProperty]
    public partial bool IsCheckedAny { get; private set; }

    public bool? IsCheckedAll
    {
        get => _isCheckedAll;
        set
        {
            // 要素数がゼロの場合は常にfalseとなるようにする。
            if (FilteredItems.Count == 0)
            {
                _isCheckedAll = false;
                OnPropertyChanged();
                return;
            }

            if (_isCheckedAll != value)
            {
                _isCheckedAll = value;
                OnIsCheckedAllChanged(value);
                OnPropertyChanged();
            }
        }
    }

    /// <summary>
    /// 項目を一覧の末尾に追加する。
    /// </summary>
    /// <param name="item"></param>
    public void Add(T item)
    {
        App.Current?.UIQueue?.TryEnqueue(() =>
        {
            Items.Add(item);
        });
    }

    /// <summary>
    /// 項目をチェック済み状態で一覧の末尾に追加する。
    /// </summary>
    /// <param name="item"></param>
    public void AddChecked(T item)
    {
        App.Current?.UIQueue?.TryEnqueue(() =>
        {
            Items.Add(item);

            // 初期状態でチェック済みにする。
            var view = View.Filtered.Concat(View.Unfiltered)
                            .Where(pair => EqualityComparer<T>.Default.Equals(pair.Value, item))
                            .Select(pair => pair.View)
                            .FirstOrDefault();
            view?.IsChecked = true;
        });
    }

    /// <summary>
    /// ソート済みの項目一覧に対して、これを維持する位置に項目を追加する。
    /// </summary>
    /// <param name="item"></param>
    /// <param name="comparer"></param>
    public void AddSorted(T item, Comparer<T>? comparer = null)
    {
        App.Current?.UIQueue?.TryEnqueue(() =>
        {
            var index = FindInsertIndex(Items, item, comparer);
            Items.Insert(index, item);
        });

        static int FindInsertIndex<T>(ObservableList<T> list, T item, IComparer<T>? comparer = null)
        {
            comparer ??= Comparer<T>.Default;

            int low = 0;
            int high = list.Count;

            while (low < high)
            {
                int mid = low + ((high - low) >> 1);

                if (comparer.Compare(list[mid], item) < 0)
                    low = mid + 1;
                else
                    high = mid;
            }

            return low;
        }
    }

    public void Remove(T item)
    {
        Items.Remove(item);
    }

    public void RemoveRange(IEnumerable<T> items)
    {
        foreach (var item in items)
        {
            Items.Remove(item);
        }
    }

    private void OnIsCheckedAllChanged(bool? value)
    {
        if (value is not bool newValue)
            return;

        if (newValue)
        {
            _countChecked = FilteredItems.Count;
            _countUnchecked = 0;
        }
        else
        {
            _countChecked = 0;
            _countUnchecked = FilteredItems.Count;
        }

        _isCheckedAllChanging = true;
        foreach (var item in FilteredItems)
        {
            item.IsChecked = newValue;
        }
        _isCheckedAllChanging = false;

        IsCheckedAny = _countChecked != 0;
    }

    private void OnViewChanged(in SynchronizedViewChangedEventArgs<T, TView> e)
    {
        switch (e.Action)
        {
            case NotifyCollectionChangedAction.Reset:
                _countChecked = 0;
                _countUnchecked = 0;
                foreach (var (_, view) in View.Filtered)
                {
                    if (view.IsChecked)
                        _countChecked++;
                    else
                        _countUnchecked++;
                }
                break;

            case NotifyCollectionChangedAction.Add:
                foreach (var view in e.IsSingleItem ? [e.NewItem.View] : e.NewViews)
                {
                    if (view.IsChecked)
                        _countChecked++;
                    else
                        _countUnchecked++;
                }
                break;

            case NotifyCollectionChangedAction.Remove:
                foreach (var view in e.IsSingleItem ? [e.OldItem.View] : e.OldViews)
                {
                    if (view.IsChecked)
                        _countChecked--;
                    else
                        _countUnchecked--;
                }
                break;

            default:
                return;
        }

        var old = _isCheckedAll;
        _isCheckedAll =
            _countChecked == 0 ? false :
            _countUnchecked == 0 ? true : null;
        if (_isCheckedAll != old)
            OnPropertyChanged(nameof(IsCheckedAll));

        IsCheckedAny = _countChecked != 0;
    }

    private void OnIsCheckedChanged(bool value)
    {
        if (_isCheckedAllChanging)
            return;

        if (value)
        {
            _countChecked++;
            _countUnchecked--;
        }
        else
        {
            _countChecked--;
            _countUnchecked++;
        }

        var old = _isCheckedAll;
        _isCheckedAll =
            _countChecked == 0 ? false :
            _countUnchecked == 0 ? true : null;
        if (_isCheckedAll != old)
            OnPropertyChanged(nameof(IsCheckedAll));

        IsCheckedAny = _countChecked != 0;
    }
}
