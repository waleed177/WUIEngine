using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LowLevelNetworking.Shared;
using WUIShared.Objects;
using WUIShared.Packets;

namespace WUIServer.Components {
    public abstract class Collider : GameObject {
        public delegate void CollisionEvent(Collider sender, Collider other);
        public event CollisionEvent OnCollisionStay;

        public bool ContinouslyCheckCollisions { get; set; } = false;

        public Collider(Objects type) : base(type, false) {
            On<MovingObjectClientCollision>(OnMovingObjectClientCollision);
        }

        public abstract bool CollidesWith(Collider other);

        public override void OnUpdate(float deltaTime) {
            base.OnUpdate(deltaTime);
            if (Parent == null || Parent.Parent == null) return;

            if (ContinouslyCheckCollisions) {
                //TODO add a way to change Program.world to world
                foreach (GameObject child in Program.world.GetCurrentChildren()) {
                    Collider coll = child.GetFirst<Collider>();
                    if (coll == null || coll.Parent == null || coll == this || coll.Parent.transform == null) continue;

                    if (CollidesWith(coll)) {
                        OnCollisionStay?.Invoke(this, coll);
                        if (Parent == null) return;
                    }
                }
            }
        }

        public int GetCollisions(Collider[] collisions) {
            int amt = 0;
            foreach (GameObject child in Program.world.GetAllChildren()) {
                Collider coll = child.GetFirst<Collider>();
                if (coll == null || coll == this || coll.Parent.transform == null) continue;
                if (amt >= collisions.Length) return amt;
                if (CollidesWith(coll)) collisions[amt++] = coll;
            }
            return amt;
        }

        public bool IsColliding() {
            foreach (GameObject child in Program.world.GetAllChildren()) {
                Collider coll = child.GetFirst<Collider>();
                if (coll == null || coll == this || coll.Parent.transform == null) continue;
                if (CollidesWith(coll)) return true;
            }
            return false;
        }

        private void OnMovingObjectClientCollision(ClientBase sender, MovingObjectClientCollision packet) {
            for (int i = 0; i < packet.uidsLength; i++) {
                Collider collider = ((Collider)Program.networkManager.Get(packet.uids[i]));
                if (collider == null || collider.Parent == null || collider.Parent.Parent == null) continue;
                //TODO CHECK IF ITS BETTER TO BATCH CALL INSTEAD OF INVOKE MANY TIMES.
                Invoke(InvokeCollision);
                void InvokeCollision() {
                    if (collider == null || collider.Parent == null || collider.Parent.Parent == null || Parent == null || Parent.Parent == null) return;
                    collider.OnCollisionStay?.Invoke(collider, this);
                    OnCollisionStay?.Invoke(this, collider);
                }
            }
        }

    }
}
