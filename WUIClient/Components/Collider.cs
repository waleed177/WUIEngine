﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WUIShared.Objects;
using WUIShared.Packets;

namespace WUIClient.Components {
    public abstract class Collider : GameObject {
        public delegate void CollisionEvent(Collider sender, Collider other);
        public event CollisionEvent OnCollisionStay;

        public bool ContinouslyCheckCollisions { get; set; } = false;

        public Collider(Objects type) : base(type, false) { }

        public abstract bool CollidesWith(Collider other);

        private MovingObjectClientCollision objectClientCollision;
        private Collider[] collisions;

        public override void OnUpdate(float deltaTime) {
            base.OnUpdate(deltaTime);
            if (Parent == null || Parent.Parent == null) return;

            if (ContinouslyCheckCollisions) {
                //TODO add a way to change Program.world to world
                ExecuteEventsWithCurrentCollisions();
            }
        }


        public int GetCollisions(Collider[] collisions) {
            int amt = 0;
            foreach (GameObject child in Game1.instance.world.GetAllChildren()) {
                Collider coll = child.GetFirst<Collider>();
                if (coll == null || coll == this || coll.Parent == null || coll.Parent.transform == null) continue;
                if (amt >= collisions.Length) return amt;
                if (CollidesWith(coll)) collisions[amt++] = coll;
            }
            return amt;
        }

        public bool IsColliding() {
            foreach (GameObject child in Game1.instance.world.GetAllChildren()) {
                Collider coll = child.GetFirst<Collider>();
                if (coll == null || coll == this || coll.Parent == null || coll.Parent.transform == null) continue;
                if (CollidesWith(coll)) return true;
            }
            return false;
        }

        public void InitializeClientSidedCollision() {
            objectClientCollision = new MovingObjectClientCollision();
            collisions = new Collider[25];
        }

        //TODO: Obsolete this for security reasons.
        public void SendCurrentCollisions() {
            int amt = GetCollisions(collisions);
            for (int i = 0; i < amt && i < collisions.Length; i++)
                objectClientCollision.uids[i] = collisions[i].UID;
            objectClientCollision.uidsLength = amt;
            Send(objectClientCollision);
        }

        public void ExecuteEventsWithCurrentCollisions(bool tellOtherColliderAboutCollision = false) {
            foreach (GameObject child in Game1.instance.world.GetCurrentChildren()) {
                Collider coll = child.GetFirst<Collider>();
                if (coll == null || coll.Parent == null || coll == this || coll.Parent.transform == null) continue;
                if (CollidesWith(coll)) {
                    OnCollisionStay?.Invoke(this, coll);
                    if (tellOtherColliderAboutCollision) coll.OnCollisionStay?.Invoke(coll, this);
                    if (Parent == null) return;
                }
            }
        }
    }
}
