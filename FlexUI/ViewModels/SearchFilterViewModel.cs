using System.Collections.Specialized;
using System.Diagnostics;
using System.Text.RegularExpressions;
using CommunityToolkit.Mvvm.ComponentModel;
using FlexID.Models;
using ObservableCollections;
using R3;
using Windows.Foundation;

namespace FlexID.ViewModels;

public partial class SearchFilterViewModel : ViewModelBase, ISynchronizedViewFilter<InputTarget, InputTargetViewModel>
{
    public SearchFilterViewModel()
    {
        TargetElements.ObserveChanged()
        .Subscribe(TargetElementsChanged).AddTo(disposables);

        //Observable.Merge(
        //    this.ObservePropertyChanged(vm => TargetIsInjecion),
        //    this.ObservePropertyChanged(vm => TargetIsIngestion),
        //    this.ObservePropertyChanged(vm => TargetIsInhalation),
        //    this.ObservePropertyChanged(vm => TargetIsTypeF),
        //    this.ObservePropertyChanged(vm => TargetIsTypeM),
        //    this.ObservePropertyChanged(vm => TargetIsTypeS))
        //.Subscribe(IntakeFormsChanged).AddTo(disposables);

        this.ObservePropertyChanged(vm => vm.TargetTitlePattern)
        .Debounce(TimeSpan.FromMilliseconds(300))
        .ObserveOnCurrentSynchronizationContext()
        .Subscribe(TargetTitlePatternChanged).AddTo(disposables);
    }

    private Regex? _elementsPattern;

    private Regex? _intakeFormsPattern;

    private Regex? _titlePattern;

    public ObservableList<string> TargetElements { get; } = [];

    [ObservableProperty]
    public partial bool TargetIsInjecion { get; set; }

    [ObservableProperty]
    public partial bool TargetIsIngestion { get; set; }

    [ObservableProperty]
    public partial bool TargetIsInhalation { get; set; }

    [ObservableProperty]
    public partial bool TargetIsTypeF { get; set; }

    [ObservableProperty]
    public partial bool TargetIsTypeM { get; set; }

    [ObservableProperty]
    public partial bool TargetIsTypeS { get; set; }

    [ObservableProperty]
    public partial string TargetTitlePattern { get; set; } = "";

    public event TypedEventHandler<SearchFilterViewModel, EventArgs>? Updated;

    private void TargetElementsChanged(CollectionChangedEvent<string> e)
    {
        Debug.WriteLine($"TargetElementsChanged, e.Action = {e.Action}");
        switch (e.Action)
        {
            case NotifyCollectionChangedAction.Add:
            case NotifyCollectionChangedAction.Remove:
            case NotifyCollectionChangedAction.Replace:
                _elementsPattern = new Regex($@"\b({string.Join("|", TargetElements)})\b");
                break;

            case NotifyCollectionChangedAction.Reset:
                _elementsPattern = null;
                break;
        }

        Updated?.Invoke(this, EventArgs.Empty);
    }

    partial void OnTargetIsInjecionChanged(bool value) => IntakeFormsChanged();
    partial void OnTargetIsIngestionChanged(bool value) => IntakeFormsChanged();
    partial void OnTargetIsInhalationChanged(bool value) => IntakeFormsChanged();
    partial void OnTargetIsTypeFChanged(bool value) => IntakeFormsChanged();
    partial void OnTargetIsTypeMChanged(bool value) => IntakeFormsChanged();
    partial void OnTargetIsTypeSChanged(bool value) => IntakeFormsChanged();

    private void IntakeFormsChanged()
    {
        //Debug.WriteLine($"IntakeFormsChanged");
        int i = 0;
        var intakeForms = new string[6];
        if (TargetIsInjecion   /**/) intakeForms[i++] = "Injection";
        if (TargetIsIngestion  /**/) intakeForms[i++] = "Ingestion";
        if (TargetIsInhalation /**/) intakeForms[i++] = "Inhalation";
        if (TargetIsTypeF      /**/) intakeForms[i++] = "Type[- ]?F";
        if (TargetIsTypeM      /**/) intakeForms[i++] = "Type[- ]?M";
        if (TargetIsTypeS      /**/) intakeForms[i++] = "Type[- ]?S";
        if (i != 0)
        {
            _intakeFormsPattern = new Regex($@"\b({string.Join("|", intakeForms.AsSpan()[0..i])})\b");
        }
        else
        {
            _intakeFormsPattern = null;
        }

        Updated?.Invoke(this, EventArgs.Empty);
    }

    private void TargetTitlePatternChanged(string value)
    {
        Regex? regex = null;
        if (value.Length != 0)
        {
            try { regex = new Regex(value); }
            catch { }
        }
        if (!ReferenceEquals(_titlePattern, regex))
        {
            _titlePattern = regex;
            Updated?.Invoke(this, EventArgs.Empty);
        }
    }

    public bool IsMatch(InputTarget target, InputTargetViewModel _)
    {
        return
            (_elementsPattern?.IsMatch(target.Nuclide) ?? true) &&
            (_intakeFormsPattern?.IsMatch(target.Title) ?? true) &&
            (_titlePattern?.IsMatch(target.Title) ?? true);
    }
}
