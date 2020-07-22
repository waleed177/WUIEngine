using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WUIShared.Objects;

namespace WUIClient.Components {
    public class ClientDontReplicate : GameObject {

        public ClientDontReplicate() : base(Objects.clientDontReplicate, false) {
            
        }

        public override void OnAdded() {
            base.OnAdded();
            Parent.SetMultiplayerRecursively(false);
        }

    }
}
