using System;
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
            client.On<DestroyGameObject>(Client_DestroyGameObject);
            client.OnStart += Client_OnStart;
            client.OnDisconnect += Client_OnDisconnect;
        }

        private void Client_OnDisconnect(ClientBase client) {
            if (clientObjectIdsToServerObjectIds.ContainsKey(client))
                clientObjectIdsToServerObjectIds.Remove(client);
        }

        private void Client_OnStart(ClientBase client) {
            world.SendTo(client);
            GameObject plr = Program.gameWorldFile.InstantiatePlayer();
            plr.Owner = client;
            client.Send(new OwnershipPacket() { UID = plr.UID, Owned = true });
        }

        private void Client_DestroyGameObject(ClientBase sender, DestroyGameObject packet) {
            if (!gameObjects.ContainsKey(packet.UID)) return;
            GameObject gameObject = gameObjects[packet.UID];
            gameObject.Parent.RemoveChild(gameObject);
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
                gameObjects[packet.UID].Emit(sender, packet.data);
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

        public void Remove(GameObject gameObject, bool sendToOthers) {
            gameObjects.Remove(gameObject.UID);
            if (sendToOthers && (gameObjects.ContainsKey(gameObject.Parent.UID) || gameObject.Parent.UID == 0))
                Program.broadcaster.Broadcast(new DestroyGameObject() { UID = gameObject.UID });
        }
    }
}
