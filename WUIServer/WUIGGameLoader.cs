﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WUIServer.Components;
using WUIShared;
using WUIShared.Languages;

namespace WUIServer {
    public class WUIGGameLoader {
        private WUIGLanguage lang;
        private Dictionary<string, GameObject> gameObjects;
        private Dictionary<string, object> instanceVariables;
        private Dictionary<string, Action> instantiateInstructions;

        private List<string> imagesDirectory = new List<string>();
        private readonly GameObject world;
        private int tempObjectId = 0;
        private string PlayerObject = "Player";
        WUIActionLanguage ActionScript;

        public WUIGGameLoader(GameObject world) {
            ActionScript = new WUIActionLanguage();
            lang = new WUIGLanguage();
            lang.CreateObjectBinder += Lang_CreateObjectBinder;
            lang.ComponentAddBinder += Lang_ComponentAddBinder;
            lang.SetPropertyBinder += Lang_SetPropertyBinder;
            gameObjects = new Dictionary<string, GameObject>();
            instanceVariables = new Dictionary<string, object>();
            instantiateInstructions = new Dictionary<string, Action>();
            Random random = new Random();
            this.world = world;

            ActionScript.Bind("print", args => {
                Console.WriteLine(args[0].ToString());
                return null;
            });

            ActionScript.Bind("remove", args => {
                ((GameObject)args[0]).Remove();
                return null;
            });

            ActionScript.Bind("teleport", args => {
                ((GameObject)args[0]).transform.Position = new Math.Vector2((int)args[1], (int)args[2]);
                return null;
            });

            ActionScript.Bind("move", args => {
                ((GameObject)args[0]).transform.Position += new Math.Vector2((int)args[1], (int)args[2]);
                return null;
            });

            ActionScript.Bind("size", args => {
                ((GameObject)args[0]).transform.Size = new Math.Vector2((int)args[1], (int)args[2]);
                return null;
            });

            ActionScript.Bind("inflate", args => {
                ((GameObject)args[0]).transform.Size += new Math.Vector2((int)args[1], (int)args[2]);
                return null;
            });

            ActionScript.Bind("getX", args => {
                return (int)((GameObject)args[0]).transform.Position.X;
            });

            ActionScript.Bind("getY", args => {
                return (int)((GameObject)args[0]).transform.Position.Y;
            });

            ActionScript.Bind("instantiate", args => {
                GameObject gameObject = Instantiate(args[0].ToString());
                if (args.Length == 3)
                    gameObject.transform.Position = new Math.Vector2(float.Parse(args[1].ToString()), float.Parse(args[2].ToString()));
                return gameObject;
            });

            ActionScript.Bind("random", args => {
                return random.Next((int)args[0], (int)args[1]);
            });
        }

        public GameObject InstantiatePlayer() {
            return Instantiate(PlayerObject);
        }

        public GameObject Instantiate(string name) {
            GameObject gameObject;
            lock (instantiateInstructions) {
                instantiateInstructions[name]();
                gameObject = gameObjects["$$TEMP$$" + tempObjectId];
                tempObjectId++;
            }
            return gameObject;
        }

        public void Evaluate(string code) {
            lang.Evaluate(code);
        }

        private void Lang_CreateObjectBinder(string objectName) {
            if (objectName != "$$TEMP$$" + tempObjectId && gameObjects.ContainsKey(objectName))
                throw new Exception($"{objectName} already exists..");
            if (objectName != "$$TEMP$$" + tempObjectId)
                instantiateInstructions[objectName] = () => Lang_CreateObjectBinder("$$TEMP$$" + tempObjectId);
            if (objectName == PlayerObject) return;

            GameObject obj = new GameObject();
            gameObjects[objectName] = obj;
            obj.name = objectName;
            ActionScript.SetVariable(new string[] { objectName }, new Dictionary<string, object>());
            ActionScript.SetVariable(new string[] { objectName, "object" }, obj);
            world.AddChild(obj);

            string _objectName = objectName;
            obj.OnDestroyedEvent += obj_OnDestroyedEvent;

            void obj_OnDestroyedEvent(GameObject sender) {
                ActionScript.RemoveVariable(new string[] { _objectName });
                gameObjects.Remove(_objectName);
            }
        }

        private void Lang_ComponentAddBinder(string objectName, string componentName) {
            if (objectName != "$$TEMP$$" + tempObjectId)
                instantiateInstructions[objectName] += () => Lang_ComponentAddBinder("$$TEMP$$" + tempObjectId, componentName);
            if (objectName == PlayerObject) return;

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
                case "topDownPlayer":
                    gameObject.AddChild(new PlayerController());
                    break;
                case "camera":
                    gameObject.AddChild(new CameraComponent());
                    break;
                default:
                    break;
            }
        }

        private void Lang_SetPropertyBinder(string objectName, string propertyName, string propertyValue) {
            if (objectName == null) {
                if (propertyName == "images") {
                    imagesDirectory.Add(propertyValue);
                } else if (propertyName == "player") {
                    PlayerObject = propertyValue;
                }
                return;
            }

            if (objectName != "$$TEMP$$" + tempObjectId)
                instantiateInstructions[objectName] += () => Lang_SetPropertyBinder("$$TEMP$$" + tempObjectId, propertyName, propertyValue);

            if (objectName == PlayerObject) return;

            GameObject gameObject = gameObjects[objectName];

            if (propertyName.StartsWith(".")) {
                string variableName = propertyName.Substring(1);
                object val = propertyValue;
                if (int.TryParse(propertyValue, out int intPropertyValue)) val = intPropertyValue;
                ActionScript.SetVariable(new string[] { objectName, variableName }, val);
            } else switch (propertyName) {
                    case "texture": {
                            RawTextureRenderer tex = gameObject.GetFirst<RawTextureRenderer>();
                            if (tex == null)
                                gameObject.AddChild(tex = new RawTextureRenderer());
                            if (File.Exists(propertyValue)) {
                                //TODO: MOVE THIS CODE ELSEWHERE
                                tex.texture = new Texture2D(File.ReadAllBytes(propertyValue));
                                tex.texture.name = Path.GetFileName(propertyValue);
                                Program.assetManager.AddAsset(tex.texture.name, tex.texture.bytes);

                            } else foreach (var item in imagesDirectory)
                                    if (File.Exists(item + propertyValue)) {
                                        //TODO: MOVE THIS CODE ELSEWHERE
                                        tex.texture = new Texture2D(File.ReadAllBytes(item + propertyValue));
                                        tex.texture.name = Path.GetFileName(item + propertyValue);
                                        Program.assetManager.AddAsset(tex.texture.name, tex.texture.bytes);
                                        break;
                                    }
                        }
                        break;
                    case "player-speed": {
                            PlayerController plr = gameObject.GetFirst<PlayerController>();
                            if (plr == null)
                                gameObject.AddChild(plr = new PlayerController());
                            string[] sp = propertyValue.Split(' ');
                            plr.HorizontalSpeed = int.Parse(sp[0]);
                            plr.VerticalSpeed = int.Parse(sp[1]);
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
                    case "camera-follow": {
                            CameraComponent camera = gameObject.GetFirst<CameraComponent>();
                            if (camera == null)
                                gameObject.AddChild(camera = new CameraComponent());
                            string[] sp = propertyValue.Split(' ');
                            if (sp[0] == "localPlayer")
                                camera.FollowLocalPlayer = true;
                        }
                        break;
                    case "onCollisionStay": {
                            Collider collider = gameObject.GetFirst<Collider>();
                            if (collider == null)
                                gameObject.AddChild(collider = new BoxCollider());

                            string objName = objectName;
                            ActionScript.LoadCode(";\n" + propertyValue); //the extra semicolon is to fix a bug where an if statement  doesnt work if it was the first statement, TODO: this should be fixed properly.....
                            Action func = ActionScript.Compile();

                            collider.ContinouslyCheckCollisions = true;
                            collider.OnCollisionStay += Collider_OnCollisionStay;


                            void Collider_OnCollisionStay(Collider sender, Collider other) {
                                if (sender.Parent == null || other.Parent == null) return;
                                ActionScript.SetVariable(new string[] { "other" }, ActionScript.GetVariable(new string[] { other.Parent.name }));
                                ActionScript.SetVariable(new string[] { "this" }, ActionScript.GetVariable(new string[] { sender.Parent.name }));
                                func();
                            }
                        }
                        break;
                    case "onUpdate": {
                            ActionScript.LoadCode(";\n" + propertyValue); //the extra semicolon is to fix a bug where an if statement  doesnt work if it was the first statement, TODO: this should be fixed properly.....
                            Action func = ActionScript.Compile();
                            gameObject.OnUpdateEvent += Update;

                            void Update(GameObject sender) {
                                ActionScript.SetVariable(new string[] { "this" }, ActionScript.GetVariable(new string[] { gameObject.name }));
                                func();
                            }
                        }
                        break;
                    case "onLoad": {
                            ActionScript.LoadCode(";\n" + propertyValue); //the extra semicolon is to fix a bug where an if statement  doesnt work if it was the first statement, TODO: this should be fixed properly.....
                            Action func = ActionScript.Compile();
                            gameObject.OnAddedEvent += OnAdded;

                            void OnAdded(GameObject sender) {
                                ActionScript.SetVariable(new string[] { "this" }, ActionScript.GetVariable(new string[] { gameObject.name }));
                                func();
                            }
                        }
                        break;
                    default:
                        break;
                }
        }
    }
}
