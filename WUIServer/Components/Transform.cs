using BinaryConversions;
using LowLevelNetworking.Shared;
using System;
using WUIServer.Math;
using WUIShared.Objects;
using WUIShared.Packets;

namespace WUIServer.Components {
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
                positionPacket.x = value.X;
                positionPacket.y = value.Y;
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
                sizePacket.x = value.X;
                sizePacket.y = value.Y;
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

        public RectangleF Bounds => backingField_bounds;

        private float networkPeriod = 0.1f;
        private float networkUpdateTimeLeft = 0;

        private TransformPositionSet positionPacket;
        private TransformSizeSet sizePacket;


        public Transform() : base(Objects.Transform, false) {
            positionPacket = new TransformPositionSet();
            sizePacket = new TransformSizeSet();
            On<TransformPositionSet>(TransformPositionSet);
            On<TransformSizeSet>(TransformSizeSet);
        }

        private void TransformSizeSet(ClientBase sender, TransformSizeSet packet) {
            Size = new Vector2(packet.x, packet.y);
            Send(packet);
            DirtySize = false;
        }

        private void TransformPositionSet(ClientBase sender, WUIShared.Packets.TransformPositionSet transformPositionSet) {
            Position = new Vector2(transformPositionSet.x, transformPositionSet.y);
            Send(transformPositionSet);
            DirtyPosition = false;
        }

        public override void OnUpdate(float deltaTime) {
            base.OnUpdate(deltaTime);
            if (Parent.multiplayer) {
                networkUpdateTimeLeft -= deltaTime;
                if (networkUpdateTimeLeft <= 0) {
                    networkUpdateTimeLeft = networkPeriod;
                    if (DirtyPosition) {
                        Send(positionPacket);
                        DirtyPosition = false;
                    }
                    if (DirtySize) {
                        Send(sizePacket);
                        DirtySize = false;
                    }
                }
            }
        }

        public override void SendTo(ClientBase client) {
            base.SendTo(client);
            Send(client, positionPacket);
            Send(client, sizePacket);
        }
    }
}
