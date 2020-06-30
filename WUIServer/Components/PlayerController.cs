
using LowLevelNetworking.Shared;
using WUIServer;
using WUIShared.Objects;

namespace WUIServer.Components {
    public class PlayerController : GameObject {
        public float HorizontalSpeed { set; get; }
        public float VerticalSpeed { set; get; }

        public PlayerController() : base(Objects.PlayerController, false) {
            
        }


        public override void OnAdded() {
            base.OnAdded();
            Send(SpeedPacket());

        }

        private WUIShared.Packets.PlayerSpeedSet SpeedPacket() {
            return new WUIShared.Packets.PlayerSpeedSet() {
                speedX = HorizontalSpeed,
                speedY = VerticalSpeed
            };
        }

        public override void SendTo(ClientBase client) {
            base.SendTo(client);
            Send(client, SpeedPacket());
        }
    }
}
