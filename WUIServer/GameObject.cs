using LowLevelNetworking.Packets;
using LowLevelNetworking.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using WUIServer;
using WUIServer.Components;
using WUIShared.Objects;

namespace WUIShared.Objects {
    public partial class GameObject {
        internal static NetworkManager networkManager;
        //Multiplayer stuff
        public ClientBase Owner { set; get; }

        public void Send(Packet packet) {
            lock (byteArrayUserPacket) {
                packet.SerializeTo(byteArrayUserPacket.data);
                byteArrayUserPacket.dataLength = packet.RawSerializeSize;
                byteArrayUserPacket.UID = UID;
                Program.broadcaster.Broadcast(byteArrayUserPacket);
            }
        }

        public void Send(Packet packet, ClientBase except) {
            lock (byteArrayUserPacket) {
                packet.SerializeTo(byteArrayUserPacket.data);
                byteArrayUserPacket.dataLength = packet.RawSerializeSize;
                byteArrayUserPacket.UID = UID;
                Program.broadcaster.Broadcast(byteArrayUserPacket, except);
            }
        }

        public void Send(ClientBase client, Packet packet) {
            lock (byteArrayUserPacket) {
                packet.SerializeTo(byteArrayUserPacket.data);
                byteArrayUserPacket.dataLength = packet.RawSerializeSize;
                byteArrayUserPacket.UID = UID;
                client.Send(byteArrayUserPacket);
            }
        }

        public Packet GetSpawnPacket() {
            if (Parent == null)
                return null;
            return new WUIShared.Packets.SpawnGameObject() {
                UID = UID,
                parentUID = Parent.UID,
                ObjType = (int)ObjType,
                name = name
            };
        }

        //TODO: THREAD SAFETY.
        public virtual void SendTo(ClientBase client) {
            lock (childModification) {
                Console.WriteLine("Sending Type: " + ObjType + ", UID: " + UID + ", Parent: " + Parent?.UID + " To client " + client.Id );
                Packet packet = GetSpawnPacket();
                if (packet != null)
                    client.Send(packet);
                foreach (var item in GetAllChildren().ToArray())
                    item.SendTo(client);
            }
        }

        public virtual void StringSerialize(StringBuilder stringBuilder, int tabLevel, string nameIfNameIsNull) {
            //if (ObjType != Objects.Empty) throw new Exception("SERIALIZATION NOT IMPLEMENTED FOR " + ObjType);
            if (ObjType != Objects.Empty) return;
            if (tabLevel > 0) throw new NotSupportedException("Non component nesting not implemented yet.");
            //TODO: DONT DO THIS HERE.
            if (name == null)
                name = nameIfNameIsNull;
            stringBuilder.Append(name);
            stringBuilder.Append(":\n");
            foreach (var item in children) {
                //TODO: ADD SUPPORT FOR NON-COMPONENT CHILDREN.
                item.StringSerialize(stringBuilder, tabLevel + 1, null);
            }
            stringBuilder.AppendLine();
        }
    }

}
