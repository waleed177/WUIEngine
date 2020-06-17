

using Microsoft.Xna.Framework;

namespace WUIClient {
    public class MouseClickable<T> {
        public delegate void MouseEvent(T sender);

        public event MouseEvent OnMouseLeftClickDown;
        public event MouseEvent OnMouseRightClickDown;
        public event MouseEvent OnMouseLeftClickUp;
        public event MouseEvent OnMouseRightClickUp;
        public event MouseEvent WhileMouseOver;
        public event MouseEvent OnMouseEnter;
        public event MouseEvent OnMouseLeave;

        private bool prevMouseOver;
        private bool prevMouseLeftIn;
        private bool prevMouseRightIn;


        public bool MouseLeftClickDown { get; private set; }
        public bool MouseRightClickDown { get; private set; }
        public bool MouseLeftClickUp { get; private set; }
        public bool MouseRightClickUp { get; private set; }
        public bool MouseOver { get; private set; }
        public bool MouseEnter { get; private set; }
        public bool MouseLeave { get; private set; }

        public void Update(T sender, RectangleF clickable, Vector2 mousePosition) {
            prevMouseOver = MouseOver;
            MouseOver = clickable.Contains(mousePosition);

            if (prevMouseOver != MouseOver) {
                if (MouseOver) {
                    MouseEnter = true;
                    OnMouseEnter?.Invoke(sender);
                } else {
                    MouseLeave = true;
                    OnMouseLeave?.Invoke(sender);
                }
            } else {
                MouseEnter = MouseLeave = false;
            }

            if (MouseOver) {
                if (MouseLeftClickDown = WMouse.LeftMouseClick()) {
                    prevMouseLeftIn = true;
                    OnMouseLeftClickDown?.Invoke(sender);
                }
                   
                if (MouseRightClickDown = WMouse.RightMouseClick()) {
                    prevMouseRightIn = true;
                    OnMouseRightClickDown?.Invoke(sender);
                }
                WhileMouseOver?.Invoke(sender);
            }

            if (prevMouseLeftIn && (MouseLeftClickUp = WMouse.LeftMouseClickUp())) {
                prevMouseLeftIn = false;
                OnMouseLeftClickUp?.Invoke(sender);
            }

            if (prevMouseRightIn && (MouseRightClickUp = WMouse.RightMouseClickUp())) {
                prevMouseRightIn = false;
                OnMouseRightClickUp?.Invoke(sender);
            }
        }
    }
}
