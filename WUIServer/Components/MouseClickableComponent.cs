using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WUIShared.Objects;

namespace WUIServer.Components {
    class MouseClickableComponent : GameObject {
        public bool screenPosition = false;

        public MouseClickableComponent() : this(false) { }

        public MouseClickableComponent(bool screenPosition) : base(Objects.clickable, false) {
            
            this.screenPosition = screenPosition;
        }

        public override void OnUpdate(float deltaTime) {
            base.OnUpdate(deltaTime);
            
        }

    }
}
