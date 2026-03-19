using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using FlexID.Models;

namespace FlexID.ViewModels;

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

    private ParallelRunner<ProgressTargetViewModel>? runner;

    private readonly Dictionary<InputTarget, Exception> errors = [];

    public ObservableCollection<ProgressTargetViewModel> Targets { get; } = [];

    public Action? ShowAction { get; set; }
    public Action? CloseAction { get; set; }

    private void OnStartItem(ProgressTargetViewModel targetVM)
    {
        App.Current.UIQueue.TryEnqueue(() =>
        {
            targetVM.IsRunning = true;
        });
    }

    private void OnSuccessItem(ProgressTargetViewModel targetVM)
    {
        App.Current.UIQueue.TryEnqueue(() =>
        {
            targetVM.IsRunning = false;
            targetVM.ErrorText = "";
        });
    }

    private void OnFailureItem(ProgressTargetViewModel targetVM, Exception exception)
    {
        App.Current.UIQueue.TryEnqueue(() =>
        {
            targetVM.IsRunning = false;
            targetVM.ErrorText = exception.Message;
            errors[targetVM.InputTarget] = exception;
        });
    }

    public void Connect(ParallelRunner<ProgressTargetViewModel> parallelRunner)
    {
        runner = parallelRunner;

        errors.Clear();
        Targets.AddRange(runner.Items);

        runner.StartItem += OnStartItem;
        runner.SuccessItem += OnSuccessItem;
        runner.FailureItem += OnFailureItem;

        ShowAction?.Invoke();
    }

    public void Disconnect()
    {
        CloseAction?.Invoke();

        errors.Clear();
        Targets.Clear();

        runner?.StartItem -= OnStartItem;
        runner?.SuccessItem -= OnSuccessItem;
        runner?.FailureItem -= OnFailureItem;

        runner = null;
    }
}
