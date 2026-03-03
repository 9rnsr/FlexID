using System.Runtime.InteropServices;
using CommunityToolkit.Mvvm.DependencyInjection;
using FlexID.ViewModels;
using WinUIEx;

namespace FlexID.Views;

public sealed partial class ProgressWindow
{
    public ProgressWindow(ProgressViewModel vm)
    {
        ViewModel = vm;

        InitializeComponent();

        var mainWindow = Ioc.Default.GetRequiredService<MainWindow>();

        var hWnd = this.GetWindowHandle();
        var hWndOwner = mainWindow.GetWindowHandle();
        SetWindowLongPtr(hWnd, GWLP_HWNDPARENT, hWndOwner);

        // ウインドウは、通常操作ではHideするだけでCloseはしない。
        AppWindow.Closing += (_, args) =>
        {
            if (!ViewModel.InProgress)
                this.Hide();
            args.Cancel = true;
        };

        // メインウインドウがCloseした場合のみ連動してCloseする。
        mainWindow.Closed += (_, _) => Close();
    }

    public ProgressViewModel ViewModel { get; }

    public string GetButtonText(bool inProgress) => inProgress ? "Abort" : "Close";

    [LibraryImport("user32.dll", EntryPoint = "SetWindowLongPtrW", SetLastError = true)]
    private static partial IntPtr SetWindowLongPtr(IntPtr hWnd, int nIndex, IntPtr dwNewLong);

    const int GWLP_HWNDPARENT = -8;
}
