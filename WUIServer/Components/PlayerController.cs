
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
            byte[] data = new byte[8];
            BinaryConversions.BinConversion.GetBytes(data, 0, HorizontalSpeed);
            BinaryConversions.BinConversion.GetBytes(data, 4, VerticalSpeed);
            Send(0, data, data.Length);
        }

        public override void SendTo(ClientBase client) {
            base.SendTo(client);
            byte[] data = new byte[8];
            BinaryConversions.BinConversion.GetBytes(data, 0, HorizontalSpeed);
            BinaryConversions.BinConversion.GetBytes(data, 4, VerticalSpeed);
            Send(client, 0, data, data.Length);
        }
    }
}
