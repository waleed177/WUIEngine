using System;
using System.Collections.Generic;
using LowLevelNetworking.Shared;
using WUIShared;
using WUIShared.Objects;
using WUIShared.Packets;
using static WUIClient.Components.Collider;

namespace WUIClient.Components {
    public class LocalScriptsComponent : GameObject {
        private Action<GameObject>[] functions;
        private CollisionEvent onCollisionStay;

        public LocalScriptsComponent() : base(Objects.LocalScriptsComponent, false) {
            On<SendLocalScripts>(OnRecieveLocalScripts);
            functions = new Action<GameObject>[3];
        }

        private void OnRecieveLocalScripts(ClientBase sender, SendLocalScripts packet) {
            for (int i = 0; i < packet.eventId.Length; i++) {
                int eventId = packet.eventId[i];
                string code = packet.code[i];
                if (code.Trim() == "") continue;

                Game1.worldActionScript.LoadCode(code);
                Action func = Game1.worldActionScript.Compile();

                switch ((EventTypes)eventId) {
                    case EventTypes.OnLoad:
                        Parent.OnAddedEvent -= functions[eventId];
                        Parent.OnAddedEvent += OnlyThis;
                        functions[eventId] = OnlyThis;
                        break;
                    case EventTypes.OnUpdate:
                        Parent.OnUpdateEvent -= functions[eventId];
                        Parent.OnUpdateEvent += OnlyThis;
                        functions[eventId] = OnlyThis;
                        break;
                    case EventTypes.OnCollisionStay:
                        Collider collider = Parent.GetFirst<Collider>();
                        collider.ContinouslyCheckCollisions = true;
                        collider.OnCollisionStay -= onCollisionStay;
                        collider.OnCollisionStay += Collider_OnCollisionStay;
                        functions[eventId] = OnlyThis;
                        break;
                    case EventTypes.OnStringMessage:
                        //TODO: Have an UnOn function in the low level library.
                        Parent.On<WUIShared.Packets.ScriptSendString>(OnHandleScriptSendString);
                        break;
                    default:
                        break;
                }

                void OnlyThis(GameObject go) {
                    Game1.worldActionScript.SetVariable(new string[] { "this" }, Game1.worldActionScript.GetVariable(new string[] { go.name }));
                    func();
                }

                void Collider_OnCollisionStay(Collider go, Collider other) {
                    if (go.Parent == null || other.Parent == null) return;
                    Game1.worldActionScript.SetVariable(new string[] { "other" }, Game1.worldActionScript.GetVariable(new string[] { other.Parent.name }));
                    Game1.worldActionScript.SetVariable(new string[] { "this" }, Game1.worldActionScript.GetVariable(new string[] { go.Parent.name }));
                    func();
                }

                void OnHandleScriptSendString(ClientBase sender2, ScriptSendString packet2) {
                    string[] path = new string[] { "message" };
                    Game1.worldActionScript.SetVariable(new string[] { "this" }, Game1.worldActionScript.GetVariable(new string[] { Parent.name }));
                    Game1.worldActionScript.SetVariable(path, new Dictionary<string, object>() { { "value", packet2.message } });
                    func();
                    Game1.worldActionScript.SetVariable(path, null); //To force it to be shortlived. (Fast GC).
                }

            }


        }


    }
}
