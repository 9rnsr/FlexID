using CommunityToolkit.Mvvm.ComponentModel;
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
