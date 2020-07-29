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
            Bind("This", args => GetVariable(new string[] { "this" }));

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

            #region ArrayInt
            //TODO: Create syntax sugar for these.

            Bind("ArrayIntNew", args => {
                return new List<int>();
            });

            Bind("ArrayIntAddRepeat", args => {
                List<int> list = ((List<int>)args[0]);
                for (int i = 0; i < (int)args[1]; i++)
                    list.Add((int)args[2]);
                return null;
            });

            Bind("ArrayIntAdd", args => {
                ((List<int>)args[0]).Add((int)args[1]);
                return null;
            });

            Bind("ArrayIntSet", args => {
                return ((List<int>)args[0])[(int)args[1]] = (int)args[2];
            });

            Bind("ArrayIntGet", args => {
                return ((List<int>)args[0])[(int)args[1]];
            });

            Bind("ArrayIntClear", args => {
                ((List<int>)args[0]).Clear();
                return null;
            });

            Bind("ArrayIntRemove", args => {
                ((List<int>)args[0]).Remove((int)args[1]);
                return null;
            });

            Bind("ArrayIntRemoveAt", args => {
                ((List<int>)args[0]).RemoveAt((int)args[1]);
                return null;
            });

            Bind("ArrayIntCount", args => ((List<int>)args[0]).Count);
            Bind("ArrayIntIndexOf", args => ((List<int>)args[0]).IndexOf((int)args[1]));

            #endregion
            #region ArrayObj
            //TODO: Create syntax sugar for these.

            Bind("ArrayObjNew", args => {
                return new List<object>();
            });

            Bind("ArrayObjAdd", args => {
                ((List<object>)args[0]).Add(args[1]);
                return null;
            });

            Bind("ArrayObjAddRepeat", args => {
                List<object> list = ((List<object>)args[0]);
                for (int i = 0; i < (int)args[1]; i++)
                    list.Add(args[2]);
                return null;
            });

            Bind("ArrayObjSet", args => {
                return ((List<object>)args[0])[(int)args[1]] = args[2];
            });

            Bind("ArrayObjGet", args => {
                return ((List<object>)args[0])[(int)args[1]];
            });

            Bind("ArrayObjClear", args => {
                ((List<object>)args[0]).Clear();
                return null;
            });

            Bind("ArrayObjRemove", args => {
                ((List<object>)args[0]).Remove(args[1]);
                return null;
            });

            Bind("ArrayObjRemoveAt", args => {
                ((List<object>)args[0]).RemoveAt((int)args[1]);
                return null;
            });

            Bind("ArrayObjCount", args => ((List<object>)args[0]).Count);
            Bind("ArrayObjIndexOf", args => ((List<object>)args[0]).IndexOf(args[1]));
            #endregion
            #region "String"
            Bind("StringConcat", args => {
                string res = "";
                for (int i = 0; i < args.Length; i++)
                    res += args[i].ToString();
                return res;
            });
            Bind("StringSub", args => ((string)args[0]).Substring((int)args[0], (int)args[1]));
            Bind("StringSplit", args => new List<object>(((string)args[0]).Split(new string[] { (string)args[1] }, StringSplitOptions.None)));
            #endregion
            #region "Casting"
            Bind("Int", args => int.Parse((string)args[0]));
            #endregion
            #region "Instance Functions"
            Bind("InstanceFunctionBind", args => {
                SetVariable(new string[] { (string)args[0] }, (Action) args[1]);
                return null;
            });
            #endregion
        }
    }
}
