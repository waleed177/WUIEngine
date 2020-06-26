using LowLevelNetworking.Server;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using WUIShared.Objects;

namespace WUIServer {
    class Program {
        public static Server<ClientHandler> server;
        public static PacketBroadcaster broadcaster;
        public static GameObject world;
        internal static NetworkManager networkManager;

        private static Timer timer;

        static void Main(string[] args) {
            server = new Server<ClientHandler>("127.0.0.1", 3333, 8388608);
            broadcaster = new PacketBroadcaster(8388608);
            server.Start();
            world = new GameObject(Objects.Empty, false) {
                multiplayer = true
            };
            networkManager = new NetworkManager(world);

            timer = new Timer(1000 / 5);
            timer.Elapsed += Timer_Elapsed;
            timer.Start();

            new WUIGGameLoader(world).Evaluate(File.ReadAllText(@"C:\Users\waldohp\source\repos\WUILibrary\GameTest.txt"));
            Console.WriteLine("Server started!");
        }

        private static void Timer_Elapsed(object sender, ElapsedEventArgs e) {
            //TODO: Properly calculate the 0.2f (The elapsed time)
            world.Update(0.2f);
        }
    }
}
