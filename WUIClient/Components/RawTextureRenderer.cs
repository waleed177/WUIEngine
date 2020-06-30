using LowLevelNetworking.Shared;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WUIShared.Objects;
using WUIShared.Packets;

namespace WUIClient.Components {
    class RawTextureRenderer : GameObject {

        public Texture2D texture;
        public Color color = Color.White;
        public float rotation = 0;
        public Vector2 pivot = Vector2.Zero;

        public RawTextureRenderer() : base(Objects.RawTextureRenderer, false) {
            On<RawTextureRendererTextureSet>(Object_RawTextureRendererTextureSet);
        }

        private void Object_RawTextureRendererTextureSet(ClientBase sender, RawTextureRendererTextureSet packet) {
            texture = Game1.assetManager.GetAsset<Texture2D>(packet.assetName);
        }

        public override void OnRender(SpriteBatch batch, float deltaTime) {
            base.OnRender(batch, deltaTime);
            if (texture != null)
                batch.Draw(texture, transform.IntBounds, null, color, rotation, pivot, SpriteEffects.None, 0);
        }

        public void SendTexture(byte[] texture) {
            //Send(0, texture, texture.Length);
        }

    }
}
