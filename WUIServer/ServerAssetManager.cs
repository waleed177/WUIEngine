using LowLevelNetworking.Shared;
using System;
using System.Collections.Generic;
using System.IO;
using WUIShared.Packets;

namespace WUIServer {
    public class ServerAssetManager {

        private Dictionary<string, byte[]> assets;

        public ServerAssetManager() {
            assets = new Dictionary<string, byte[]>();
        }

        public void AddAsset(string name, byte[] asset) {
            lock (assets) {
                if (assets.ContainsKey(name)) return;
                Console.WriteLine("Broadcasting asset " + name + " to clients, the asset is of size: " + asset.Length + ".");
                assets[name] = asset;
                Program.broadcaster.Broadcast(new WUIShared.Packets.AssetSend() { assetName = name, asset = asset });
            }
        }

        public void SendAllAssetsTo(ClientBase clientBase) {
            lock (assets) foreach (var item in assets) {
                Console.WriteLine("Sending asset " + item.Key + " to client id " + clientBase.Id + ".");
                clientBase.Send(new AssetSend() { assetName = item.Key, asset = item.Value });
            }
        }

        public void Handle(ClientBase clientHandler) {
            clientHandler.On<AssetSend>(OnAssetSend);
            clientHandler.OnStart += ClientHandler_OnStart;
        }

        private void ClientHandler_OnStart(ClientBase client) {
            SendAllAssetsTo(client);
        }

        //TODO: ADD SECURITY MEASURES TO SERVER RECEIVING ASSETS.
        private void OnAssetSend(ClientBase sender, AssetSend packet) {
            Console.WriteLine("Recieved asset " + packet.assetName + " from client.");
            AddAsset(packet.assetName, packet.asset);
        }

    }
}