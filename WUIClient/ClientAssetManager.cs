using LowLevelNetworking.Shared;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.IO;
using WUIShared;
using WUIShared.Packets;

namespace WUIClient {
    public class ClientAssetManager {
        private Dictionary<string, object> assets;
        ClientBase client;

        public ClientAssetManager (ClientBase client) {
            this.client = client;
            assets = new Dictionary<string, object>();
            client.On<AssetSend>(OnAssetSend);
        }

        public void SetAsset(string name, byte[] asset) {
            client.Send(new WUIShared.Packets.AssetSend() { assetName = name, asset = asset });
        }

        private void OnAssetSend(ClientBase sender, AssetSend packet) {
            using (MemoryStream memoryStream = new MemoryStream(packet.asset))
                assets[packet.assetName] = Texture2D.FromStream(Game1.instance.GraphicsDevice, memoryStream);
        }

        public T GetAsset<T>(string name) {
            return (T) assets[name];
        }
    }
}
