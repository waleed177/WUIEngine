using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WUIClient {
    public class Camera {

        public float X = 0;
        public float Y = 0;
        private Game game;
        public Matrix transformMatrix;

        public void Update(Game game) {
            this.game = game;
            transformMatrix = Matrix.CreateTranslation(-X, -Y, 0);
        }

        public void CenterViewOn(float x, float y, float w, float h) {
            X = x + w / 2 - game.GraphicsDevice.Viewport.Width / 2;
            Y = y + h / 2 - game.GraphicsDevice.Viewport.Height / 2;
        }

    }
}
