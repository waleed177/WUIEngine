using LowLevelNetworking.Shared;
using System.IO;
using System.Text;
using WUIShared.Objects;
using WUIShared.Packets;

namespace WUIServer {
    public class WUIGGameSaver {
        private GameObject world;

        public WUIGGameSaver(GameObject world) {
            this.world = world;
        }

        public void HandleClient(ClientBase client) {
            client.On<SaveWorldPacket>(SaveWorld_Requested);
        }

        private void SaveWorld_Requested(ClientBase sender, SaveWorldPacket packet) {
            SaveWorld(packet.name);
        }

        private void SaveWorld(string name) {
            //TODO: Make use of the name. make sure its sanitized.
            StringBuilder stringBuilder = new StringBuilder(1000);
            int num = 0;
            foreach (var item in world.GetAllChildren()) {
                item.StringSerialize(stringBuilder, 0, "Object_" + num);
                num++;
            }

            File.WriteAllText(@"Save.txt", stringBuilder.ToString());
        }
    }
}
