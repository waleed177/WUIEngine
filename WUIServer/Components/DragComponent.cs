using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WUIShared.Objects;

namespace WUIServer.Components {
    class DragComponent : GameObject {
        public bool screenPosition = false;

        public DragComponent() : base(Objects.draggable, false ) {}

        public override void OnAdded() {
            base.OnAdded();
            
        }

        private void MouseClickable_OnMouseLeftClick(GameObject sender) {
            
        }

        private void MouseClickable_OnMouseLeftClickUp(GameObject sender) {
            
        }

        public override void OnUpdate(float deltaTime) {
            base.OnUpdate(deltaTime);
            
        }
    }
}
