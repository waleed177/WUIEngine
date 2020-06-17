using LowLevelNetworking.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WUIShared.Objects;

namespace WUIServer {
    class Program {
        public static Server<ClientHandler> server;
        public static PacketBroadcaster broadcaster;
        public static GameObject world;
        internal static NetworkManager networkManager;

        static void Main(string[] args) {
            server = new Server<ClientHandler>("127.0.0.1", 3333, 8388608);
            broadcaster = new PacketBroadcaster(8388608);
            server.Start();
            world = new GameObject(Objects.Empty, false) {
                multiplayer = true
            };
            networkManager = new NetworkManager(world);
            Console.WriteLine("Server started!");
        }
    }
}
