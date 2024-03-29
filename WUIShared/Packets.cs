using LowLevelNetworking.Shared;
using System;
using System.Text;
using System.Linq;
using BinaryConversions;

namespace WUIShared.Packets {

    public enum PacketTypes {
        PlayerJoined,
        PlayerLeft,
        SpawnGameObject,
        ChangeGameObjectUID,
        FreeTempUID,
        DestroyGameObject,
        SetParentOfGameObject,
        OwnershipPacket,
        AssetSend,
        TransformPositionSet,
        TransformSizeSet,
        RawTextureRendererTextureSet,
        RawTextureRendererRotationSet,
        PlayerSpeedSet,
        MovingObjectClientCollision,
        CameraSetFollow,
        SendLocalScripts,
        ScriptSendString,
        ByteArrayUserPacket,
        SaveWorldPacket,
    }

    public class PlayerJoined : Packet {
        public override int PacketType { get; } = (int)PacketTypes.PlayerJoined;
        public override int Size { get => +4 + 4 + 4 + username.Length; }

        public override int RawSerializeSize => Size + 5;
        public int PID;
        public int UID;
        public string username;

        public PlayerJoined() {
        }
        public PlayerJoined(byte[] arr, int start = 0) {
            DeserializeFrom(arr, start);
        }
        public override int SerializeTo(byte[] arr, int start = 0) {
            arr[start++] = (byte)PacketType;
            unchecked {
                arr[start++] = (byte)(Size >> 0);
                arr[start++] = (byte)(Size >> 8);
                arr[start++] = (byte)(Size >> 16);
                arr[start++] = (byte)(Size >> 24);
            }
            unchecked {
                arr[start++] = (byte)(PID >> 0);
                arr[start++] = (byte)(PID >> 8);
                arr[start++] = (byte)(PID >> 16);
                arr[start++] = (byte)(PID >> 24);
            }
            unchecked {
                arr[start++] = (byte)(UID >> 0);
                arr[start++] = (byte)(UID >> 8);
                arr[start++] = (byte)(UID >> 16);
                arr[start++] = (byte)(UID >> 24);
            }
            unchecked {
                arr[start++] = (byte)(username.Length >> 0);
                arr[start++] = (byte)(username.Length >> 8);
                arr[start++] = (byte)(username.Length >> 16);
                arr[start++] = (byte)(username.Length >> 24);
            }
            Encoding.ASCII.GetBytes(username, 0, username.Length, arr, start);
            start += username.Length;
            return start;
        }
        public override int DeserializeFrom(byte[] arr, int start = 0) {
            PID = arr[start++] << 0 | arr[start++] << 8 | arr[start++] << 16 | arr[start++] << 24;
            UID = arr[start++] << 0 | arr[start++] << 8 | arr[start++] << 16 | arr[start++] << 24;
            {
                int strlen = arr[start++] << 0 | arr[start++] << 8 | arr[start++] << 16 | arr[start++] << 24;
                username = ASCIIEncoding.ASCII.GetString(arr, start, strlen);
                start += strlen;
            }
            return start;
        }
        public override string ToString() => $"int PID = {PID}\nint UID = {UID}\nstring username = {username}\n";
    }

    public class PlayerLeft : Packet {
        public override int PacketType { get; } = (int)PacketTypes.PlayerLeft;
        public override int Size { get => +4; }

        public override int RawSerializeSize => Size + 5;
        public int PID;

        public PlayerLeft() {
        }
        public PlayerLeft(byte[] arr, int start = 0) {
            DeserializeFrom(arr, start);
        }
        public override int SerializeTo(byte[] arr, int start = 0) {
            arr[start++] = (byte)PacketType;
            unchecked {
                arr[start++] = (byte)(Size >> 0);
                arr[start++] = (byte)(Size >> 8);
                arr[start++] = (byte)(Size >> 16);
                arr[start++] = (byte)(Size >> 24);
            }
            unchecked {
                arr[start++] = (byte)(PID >> 0);
                arr[start++] = (byte)(PID >> 8);
                arr[start++] = (byte)(PID >> 16);
                arr[start++] = (byte)(PID >> 24);
            }
            return start;
        }
        public override int DeserializeFrom(byte[] arr, int start = 0) {
            PID = arr[start++] << 0 | arr[start++] << 8 | arr[start++] << 16 | arr[start++] << 24;
            return start;
        }
        public override string ToString() => $"int PID = {PID}\n";
    }

    public class SpawnGameObject : Packet {
        public override int PacketType { get; } = (int)PacketTypes.SpawnGameObject;
        public override int Size { get => +4 + 4 + 4 + 4 + name.Length; }

        public override int RawSerializeSize => Size + 5;
        public int ObjType;
        public int UID;
        public int parentUID;
        public string name;

        public SpawnGameObject() {
        }
        public SpawnGameObject(byte[] arr, int start = 0) {
            DeserializeFrom(arr, start);
        }
        public override int SerializeTo(byte[] arr, int start = 0) {
            arr[start++] = (byte)PacketType;
            unchecked {
                arr[start++] = (byte)(Size >> 0);
                arr[start++] = (byte)(Size >> 8);
                arr[start++] = (byte)(Size >> 16);
                arr[start++] = (byte)(Size >> 24);
            }
            unchecked {
                arr[start++] = (byte)(ObjType >> 0);
                arr[start++] = (byte)(ObjType >> 8);
                arr[start++] = (byte)(ObjType >> 16);
                arr[start++] = (byte)(ObjType >> 24);
            }
            unchecked {
                arr[start++] = (byte)(UID >> 0);
                arr[start++] = (byte)(UID >> 8);
                arr[start++] = (byte)(UID >> 16);
                arr[start++] = (byte)(UID >> 24);
            }
            unchecked {
                arr[start++] = (byte)(parentUID >> 0);
                arr[start++] = (byte)(parentUID >> 8);
                arr[start++] = (byte)(parentUID >> 16);
                arr[start++] = (byte)(parentUID >> 24);
            }
            unchecked {
                arr[start++] = (byte)(name.Length >> 0);
                arr[start++] = (byte)(name.Length >> 8);
                arr[start++] = (byte)(name.Length >> 16);
                arr[start++] = (byte)(name.Length >> 24);
            }
            Encoding.ASCII.GetBytes(name, 0, name.Length, arr, start);
            start += name.Length;
            return start;
        }
        public override int DeserializeFrom(byte[] arr, int start = 0) {
            ObjType = arr[start++] << 0 | arr[start++] << 8 | arr[start++] << 16 | arr[start++] << 24;
            UID = arr[start++] << 0 | arr[start++] << 8 | arr[start++] << 16 | arr[start++] << 24;
            parentUID = arr[start++] << 0 | arr[start++] << 8 | arr[start++] << 16 | arr[start++] << 24;
            {
                int strlen = arr[start++] << 0 | arr[start++] << 8 | arr[start++] << 16 | arr[start++] << 24;
                name = ASCIIEncoding.ASCII.GetString(arr, start, strlen);
                start += strlen;
            }
            return start;
        }
        public override string ToString() => $"int ObjType = {ObjType}\nint UID = {UID}\nint parentUID = {parentUID}\nstring name = {name}\n";
    }

    public class ChangeGameObjectUID : Packet {
        public override int PacketType { get; } = (int)PacketTypes.ChangeGameObjectUID;
        public override int Size { get => +4 + 4; }

        public override int RawSerializeSize => Size + 5;
        public int oldUID;
        public int newUID;

        public ChangeGameObjectUID() {
        }
        public ChangeGameObjectUID(byte[] arr, int start = 0) {
            DeserializeFrom(arr, start);
        }
        public override int SerializeTo(byte[] arr, int start = 0) {
            arr[start++] = (byte)PacketType;
            unchecked {
                arr[start++] = (byte)(Size >> 0);
                arr[start++] = (byte)(Size >> 8);
                arr[start++] = (byte)(Size >> 16);
                arr[start++] = (byte)(Size >> 24);
            }
            unchecked {
                arr[start++] = (byte)(oldUID >> 0);
                arr[start++] = (byte)(oldUID >> 8);
                arr[start++] = (byte)(oldUID >> 16);
                arr[start++] = (byte)(oldUID >> 24);
            }
            unchecked {
                arr[start++] = (byte)(newUID >> 0);
                arr[start++] = (byte)(newUID >> 8);
                arr[start++] = (byte)(newUID >> 16);
                arr[start++] = (byte)(newUID >> 24);
            }
            return start;
        }
        public override int DeserializeFrom(byte[] arr, int start = 0) {
            oldUID = arr[start++] << 0 | arr[start++] << 8 | arr[start++] << 16 | arr[start++] << 24;
            newUID = arr[start++] << 0 | arr[start++] << 8 | arr[start++] << 16 | arr[start++] << 24;
            return start;
        }
        public override string ToString() => $"int oldUID = {oldUID}\nint newUID = {newUID}\n";
    }

    public class FreeTempUID : Packet {
        public override int PacketType { get; } = (int)PacketTypes.FreeTempUID;
        public override int Size { get => +4; }

        public override int RawSerializeSize => Size + 5;
        public int UID;

        public FreeTempUID() {
        }
        public FreeTempUID(byte[] arr, int start = 0) {
            DeserializeFrom(arr, start);
        }
        public override int SerializeTo(byte[] arr, int start = 0) {
            arr[start++] = (byte)PacketType;
            unchecked {
                arr[start++] = (byte)(Size >> 0);
                arr[start++] = (byte)(Size >> 8);
                arr[start++] = (byte)(Size >> 16);
                arr[start++] = (byte)(Size >> 24);
            }
            unchecked {
                arr[start++] = (byte)(UID >> 0);
                arr[start++] = (byte)(UID >> 8);
                arr[start++] = (byte)(UID >> 16);
                arr[start++] = (byte)(UID >> 24);
            }
            return start;
        }
        public override int DeserializeFrom(byte[] arr, int start = 0) {
            UID = arr[start++] << 0 | arr[start++] << 8 | arr[start++] << 16 | arr[start++] << 24;
            return start;
        }
        public override string ToString() => $"int UID = {UID}\n";
    }

    public class DestroyGameObject : Packet {
        public override int PacketType { get; } = (int)PacketTypes.DestroyGameObject;
        public override int Size { get => +4; }

        public override int RawSerializeSize => Size + 5;
        public int UID;

        public DestroyGameObject() {
        }
        public DestroyGameObject(byte[] arr, int start = 0) {
            DeserializeFrom(arr, start);
        }
        public override int SerializeTo(byte[] arr, int start = 0) {
            arr[start++] = (byte)PacketType;
            unchecked {
                arr[start++] = (byte)(Size >> 0);
                arr[start++] = (byte)(Size >> 8);
                arr[start++] = (byte)(Size >> 16);
                arr[start++] = (byte)(Size >> 24);
            }
            unchecked {
                arr[start++] = (byte)(UID >> 0);
                arr[start++] = (byte)(UID >> 8);
                arr[start++] = (byte)(UID >> 16);
                arr[start++] = (byte)(UID >> 24);
            }
            return start;
        }
        public override int DeserializeFrom(byte[] arr, int start = 0) {
            UID = arr[start++] << 0 | arr[start++] << 8 | arr[start++] << 16 | arr[start++] << 24;
            return start;
        }
        public override string ToString() => $"int UID = {UID}\n";
    }

    public class SetParentOfGameObject : Packet {
        public override int PacketType { get; } = (int)PacketTypes.SetParentOfGameObject;
        public override int Size { get => +4 + 4; }

        public override int RawSerializeSize => Size + 5;
        public int UID;
        public int newParentUID;

        public SetParentOfGameObject() {
        }
        public SetParentOfGameObject(byte[] arr, int start = 0) {
            DeserializeFrom(arr, start);
        }
        public override int SerializeTo(byte[] arr, int start = 0) {
            arr[start++] = (byte)PacketType;
            unchecked {
                arr[start++] = (byte)(Size >> 0);
                arr[start++] = (byte)(Size >> 8);
                arr[start++] = (byte)(Size >> 16);
                arr[start++] = (byte)(Size >> 24);
            }
            unchecked {
                arr[start++] = (byte)(UID >> 0);
                arr[start++] = (byte)(UID >> 8);
                arr[start++] = (byte)(UID >> 16);
                arr[start++] = (byte)(UID >> 24);
            }
            unchecked {
                arr[start++] = (byte)(newParentUID >> 0);
                arr[start++] = (byte)(newParentUID >> 8);
                arr[start++] = (byte)(newParentUID >> 16);
                arr[start++] = (byte)(newParentUID >> 24);
            }
            return start;
        }
        public override int DeserializeFrom(byte[] arr, int start = 0) {
            UID = arr[start++] << 0 | arr[start++] << 8 | arr[start++] << 16 | arr[start++] << 24;
            newParentUID = arr[start++] << 0 | arr[start++] << 8 | arr[start++] << 16 | arr[start++] << 24;
            return start;
        }
        public override string ToString() => $"int UID = {UID}\nint newParentUID = {newParentUID}\n";
    }

    public class OwnershipPacket : Packet {
        public override int PacketType { get; } = (int)PacketTypes.OwnershipPacket;
        public override int Size { get => +4 + 1; }

        public override int RawSerializeSize => Size + 5;
        public int UID;
        public bool Owned;

        public OwnershipPacket() {
        }
        public OwnershipPacket(byte[] arr, int start = 0) {
            DeserializeFrom(arr, start);
        }
        public override int SerializeTo(byte[] arr, int start = 0) {
            arr[start++] = (byte)PacketType;
            unchecked {
                arr[start++] = (byte)(Size >> 0);
                arr[start++] = (byte)(Size >> 8);
                arr[start++] = (byte)(Size >> 16);
                arr[start++] = (byte)(Size >> 24);
            }
            unchecked {
                arr[start++] = (byte)(UID >> 0);
                arr[start++] = (byte)(UID >> 8);
                arr[start++] = (byte)(UID >> 16);
                arr[start++] = (byte)(UID >> 24);
            }
            arr[start++] = (byte)(Owned ? 1 : 0);
            return start;
        }
        public override int DeserializeFrom(byte[] arr, int start = 0) {
            UID = arr[start++] << 0 | arr[start++] << 8 | arr[start++] << 16 | arr[start++] << 24;
            Owned = arr[start++] == 1;
            return start;
        }
        public override string ToString() => $"int UID = {UID}\nbool Owned = {Owned}\n";
    }

    public class AssetSend : Packet {
        public override int PacketType { get; } = (int)PacketTypes.AssetSend;
        public override int Size { get => +4 + assetName.Length + 4 + asset.Length * +1; }

        public override int RawSerializeSize => Size + 5;
        public string assetName;
        public byte[] asset;

        public AssetSend() {
        }
        public AssetSend(byte[] arr, int start = 0) {
            DeserializeFrom(arr, start);
        }
        public override int SerializeTo(byte[] arr, int start = 0) {
            arr[start++] = (byte)PacketType;
            unchecked {
                arr[start++] = (byte)(Size >> 0);
                arr[start++] = (byte)(Size >> 8);
                arr[start++] = (byte)(Size >> 16);
                arr[start++] = (byte)(Size >> 24);
            }
            unchecked {
                arr[start++] = (byte)(assetName.Length >> 0);
                arr[start++] = (byte)(assetName.Length >> 8);
                arr[start++] = (byte)(assetName.Length >> 16);
                arr[start++] = (byte)(assetName.Length >> 24);
            }
            Encoding.ASCII.GetBytes(assetName, 0, assetName.Length, arr, start);
            start += assetName.Length;
            unchecked {
                arr[start++] = (byte)(asset.Length >> 0);
                arr[start++] = (byte)(asset.Length >> 8);
                arr[start++] = (byte)(asset.Length >> 16);
                arr[start++] = (byte)(asset.Length >> 24);
            }
            for (int i = 0; i < asset.Length; i++) { arr[start++] = asset[i]; }
            return start;
        }
        public override int DeserializeFrom(byte[] arr, int start = 0) {
            {
                int strlen = arr[start++] << 0 | arr[start++] << 8 | arr[start++] << 16 | arr[start++] << 24;
                assetName = ASCIIEncoding.ASCII.GetString(arr, start, strlen);
                start += strlen;
            }
            {
                int length = arr[start++] << 0 | arr[start++] << 8 | arr[start++] << 16 | arr[start++] << 24;
                asset = new byte[length];
                for (int i = 0; i < asset.Length; i++) {
                    asset[i] = arr[start++];
                }
            }
            return start;
        }
        public override string ToString() => $"string assetName = {assetName}\nbyte[] asset = {asset}\n";
    }

    public class TransformPositionSet : Packet {
        public override int PacketType { get; } = (int)PacketTypes.TransformPositionSet;
        public override int Size { get => +4 + 4; }

        public override int RawSerializeSize => Size + 1;
        public float x;
        public float y;

        public TransformPositionSet() {
        }
        public TransformPositionSet(byte[] arr, int start = 0) {
            DeserializeFrom(arr, start);
        }
        public override int SerializeTo(byte[] arr, int start = 0) {
            arr[start++] = (byte)PacketType;
            BinConversion.GetBytes(arr, start, x);
            start += 4;
            BinConversion.GetBytes(arr, start, y);
            start += 4;
            return start;
        }
        public override int DeserializeFrom(byte[] arr, int start = 0) {
            BinConversion.GetFloat(arr, start, out x);
            start += 4;
            BinConversion.GetFloat(arr, start, out y);
            start += 4;
            return start;
        }
        public override string ToString() => $"float x = {x}\nfloat y = {y}\n";
    }

    public class TransformSizeSet : Packet {
        public override int PacketType { get; } = (int)PacketTypes.TransformSizeSet;
        public override int Size { get => +4 + 4; }

        public override int RawSerializeSize => Size + 1;
        public float x;
        public float y;

        public TransformSizeSet() {
        }
        public TransformSizeSet(byte[] arr, int start = 0) {
            DeserializeFrom(arr, start);
        }
        public override int SerializeTo(byte[] arr, int start = 0) {
            arr[start++] = (byte)PacketType;
            BinConversion.GetBytes(arr, start, x);
            start += 4;
            BinConversion.GetBytes(arr, start, y);
            start += 4;
            return start;
        }
        public override int DeserializeFrom(byte[] arr, int start = 0) {
            BinConversion.GetFloat(arr, start, out x);
            start += 4;
            BinConversion.GetFloat(arr, start, out y);
            start += 4;
            return start;
        }
        public override string ToString() => $"float x = {x}\nfloat y = {y}\n";
    }

    public class RawTextureRendererTextureSet : Packet {
        public override int PacketType { get; } = (int)PacketTypes.RawTextureRendererTextureSet;
        public override int Size { get => +4 + assetName.Length + 4 + 4 + 4; }

        public override int RawSerializeSize => Size + 1;
        public string assetName;
        public float r;
        public float g;
        public float b;

        public RawTextureRendererTextureSet() {
        }
        public RawTextureRendererTextureSet(byte[] arr, int start = 0) {
            DeserializeFrom(arr, start);
        }
        public override int SerializeTo(byte[] arr, int start = 0) {
            arr[start++] = (byte)PacketType;
            unchecked {
                arr[start++] = (byte)(assetName.Length >> 0);
                arr[start++] = (byte)(assetName.Length >> 8);
                arr[start++] = (byte)(assetName.Length >> 16);
                arr[start++] = (byte)(assetName.Length >> 24);
            }
            Encoding.ASCII.GetBytes(assetName, 0, assetName.Length, arr, start);
            start += assetName.Length;
            BinConversion.GetBytes(arr, start, r);
            start += 4;
            BinConversion.GetBytes(arr, start, g);
            start += 4;
            BinConversion.GetBytes(arr, start, b);
            start += 4;
            return start;
        }
        public override int DeserializeFrom(byte[] arr, int start = 0) {
            {
                int strlen = arr[start++] << 0 | arr[start++] << 8 | arr[start++] << 16 | arr[start++] << 24;
                assetName = ASCIIEncoding.ASCII.GetString(arr, start, strlen);
                start += strlen;
            }
            BinConversion.GetFloat(arr, start, out r);
            start += 4;
            BinConversion.GetFloat(arr, start, out g);
            start += 4;
            BinConversion.GetFloat(arr, start, out b);
            start += 4;
            return start;
        }
        public override string ToString() => $"string assetName = {assetName}\nfloat r = {r}\nfloat g = {g}\nfloat b = {b}\n";
    }

    public class RawTextureRendererRotationSet : Packet {
        public override int PacketType { get; } = (int)PacketTypes.RawTextureRendererRotationSet;
        public override int Size { get => +4; }

        public override int RawSerializeSize => Size + 1;
        public float rotation;

        public RawTextureRendererRotationSet() {
        }
        public RawTextureRendererRotationSet(byte[] arr, int start = 0) {
            DeserializeFrom(arr, start);
        }
        public override int SerializeTo(byte[] arr, int start = 0) {
            arr[start++] = (byte)PacketType;
            BinConversion.GetBytes(arr, start, rotation);
            start += 4;
            return start;
        }
        public override int DeserializeFrom(byte[] arr, int start = 0) {
            BinConversion.GetFloat(arr, start, out rotation);
            start += 4;
            return start;
        }
        public override string ToString() => $"float rotation = {rotation}\n";
    }

    public class PlayerSpeedSet : Packet {
        public override int PacketType { get; } = (int)PacketTypes.PlayerSpeedSet;
        public override int Size { get => +4 + 4; }

        public override int RawSerializeSize => Size + 1;
        public float speedX;
        public float speedY;

        public PlayerSpeedSet() {
        }
        public PlayerSpeedSet(byte[] arr, int start = 0) {
            DeserializeFrom(arr, start);
        }
        public override int SerializeTo(byte[] arr, int start = 0) {
            arr[start++] = (byte)PacketType;
            BinConversion.GetBytes(arr, start, speedX);
            start += 4;
            BinConversion.GetBytes(arr, start, speedY);
            start += 4;
            return start;
        }
        public override int DeserializeFrom(byte[] arr, int start = 0) {
            BinConversion.GetFloat(arr, start, out speedX);
            start += 4;
            BinConversion.GetFloat(arr, start, out speedY);
            start += 4;
            return start;
        }
        public override string ToString() => $"float speedX = {speedX}\nfloat speedY = {speedY}\n";
    }

    public class MovingObjectClientCollision : Packet {
        public override int PacketType { get; } = (int)PacketTypes.MovingObjectClientCollision;
        public override int Size { get => +4 + uidsLength * +4; }

        public override int RawSerializeSize => Size + 1;
        public int[] uids;
        public int uidsLength;

        public MovingObjectClientCollision() {
            uids = new int[25];
        }
        public MovingObjectClientCollision(byte[] arr, int start = 0) {
            uids = new int[25];
            DeserializeFrom(arr, start);
        }
        public override int SerializeTo(byte[] arr, int start = 0) {
            arr[start++] = (byte)PacketType;
            unchecked {
                arr[start++] = (byte)(uidsLength >> 0);
                arr[start++] = (byte)(uidsLength >> 8);
                arr[start++] = (byte)(uidsLength >> 16);
                arr[start++] = (byte)(uidsLength >> 24);
            }
            for (int i = 0; i < uidsLength; i++) {
                unchecked {
                    arr[start++] = (byte)(uids[i] >> 0);
                    arr[start++] = (byte)(uids[i] >> 8);
                    arr[start++] = (byte)(uids[i] >> 16);
                    arr[start++] = (byte)(uids[i] >> 24);
                }
            }
            return start;
        }
        public override int DeserializeFrom(byte[] arr, int start = 0) {
            uidsLength = arr[start++] << 0 | arr[start++] << 8 | arr[start++] << 16 | arr[start++] << 24;
            for (int i = 0; i < uidsLength; i++) {
                uids[i] = arr[start++] << 0 | arr[start++] << 8 | arr[start++] << 16 | arr[start++] << 24;
            }
            return start;
        }
        public override string ToString() => $"int[p25] uids = {uids}\n";
    }

    public class CameraSetFollow : Packet {
        public override int PacketType { get; } = (int)PacketTypes.CameraSetFollow;
        public override int Size { get => +1 + 1 + 4; }

        public override int RawSerializeSize => Size + 1;
        public bool followLocalPlayer;
        public bool followEnabled;
        public int followUID;

        public CameraSetFollow() {
        }
        public CameraSetFollow(byte[] arr, int start = 0) {
            DeserializeFrom(arr, start);
        }
        public override int SerializeTo(byte[] arr, int start = 0) {
            arr[start++] = (byte)PacketType;
            arr[start++] = (byte)(followLocalPlayer ? 1 : 0);
            arr[start++] = (byte)(followEnabled ? 1 : 0);
            unchecked {
                arr[start++] = (byte)(followUID >> 0);
                arr[start++] = (byte)(followUID >> 8);
                arr[start++] = (byte)(followUID >> 16);
                arr[start++] = (byte)(followUID >> 24);
            }
            return start;
        }
        public override int DeserializeFrom(byte[] arr, int start = 0) {
            followLocalPlayer = arr[start++] == 1;
            followEnabled = arr[start++] == 1;
            followUID = arr[start++] << 0 | arr[start++] << 8 | arr[start++] << 16 | arr[start++] << 24;
            return start;
        }
        public override string ToString() => $"bool followLocalPlayer = {followLocalPlayer}\nbool followEnabled = {followEnabled}\nint followUID = {followUID}\n";
    }

    public class SendLocalScripts : Packet {
        public override int PacketType { get; } = (int)PacketTypes.SendLocalScripts;
        public override int Size { get => +4 + eventId.Length * +4 + 4 + code.Sum((x) => x != null ? (4 + x.Length) : 0); }

        public override int RawSerializeSize => Size + 1;
        public int[] eventId;
        public string[] code;

        public SendLocalScripts() {
        }
        public SendLocalScripts(byte[] arr, int start = 0) {
            DeserializeFrom(arr, start);
        }
        public override int SerializeTo(byte[] arr, int start = 0) {
            arr[start++] = (byte)PacketType;
            unchecked {
                arr[start++] = (byte)(eventId.Length >> 0);
                arr[start++] = (byte)(eventId.Length >> 8);
                arr[start++] = (byte)(eventId.Length >> 16);
                arr[start++] = (byte)(eventId.Length >> 24);
            }
            for (int i = 0; i < eventId.Length; i++) {
                unchecked {
                    arr[start++] = (byte)(eventId[i] >> 0);
                    arr[start++] = (byte)(eventId[i] >> 8);
                    arr[start++] = (byte)(eventId[i] >> 16);
                    arr[start++] = (byte)(eventId[i] >> 24);
                }
            }
            unchecked {
                arr[start++] = (byte)(code.Length >> 0);
                arr[start++] = (byte)(code.Length >> 8);
                arr[start++] = (byte)(code.Length >> 16);
                arr[start++] = (byte)(code.Length >> 24);
            }
            for (int i = 0; i < code.Length; i++) {
                unchecked {
                    arr[start++] = (byte)(code[i].Length >> 0);
                    arr[start++] = (byte)(code[i].Length >> 8);
                    arr[start++] = (byte)(code[i].Length >> 16);
                    arr[start++] = (byte)(code[i].Length >> 24);
                }
                Encoding.ASCII.GetBytes(code[i], 0, code[i].Length, arr, start);
                start += code[i].Length;
            }
            return start;
        }
        public override int DeserializeFrom(byte[] arr, int start = 0) {
            {
                int length = arr[start++] << 0 | arr[start++] << 8 | arr[start++] << 16 | arr[start++] << 24;
                eventId = new int[length];
                for (int i = 0; i < eventId.Length; i++) {
                    eventId[i] = arr[start++] << 0 | arr[start++] << 8 | arr[start++] << 16 | arr[start++] << 24;
                }
            }
            {
                int length = arr[start++] << 0 | arr[start++] << 8 | arr[start++] << 16 | arr[start++] << 24;
                code = new string[length];
                for (int i = 0; i < code.Length; i++) {
                    {
                        int strlen = arr[start++] << 0 | arr[start++] << 8 | arr[start++] << 16 | arr[start++] << 24;
                        code[i] = ASCIIEncoding.ASCII.GetString(arr, start, strlen);
                        start += strlen;
                    }
                }
            }
            return start;
        }
        public override string ToString() => $"int[] eventId = {eventId}\nstring[] code = {code}\n";
    }

    public class ScriptSendString : Packet {
        public override int PacketType { get; } = (int)PacketTypes.ScriptSendString;
        public override int Size { get => +4 + message.Length; }

        public override int RawSerializeSize => Size + 1;
        public string message;

        public ScriptSendString() {
        }
        public ScriptSendString(byte[] arr, int start = 0) {
            DeserializeFrom(arr, start);
        }
        public override int SerializeTo(byte[] arr, int start = 0) {
            arr[start++] = (byte)PacketType;
            unchecked {
                arr[start++] = (byte)(message.Length >> 0);
                arr[start++] = (byte)(message.Length >> 8);
                arr[start++] = (byte)(message.Length >> 16);
                arr[start++] = (byte)(message.Length >> 24);
            }
            Encoding.ASCII.GetBytes(message, 0, message.Length, arr, start);
            start += message.Length;
            return start;
        }
        public override int DeserializeFrom(byte[] arr, int start = 0) {
            {
                int strlen = arr[start++] << 0 | arr[start++] << 8 | arr[start++] << 16 | arr[start++] << 24;
                message = ASCIIEncoding.ASCII.GetString(arr, start, strlen);
                start += strlen;
            }
            return start;
        }
        public override string ToString() => $"string message = {message}\n";
    }

    public class ByteArrayUserPacket : Packet {
        public override int PacketType { get; } = (int)PacketTypes.ByteArrayUserPacket;
        public override int Size { get => +4 + 4 + dataLength * +1; }

        public override int RawSerializeSize => Size + 5;
        public int UID;
        public byte[] data;
        public int dataLength;

        public ByteArrayUserPacket() {
            data = new byte[8388608];
        }
        public ByteArrayUserPacket(byte[] arr, int start = 0) {
            data = new byte[8388608];
            DeserializeFrom(arr, start);
        }
        public override int SerializeTo(byte[] arr, int start = 0) {
            arr[start++] = (byte)PacketType;
            unchecked {
                arr[start++] = (byte)(Size >> 0);
                arr[start++] = (byte)(Size >> 8);
                arr[start++] = (byte)(Size >> 16);
                arr[start++] = (byte)(Size >> 24);
            }
            unchecked {
                arr[start++] = (byte)(UID >> 0);
                arr[start++] = (byte)(UID >> 8);
                arr[start++] = (byte)(UID >> 16);
                arr[start++] = (byte)(UID >> 24);
            }
            unchecked {
                arr[start++] = (byte)(dataLength >> 0);
                arr[start++] = (byte)(dataLength >> 8);
                arr[start++] = (byte)(dataLength >> 16);
                arr[start++] = (byte)(dataLength >> 24);
            }
            for (int i = 0; i < dataLength; i++) { arr[start++] = data[i]; }
            return start;
        }
        public override int DeserializeFrom(byte[] arr, int start = 0) {
            UID = arr[start++] << 0 | arr[start++] << 8 | arr[start++] << 16 | arr[start++] << 24;
            dataLength = arr[start++] << 0 | arr[start++] << 8 | arr[start++] << 16 | arr[start++] << 24;
            for (int i = 0; i < dataLength; i++) {
                data[i] = arr[start++];
            }
            return start;
        }
        public override string ToString() => $"int UID = {UID}\nbyte[p8388608] data = {data}\n";
    }

    public class SaveWorldPacket : Packet {
        public override int PacketType { get; } = (int)PacketTypes.SaveWorldPacket;
        public override int Size { get => +4 + name.Length; }

        public override int RawSerializeSize => Size + 5;
        public string name;

        public SaveWorldPacket() {
        }
        public SaveWorldPacket(byte[] arr, int start = 0) {
            DeserializeFrom(arr, start);
        }
        public override int SerializeTo(byte[] arr, int start = 0) {
            arr[start++] = (byte)PacketType;
            unchecked {
                arr[start++] = (byte)(Size >> 0);
                arr[start++] = (byte)(Size >> 8);
                arr[start++] = (byte)(Size >> 16);
                arr[start++] = (byte)(Size >> 24);
            }
            unchecked {
                arr[start++] = (byte)(name.Length >> 0);
                arr[start++] = (byte)(name.Length >> 8);
                arr[start++] = (byte)(name.Length >> 16);
                arr[start++] = (byte)(name.Length >> 24);
            }
            Encoding.ASCII.GetBytes(name, 0, name.Length, arr, start);
            start += name.Length;
            return start;
        }
        public override int DeserializeFrom(byte[] arr, int start = 0) {
            {
                int strlen = arr[start++] << 0 | arr[start++] << 8 | arr[start++] << 16 | arr[start++] << 24;
                name = ASCIIEncoding.ASCII.GetString(arr, start, strlen);
                start += strlen;
            }
            return start;
        }
        public override string ToString() => $"string name = {name}\n";
    }


}
