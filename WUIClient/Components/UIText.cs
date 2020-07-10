using System;
using LowLevelNetworking.Shared;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using WUIShared.Objects;
using WUIShared.Packets;

namespace WUIClient.Components {
    public class UIText : GameObject {

        private string backingfield_text = "";
        public string Text {
            get => backingfield_text;
            set {
                backingfield_text = value;
                //TODO: Add networking for text if needed.
            }
        }

        public UIText() : base(Objects.UIText, false) {
            On<WUIShared.Packets.ScriptSendString>(OnTextRecieved);
        }

        private void OnTextRecieved(ClientBase sender, ScriptSendString packet) {
            Text = packet.message;
        }

        public override void OnRender(SpriteBatch batch, float deltaTime) {
            base.OnRender(batch, deltaTime);
            batch.DrawString(Game1.instance.arial, Text, transform.Position, Color.Black);
        }
    }
}
