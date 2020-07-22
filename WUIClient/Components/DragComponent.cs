using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WUIShared.Objects;

namespace WUIClient.Components {
    class DragComponent : GameObject {
        public enum Axis {
            All,
            X,
            Y
        }
        public bool screenPosition = false;
        public Transform dragTransform;
        public Axis axis = Axis.All;
        private Vector2 originalPosition;
        private Vector2 offset;
        private bool dragging = false;

        private MouseClickable<GameObject> mouseClickable;

        public DragComponent() : base(Objects.draggable, false) {
            
        }

        public override void OnAdded() {
            base.OnAdded();
            if(dragTransform == null)
                dragTransform = transform;
            mouseClickable = Parent.GetFirst<MouseClickableComponent>().mouseClickable;
            mouseClickable.OnMouseLeftClickDown += MouseClickable_OnMouseLeftClick;
            mouseClickable.OnMouseLeftClickUp += MouseClickable_OnMouseLeftClickUp;
        }

        private void MouseClickable_OnMouseLeftClick(GameObject sender) {
            originalPosition = dragTransform.Position;
            offset = dragTransform.Position - WMouse.GetPosition(screenPosition);
            dragging = true;
        }

        private void MouseClickable_OnMouseLeftClickUp(GameObject sender) {
            dragging = false;
        }

        public override void OnUpdate(float deltaTime) {
            base.OnUpdate(deltaTime);
            if (dragging) {
                Vector2 pos = offset + WMouse.GetPosition(screenPosition);
                switch (axis) {
                    case Axis.All:
                        dragTransform.Position = pos;
                        break;
                    case Axis.X:
                        dragTransform.Position = new Vector2(pos.X, originalPosition.Y);
                        break;
                    case Axis.Y:
                        dragTransform.Position = new Vector2(originalPosition.X, pos.Y);
                        break;
                }
            }
        }
    }
}
