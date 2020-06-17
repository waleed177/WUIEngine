using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WUIShared.Objects;

namespace WUIClient.Components {
    public class ButtonComponent : GameObject {

        public Color normalColor = Color.White;
        public Color mouseHoverColor = new Color(0.9f, 0.9f, 0.9f);
        public Color mouseDownColor = new Color(0.75f,0.75f,0.75f);
        private RawTextureRenderer renderer;
        private MouseClickableComponent mouseClickable;

        public ButtonComponent() : base(Objects.ButtonComponent, false) {}

        public override void OnAdded() {
            base.OnAdded();
            renderer = Parent.GetFirst<RawTextureRenderer>();
            mouseClickable = Parent.GetFirst<MouseClickableComponent>();
            mouseClickable.mouseClickable.OnMouseEnter += MouseClickable_OnMouseEnter;
            mouseClickable.mouseClickable.OnMouseLeave += MouseClickable_OnMouseLeave;
            mouseClickable.mouseClickable.OnMouseLeftClickDown += MouseClickable_OnMouseLeftClick;
            mouseClickable.mouseClickable.OnMouseLeftClickUp += MouseClickable_OnMouseLeftClickUp;
        }

        private void MouseClickable_OnMouseLeftClickUp(GameObject sender) {
            if(mouseClickable.mouseClickable.MouseOver)
                renderer.color = mouseHoverColor;
        }

        private void MouseClickable_OnMouseLeftClick(GameObject sender) {
            renderer.color = mouseDownColor;
        }

        private void MouseClickable_OnMouseLeave(GameObject sender) {
            renderer.color = normalColor;
        }

        private void MouseClickable_OnMouseEnter(GameObject sender) {
            renderer.color = mouseHoverColor;
        }
    }
}
