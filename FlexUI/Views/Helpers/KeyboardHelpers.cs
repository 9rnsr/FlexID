using Microsoft.UI.Input;
using Windows.System;

namespace FlexID.Views;

internal static class KeyboardHelpers
{
    extension(InputKeyboardSource source)
    {
        public static bool IsKeyDown(VirtualKey key)
        {
            // 現在のスレッドにおいて、イベント処理中の入力メッセージに対応するキー状態を取得する。
            var keyState = InputKeyboardSource.GetKeyStateForCurrentThread(key);
            var keyDown = (keyState & Windows.UI.Core.CoreVirtualKeyStates.Down) != 0;
            return keyDown;
        }
    }
}
