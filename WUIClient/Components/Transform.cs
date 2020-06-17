using BinaryConversions;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WUIShared.Objects;

namespace WUIClient.Components {
    public class Transform : GameObject {
        private Vector2 backingField_Position;
        private bool DirtyPosition = false;
        public Vector2 Position {
            get => backingField_Position;
            set {
                foreach (var child in children)
                    child.transform.Position = child.transform.LocalPosition + value;
                backingField_Position = value;
                backingField_bounds.x = value.X;
                backingField_bounds.y = value.Y;
                backingField_intBounds.X = (int)value.X;
                backingField_intBounds.Y = (int)value.Y;
                DirtyPosition = true;
            }
        }

        private Vector2 backingField_Size;
        private bool DirtySize = false;
        public Vector2 Size {
            get => backingField_Size;
            set {
                backingField_Size = value;
                backingField_bounds.width = value.X;
                backingField_bounds.height = value.Y;
                backingField_intBounds.Width = (int)value.X;
                backingField_intBounds.Height = (int)value.Y;
                DirtySize = true;
            }
        }

        public Vector2 LocalPosition {
            get => Position - Parent.transform.Position;
            set {
                Position = value + Parent.transform.Position;
            }
        }
        //public Vector3 LocalSize { get; set; }

        private RectangleF backingField_bounds;
        private Rectangle backingField_intBounds;

        public RectangleF Bounds => backingField_bounds;
        public Rectangle IntBounds => backingField_intBounds;

        private float networkPeriod = 0.1f;
        private float networkUpdateTimeLeft = 0;

        byte[] networkBuffer;

        public Transform() : base(Objects.Transform, false) {
            networkBuffer = new byte[16];
            On(0, NetworkTransformUpdate);

            OnNetworkReady += Transform_OnNetworkReady;
        }

        private void Transform_OnNetworkReady(GameObject sender) {
            ForceNetworkUpdatePosSiz();
        }

        private void NetworkTransformUpdate(GameObject sender, byte[] bytes, int length) {
            BinConversion.GetFloat(bytes, 0, out float x);
            BinConversion.GetFloat(bytes, 4, out float y);
            BinConversion.GetFloat(bytes, 8, out float w);
            BinConversion.GetFloat(bytes, 12, out float h);
            Position = new Vector2(x, y);
            Size = new Vector2(w, h);
            DirtyPosition = false;
            DirtySize = false;
        }

        public override void OnUpdate(float deltaTime) {
            base.OnUpdate(deltaTime);
            if (Parent.multiplayer) {
                networkUpdateTimeLeft -= deltaTime;
                if (networkUpdateTimeLeft <= 0) {
                    networkUpdateTimeLeft = networkPeriod;
                    if (DirtyPosition || DirtySize) {
                        ForceNetworkUpdatePosSiz();

                    }
                }
            }
        }

        private void ForceNetworkUpdatePosSiz() {
            BinConversion.GetBytes(networkBuffer, 0, Position.X);
            BinConversion.GetBytes(networkBuffer, 4, Position.Y);
            BinConversion.GetBytes(networkBuffer, 8, Size.X);
            BinConversion.GetBytes(networkBuffer, 12, Size.Y);
            Send(0, networkBuffer, 16);
            DirtyPosition = false;
            DirtySize = false;
        }
    }
}
