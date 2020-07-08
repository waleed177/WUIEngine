using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LowLevelNetworking.Shared;
using WUIShared;
using WUIShared.Objects;

namespace WUIServer.Components {
    public class LocalScriptsComponent : GameObject {
        private string[] codes;

        public LocalScriptsComponent() : base(Objects.LocalScriptsComponent, false) {
            codes = new string[3] { "", "", "" };
        }

        public void SetScript(EventTypes eventType, string code) {
            codes[(int)eventType] = code;
            Send(new WUIShared.Packets.SendLocalScripts() {
                eventId = new int[] { (int)eventType },
                code = new string[] { code }
            });
        }

        private WUIShared.Packets.SendLocalScripts GeneratePacket() {
            return new WUIShared.Packets.SendLocalScripts() {
                eventId = new int[] { (int) EventTypes.OnLoad, (int) EventTypes.OnUpdate, (int) EventTypes.OnCollisionStay },
                code = codes
            };
        }

        public override void SendTo(ClientBase client) {
            base.SendTo(client);
            Send(client, GeneratePacket());
        }

        public override void OnAdded() {
            base.OnAdded();
            Send(GeneratePacket());
        }
        
    }
}
