using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WUIServer.Components;
using WUIShared.Languages;

namespace WUIServer {
    public class WUIGGameLoader {
        private WUIGLanguage lang;
        private Dictionary<string, GameObject> gameObjects;
        private readonly GameObject world;

        public WUIGGameLoader(GameObject world) {
            lang = new WUIGLanguage();
            lang.CreateObjectBinder += Lang_CreateObjectBinder;
            lang.ComponentAddBinder += Lang_ComponentAddBinder;
            lang.SetPropertyBinder += Lang_SetPropertyBinder;
            gameObjects = new Dictionary<string, GameObject>();
            this.world = world;
        }

        public void Evaluate(string code) {
            lang.Evaluate(code);
        }

        private void Lang_CreateObjectBinder(string objectName) {
            if (gameObjects.ContainsKey(objectName))
                throw new Exception($"{objectName} already exists..");
            GameObject obj = new GameObject();
            gameObjects[objectName] = obj;
            world.AddChild(obj);
        }

        private void Lang_ComponentAddBinder(string objectName, string componentName) {
            GameObject gameObject = gameObjects[objectName];

            switch (componentName) {
                case "texture":
                    gameObject.AddChild(new RawTextureRenderer());
                    break;
                case "clickable":
                    gameObject.AddChild(new MouseClickableComponent());
                    break;
                case "draggable":
                    if(gameObject.GetFirst<MouseClickableComponent>() == null)
                        gameObject.AddChild(new MouseClickableComponent());
                    gameObject.AddChild(new DragComponent());
                    break;
                default:
                    break;
            }
        }

        private void Lang_SetPropertyBinder(string objectName, string propertyName, string propertyValue) {
            Console.WriteLine(objectName + "$" + propertyName + "$" + propertyValue);
            switch (propertyName) {
                case "texture": {
                        RawTextureRenderer tex = gameObjects[objectName].GetFirst<RawTextureRenderer>();
                        if (tex == null)
                            gameObjects[objectName].AddChild(tex = new RawTextureRenderer());
                        tex.texture = new Texture2D(File.ReadAllBytes(propertyValue));
                    }
                    break;
                case "size": {
                        string[] sp = propertyValue.Split(' ');
                        gameObjects[objectName].transform.Size = new Math.Vector2(int.Parse(sp[0]), int.Parse(sp[1]));
                    }
                    break;
                case "position": {
                        string[] sp = propertyValue.Split(' ');
                        gameObjects[objectName].transform.Position = new Math.Vector2(int.Parse(sp[0]), int.Parse(sp[1]));
                    }
                    break;
                default:
                    break;
            }
        }
    }
}
