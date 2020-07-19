using System;
using System.Collections.Generic;
using System.IO;
using LowLevelNetworking.Shared;
using WUIServer.Components;
using WUIShared;
using WUIShared.Languages;
using WUIShared.Objects;
using WUIShared.Packets;

namespace WUIServer {
    public class WUIGGameLoader {
        private WUIGLanguage lang;
        private Dictionary<string, GameObject> gameObjects;
        private Dictionary<string, object> instanceVariables;
        private Dictionary<string, Action> instantiateInstructions;

        private HashSet<string> dontSpawnList;

        private List<string> imagesDirectory = new List<string>();
        private readonly GameObject world;
        private int tempObjectId = 0;
        private string PlayerObject = "Player";
        WUIWorldActionScript ActionScript;
        private ClientBase.NoParametersDelegate ClientHandler_OnDisconnect;
        private ClientBase.NoParametersDelegate ClientHandler_OnStart;

        public WUIGGameLoader(GameObject world) {
            ActionScript = new WUIWorldActionScript();
            ActionScript.Instantiate = Instantiate;
            lang = new WUIGLanguage();
            lang.CreateObjectBinder += Lang_CreateObjectBinder;
            lang.ComponentAddBinder += Lang_ComponentAddBinder;
            lang.SetPropertyBinder += Lang_SetPropertyBinder;
            lang.EndOfObjectBinder += Lang_EndOfObjectBinder;
            gameObjects = new Dictionary<string, GameObject>();
            instanceVariables = new Dictionary<string, object>();
            instantiateInstructions = new Dictionary<string, Action>();
            dontSpawnList = new HashSet<string>();

            //Server specific bindings
            ActionScript.Bind("MessageStringSendTo", args => {
                ((GameObject)args[0]).Send((ClientBase)args[1], new ScriptSendString() {
                    message = args[2].ToString()
                });
                return null;
            });

            this.world = world;
        }


        public GameObject InstantiatePlayer() {
            return Instantiate(PlayerObject);
        }

        private GameObject Instantiate(string name) {
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
            dontSpawnList.Clear();
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

            lock (ActionScript) {
                ActionScript.SetVariable(new string[] { objectName }, new Dictionary<string, object>());
                ActionScript.SetVariable(new string[] { objectName, "object" }, obj);
            }


            string _objectName = objectName;
            obj.OnDestroyedEvent += obj_OnDestroyedEvent;

            void obj_OnDestroyedEvent(GameObject sender) {
                ActionScript.RemoveVariable(new string[] { _objectName });
                gameObjects.Remove(_objectName);
            }
        }

        private void Lang_EndOfObjectBinder(string objectName) {
            if (objectName != "$$TEMP$$" + tempObjectId) {
                instantiateInstructions[objectName] += () => Lang_EndOfObjectBinder("$$TEMP$$" + tempObjectId);
                if (dontSpawnList.Contains(objectName)) {
                    ActionScript.RemoveVariable(new string[] { objectName });
                    gameObjects.Remove(objectName);
                }
            }

            if (gameObjects.ContainsKey(objectName)) //Because not every object will be added to the world, some will be types (to be implemented).
                world.AddChild(gameObjects[objectName]);
        }

        private void Lang_ComponentAddBinder(string objectName, string componentName) {
            if (objectName != "$$TEMP$$" + tempObjectId) {
                instantiateInstructions[objectName] += () => Lang_ComponentAddBinder("$$TEMP$$" + tempObjectId, componentName);
                if (dontSpawnList.Contains(objectName) || objectName == PlayerObject) return;
            }

            GameObject gameObject = gameObjects[objectName];

            switch (componentName) {
                case "dontSpawn": {
                        //TODO: Add a check to make sure this is topmost.
                        if (!objectName.StartsWith("$$TEMP$$"))
                            dontSpawnList.Add(objectName);
                    }
                    break;
                case "draggable":
                    if (gameObject.GetFirst<MouseClickableComponent>() == null)
                        gameObject.AddChild(new MouseClickableComponent());
                    gameObject.AddChild(new DragComponent());
                    break;
                default:
                    ObjectInstantiator.Instantiate(componentName);
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

            if (objectName != "$$TEMP$$" + tempObjectId) {
                instantiateInstructions[objectName] += () => Lang_SetPropertyBinder("$$TEMP$$" + tempObjectId, propertyName, propertyValue);
                if (dontSpawnList.Contains(objectName) || objectName == PlayerObject) return;
            }
            GameObject gameObject = gameObjects[objectName];

            if (propertyName.StartsWith(".")) {
                string variableName = propertyName.Substring(1);
                object val = propertyValue;
                if (int.TryParse(propertyValue, out int intPropertyValue)) val = intPropertyValue;
                lock (ActionScript) ActionScript.SetVariable(new string[] { objectName, variableName }, val);
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
                    case "gridSpawn": {
                            world.Invoke(GridSpawn);

                            void GridSpawn() {
                                string[] lines = propertyValue.Split('\n');
                                float sizeX = 0, sizeY = 0;
                                float offsetX = 0, offsetY = 0;
                                int curY = 0;
                                int state = 0; //0 properties //1 tiles //2 map

                                Dictionary<char, string> tileName = new Dictionary<char, string>();

                                foreach (string line in lines) {
                                    if (line.Trim() == "") continue;
                                    string[] sp = line.Split(' ');
                                    switch (state) {
                                        case 0: //Reading properties
                                            if (sp[0] == "size") {
                                                sizeX = float.Parse(sp[1]);
                                                sizeY = float.Parse(sp[2]);
                                            } else if (sp[0] == "offset") {
                                                offsetX = float.Parse(sp[1]);
                                                offsetY = float.Parse(sp[2]);
                                            } else if (sp[0] == "tiles") {
                                                state = 1;
                                            }
                                            break;
                                        case 1: //Reading tiles
                                            if (sp[0] == "map") {
                                                state = 2;
                                            } else {
                                                if (sp[0].Length > 1) throw new NotSupportedException("You cannot name your tile with more than one letter.");
                                                tileName[sp[0][0]] = sp[1];
                                            }
                                            break;
                                        case 2: //Reading map
                                            for (int i = 0; i < line.Length; i++) {
                                                char currentTileId = line[i];
                                                if (currentTileId == '0') continue;
                                                GameObject go = Instantiate(tileName[currentTileId]);
                                                go.transform.Position = new Math.Vector2(offsetX + i * sizeX, offsetY + curY * sizeY);
                                            }
                                            curY++;
                                            break;
                                        default:
                                            break;
                                    }
                                }
                            }
                        }
                        break;
                    case "UIText": {
                            UIText obj = gameObject.GetFirst<UIText>();
                            if (obj == null)
                                gameObject.AddChild(obj = new UIText());
                            obj.Text = propertyValue;
                        }
                        break;
                    case "onClientJoin": {
                            Action func;
                            lock (ActionScript) {
                                ActionScript.LoadCode(";\n" + propertyValue); //the extra semicolon is to fix a bug where an if statement  doesnt work if it was the first statement, TODO: this should be fixed properly.....
                                func = ActionScript.Compile();
                            }
                            ClientHandler_OnStart += OnStart;

                            void OnStart(ClientBase client) {
                                world.Invoke(OnStartInvoked);

                                void OnStartInvoked() {
                                    lock (ActionScript) {
                                        ActionScript.SetVariable(new string[] { "args" }, new Dictionary<string, object>() { { "client", client } });
                                        ActionScript.SetVariable(new string[] { "this" }, ActionScript.GetVariable(new string[] { gameObject.name }));
                                        func();
                                    }
                                    ClientHandler_OnStart -= OnStart;
                                }
                            }
                        }
                        break;
                    case "onClientLeave": {
                            Action func;
                            lock (ActionScript) {
                                ActionScript.LoadCode(";\n" + propertyValue); //the extra semicolon is to fix a bug where an if statement  doesnt work if it was the first statement, TODO: this should be fixed properly.....
                                func = ActionScript.Compile();
                            }
                            ClientHandler_OnDisconnect += OnDisconnect;

                            void OnDisconnect(ClientBase client) {
                                world.Invoke(OnDisconnectInvoked);

                                void OnDisconnectInvoked() {
                                    lock (ActionScript) {
                                        ActionScript.SetVariable(new string[] { "args" }, new Dictionary<string, object>() { { "client", client } });
                                        ActionScript.SetVariable(new string[] { "this" }, ActionScript.GetVariable(new string[] { gameObject.name }));
                                        func();
                                    }
                                    ClientHandler_OnDisconnect -= OnDisconnect;
                                }
                            }
                        }
                        break;
                    case "onCollisionStay": {
                            Collider collider = gameObject.GetFirst<Collider>();
                            if (collider == null)
                                gameObject.AddChild(collider = new BoxCollider());

                            string objName = objectName;
                            Action func;
                            lock (ActionScript) {
                                ActionScript.LoadCode(";\n" + propertyValue); //the extra semicolon is to fix a bug where an if statement  doesnt work if it was the first statement, TODO: this should be fixed properly.....
                                func = ActionScript.Compile();
                            }
                            collider.ContinouslyCheckCollisions = true;
                            collider.OnCollisionStay += Collider_OnCollisionStay;


                            void Collider_OnCollisionStay(Collider sender, Collider other) {
                                if (sender.Parent == null || other.Parent == null) return;
                                lock (ActionScript) {
                                    ActionScript.SetVariable(new string[] { "other" }, ActionScript.GetVariable(new string[] { other.Parent.name }));
                                    ActionScript.SetVariable(new string[] { "this" }, ActionScript.GetVariable(new string[] { sender.Parent.name }));
                                    func();
                                }
                            }
                        }
                        break;
                    case "onUpdate": {
                            Action func;
                            lock (ActionScript) {
                                ActionScript.LoadCode(";\n" + propertyValue); //the extra semicolon is to fix a bug where an if statement  doesnt work if it was the first statement, TODO: this should be fixed properly.....
                                func = ActionScript.Compile();
                            }
                            gameObject.OnUpdateEvent += Update;

                            void Update(GameObject sender) {
                                lock (ActionScript) {
                                    ActionScript.SetVariable(new string[] { "this" }, ActionScript.GetVariable(new string[] { gameObject.name }));
                                    func();
                                }
                            }
                        }
                        break;
                    case "onLoad": {
                            Action func;
                            lock (ActionScript) {
                                ActionScript.LoadCode(";\n" + propertyValue); //the extra semicolon is to fix a bug where an if statement  doesnt work if it was the first statement, TODO: this should be fixed properly.....
                                func = ActionScript.Compile();
                            }
                            gameObject.OnAddedEvent += OnAdded;

                            void OnAdded(GameObject sender) {
                                lock (ActionScript) {
                                    ActionScript.SetVariable(new string[] { "this" }, ActionScript.GetVariable(new string[] { gameObject.name }));
                                    func();
                                }
                            }
                        }
                        break;
                    case "onMessageStringRecieved": {
                            Action func;
                            lock (ActionScript) {
                                ActionScript.LoadCode(";\n" + propertyValue); //the extra semicolon is to fix a bug where an if statement  doesnt work if it was the first statement, TODO: this should be fixed properly.....
                                func = ActionScript.Compile();
                            }
                            string[] path = new string[] { "args" };
                            gameObject.On<ScriptSendString>(OnMessageString);

                            //This runs on a different thread than the world.
                            void OnMessageString(ClientBase sender, ScriptSendString packet) {
                                world.Invoke(OnMessageStringInvoked); //For thread safety!
                                void OnMessageStringInvoked() {
                                    lock (ActionScript) {
                                        ActionScript.SetVariable(new string[] { "this" }, ActionScript.GetVariable(new string[] { gameObject.name }));
                                        ActionScript.SetVariable(path, new Dictionary<string, object>() { { "message", packet.message }, { "sender", sender } });
                                        func();
                                        ActionScript.SetVariable(path, null); //To force it to be shortlived. (Fast GC).
                                    }
                                }
                            }

                        }
                        break;
                    case "client-onLoad": {
                            LocalScriptsComponent localscripts = gameObject.GetFirst<LocalScriptsComponent>();
                            if (localscripts == null)
                                gameObject.AddChild(localscripts = new LocalScriptsComponent());
                            localscripts.SetScript(EventTypes.OnLoad, propertyValue);
                        }
                        break;
                    case "client-onUpdate": {
                            LocalScriptsComponent localscripts = gameObject.GetFirst<LocalScriptsComponent>();
                            if (localscripts == null)
                                gameObject.AddChild(localscripts = new LocalScriptsComponent());
                            localscripts.SetScript(EventTypes.OnUpdate, propertyValue);
                        }
                        break;
                    case "client-onCollisionStay": {
                            LocalScriptsComponent localscripts = gameObject.GetFirst<LocalScriptsComponent>();
                            if (localscripts == null)
                                gameObject.AddChild(localscripts = new LocalScriptsComponent());
                            localscripts.SetScript(EventTypes.OnCollisionStay, propertyValue);
                        }
                        break;
                    case "client-onMessageStringRecieved": {
                            LocalScriptsComponent localscripts = gameObject.GetFirst<LocalScriptsComponent>();
                            if (localscripts == null)
                                gameObject.AddChild(localscripts = new LocalScriptsComponent());
                            localscripts.SetScript(EventTypes.OnStringMessage, propertyValue);
                        }
                        break;
                    default:
                        break;
                }
        }


        public void HandleClient(ClientHandler clientHandler) {
            clientHandler.OnDisconnect += client => ClientHandler_OnDisconnect?.Invoke(client);
            clientHandler.OnStart += client => ClientHandler_OnStart?.Invoke(client); ;
        }
    }
}
