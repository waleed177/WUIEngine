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
        TransformPositionSet,
        RawTextureRendererTextureSet,
        RawTextureRendererRotationSet,
        AssetSend,
        ByteArrayUserPacket,
        FloatArrayUserPacket,
        IntArrayUserPacket,
        StringArrayUserPacket,
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
        public override int Size { get => +4 + 4 + 4; }

        public override int RawSerializeSize => Size + 5;
        public int ObjType;
        public int UID;
        public int parentUID;

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
            return start;
        }
        public override int DeserializeFrom(byte[] arr, int start = 0) {
            ObjType = arr[start++] << 0 | arr[start++] << 8 | arr[start++] << 16 | arr[start++] << 24;
            UID = arr[start++] << 0 | arr[start++] << 8 | arr[start++] << 16 | arr[start++] << 24;
            parentUID = arr[start++] << 0 | arr[start++] << 8 | arr[start++] << 16 | arr[start++] << 24;
            return start;
        }
        public override string ToString() => $"int ObjType = {ObjType}\nint UID = {UID}\nint parentUID = {parentUID}\n";
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

    public class TransformPositionSet : Packet {
        public override int PacketType { get; } = (int)PacketTypes.TransformPositionSet;
        public override int Size { get => +4 + 4 + 4 + 4; }

        public override int RawSerializeSize => Size + 5;
        public int UID;
        public float x;
        public float y;
        public float z;

        public TransformPositionSet() {
        }
        public TransformPositionSet(byte[] arr, int start = 0) {
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
            BinConversion.GetBytes(arr, start, x);
            start += 4;
            BinConversion.GetBytes(arr, start, y);
            start += 4;
            BinConversion.GetBytes(arr, start, z);
            start += 4;
            return start;
        }
        public override int DeserializeFrom(byte[] arr, int start = 0) {
            UID = arr[start++] << 0 | arr[start++] << 8 | arr[start++] << 16 | arr[start++] << 24;
            BinConversion.GetFloat(arr, start, out x);
            start += 4;
            BinConversion.GetFloat(arr, start, out y);
            start += 4;
            BinConversion.GetFloat(arr, start, out z);
            start += 4;
            return start;
        }
        public override string ToString() => $"int UID = {UID}\nfloat x = {x}\nfloat y = {y}\nfloat z = {z}\n";
    }

    public class RawTextureRendererTextureSet : Packet {
        public override int PacketType { get; } = (int)PacketTypes.RawTextureRendererTextureSet;
        public override int Size { get => +4 + 4 + 4 + 4 + 4; }

        public override int RawSerializeSize => Size + 5;
        public int UID;
        public int assetUID;
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
                arr[start++] = (byte)(assetUID >> 0);
                arr[start++] = (byte)(assetUID >> 8);
                arr[start++] = (byte)(assetUID >> 16);
                arr[start++] = (byte)(assetUID >> 24);
            }
            BinConversion.GetBytes(arr, start, r);
            start += 4;
            BinConversion.GetBytes(arr, start, g);
            start += 4;
            BinConversion.GetBytes(arr, start, b);
            start += 4;
            return start;
        }
        public override int DeserializeFrom(byte[] arr, int start = 0) {
            UID = arr[start++] << 0 | arr[start++] << 8 | arr[start++] << 16 | arr[start++] << 24;
            assetUID = arr[start++] << 0 | arr[start++] << 8 | arr[start++] << 16 | arr[start++] << 24;
            BinConversion.GetFloat(arr, start, out r);
            start += 4;
            BinConversion.GetFloat(arr, start, out g);
            start += 4;
            BinConversion.GetFloat(arr, start, out b);
            start += 4;
            return start;
        }
        public override string ToString() => $"int UID = {UID}\nint assetUID = {assetUID}\nfloat r = {r}\nfloat g = {g}\nfloat b = {b}\n";
    }

    public class RawTextureRendererRotationSet : Packet {
        public override int PacketType { get; } = (int)PacketTypes.RawTextureRendererRotationSet;
        public override int Size { get => +4 + 4; }

        public override int RawSerializeSize => Size + 5;
        public int UID;
        public float rotation;

        public RawTextureRendererRotationSet() {
        }
        public RawTextureRendererRotationSet(byte[] arr, int start = 0) {
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
            BinConversion.GetBytes(arr, start, rotation);
            start += 4;
            return start;
        }
        public override int DeserializeFrom(byte[] arr, int start = 0) {
            UID = arr[start++] << 0 | arr[start++] << 8 | arr[start++] << 16 | arr[start++] << 24;
            BinConversion.GetFloat(arr, start, out rotation);
            start += 4;
            return start;
        }
        public override string ToString() => $"int UID = {UID}\nfloat rotation = {rotation}\n";
    }

    public class AssetSend : Packet {
        public override int PacketType { get; } = (int)PacketTypes.AssetSend;
        public override int Size { get => +4 + 4 + asset.Length * +1; }

        public override int RawSerializeSize => Size + 5;
        public int assetUID;
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
                arr[start++] = (byte)(assetUID >> 0);
                arr[start++] = (byte)(assetUID >> 8);
                arr[start++] = (byte)(assetUID >> 16);
                arr[start++] = (byte)(assetUID >> 24);
            }
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
            assetUID = arr[start++] << 0 | arr[start++] << 8 | arr[start++] << 16 | arr[start++] << 24;
            {
                int length = arr[start++] << 0 | arr[start++] << 8 | arr[start++] << 16 | arr[start++] << 24;
                for (int i = 0; i < asset.Length; i++) {
                    asset[i] = arr[start++];
                }
            }
            return start;
        }
        public override string ToString() => $"int assetUID = {assetUID}\nbyte[] asset = {asset}\n";
    }

    public class ByteArrayUserPacket : Packet {
        public override int PacketType { get; } = (int)PacketTypes.ByteArrayUserPacket;
        public override int Size { get => +4 + 1 + 4 + dataLength * +1; }

        public override int RawSerializeSize => Size + 5;
        public int UID;
        public byte type;
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
            arr[start++] = type; unchecked {
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
            type = arr[start++];
            dataLength = arr[start++] << 0 | arr[start++] << 8 | arr[start++] << 16 | arr[start++] << 24;
            for (int i = 0; i < dataLength; i++) {
                data[i] = arr[start++];
            }
            return start;
        }
        public override string ToString() => $"int UID = {UID}\nbyte type = {type}\nbyte[p8388608] data = {data}\n";
    }

    public class FloatArrayUserPacket : Packet {
        public override int PacketType { get; } = (int)PacketTypes.FloatArrayUserPacket;
        public override int Size { get => +4 + 1 + 4 + dataLength * +4; }

        public override int RawSerializeSize => Size + 5;
        public int UID;
        public byte type;
        public float[] data;
        public int dataLength;

        public FloatArrayUserPacket() {
            data = new float[8];
        }
        public FloatArrayUserPacket(byte[] arr, int start = 0) {
            data = new float[8];
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
            arr[start++] = type; unchecked {
                arr[start++] = (byte)(dataLength >> 0);
                arr[start++] = (byte)(dataLength >> 8);
                arr[start++] = (byte)(dataLength >> 16);
                arr[start++] = (byte)(dataLength >> 24);
            }
            for (int i = 0; i < dataLength; i++) {
                BinConversion.GetBytes(arr, start, data[i]);
                start += 4;
            }
            return start;
        }
        public override int DeserializeFrom(byte[] arr, int start = 0) {
            UID = arr[start++] << 0 | arr[start++] << 8 | arr[start++] << 16 | arr[start++] << 24;
            type = arr[start++];
            dataLength = arr[start++] << 0 | arr[start++] << 8 | arr[start++] << 16 | arr[start++] << 24;
            for (int i = 0; i < dataLength; i++) {
                BinConversion.GetFloat(arr, start, out data[i]);
                start += 4;
            }
            return start;
        }
        public override string ToString() => $"int UID = {UID}\nbyte type = {type}\nfloat[p8] data = {data}\n";
    }

    public class IntArrayUserPacket : Packet {
        public override int PacketType { get; } = (int)PacketTypes.IntArrayUserPacket;
        public override int Size { get => +4 + 1 + 4 + dataLength * +4; }

        public override int RawSerializeSize => Size + 5;
        public int UID;
        public byte type;
        public int[] data;
        public int dataLength;

        public IntArrayUserPacket() {
            data = new int[8];
        }
        public IntArrayUserPacket(byte[] arr, int start = 0) {
            data = new int[8];
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
            arr[start++] = type; unchecked {
                arr[start++] = (byte)(dataLength >> 0);
                arr[start++] = (byte)(dataLength >> 8);
                arr[start++] = (byte)(dataLength >> 16);
                arr[start++] = (byte)(dataLength >> 24);
            }
            for (int i = 0; i < dataLength; i++) {
                unchecked {
                    arr[start++] = (byte)(data[i] >> 0);
                    arr[start++] = (byte)(data[i] >> 8);
                    arr[start++] = (byte)(data[i] >> 16);
                    arr[start++] = (byte)(data[i] >> 24);
                }
            }
            return start;
        }
        public override int DeserializeFrom(byte[] arr, int start = 0) {
            UID = arr[start++] << 0 | arr[start++] << 8 | arr[start++] << 16 | arr[start++] << 24;
            type = arr[start++];
            dataLength = arr[start++] << 0 | arr[start++] << 8 | arr[start++] << 16 | arr[start++] << 24;
            for (int i = 0; i < dataLength; i++) {
                data[i] = arr[start++] << 0 | arr[start++] << 8 | arr[start++] << 16 | arr[start++] << 24;
            }
            return start;
        }
        public override string ToString() => $"int UID = {UID}\nbyte type = {type}\nint[p8] data = {data}\n";
    }

    public class StringArrayUserPacket : Packet {
        public override int PacketType { get; } = (int)PacketTypes.StringArrayUserPacket;
        public override int Size { get => +4 + 1 + 4 + data.Sum((x) => x != null ? (4 + x.Length) : 0); }

        public override int RawSerializeSize => Size + 5;
        public int UID;
        public byte type;
        public string[] data;
        public int dataLength;

        public StringArrayUserPacket() {
            data = new string[3];
        }
        public StringArrayUserPacket(byte[] arr, int start = 0) {
            data = new string[3];
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
            arr[start++] = type; unchecked {
                arr[start++] = (byte)(dataLength >> 0);
                arr[start++] = (byte)(dataLength >> 8);
                arr[start++] = (byte)(dataLength >> 16);
                arr[start++] = (byte)(dataLength >> 24);
            }
            for (int i = 0; i < dataLength; i++) {
                unchecked {
                    arr[start++] = (byte)(data[i].Length >> 0);
                    arr[start++] = (byte)(data[i].Length >> 8);
                    arr[start++] = (byte)(data[i].Length >> 16);
                    arr[start++] = (byte)(data[i].Length >> 24);
                }
                Encoding.ASCII.GetBytes(data[i], 0, data[i].Length, arr, start);
                start += data[i].Length;
            }
            return start;
        }
        public override int DeserializeFrom(byte[] arr, int start = 0) {
            UID = arr[start++] << 0 | arr[start++] << 8 | arr[start++] << 16 | arr[start++] << 24;
            type = arr[start++];
            dataLength = arr[start++] << 0 | arr[start++] << 8 | arr[start++] << 16 | arr[start++] << 24;
            for (int i = 0; i < dataLength; i++) {
                {
                    int strlen = arr[start++] << 0 | arr[start++] << 8 | arr[start++] << 16 | arr[start++] << 24;
                    data[i] = ASCIIEncoding.ASCII.GetString(arr, start, strlen);
                    start += strlen;
                }
            }
            return start;
        }
        public override string ToString() => $"int UID = {UID}\nbyte type = {type}\nstring[p3] data = {data}\n";
    }


}
