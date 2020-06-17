using BinaryConversions;
using LowLevelNetworking.Shared;
using System;
using WUIShared.Packets;

namespace WUIServer {
    public class ClientHandler : ClientBase {

        public ClientHandler() {
            OnStart += ClientHandler_OnStart;
            OnDisconnect += ClientHandler_OnDisconnect;

            Program.networkManager.HandleClient(this);

            On<WUIShared.Packets.SpawnGameObject>(Client_SpawnGameObjectRequest);
            On<WUIShared.Packets.ByteArrayUserPacket>(Client_ByteArrayUserPacket);
        }

        private void Client_ByteArrayUserPacket(ClientBase sender, ByteArrayUserPacket packet) {
            
        }

        private void Client_SpawnGameObjectRequest(ClientBase sender, SpawnGameObject packet) {
            Console.WriteLine($"Client {Id} requested to spawn {packet.UID}:{packet.parentUID} of type {packet.ObjType};");

        }

        private void ClientHandler_OnStart(ClientBase client) {
            Program.broadcaster.Add(this);
            Console.WriteLine($"Client {Id} Joined.");
        }

        private void ClientHandler_OnDisconnect(ClientBase client) {
            Console.WriteLine($"Client {Id} Left.");
        }
    }
}