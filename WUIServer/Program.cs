using LowLevelNetworking.Server;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WUIShared;
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
            server = new Server<ClientHandler>("127.0.0.1", 3333, 8388608);
            broadcaster = new PacketBroadcaster(8388608);
            server.Start();
            world = new GameObject(Objects.Empty, false) {
                multiplayer = true
            };
            networkManager = new NetworkManager(world);
            assetManager = new ServerAssetManager();

            gameWorldFile = new WUIGGameLoader(world);
            gameWorldFile.Evaluate(File.ReadAllText(@"C:\Users\waldohp\source\repos\WUILibrary\TestGenerated.txt"));

            gameSaver = new WUIGGameSaver(world);

            Console.WriteLine("Server started!");

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
