using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WUIShared.Objects;

namespace WUIServer.Components {
    public class BoxCollider : Collider {

        public BoxCollider() : base(Objects.boxCollider) {

        }

        public override bool CollidesWith(Collider other) {
            return Parent.transform.Bounds.OverlapsWith(other.Parent.transform.Bounds);
        }

    }
}
