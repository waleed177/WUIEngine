using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WUIClient.Components {
    public class CameraComponent : GameObject {

        public CameraComponent() : base(WUIShared.Objects.Objects.Camera, false) {

        }

        public override void OnUpdate(float deltaTime) {
            base.OnUpdate(deltaTime);
            Game1.instance.camera.X = transform.Position.X;
            Game1.instance.camera.Y = transform.Position.Y;
        }
    }
}
