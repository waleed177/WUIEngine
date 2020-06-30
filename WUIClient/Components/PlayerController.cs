using LowLevelNetworking.Shared;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WUIShared.Objects;
using WUIShared.Packets;

namespace WUIClient.Components {
    public class PlayerController : GameObject {
        public float HorizontalSpeed { get; private set; } = 32;
        public float VerticalSpeed { get; private set; } = 32;
        private Collider collider;

        public PlayerController() : base(Objects.PlayerController, false) {
            On<PlayerSpeedSet>(PlayerSpeedSet);
        }

        private void PlayerSpeedSet(ClientBase sender, PlayerSpeedSet packet) {
            HorizontalSpeed = packet.speedX;
            VerticalSpeed = packet.speedY;
        }

        public override void OnAdded() {
            base.OnAdded();
            collider = Parent.GetFirst<BoxCollider>();
            collider.InitializeClientSidedCollision();
        }

        public override void OnUpdate(float deltaTime) {
            base.OnUpdate(deltaTime);
            if (Parent.ClientOwned) {
                Vector2 prevPosition = transform.Position;
                if (WKeyboard.currentKeyboardState.IsKeyDown(Keys.D))
                    transform.Position += new Microsoft.Xna.Framework.Vector2(HorizontalSpeed * deltaTime, 0);
                if (WKeyboard.currentKeyboardState.IsKeyDown(Keys.A))
                    transform.Position -= new Microsoft.Xna.Framework.Vector2(HorizontalSpeed * deltaTime, 0);
                if (WKeyboard.currentKeyboardState.IsKeyDown(Keys.W))
                    transform.Position -= new Microsoft.Xna.Framework.Vector2(0, VerticalSpeed * deltaTime);
                if (WKeyboard.currentKeyboardState.IsKeyDown(Keys.S))
                    transform.Position += new Microsoft.Xna.Framework.Vector2(0, VerticalSpeed * deltaTime);
                if(collider != null && collider.IsColliding()) {
                    collider.SendCurrentCollisions();
                    transform.Position = prevPosition;
                }
            }
        }

    }
}
