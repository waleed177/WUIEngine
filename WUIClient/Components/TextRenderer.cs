using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using WUIShared.Objects;

namespace WUIClient.Components {
    class TextRenderer : GameObject {
        public string text;
        private Color color;

        public TextRenderer(string text, Color color) : base(Objects.TextRenderer, false) {
            this.text = text;
            this.color = color;
        }

        public override void OnRender(SpriteBatch batch, float deltaTime) {
            base.OnRender(batch, deltaTime);
            batch.DrawString(Game1.instance.arial, text, new Vector2(8 + transform.Position.X, transform.Position.Y + transform.Size.Y / 2 - 8), color);
        }
    }
}
