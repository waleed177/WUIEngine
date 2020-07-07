
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using WUIShared.Objects;

namespace WUIClient {
    public static class WMouse {
        public static Vector2 Position { get; private set; }
        public static Camera camera;

        public static MouseState prevState;
        public static MouseState mouseState;

        public static Vector2 WorldPosition { get; private set; }

        public static void Update() {
            prevState = mouseState;
            mouseState = Mouse.GetState();
            Position = new Vector2(mouseState.X, mouseState.Y);
            WorldPosition = new Vector2((mouseState.X + camera.X), (mouseState.Y + camera.Y));
        }

        public static Vector2 GetPosition(bool screenPosition) {
            return screenPosition ? Position : WorldPosition;
        }

        public static bool ScreenHoveringOver(Rectangle rect) => rect.Contains(Position);
        public static bool ScreenHoveringOver(RectangleF rect) => rect.Contains(Position);


        public static bool LeftMouseClick() {
            return TryUseMouse(mouseState.LeftButton == ButtonState.Pressed && prevState.LeftButton != ButtonState.Pressed);
        }

        public static bool RightMouseClick() {
            return TryUseMouse(mouseState.RightButton == ButtonState.Pressed && prevState.RightButton != ButtonState.Pressed);
        }

        public static bool LeftMouseClickUp() {
            return TryUseMouse(mouseState.LeftButton != ButtonState.Pressed && prevState.LeftButton == ButtonState.Pressed);
        }

        public static bool RightMouseClickUp() {
            return TryUseMouse(mouseState.RightButton != ButtonState.Pressed && prevState.RightButton == ButtonState.Pressed);
        }

        private static bool TryUseMouse(bool inp) {
            return inp;
        }

        public static GameObject GetGameObjectUnderneathMouse(GameObject root, bool screenPosition) {
            Vector2 mousePos = GetPosition(screenPosition);
            foreach (var item in root.GetAllChildren()) {
                if (item.transform != null && item.transform.Bounds.Contains(mousePos))
                    return item;
            }
            return null;
        }
    }
}
