using LowLevelNetworking.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WUIServer.Math;
using WUIShared.Objects;

namespace WUIServer.Components {
    class RawTextureRenderer : GameObject {
        public Texture2D texture;
        public Color color;
        public float rotation;
        public Vector2 pivot;

        public RawTextureRenderer() : base(Objects.RawTextureRenderer, false) {
            texture = new Texture2D(null);
            On(0, RecievedTexture);
        }

        private void RecievedTexture(ClientBase sender, byte[] bytes, int length) {
            texture.bytes = new byte[length];
            Array.Copy(bytes, 0, texture.bytes, 0, length);
            Send(0, bytes, length);
        }

        public override void SendTo(ClientBase client) {
            base.SendTo(client);
            Send(client, 0, texture.bytes, texture.bytes.Length);
        }
    }
}
