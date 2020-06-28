using LowLevelNetworking.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WUIServer.Math;
using WUIShared.Objects;
using WUIShared.Packets;

namespace WUIServer.Components {
    class RawTextureRenderer : GameObject {
        public Texture2D texture;
        public Color color;
        public float rotation;
        public Vector2 pivot;

        private byte[] texturePacket;

        public RawTextureRenderer() : base(Objects.RawTextureRenderer, false) {
            texture = new Texture2D(null);
            On(0, RecievedTexture);
            
        }

        private void RecievedTexture(ClientBase sender, byte[] bytes, int length) {
            //TODO: IMPLEMENT RECIEVED TEXTURE
            Console.WriteLine("RecievedTexture not implemented");
            
        }

        private Packet GetTexturePacket() {
            return new RawTextureRendererTextureSet() {
                assetName = texture.name,
                r = 1,
                g = 1,
                b = 1
            };
        }

        private void SerializeTexturePacket() {
            Packet packet = GetTexturePacket();
            texturePacket = new byte[packet.RawSerializeSize];
            packet.SerializeTo(texturePacket);
        }

        public override void SendTo(ClientBase client) {
            base.SendTo(client);

            if (texture != null && texture.bytes != null) {
                if(texturePacket == null) SerializeTexturePacket();
                Send(client, 0, texturePacket, texturePacket.Length);
            }
   
        }

        public override void OnAdded() {
            base.OnAdded();
            if (texture != null && texture.bytes != null) {
                SerializeTexturePacket();
                Send(0, texturePacket, texturePacket.Length);
            }
        }
    }
}
