using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WUIClient.Components {
    public class BoxCollider : Collider {

        public BoxCollider() : base (WUIShared.Objects.Objects.boxCollider) {

        }

        public override bool CollidesWith(Collider other) {
            return Parent.transform.Bounds.OverlapsWith(other.Parent.transform.Bounds);
        }

    }
}
