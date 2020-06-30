﻿using LowLevelNetworking.Shared;
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

        public RawTextureRenderer() : base(Objects.RawTextureRenderer, false) {
            texture = new Texture2D(null);
            On<RawTextureRendererTextureSet>(RecievedTexture);
        }

        private void RecievedTexture(ClientBase sender, RawTextureRendererTextureSet rawTextureRendererTextureSet) {
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

        public override void SendTo(ClientBase client) {
            base.SendTo(client);

            if (texture != null && texture.bytes != null)
                Send(client, GetTexturePacket());
        }

        public override void OnAdded() {
            base.OnAdded();
            if (texture != null && texture.bytes != null) {
                Send(GetTexturePacket());
            }
        }
    }
}
