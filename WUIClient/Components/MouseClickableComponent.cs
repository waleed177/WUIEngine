using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WUIShared.Objects;

namespace WUIClient.Components {
    class MouseClickableComponent : GameObject {
        public bool screenPosition = false;

        public MouseClickable<GameObject> mouseClickable;

        public MouseClickableComponent() : this(false) { }

        public MouseClickableComponent(bool screenPosition) : base(Objects.MouseClickableComponent, false) {
            mouseClickable = new MouseClickable<GameObject>();
            this.screenPosition = screenPosition;
        }

        public static Objects Objectsfalse { get; }

        public override void OnUpdate(float deltaTime) {
            base.OnUpdate(deltaTime);
            mouseClickable.Update(this, transform.Bounds, WMouse.GetPosition(screenPosition));
        }

    }
}
