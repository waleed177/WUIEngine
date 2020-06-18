using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WUIServer.Components {
    public class BoxCollider : Collider {
        
        public override bool CollidesWith(Collider other) {
            return Parent.transform.Bounds.OverlapsWith(other.Parent.transform.Bounds);
        }

    }
}
