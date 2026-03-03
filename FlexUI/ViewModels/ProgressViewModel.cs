using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.DependencyInjection;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using FlexID.Models;
using FlexID.Views;
using WinUIEx;

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
    [NotifyPropertyChangedFor(nameof(IsBlocked))]
    [NotifyPropertyChangedFor(nameof(IsSuccess))]
    [NotifyPropertyChangedFor(nameof(IsFailure))]
    [NotifyPropertyChangedFor(nameof(IsFailureDetails))]
    public partial bool IsRunning { get; set; }

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsBlocked))]
    [NotifyPropertyChangedFor(nameof(IsSuccess))]
    [NotifyPropertyChangedFor(nameof(IsFailure))]
    [NotifyPropertyChangedFor(nameof(IsFailureDetails))]
    public partial string? ErrorText { get; set; }

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsFailureDetails))]
    public partial bool IsSelected { get; set; }

    public bool IsBlocked => !IsRunning && ErrorText is null;

    public bool IsSuccess => !IsRunning && ErrorText?.Length == 0;

    public bool IsFailure => !IsRunning && ErrorText?.Length > 0;

    public bool IsFailureDetails => IsFailure && IsSelected;

    public Microsoft.UI.Xaml.Visibility BoolToVisibility(bool v) =>
        v ? Microsoft.UI.Xaml.Visibility.Visible : Microsoft.UI.Xaml.Visibility.Collapsed;
}

public partial class ProgressViewModel : ObservableObject
{
    /// <summary>
    /// コンストラクタ。
    /// </summary>
    public ProgressViewModel()
    {
        WeakReferenceMessenger.Default.Register<BusyState>(this, (r, m) => InProgress = m.Value);
    }

    [ObservableProperty]
    public partial string Title { get; set; } = "Progress";

    public ObservableCollection<ProgressTargetViewModel> Targets { get; } = [];

    [ObservableProperty]
    public partial bool InProgress { get; private set; }

    private ParallelRunner<InputTarget>? runner;
    private CancellationTokenSource? cts;
    private readonly Dictionary<InputTarget, Exception> errors = [];
    private readonly Dictionary<InputTarget, ProgressTargetViewModel> mapVM = [];

    public CancellationToken Connect(ParallelRunner<InputTarget> runner)
    {
        var progressWnd = Ioc.Default.GetRequiredService<ProgressWindow>();
        progressWnd.Activate();

        this.runner = runner;
        this.cts = new();

        this.errors.Clear();
        this.mapVM.Clear();

        Targets.Clear();
        Targets.AddRange(runner.Items.Select(target =>
        {
            var progressVM = new ProgressTargetViewModel(target);
            mapVM.Add(target, progressVM);
            return progressVM;
        }));

        runner.StartItem += OnStartItem;
        runner.SuccessItem += OnSuccessItem;
        runner.FailureItem += OnFailureItem;

        return cts.Token;
    }

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
        });
    }

    [RelayCommand]
    private void AbortOrClose()
    {
        if (InProgress)
        {
            cts?.Cancel();
        }
        else
        {
            var progressWnd = Ioc.Default.GetRequiredService<ProgressWindow>();
            progressWnd.Hide();

            runner?.StartItem -= OnStartItem;
            runner?.SuccessItem -= OnSuccessItem;
            runner?.FailureItem -= OnFailureItem;

            this.runner = null;
            this.cts = null;
        }
    }
}
