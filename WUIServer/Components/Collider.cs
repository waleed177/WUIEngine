using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WUIServer.Components {
    public abstract class Collider : GameObject {
        public delegate void CollisionEvent(Collider sender, Collider other);
        public event CollisionEvent OnCollisionStay;

        public bool ContinouslyCheckCollisions { get; set; } = false;

        public abstract bool CollidesWith(Collider other);

        public override void OnUpdate(float deltaTime) {
            base.OnUpdate(deltaTime);
            if(ContinouslyCheckCollisions) {
                //TODO add a way to change Program.world to world
                foreach(GameObject child in Program.world.GetAllChildren()) {
                    Collider coll = child.GetFirst<Collider>();
                    if (coll == null || coll == this) continue;
                    if (CollidesWith(coll))
                        OnCollisionStay?.Invoke(this, coll);
                }
            }
        }
    }
}
