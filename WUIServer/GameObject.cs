using LowLevelNetworking.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using WUIServer.Components;
using WUIShared.Objects;

namespace WUIServer {
    public class GameObject {
        internal protected List<GameObject> children;
        private Queue<GameObject> toBeRemoved;
        private Queue<GameObject> toBeAdded;
        private Queue<Action> invokationQueue;

        public delegate void ChildDelegate(GameObject sender, GameObject gameObject);
        public event ChildDelegate OnChildAdded, OnChildRemoved;

        public GameObject Parent { get; private set; }

        public Transform transform { get; internal set; }
        public int UID { get; internal set; }
        public Objects ObjType { get; internal set; }

        public event Action<GameObject> OnUpdateEvent;
        public event Action<GameObject> OnAddedEvent;
        public event Action<GameObject> OnDestroyedEvent;

        public string name;

        private bool childrenChanged = false;

        //Multiplayer stuff
        public bool multiplayer = false;
        public delegate void RecievedBytesDelegate(ClientBase sender, byte[] bytes, int length);
        private Dictionary<byte, RecievedBytesDelegate> networkBytePacketHandlers;
        private static WUIShared.Packets.ByteArrayUserPacket byteArrayUserPacket = new WUIShared.Packets.ByteArrayUserPacket() { data = new byte[8388608] };
        public ClientBase Owner { set; get; }

        //Threading locks
        private object childModification = 1;

        public GameObject() : this(Objects.Empty, true) { }

        public GameObject(Objects type, bool hasTransform) {
            children = new List<GameObject>();
            toBeRemoved = new Queue<GameObject>();
            toBeAdded = new Queue<GameObject>();
            invokationQueue = new Queue<Action>();
            if (hasTransform) {
                transform = new Transform();
                AddChild(transform);
            }

            ObjType = type;
            networkBytePacketHandlers = new Dictionary<byte, RecievedBytesDelegate>();
            OnChildAdded += GameObject_OnChildAdded;
        }

        private void GameObject_OnChildAdded(GameObject sender, GameObject gameObject) {
            if (gameObject is Transform)
                transform = (Transform)gameObject;
        }

        private void Added() {
            if (transform == null && Parent != null)
                transform = Parent.transform;
            OnAdded();
            OnAddedEvent?.Invoke(this);
        }

        private void Destroyed() {
            OnDestroyed();
            OnDestroyedEvent?.Invoke(this);
        }

        //Thread issue with the queues. something else is dequeuing them some how.
        public void Update(float deltaTime) {
            OnUpdate(deltaTime);
            if (childrenChanged) {
                lock (childModification) {
                    while (toBeRemoved.Count > 0) {
                        GameObject obj = toBeRemoved.Dequeue();
                        children.Remove(obj);
                        invokationQueue.Enqueue(obj.Destroyed);
                        invokationQueue.Enqueue(() => { OnChildRemoved?.Invoke(this, obj); });
                    }
                    while (toBeAdded.Count > 0) {
                        GameObject obj = toBeAdded.Dequeue();
                        children.Add(obj);
                        invokationQueue.Enqueue(obj.Added);
                        invokationQueue.Enqueue(() => { OnChildAdded?.Invoke(this, obj); });
                    }
                    while (invokationQueue.Count > 0)
                        invokationQueue.Dequeue()();
                    childrenChanged = false;
                }
            }

            //This lock was (possibily) causing deadlocks so i removed it, ill keep it commented for now though.
            //lock (childModification) {
                foreach (var child in children)
                    child.Update(deltaTime);
                OnUpdateEvent?.Invoke(this);
            //}
        }

        public virtual void OnUpdate(float deltaTime) {

        }

        public virtual void OnAdded() {

        }

        public virtual void OnDestroyed() {

        }


        public void AddChild(GameObject child, bool sendToOthers = true) {
            lock (childModification) {
                childrenChanged = true;
                child.Parent = this;
                bool usedToBeMultiplayer = child.multiplayer;
                child.multiplayer = multiplayer;
                if (sendToOthers) {
                    if (multiplayer && !usedToBeMultiplayer)
                        Program.networkManager.Add(child);
                    if (multiplayer && !usedToBeMultiplayer) {
                        //TODO fix problem if children have children.
                        foreach (var item in child.GetAllChildren()) {
                            if (!item.multiplayer) {
                                Program.networkManager.Add(item);
                                item.multiplayer = true;
                            }
                        }
                    }
                }
                toBeAdded.Enqueue(child);
            }
        }

        public void RemoveChild(GameObject gameObject, bool sendToOthers = true) {
            lock (childModification) {
                childrenChanged = true;
                toBeRemoved.Enqueue(gameObject);
                if (multiplayer && gameObject.Parent != null) {
                    Program.networkManager.Remove(gameObject, sendToOthers);
                    foreach (var item in gameObject.children)
                        item.Remove();
                }
                gameObject.Parent = null;
            }
        }

        //Parent is null at random? TODO: FIX
        public void Remove(bool sendToOthers = true) {
            Parent.RemoveChild(this, sendToOthers);
        }

        public T GetFirst<T>() where T : GameObject {
            foreach (var child in children)
                if (child is T)
                    return (T)child;
            foreach (var child in toBeAdded)
                if (child is T)
                    return (T)child;
            return default;
        }

        public IEnumerable<GameObject> GetAllChildren() {
            return children.Union(toBeAdded).Except(toBeRemoved);
        }

        public void Send(byte type, byte[] data, int length) {
            byteArrayUserPacket.data = data;
            byteArrayUserPacket.UID = UID;
            byteArrayUserPacket.type = type;
            byteArrayUserPacket.dataLength = length;
            Program.broadcaster.Broadcast(byteArrayUserPacket);
        }

        public void Send(byte type, byte[] data, int length, ClientBase except) {
            byteArrayUserPacket.data = data;
            byteArrayUserPacket.UID = UID;
            byteArrayUserPacket.type = type;
            byteArrayUserPacket.dataLength = length;
            Program.broadcaster.Broadcast(byteArrayUserPacket, except);
        }

        public void Send(ClientBase client, byte type, byte[] data, int length) {
            byteArrayUserPacket.data = data;
            byteArrayUserPacket.UID = UID;
            byteArrayUserPacket.type = type;
            byteArrayUserPacket.dataLength = length;
            client.Send(byteArrayUserPacket);
        }

        public void On(byte type, RecievedBytesDelegate recievedBytesHandler) {
            if (networkBytePacketHandlers.ContainsKey(type))
                networkBytePacketHandlers[type] += recievedBytesHandler;
            else
                networkBytePacketHandlers.Add(type, recievedBytesHandler);
        }

        internal void Emit(ClientBase sender, byte type, byte[] data, int length) {
            if (networkBytePacketHandlers.ContainsKey(type))
                networkBytePacketHandlers[type]?.Invoke(sender, data, length);
        }


        public Packet GetSpawnPacket() {
            if (Parent == null)
                return null;
            return new WUIShared.Packets.SpawnGameObject() {
                UID = UID,
                parentUID = Parent.UID,
                ObjType = (int)ObjType
            };
        }

        //TODO: THREAD SAFETY.
        public virtual void SendTo(ClientBase client) {
            lock (childModification) {
                Packet packet = GetSpawnPacket();
                if (packet != null)
                    client.Send(packet);
                foreach (var item in GetAllChildren().ToArray())
                    item.SendTo(client);
            }
        }


    }

}
