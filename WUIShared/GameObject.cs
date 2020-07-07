using LowLevelNetworking.Packets;
using LowLevelNetworking.Shared;
using System;
using System.Collections.Generic;
using System.Linq;

#if WUIServer
using WUIServer.Components;
#elif WUIClient
using WUIClient.Components;
#endif

namespace WUIShared.Objects {
    public partial class GameObject {
        public string name;

        private List<GameObject> children;
        private Queue<GameObject> toBeRemoved;
        private Queue<GameObject> toBeAdded;
        private Queue<Action> invokationQueue;

        public delegate void ChildDelegate(GameObject sender, GameObject gameObject);
        public event ChildDelegate OnChildAdded, OnChildRemoved;

        public GameObject Parent { get; private set; }

        public Transform transform { get; internal set; }
        public int UID { get; internal set; }
        public Objects ObjType { get; internal set; }

        private bool childrenChanged = false;

        //Multiplayer stuff
        public bool multiplayer = false;
        private static WUIShared.Packets.ByteArrayUserPacket byteArrayUserPacket = new WUIShared.Packets.ByteArrayUserPacket() { data = new byte[8388608] };
        private PacketHandler<ClientBase> packetHandler;

        public delegate void RecievedNetworkUIDDelegate(GameObject sender, int UID);
        public event RecievedNetworkUIDDelegate OnRecieveNetworkUID;

        public delegate void NetworkReadyDelegate(GameObject sender);
        public event NetworkReadyDelegate OnNetworkReady;
        //Ownership
        public bool ClientOwned { get; set; }

        //Events
        public event Action<GameObject> OnUpdateEvent;
        public event Action<GameObject> OnAddedEvent;
        public event Action<GameObject> OnDestroyedEvent;

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
            packetHandler = new PacketHandler<ClientBase>();
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
            if (Parent == null && UID != 0) return;
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
                    childrenChanged = false;
                }
            }

            lock (childModification)
                while (invokationQueue.Count > 0)
                    invokationQueue.Dequeue()();

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
                        networkManager.Add(child);
                    if (multiplayer && !usedToBeMultiplayer) {
                        //TODO fix problem if children have children.
                        foreach (var item in child.GetAllChildren()) {
                            if (!item.multiplayer) {
                                networkManager.Add(item);
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
                    networkManager.Remove(gameObject, sendToOthers);
                    foreach (var item in gameObject.children)
                        item.Remove(false);
                }
                gameObject.Parent = null;
            }
        }

        //Parent is null at random? TODO: FIX
        public void Remove(bool sendToOthers = true) {
            Parent?.RemoveChild(this, sendToOthers);
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

        public void On<PacketType>(PacketHandler<ClientBase>.HandlePacket<PacketType> handlePacket) where PacketType : Packet, new() {
            packetHandler.On(handlePacket);
        }

        internal void Emit(ClientBase sender, byte[] data) {
            packetHandler.Emit(sender, data[0], data, 1);
        }

        public void Invoke(Action action) {
            lock (childModification) {
                invokationQueue.Enqueue(action);
            }
        }

        public void SetPermanentNetworkUID(int UID) {
            this.UID = UID;
            OnRecieveNetworkUID?.Invoke(this, UID);
            OnNetworkReady?.Invoke(this);
        }
    }

}
