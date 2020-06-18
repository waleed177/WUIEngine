using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WUIClient.Components;
using WUIShared.Objects;

namespace WUIClient {
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

        private bool childrenChanged = false;

        //Multiplayer stuff
        public bool multiplayer = false;
        public delegate void RecievedBytesDelegate(GameObject sender, byte[] bytes, int length);
        private Dictionary<byte, RecievedBytesDelegate> networkBytePacketHandlers;
        private static WUIShared.Packets.ByteArrayUserPacket byteArrayUserPacket = new WUIShared.Packets.ByteArrayUserPacket() { data = new byte[8388608] };

        public delegate void RecievedNetworkUIDDelegate(GameObject sender, int UID);
        public event RecievedNetworkUIDDelegate OnRecieveNetworkUID;

        public delegate void NetworkReadyDelegate(GameObject sender);
        public event NetworkReadyDelegate OnNetworkReady;

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
        }

        private void Destroyed() {
            OnDestroyed();
        }

        public void Update(float deltaTime) {
            OnUpdate(deltaTime);
            if (childrenChanged) {
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

            foreach (var child in children)
                child.Update(deltaTime);
        }

        public void Render(SpriteBatch batch, float deltaTime) {
            OnRender(batch, deltaTime);
            foreach (var child in children)
                child.Render(batch, deltaTime);
        }

        public virtual void OnUpdate(float deltaTime) {

        }

        public virtual void OnRender(SpriteBatch batch, float deltaTime) {

        }

        public virtual void OnAdded() {

        }

        public virtual void OnDestroyed() {

        }


        public void AddChild(GameObject child, bool sendToOthers=  true) {
            childrenChanged = true;
            child.Parent = this;
            bool usedToBeMultiplayer = child.multiplayer;
            child.multiplayer = multiplayer;
            if(sendToOthers) {
                if (multiplayer && !usedToBeMultiplayer)
                    Game1.networkManager.Add(child);
                if (multiplayer && !usedToBeMultiplayer) {
                    foreach (var item in child.GetAllChildren()) {
                        if (!item.multiplayer) {
                            Game1.networkManager.Add(item);
                            item.multiplayer = true;
                        }
                    }
                }
            }
            toBeAdded.Enqueue(child);
        }

        public void RemoveChild(GameObject gameObject, bool sendToOthers = true) {
            childrenChanged = true;
            toBeRemoved.Enqueue(gameObject);
            if (multiplayer && gameObject.Parent != null) {
                Game1.networkManager.Remove(gameObject, sendToOthers);
                foreach (var item in gameObject.children)
                    item.Remove();
            }
            gameObject.Parent = null;
        }

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
           return children.Union(toBeAdded);
        }

        public void Send(byte type, byte[] data, int length) {
            byteArrayUserPacket.data = data;
            byteArrayUserPacket.UID = UID;
            byteArrayUserPacket.type = type;
            byteArrayUserPacket.dataLength = length;
            Game1.client.Send(byteArrayUserPacket);
        }

        public void On(byte type, RecievedBytesDelegate recievedBytesHandler) {
            if (networkBytePacketHandlers.ContainsKey(type))
                networkBytePacketHandlers[type] += recievedBytesHandler;
            else
                networkBytePacketHandlers.Add(type, recievedBytesHandler);
        }

        internal void Emit(byte type, byte[] data, int length) {
            if (networkBytePacketHandlers.ContainsKey(type))
                networkBytePacketHandlers[type]?.Invoke(this, data, length);
        }

        public void SetPermanentNetworkUID(int UID) {
            this.UID = UID;
            OnRecieveNetworkUID?.Invoke(this, UID);
            OnNetworkReady?.Invoke(this);
        }
    }

}
