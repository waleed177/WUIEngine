using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LowLevelNetworking.Shared;
using WUIShared.Objects;
using WUIShared.Packets;

namespace WUIServer.Components {
    public class UIText : GameObject {

        private string backingfield_text;
        public string Text {
            get => backingfield_text;
            set {
                backingfield_text = value;
                if (Parent.multiplayer) {
                    Send(new WUIShared.Packets.ScriptSendString() {
                        message = Text
                    });
                }
            }
        }

        public UIText() : base(Objects.UIText, false) { }

        public override void OnAdded() {
            base.OnAdded();
            Send(new ScriptSendString() { message = Text });
        }

        public override void SendTo(ClientBase client) {
            base.SendTo(client);
            Send(client, new ScriptSendString() { message = Text });
        }

    }
}
