using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LowLevelNetworking.Shared;
using WUIShared.Packets;

namespace WUIClient.Components {
    public class CameraComponent : GameObject {
        private Transform follow;
        private bool followLocalPlayer = false;
        public CameraComponent() : base(WUIShared.Objects.Objects.Camera, false) {
            On<CameraSetFollow>(OnCameraSetFollow);
        }

        private void OnCameraSetFollow(ClientBase sender, CameraSetFollow packet) {
            if (packet.followEnabled)
                follow = Game1.networkManager.Get(packet.followUID).transform;
            else
                follow = null;
            followLocalPlayer = packet.followLocalPlayer;
        }

        public override void OnUpdate(float deltaTime) {
            base.OnUpdate(deltaTime);
            if (followLocalPlayer && Game1.localPlayer != null)
                follow = Game1.localPlayer.transform;
            if(follow != null)
                transform.Position = follow.Position;
            Game1.instance.camera.X = transform.Position.X - Game1.instance.GraphicsDevice.Viewport.Width/2;
            Game1.instance.camera.Y = transform.Position.Y - Game1.instance.GraphicsDevice.Viewport.Height/2;
            if(follow != null) {
                Game1.instance.camera.X += follow.Size.X / 2;
                Game1.instance.camera.Y += follow.Size.Y / 2;
            }
        }
    }
}
