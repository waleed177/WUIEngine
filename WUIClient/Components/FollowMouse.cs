using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WUIShared.Objects;

namespace WUIClient.Components {
    class FollowMouse : GameObject {
        public bool screenPosition;

        public FollowMouse() : base(Objects.followMouse, false) { }

        public FollowMouse(bool screenPosition) : base(Objects.followMouse, false) {
            this.screenPosition = screenPosition;
        }

        public override void OnUpdate(float deltaTime) {
            base.OnUpdate(deltaTime);
            transform.Position = WMouse.GetPosition(screenPosition);
        }
    }
}
