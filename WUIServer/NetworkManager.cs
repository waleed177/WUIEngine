﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LowLevelNetworking.Shared;
using WUIServer;
using WUIShared.Objects;
using WUIShared.Packets;

namespace WUIServer {
    public class NetworkManager {
        private Dictionary<int, GameObject> gameObjects;
        private Dictionary<ClientBase, Dictionary<int, int>> clientObjectIdsToServerObjectIds;

        private int freeId = 1;
        private GameObject world;

        public NetworkManager(GameObject world) {
            this.world = world;
            gameObjects = new Dictionary<int, GameObject>();
            clientObjectIdsToServerObjectIds = new Dictionary<ClientBase, Dictionary<int, int>>();
        }

        public void HandleClient(ClientBase client) {
            client.On<ByteArrayUserPacket>(Client_ByteArrayUserPacket);
            client.On<SpawnGameObject>(Client_SpawnGameObject);
            client.On<FreeTempUID>(Client_FreeTempUID);

            client.OnStart += Client_OnStart;
        }

        private void Client_OnStart(ClientBase client) {
            world.SendTo(client);
        }

        private void Client_FreeTempUID(ClientBase sender, FreeTempUID packet) {
            clientObjectIdsToServerObjectIds[sender].Remove(packet.UID);
        }

        private void Client_SpawnGameObject(ClientBase sender, SpawnGameObject packet) {
            //To avoid duplicates and possibly infinite spawn loop.
            //TODO: Restrict the amount of temp ids.
            if (gameObjects.ContainsKey(packet.UID)) return; //Send back an error.
            if (!clientObjectIdsToServerObjectIds.ContainsKey(sender))
                clientObjectIdsToServerObjectIds.Add(sender, new Dictionary<int, int>());
            Dictionary<int, int> clientToServerIds = clientObjectIdsToServerObjectIds[sender];
            
            GameObject gameObject = ObjectInstantiator.Instantiate((Objects)packet.ObjType);
            gameObject.UID = GenerateFreeId();
            clientToServerIds[packet.UID] = gameObject.UID;


            sender.Send(new ChangeGameObjectUID() { oldUID = packet.UID, newUID = gameObject.UID });
            if (packet.parentUID == 0)
                world.AddChild(gameObject);
            else
                gameObjects[clientToServerIds[packet.parentUID]].AddChild(gameObject);

        }

        private void Client_ByteArrayUserPacket(ClientBase sender, WUIShared.Packets.ByteArrayUserPacket packet) {
            if (gameObjects.ContainsKey(packet.UID))
                gameObjects[packet.UID].Emit(sender, packet.type, packet.data, packet.dataLength);
        }

        public void Add(GameObject gameObject) {
            if (gameObject.UID == 0)
                gameObject.UID = GenerateFreeId();
            gameObjects[gameObject.UID] = gameObject;
            Program.broadcaster.Broadcast(gameObject.GetSpawnPacket());
        }

        private int GenerateFreeId() {
            return freeId++;
        }

        public void Remove(GameObject networkReplicator) {
            gameObjects.Remove(networkReplicator.UID);
        }
    }
}
