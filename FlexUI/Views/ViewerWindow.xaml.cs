using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using FlexID.ViewModels;
using Microsoft.UI.Input;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Input;
using R3;
using Windows.System;

namespace FlexID.Views;

public partial class ViewerWindow
{
    public ViewerWindow(ViewerViewModel vm)
    {
        ViewModel = vm;

        InitializeComponent();

        IsContentEnabled = ViewModel
            .ObservePropertyChanged(vm => vm.SelectedOutput)
            .Select(o => o is not null)
            .ToReadOnlyBindableReactiveProperty();

        PlotControl.Reset(ViewModel.Graph.PlotModel);
        ViewModel.Graph.RefreshCommand = new RelayCommand(PlotControl.Refresh);
    }

    public ViewerViewModel ViewModel { get; }

    public IReadOnlyBindableReactiveProperty<bool> IsContentEnabled { get; }

    private void WindowContent_Loaded(object sender, RoutedEventArgs e)
    {
        // タブオーダーに従って初期フォーカスを設定する。
        var first = FocusManager.FindFirstFocusableElement(Content) as UIElement;
        first?.Focus(FocusState.Programmatic);
    }

    private void Page_PreviewKeyDown(object sender, KeyRoutedEventArgs e)
    {
        void CycleNavigation(bool reverse)
        {
            var count = PivotSegmented.Items.Count;
            var index = PivotSegmented.SelectedIndex;
            if (reverse)
                index = (index + count - 1) % count;
            else
                index = (index + 1) % count;
            PivotSegmented.SelectedIndex = index;
        }

        var ctrlDown = InputKeyboardSource.IsKeyDown(VirtualKey.Control);
        if (ctrlDown)
        {
            // Ctrl + (Shift +) Tab
            if (e.Key == VirtualKey.Tab)
            {
                e.Handled = true;
                var shiftDown = InputKeyboardSource.IsKeyDown(VirtualKey.Shift);
                CycleNavigation(reverse: shiftDown);
                return;
            }

            // Ctrl + PageDown
            if (e.Key == VirtualKey.PageDown)
            {
                e.Handled = true;
                CycleNavigation(reverse: false);
                return;
            }
            // Ctrl + PageUp
            if (e.Key == VirtualKey.PageUp)
            {
                e.Handled = true;
                CycleNavigation(reverse: true);
                return;
            }
        }
    }

    private void OnOpen(KeyboardAccelerator sender, KeyboardAcceleratorInvokedEventArgs args)
    {
        // KeyboardAcceleratorはHandledを設定してイベントを消費しないと適切に動作しないため
        // InvokeCommandActionなどが使えない…
        args.Handled = true;

        ViewModel.SelectOutputFilePathCommand.Execute(null);
    }

    private void OutputFilePathTextBox_KeyDown(object sender, KeyRoutedEventArgs e)
    {
        if (e.Key == VirtualKey.Enter /*&& Keyboard.Modifiers == ModifierKeys.None*/)
        {
            var binding = ((TextBox)sender).GetBindingExpression(TextBox.TextProperty);
            binding?.UpdateSource();
        }
    }

    #region Play/Pause Buttons and TimeStepSlider

    public string GetPlayPauseIcon(bool canPlay) => canPlay ? "\uF5B0" : "\uF8AE";

    public string GetPlayPauseTooltip(bool canPlay) => canPlay ? "Play" : "Pause";

    public ICommand GetPlayPauseCommand(bool canPlay) =>
        canPlay ? ViewModel.Contour.PlayCommand : ViewModel.Contour.PauseCommand;

    private bool _isInternalTimeStepSliderChange;

    private void TimeStepSlider_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
    {
        if (_isInternalTimeStepSliderChange)
            return;

        var slider = (Slider)sender;
        var timeSteps = ViewModel.Contour.TimeSteps;

        // timeStepsは長さ0の場合もある点に注意。
        var nearest = timeSteps
            .OrderBy(p => Math.Abs(p - e.NewValue))
            .FirstOrDefault();

        if (nearest == e.NewValue)
            return;

        _isInternalTimeStepSliderChange = true;
        slider.Value = nearest;
        _isInternalTimeStepSliderChange = false;
    }

    #endregion
}
