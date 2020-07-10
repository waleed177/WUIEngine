using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WUIShared.Objects;

namespace WUIServer.Components {
    public class ClientDontReplicate : GameObject {

        public ClientDontReplicate() : base(Objects.ClientDontReplicate, false) {

        }

    }
}
