using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WUIServer.Components;
using WUIShared;
using WUIShared.Languages;

namespace WUIServer {
    public class WUIGGameLoader {
        private WUIGLanguage lang;
        private Dictionary<string, GameObject> gameObjects;
        private Dictionary<string, object> instanceVariables;
        private readonly GameObject world;

        public WUIGGameLoader(GameObject world) {
            lang = new WUIGLanguage();
            lang.CreateObjectBinder += Lang_CreateObjectBinder;
            lang.ComponentAddBinder += Lang_ComponentAddBinder;
            lang.SetPropertyBinder += Lang_SetPropertyBinder;
            gameObjects = new Dictionary<string, GameObject>();
            instanceVariables = new Dictionary<string, object>();
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
            obj.name = objectName;
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
                    if (gameObject.GetFirst<MouseClickableComponent>() == null)
                        gameObject.AddChild(new MouseClickableComponent());
                    gameObject.AddChild(new DragComponent());
                    break;
                case "followMouse":
                    gameObject.AddChild(new FollowMouse());
                    break;
                default:
                    break;
            }
        }

        private void Lang_SetPropertyBinder(string objectName, string propertyName, string propertyValue) {
            GameObject gameObject = gameObjects[objectName];

            if (propertyName.StartsWith("@")) {
                string variableName = propertyName.Substring(1);
                object val = propertyValue;
                if (int.TryParse(propertyValue, out int intPropertyValue)) val = intPropertyValue;
                instanceVariables[objectName + "@" + variableName] = val;
                return;
            }

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

                        string objName = objectName;
                        string[] lines = propertyValue.Split(',');
                        collider.ContinouslyCheckCollisions = true;
                        collider.OnCollisionStay += Collider_OnCollisionStay;
                        void Collider_OnCollisionStay(Collider sender, Collider other) {
                            for (int i = 0; i < lines.Length; i++) {
                                string code = lines[i].Trim();
                                int idOfFirstNonalpha = code.FindFirstNonAlphanumeric();

                                if (code.StartsWith("remove this"))
                                    sender.Parent.Remove();
                                else if (code.StartsWith("remove other"))
                                    other.Parent.Remove();
                                else if (code[idOfFirstNonalpha] == '@') {
                                    string refersToObject = code.Substring(0, idOfFirstNonalpha);
                                    if (refersToObject == "this" || refersToObject == "")
                                        refersToObject = objName;
                                    else if (refersToObject == "other")
                                        refersToObject = other.Parent.name;

                                    int secondNonalpha = code.FindFirstNonAlphanumeric(idOfFirstNonalpha + 1);
                                    string variableName = code.Substring(idOfFirstNonalpha + 1, secondNonalpha - idOfFirstNonalpha - 1);
                                    string operation = code.Substring(secondNonalpha).Trim();
                                    switch (operation) {
                                        case "++":
                                            if (instanceVariables.ContainsKey(refersToObject + "@" + variableName) && instanceVariables[refersToObject + "@" + variableName] is int num) {
                                                instanceVariables[refersToObject + "@" + variableName] = num + 1;
                                                Console.WriteLine(instanceVariables[refersToObject + "@" + variableName]);

                                            }
                                            break;
                                        default:
                                            break;
                                    }

                                }

                            }
                        }
                    }
                    break;
                default:
                    break;
            }
        }
    }
}
