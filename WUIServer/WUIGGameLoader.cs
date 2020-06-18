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
                case "boxCollider":
                    gameObject.AddChild(new BoxCollider());
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
            GameObject gameObject = gameObjects[objectName];
            switch (propertyName) {
                case "texture": {
                        RawTextureRenderer tex = gameObject.GetFirst<RawTextureRenderer>();
                        if (tex == null)
                            gameObject.AddChild(tex = new RawTextureRenderer());
                        tex.texture = new Texture2D(File.ReadAllBytes(propertyValue));
                    }
                    break;
                case "size": {
                        string[] sp = propertyValue.Split(' ');
                        gameObject.transform.Size = new Math.Vector2(int.Parse(sp[0]), int.Parse(sp[1]));
                    }
                    break;
                case "position": {
                        string[] sp = propertyValue.Split(' ');
                        gameObject.transform.Position = new Math.Vector2(int.Parse(sp[0]), int.Parse(sp[1]));
                    }
                    break;
                case "onCollisionStay": {
                        Collider collider = gameObject.GetFirst<Collider>();
                        if (collider == null)
                            gameObject.AddChild(collider = new BoxCollider());

                        string code = propertyValue;
                        collider.ContinouslyCheckCollisions = true;
                        collider.OnCollisionStay += Collider_OnCollisionStay;
                        void Collider_OnCollisionStay(Collider sender, Collider other) {
                            //temp
                            if (code == "remove this")
                                gameObject.Parent.RemoveChild(gameObject);
                        }
                    }
                    break;
                default:
                    break;
            }
        }
    }
}
