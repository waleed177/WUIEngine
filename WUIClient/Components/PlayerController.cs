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
            //TODO: THERE IS A CHANGE EVERYTHING HERE WONT WORK, SO INVESTIGATE THE OWNERSHIP PACKET AND STUFF.
            //TODO: POSSIBLY MAKE A ONEXISTORFIND<Component> Function, or a WaitForChild function.
            collider = Parent.GetFirst<BoxCollider>();
            collider?.InitializeClientSidedCollision();
            if(Parent.ClientOwned)
                Game1.localPlayer = this;
        }

        public override void OnUpdate(float deltaTime) {
            base.OnUpdate(deltaTime);
            if (Parent.ClientOwned) {
                Vector2 prevPosition = transform.Position;
                bool moved = false;
                if (WKeyboard.currentKeyboardState.IsKeyDown(Keys.D)) {
                    transform.Position += new Microsoft.Xna.Framework.Vector2(HorizontalSpeed * deltaTime, 0);
                    moved = true;
                }
                if (WKeyboard.currentKeyboardState.IsKeyDown(Keys.A)) {
                    transform.Position -= new Microsoft.Xna.Framework.Vector2(HorizontalSpeed * deltaTime, 0);
                    moved = true;
                }
                if (moved && collider != null && collider.IsColliding()) {
                    collider.SendCurrentCollisions();
                    collider.ExecuteEventsWithCurrentCollisions(tellOtherColliderAboutCollision: true);
                    transform.Position = prevPosition;
                }
                moved = false;
                prevPosition = transform.Position;
                if (WKeyboard.currentKeyboardState.IsKeyDown(Keys.W)) {
                    transform.Position -= new Microsoft.Xna.Framework.Vector2(0, VerticalSpeed * deltaTime);
                    moved = true;
                }
                if (WKeyboard.currentKeyboardState.IsKeyDown(Keys.S)) {
                    transform.Position += new Microsoft.Xna.Framework.Vector2(0, VerticalSpeed * deltaTime);
                    moved = true;
                }
                if (moved && collider != null && collider.IsColliding()) {
                    collider.SendCurrentCollisions();
                    collider.ExecuteEventsWithCurrentCollisions(tellOtherColliderAboutCollision: true);
                    transform.Position = prevPosition;
                }

            }
        }

    }
}
