using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using FlexID.Models;
using Microsoft.UI.Xaml;

namespace FlexID.ViewModels;

public partial class ProgressTargetViewModel : ViewModelBase
{
    public ProgressTargetViewModel(InputTarget target)
    {
        InputTarget = target;
    }

    public InputTarget InputTarget { get; }

    public string Title => InputTarget.Title;

    public string Nuclide => InputTarget.Nuclide;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsSuspend))]
    [NotifyPropertyChangedFor(nameof(IsSuccess))]
    [NotifyPropertyChangedFor(nameof(IsFailure))]
    public partial bool IsRunning { get; set; }

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsSuspend))]
    [NotifyPropertyChangedFor(nameof(IsSuccess))]
    [NotifyPropertyChangedFor(nameof(IsFailure))]
    public partial string? ErrorText { get; set; }

    [ObservableProperty]
    public partial bool IsSelected { get; set; }

    public bool IsSuspend => !IsRunning && ErrorText is null;

    public bool IsSuccess => !IsRunning && ErrorText?.Length == 0;

    public bool IsFailure => !IsRunning && ErrorText?.Length > 0;

    public Visibility BoolToVisibility(bool v) => v ? Visibility.Visible : Visibility.Collapsed;
}

public partial class ProgressViewModel : ObservableObject
{
    /// <summary>
    /// コンストラクタ。
    /// </summary>
    public ProgressViewModel()
    {
        WeakReferenceMessenger.Default.Register<BusyState>(this, (r, m) => IsBusy = m.Value);
    }

    [ObservableProperty]
    public partial bool IsBusy { get; private set; }

    private ParallelRunner<InputTarget>? runner;

    private readonly Dictionary<InputTarget, Exception> errors = [];
    private readonly Dictionary<InputTarget, ProgressTargetViewModel> mapVM = [];

    public ObservableCollection<ProgressTargetViewModel> Targets { get; } = [];

    public Action? ShowAction { get; set; }
    public Action? CloseAction { get; set; }

    private void OnStartItem(InputTarget target)
    {
        var targetVM = mapVM[target];
        App.Current.UIQueue.TryEnqueue(() =>
        {
            targetVM.IsRunning = true;
        });
    }

    private void OnSuccessItem(InputTarget target)
    {
        var targetVM = mapVM[target];
        App.Current.UIQueue.TryEnqueue(() =>
        {
            targetVM.IsRunning = false;
            targetVM.ErrorText = "";
        });
    }

    private void OnFailureItem(InputTarget target, Exception exception)
    {
        var targetVM = mapVM[target];
        App.Current.UIQueue.TryEnqueue(() =>
        {
            targetVM.IsRunning = false;
            targetVM.ErrorText = exception.Message;
            errors[target] = exception;
        });
    }

    public void Connect(ParallelRunner<InputTarget> parallelRunner)
    {
        runner = parallelRunner;

        mapVM.Clear();
        errors.Clear();

        Targets.AddRange(runner.Items.Select(target =>
        {
            var progressVM = new ProgressTargetViewModel(target);
            mapVM.Add(target, progressVM);
            return progressVM;
        }));

        runner.StartItem += OnStartItem;
        runner.SuccessItem += OnSuccessItem;
        runner.FailureItem += OnFailureItem;

        ShowAction?.Invoke();
    }

    public void Disconnect()
    {
        CloseAction?.Invoke();

        mapVM.Clear();
        errors.Clear();

        Targets.Clear();

        runner?.StartItem -= OnStartItem;
        runner?.SuccessItem -= OnSuccessItem;
        runner?.FailureItem -= OnFailureItem;
    }
}
