using System;
using System.Collections.Generic;
using System.Text;
using WUIShared.Languages;
using WUIShared.Objects;

#if WUIServer
using WUIServer.Math;
using WUIServer.Components;
#elif WUIClient
using Microsoft.Xna.Framework;
using WUIClient.Components;
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
            Bind("Print", args => {
                Console.WriteLine(args[0].ToString());
                return null;
            });

            Bind("Remove", args => {
                ((GameObject)args[0]).Remove();
                return null;
            });

            Bind("PositionSet", args => {
                ((GameObject)args[0]).transform.Position = new Vector2((int)args[1], (int)args[2]);
                return null;
            });

            Bind("PositionRelativeSet", args => {
                ((GameObject)args[0]).transform.Position += new Vector2((int)args[1], (int)args[2]);
                return null;
            });

            Bind("SizeSet", args => {
                ((GameObject)args[0]).transform.Size = new Vector2((int)args[1], (int)args[2]);
                return null;
            });

            Bind("SizeGetX", args => {
                return (int)((GameObject)args[0]).transform.Size.X;
            });

            Bind("SizeGetY", args => {
                return (int)((GameObject)args[0]).transform.Size.Y;
            });

            Bind("Inflate", args => {
                ((GameObject)args[0]).transform.Size += new Vector2((int)args[1], (int)args[2]);
                return null;
            });

            Bind("GetX", args => {
                return (int)((GameObject)args[0]).transform.Position.X;
            });

            Bind("GetY", args => {
                return (int)((GameObject)args[0]).transform.Position.Y;
            });

            Bind("Instantiate", args => {
                if (Instantiate == null)
                    throw new NotImplementedException("Instantiation is not implemented");
                GameObject gameObject = Instantiate(args[0].ToString());
                if (args.Length == 3)
                    gameObject.transform.Position = new Vector2(float.Parse(args[1].ToString()), float.Parse(args[2].ToString()));
                return GetVariable(new string[] { gameObject.name });
            });

            Bind("Random", args => {
                return random.Next((int)args[0], (int)args[1]);
            });

            Bind("MessageStringSend", args => {
                ((GameObject)args[0]).Send(new Packets.ScriptSendString() {
                    message = args[1].ToString()
                });
                return null;
            });

            Bind("UITextSet", args => {
                ((UIText)args[0]).Text = args[1].ToString();
                return null;
            });

            Bind("UITextGet", args => {
                return ((UIText)args[0]).Text;
            });

            Bind("ComponentGet", args => {
                return ((GameObject)args[0]).GetFirst(args[1].ToString());
            });
        }
    }
}
