using System;
using System.Collections.Generic;
using System.Text;
using WUIShared.Languages;
using WUIShared.Objects;

#if WUIServer
using WUIServer.Math;
#elif WUIClient
using Microsoft.Xna.Framework;
#endif

namespace WUIShared {
    public class WUIWorldActionScript : WUIActionLanguage {
        public Func<string, GameObject> Instantiate;
        private Random random;

        public WUIWorldActionScript() : base() {
            random = new Random();
            GenerateBindings();
        }

        private void GenerateBindings() {
            Bind("print", args => {
                Console.WriteLine(args[0].ToString());
                return null;
            });

            Bind("remove", args => {
                ((GameObject)args[0]).Remove();
                return null;
            });

            Bind("teleport", args => {
                ((GameObject)args[0]).transform.Position = new Vector2((int)args[1], (int)args[2]);
                return null;
            });

            Bind("move", args => {
                ((GameObject)args[0]).transform.Position += new Vector2((int)args[1], (int)args[2]);
                return null;
            });

            Bind("size", args => {
                ((GameObject)args[0]).transform.Size = new Vector2((int)args[1], (int)args[2]);
                return null;
            });

            Bind("inflate", args => {
                ((GameObject)args[0]).transform.Size += new Vector2((int)args[1], (int)args[2]);
                return null;
            });

            Bind("getX", args => {
                return (int)((GameObject)args[0]).transform.Position.X;
            });

            Bind("getY", args => {
                return (int)((GameObject)args[0]).transform.Position.Y;
            });

            Bind("instantiate", args => {
                if (Instantiate == null)
                    throw new NotImplementedException("Instantiation is not implemented");
                GameObject gameObject = Instantiate(args[0].ToString());
                if (args.Length == 3)
                    gameObject.transform.Position = new Vector2(float.Parse(args[1].ToString()), float.Parse(args[2].ToString()));
                return gameObject;
            });

            Bind("random", args => {
                return random.Next((int)args[0], (int)args[1]);
            });

            Bind("sendStringMessage", args => {
                ((GameObject)args[0]).Send(new Packets.ScriptSendString() {
                    message = args[1].ToString()
                });
                return null;
            });
        }
    }
}
