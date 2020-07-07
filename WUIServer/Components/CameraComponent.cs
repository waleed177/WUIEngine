using LowLevelNetworking.Shared;
using WUIShared.Objects;
using WUIShared.Packets;

namespace WUIServer.Components {
    public class CameraComponent : GameObject {
        private Transform backingfield_follow;
        public Transform Follow {
            get => backingfield_follow;
            set {
                backingfield_follow = value;
                Send(new CameraSetFollow() { followEnabled = value != null, followUID = value == null ? -1 : value.UID });
            }
        }

        private bool backingfield_followLocalPlayer;
        public bool FollowLocalPlayer {
            get => backingfield_followLocalPlayer;
            set {
                backingfield_followLocalPlayer = value;
                backingfield_follow = null;
                Send(new CameraSetFollow() { followLocalPlayer = value, followEnabled = false, followUID = -1 });
            }
        }

        public CameraComponent() : base(WUIShared.Objects.Objects.Camera, false) {

        }

        public override void OnAdded() {
            base.OnAdded();
            Follow = Follow;
        }

        public override void SendTo(ClientBase client) {
            base.SendTo(client);
            Send(client, new CameraSetFollow() { followLocalPlayer = backingfield_followLocalPlayer, followEnabled = Follow != null, followUID = Follow == null ? -1 : Follow.UID });
        }
    }
}
