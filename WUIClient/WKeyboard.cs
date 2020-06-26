using Microsoft.Xna.Framework.Input;

namespace WUIClient {
    public static class WKeyboard {
        public static KeyboardState prevKeyboardState;
        public static KeyboardState currentKeyboardState;

        public static void Update() {
            prevKeyboardState = currentKeyboardState;
            currentKeyboardState = Keyboard.GetState();
        }

        public static bool KeyClick(Keys key) {
            return currentKeyboardState.IsKeyDown(key) && !prevKeyboardState.IsKeyDown(key);
        }
    }
}
