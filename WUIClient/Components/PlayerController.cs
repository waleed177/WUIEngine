using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WUIShared.Objects;

namespace WUIClient.Components {
    public class PlayerController : GameObject {
        public float HorizontalSpeed { get; private set; } = 32;
        public float VerticalSpeed { get; private set; } = 32;

        public PlayerController() : base(Objects.PlayerController, false) {
            On(0, OnRecieveSpeeds);
        }

        private void OnRecieveSpeeds(GameObject sender, byte[] bytes, int length) {
            BinaryConversions.BinConversion.GetFloat(bytes, 0, out float temp); HorizontalSpeed = temp;
            BinaryConversions.BinConversion.GetFloat(bytes, 0, out float temp2); VerticalSpeed = temp2;
        }

        public override void OnUpdate(float deltaTime) {
            base.OnUpdate(deltaTime);
            if (WKeyboard.currentKeyboardState.IsKeyDown(Keys.D))
                transform.Position += new Microsoft.Xna.Framework.Vector2(HorizontalSpeed * deltaTime, 0);
            if (WKeyboard.currentKeyboardState.IsKeyDown(Keys.A))
                transform.Position -= new Microsoft.Xna.Framework.Vector2(HorizontalSpeed * deltaTime, 0);
            if (WKeyboard.currentKeyboardState.IsKeyDown(Keys.W))
                transform.Position -= new Microsoft.Xna.Framework.Vector2(0, VerticalSpeed * deltaTime);
            if (WKeyboard.currentKeyboardState.IsKeyDown(Keys.S))
                transform.Position += new Microsoft.Xna.Framework.Vector2(0, VerticalSpeed * deltaTime);
        }
    }
}
