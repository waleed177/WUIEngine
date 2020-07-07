using LowLevelNetworking.Server;
using System;
using System.IO;
using System.Threading;
using WUIShared.Objects;

namespace WUIServer {
    class Program {
        public static Server<ClientHandler> server;
        public static PacketBroadcaster broadcaster;
        public static GameObject world;
        internal static NetworkManager networkManager;
        public static WUIGGameLoader gameWorldFile;
        public static WUIGGameSaver gameSaver;
        public static ServerAssetManager assetManager;

        private static Thread timerThread;

        static void Main(string[] args) {
            string[] hostingInfo = File.ReadAllLines("Config.txt");
            server = new Server<ClientHandler>(hostingInfo[0], int.Parse(hostingInfo[1]), 8388608);
            broadcaster = new PacketBroadcaster(8388608);

            world = new GameObject(Objects.Empty, false) {
                multiplayer = true
            };

            networkManager = new NetworkManager(world);
            assetManager = new ServerAssetManager();

            GameObject.networkManager = networkManager;


            gameWorldFile = new WUIGGameLoader(world);
            gameWorldFile.Evaluate(File.ReadAllText(@"GameFile.txt"));

            gameSaver = new WUIGGameSaver(world);

            Console.WriteLine("Server started!");

            server.Start();
            timerThread = new Thread(Timer_Thread);
            timerThread.Start();
        }

        private static void Timer_Thread() {
            while (true) {
                Thread.Sleep(1000/20);
                world.Update(1000 / 20);
            }
        }
    }
}
