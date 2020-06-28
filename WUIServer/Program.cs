using LowLevelNetworking.Server;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using WUIShared;
using WUIShared.Objects;

namespace WUIServer {
    class Program {
        public static Server<ClientHandler> server;
        public static PacketBroadcaster broadcaster;
        public static GameObject world;
        internal static NetworkManager networkManager;
        public static WUIGGameLoader gameWorldFile;
        public static ServerAssetManager assetManager;

        private static Timer timer;

        static void Main(string[] args) {
            server = new Server<ClientHandler>("127.0.0.1", 3333, 8388608);
            broadcaster = new PacketBroadcaster(8388608);
            server.Start();
            world = new GameObject(Objects.Empty, false) {
                multiplayer = true
            };
            networkManager = new NetworkManager(world);
            assetManager = new ServerAssetManager();

            gameWorldFile = new WUIGGameLoader(world);
            gameWorldFile.Evaluate(File.ReadAllText(@"C:\Users\waldohp\source\repos\WUILibrary\GameTest.txt"));
            Console.WriteLine("Server started!");

            timer = new Timer(1000 / 20);
            timer.Elapsed += Timer_Elapsed;
            timer.Start();

        }

        private static void Timer_Elapsed(object sender, ElapsedEventArgs e) {
            //TODO: Properly calculate the elapsed time
            world.Update(0.05f);
        }
    }
}
