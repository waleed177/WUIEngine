
using LowLevelNetworking.Shared;
using Microsoft.Xna.Framework.Graphics;
using WUIClient;

namespace WUIShared.Objects {
    public partial class GameObject {
        internal static NetworkManager networkManager;

        public void Render(SpriteBatch batch, float deltaTime) {
            OnRender(batch, deltaTime);
            foreach (var child in children)
                child.Render(batch, deltaTime);
        }

        public virtual void OnRender(SpriteBatch batch, float deltaTime) {

        }

        public void Send(Packet packet) {
            lock (byteArrayUserPacket) {
                packet.SerializeTo(byteArrayUserPacket.data);
                byteArrayUserPacket.dataLength = packet.RawSerializeSize;
                byteArrayUserPacket.UID = UID;
                Game1.client.Send(byteArrayUserPacket);
            }
        }
    }
}
