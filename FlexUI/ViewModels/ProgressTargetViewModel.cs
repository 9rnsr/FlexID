using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.DependencyInjection;
using CommunityToolkit.Mvvm.Input;
using FlexID.Models;
using FlexID.Views;
using Microsoft.UI.Xaml;

namespace FlexID.ViewModels;

public partial class ProgressTargetViewModel : ViewModelBase
{
    public ProgressTargetViewModel(InputTarget target)
    {
        InputTarget = target;
    }

    public InputTarget InputTarget { get; }

    public string Name => InputTarget.Name;

    public string Title => InputTarget.Title;

    public string Nuclide => InputTarget.Nuclide;

    public string InputFilePath => InputTarget.FilePath;

    public string OutputFilePath { get; set; } = "";

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

    private ViewerWindow? ViewerWindow { get; set; }

    [RelayCommand(CanExecute = nameof(IsSuccess))]
    private void OpenViewer()
    {
        if (!File.Exists(OutputFilePath))
            return;

        if (ViewerWindow is null)
        {
            ViewerWindow = Ioc.Default.GetRequiredService<ViewerWindow>();
            var vm = ViewerWindow.ViewModel;
            vm.OutputFilePath = OutputFilePath;

            ViewerWindow.Closed += (sender, args) => ViewerWindow = null; ;
        }
        ViewerWindow.AppWindow.MoveInZOrderAtTop();
        ViewerWindow.AppWindow.Show(true);
    }
}
