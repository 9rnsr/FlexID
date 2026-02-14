using CommunityToolkit.Mvvm.Messaging;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace FlexID.Services;

public class MessageService
{
    internal enum MessageDialogType
    {
        Confirm,
        Error,
    }

    internal record class ModalDialogMessage(MessageDialogType Type, string Message) { }

    public void Register(FrameworkElement element)
    {
        var xamlRoot = element.XamlRoot;
        if (xamlRoot is null)
        {
            element.Loaded += OnLoaded;
            return;

            void OnLoaded(object sender, RoutedEventArgs e)
            {
                element.Loaded -= OnLoaded;
                var xamlRoot = element.XamlRoot ??
                    throw new Exception("XamlRoot is null"); ;
                RegisterImpl(xamlRoot);
            }
        }

        RegisterImpl(xamlRoot);
    }

    internal void RegisterImpl(XamlRoot xamlRoot)
    {
        WeakReferenceMessenger.Default.Register<ModalDialogMessage>(this, async (r, m) =>
        {
            var title = m.Type switch
            {
                MessageDialogType.Confirm => "Confirm",
                MessageDialogType.Error => "Error",
                _ => throw new InvalidOperationException("incorrect MessageDialogType"),
            };

            var cd = new ContentDialog
            {
                XamlRoot = xamlRoot,
                Title = title,
                Content = m.Message,
                CloseButtonText = "OK"
            };

            var result = await cd.ShowAsync();
        });
    }

    public static void Confirm(string message)
    {
        WeakReferenceMessenger.Default.Send<ModalDialogMessage>(new(MessageDialogType.Confirm, message));
    }

    public static void Error(string message)
    {
        WeakReferenceMessenger.Default.Send<ModalDialogMessage>(new(MessageDialogType.Error, message));
    }
}
