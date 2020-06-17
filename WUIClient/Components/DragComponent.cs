using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WUIShared.Objects;

namespace WUIClient.Components {
    class DragComponent : GameObject {
        public bool screenPosition = false;

        private Vector2 offset;
        private bool dragging = false;

        private MouseClickable<GameObject> mouseClickable;

        public DragComponent() : base(Objects.DragComponent, false ) {}

        public override void OnAdded() {
            base.OnAdded();
            mouseClickable = Parent.GetFirst<MouseClickableComponent>().mouseClickable;
            mouseClickable.OnMouseLeftClickDown += MouseClickable_OnMouseLeftClick;
            mouseClickable.OnMouseLeftClickUp += MouseClickable_OnMouseLeftClickUp;
        }

        private void MouseClickable_OnMouseLeftClick(GameObject sender) {
            offset = transform.Position - WMouse.GetPosition(screenPosition);
            dragging = true;
        }

        private void MouseClickable_OnMouseLeftClickUp(GameObject sender) {
            dragging = false;
        }

        public override void OnUpdate(float deltaTime) {
            base.OnUpdate(deltaTime);
            if(dragging) {
                transform.Position = offset + WMouse.GetPosition(screenPosition);
            }
        }
    }
}
