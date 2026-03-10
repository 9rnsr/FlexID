using CommunityToolkit.Mvvm.DependencyInjection;
using CommunityToolkit.Mvvm.Messaging;
using CommunityToolkit.WinUI;
using FlexID.ViewModels;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace FlexID.Views;

public class ScoeffState
{
    public bool IsRead { get; set; }
    public bool IsLoaded { get; set; }
}

public sealed partial class InputScoeffPage : Page
{
    public InputScoeffPage()
    {
        ViewModel = Ioc.Default.GetRequiredService<InputScoeffViewModel>();

        InitializeComponent();

        _state = new();
        NuclidesView.IsEnabled = false;
        WeakReferenceMessenger.Default.Register<ScoeffState>(this,
            (r, m) => DispatcherQueue.EnqueueAsync(() => Receive(m)));

        Loaded += InputScoeffPage_Loaded;
        void InputScoeffPage_Loaded(object sender, RoutedEventArgs e)
        {
            //Debug.WriteLine("Loaded");
            Loaded -= InputScoeffPage_Loaded;
            WeakReferenceMessenger.Default.Send(new ScoeffState { IsLoaded = true });
        }

        DataContext = Ioc.Default.GetRequiredService<InputScoeffViewModel>();
    }

    public InputScoeffViewModel ViewModel { get; }

    // NuclidesView.ItemsSourceにバインドされたコレクションへの追加と、NuclidesViewへ
    // フォーカスを移動する操作が衝突した場合に内部例外が発生するため、これを回避するために
    // 核種一覧の読み込みと画面のロード処理の両方が完了するまでNuclidesView.IsEnabled = falseを設定する。
    // おそらく関連する問題： https://github.com/microsoft/microsoft-ui-xaml/issues/8684
    private ScoeffState? _state;

    private async void Receive(ScoeffState message)
    {
        if (_state is null)
            return;

        _state.IsRead |= message.IsRead;
        _state.IsLoaded |= message.IsLoaded;
        //Debug.WriteLine($"Receive (IsRead = {_state.IsRead}, IsLoaded = {_state.IsLoaded})");
        if (!_state.IsRead || !_state.IsLoaded)
            return;

        //Debug.WriteLine($"Receive (done)");
        _state = null;
        NuclidesView.IsEnabled = true;
        WeakReferenceMessenger.Default.Unregister<ScoeffState>(this);
    }
}
