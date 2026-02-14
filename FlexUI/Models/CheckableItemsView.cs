using System.Collections.Specialized;
using System.ComponentModel;
using System.Text.RegularExpressions;
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

        SearchFilter
        .Debounce(TimeSpan.FromMilliseconds(300))
        .ObserveOnCurrentSynchronizationContext()
        .Subscribe(pattern =>
        {
            Regex? regex = null;
            try
            {
                regex = new Regex(pattern);
            }
            catch { }
            if (regex is null)
                View.ResetFilter();
            else
                View.AttachFilter(new RegexSearchFilter(regex));
        }).AddTo(disposables);

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

    public BindableReactiveProperty<string> SearchFilter { get; } = new("");

    private class RegexSearchFilter : ISynchronizedViewFilter<T, TView>
    {
        readonly Regex regex;

        public RegexSearchFilter(Regex regex)
        {
            this.regex = regex;
        }

        public bool IsMatch(T item, TView view)
        {
            return regex.IsMatch(view.ItemText);
        }
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

    public void Add(T item, bool initialCheck = false)
    {
        App.Current?.UIQueue?.TryEnqueue(() =>
        {
            Items.Add(item);
            var view = View.Filtered.Concat(View.Unfiltered)
                           .Where(pair => EqualityComparer<T>.Default.Equals(pair.Value, item))
                           .Select(pair => pair.View)
                           .FirstOrDefault();
            view?.IsChecked = initialCheck;
        });
    }

    public async Task AddRangeAsync(IAsyncEnumerable<T> items)
    {
        await foreach (var item in items)
        {
            Add(item);
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
