using Prism.Services.Dialogs;
using System.Windows;

namespace FlexID.Services
{
    public static class MessageDialogService
    {
        public static ButtonResult Error(this IDialogService dialogService, string message, MessageBoxResult defaultResult = MessageBoxResult.None) =>
            ShowMessage(dialogService, "エラー", message, MessageBoxButton.OK, defaultResult);

        public static ButtonResult Warning(this IDialogService dialogService, string message, MessageBoxResult defaultResult = MessageBoxResult.None) =>
            ShowMessage(dialogService, "警告", message, MessageBoxButton.OK, defaultResult);

        public static ButtonResult Confirm(this IDialogService dialogService, string message, MessageBoxResult defaultResult = MessageBoxResult.None) =>
            ShowMessage(dialogService, "確認", message, MessageBoxButton.OK, defaultResult);

        public static ButtonResult ShowMessage(this IDialogService dialogService,
            string tilte, string message, MessageBoxButton buttons, MessageBoxResult defaultResult = MessageBoxResult.None)
        {
            var parameters = new DialogParameters
            {
                { "Title", tilte },
                { "Message", message },
                { "Buttons", buttons },
                { "DefaultResult", defaultResult },
            };

            ButtonResult result = default;

            // メインウインドウ表示前にメッセージ表示した場合に、
            // 勝手にShutdownが走らないようShutdownModeを切り替える。
            var app = Application.Current;
            var saveShutdownMode = app.ShutdownMode;
            app.ShutdownMode = ShutdownMode.OnExplicitShutdown;
            try
            {
                dialogService.ShowDialog(nameof(Views.MessageDialogView), parameters, r =>
                {
                    result = r.Result;
                });
            }
            finally
            {
                app.ShutdownMode = saveShutdownMode;
            }

            return result;
        }
    }
}
