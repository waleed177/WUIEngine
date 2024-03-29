﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LowLevelNetworking.Shared;
using WUIClient.Components;
using WUIShared.Objects;
using WUIShared.Packets;

namespace WUIClient {
    public class NetworkManager {
        private Dictionary<int, GameObject> gameObjects;
        private int freeId = int.MaxValue; //TODO: FIX EDGE CASE WHEN PLAYER ADDS A LOT OF THINGS TO THE WORLD.
        private GameObject world;

        public NetworkManager(GameObject world) {
            this.world = world;
            gameObjects = new Dictionary<int, GameObject>();
            Game1.client.On<WUIShared.Packets.ByteArrayUserPacket>(Client_ByteArrayUserPacket);
            Game1.client.On<WUIShared.Packets.SpawnGameObject>(Client_SpawnGameObject);
            Game1.client.On<WUIShared.Packets.ChangeGameObjectUID>(Client_ChangeGameObjectUID);
            Game1.client.On<DestroyGameObject>(Client_DestroyGameObject);
            Game1.client.On<OwnershipPacket>(Client_OwnershipPacket);
        }

        private void Client_OwnershipPacket(ClientBase sender, OwnershipPacket packet) {
            gameObjects[packet.UID].ClientOwned = packet.Owned;
        }

        private void Client_DestroyGameObject(ClientBase sender, DestroyGameObject packet) {
            Console.WriteLine("Attempting removal of " + packet.UID);
            if (gameObjects.ContainsKey(packet.UID)) {
                GameObject gameObject = gameObjects[packet.UID];
                gameObject.Parent.RemoveChild(gameObject, false);
                Console.WriteLine("Successfully removed " + packet.UID);
            }
        }

        private void Client_ChangeGameObjectUID(ClientBase sender, ChangeGameObjectUID packet) {
            GameObject gameObject = gameObjects[packet.oldUID];
            gameObject.SetPermanentNetworkUID(packet.newUID);
            gameObjects[packet.newUID] = gameObject;
            gameObjects[packet.oldUID] = null;

            Game1.client.Send(new FreeTempUID() { UID = packet.oldUID });
        }

        private void Client_SpawnGameObject(ClientBase sender, SpawnGameObject packet) {
            Console.WriteLine(packet);
            //To avoid duplicates and possibly infinite spawn loop.
            if (gameObjects.ContainsKey(packet.UID)) return;

            GameObject gameObject = ObjectInstantiator.Instantiate((Objects)packet.ObjType);
            gameObject.name = packet.name;
            gameObject.UID = packet.UID;
            if (packet.parentUID == 0)
                world.AddChild(gameObject, false);
            else
                gameObjects[packet.parentUID].AddChild(gameObject, false); //TODO: It errors here with gameObjects[packet.parentUID] not existing, but it happens randomly


            gameObjects[gameObject.UID] = gameObject;

            //TODO: MOVE THIS CODE ELSEWHERE
            Game1.worldActionScript.SetVariable(new string[] { gameObject.name }, new Dictionary<string, object>());
            Game1.worldActionScript.SetVariable(new string[] { gameObject.name, "object" }, gameObject);
        }


        private void Client_ByteArrayUserPacket(ClientBase sender, WUIShared.Packets.ByteArrayUserPacket packet) {
            if (gameObjects.ContainsKey(packet.UID))
                gameObjects[packet.UID].Emit(sender, packet.data);
        }

        public void Add(GameObject gameObject) {
            if (gameObject.UID == 0)
                gameObject.UID = freeId--;
            gameObjects[gameObject.UID] = gameObject;
            Game1.client.Send(new WUIShared.Packets.SpawnGameObject() {
                UID = gameObject.UID,
                parentUID = gameObject.Parent.UID,
                ObjType = (int)gameObject.ObjType,
                name = gameObject.name
            });
        }

        public void Remove(GameObject gameObject, bool sendToOthers) {
            gameObjects.Remove(gameObject.UID);
            if (sendToOthers && (gameObjects.ContainsKey(gameObject.Parent.UID) || gameObject.Parent.UID == 0))
                Game1.client.Send(new DestroyGameObject() { UID = gameObject.UID });
        }

        public GameObject Get(int uid) {
            return gameObjects[uid];
        }
    }
}
