using System.Collections.Generic;
#if WUIServer
using WUIServer.Components;
#elif WUIClient
using WUIClient.Components;
#endif

namespace WUIShared.Objects {
    public static class ObjectInstantiator {
        public delegate GameObject ObjectFactory();
        private static Dictionary<Objects, ObjectFactory> factories;
        private static Dictionary<string, ObjectFactory> factories_string;

        static ObjectInstantiator() {
            factories = new Dictionary<Objects, ObjectFactory>();
            factories_string = new Dictionary<string, ObjectFactory>();

            AddFactory(Objects.Empty, "__EMPTY", EmptyFactory);

            AddGeneralFactory<ButtonComponent>(Objects.ButtonComponent, "UIButton");
            AddGeneralFactory<UIText>(Objects.UIText, "UIText");
            AddGeneralFactory<MouseClickableComponent>(Objects.MouseClickableComponent, "clickable");

            AddGeneralFactory<DragComponent>(Objects.DragComponent, "draggable");
            AddGeneralFactory<FollowMouse>(Objects.FollowMouse, "followMouse");
            AddGeneralFactory<RawTextureRenderer>(Objects.RawTextureRenderer, "texture");
            AddGeneralFactory<Transform>(Objects.Transform, "transform");
            AddGeneralFactory<CameraComponent>(Objects.Camera, "camera");
            AddGeneralFactory<LocalScriptsComponent>(Objects.LocalScriptsComponent, "__LOCALSCRIPTSCOMPONENT");
            AddGeneralFactory<ClientDontReplicate>(Objects.ClientDontReplicate, "clientDontReplicate");

            AddGeneralFactory<PlayerController>(Objects.PlayerController, "topDownPlayer");

            AddGeneralFactory<BoxCollider>(Objects.BoxCollider, "boxCollider");
        }

        private static void AddFactory(Objects objType, string scriptName, ObjectFactory factory) {
            factories.Add(objType, factory);
            factories_string.Add(scriptName, factory);
        }

        private static void AddGeneralFactory<T>(Objects objType, string scriptName) where T : GameObject, new() {
            AddFactory(objType, scriptName, GeneralFactory<T>);
        }

        internal static GameObject Instantiate(Objects objType) {
            return factories[objType].Invoke();
        }

        internal static GameObject Instantiate(string objName) {
            return factories_string[objName].Invoke();
        }

        private static GameObject GeneralFactory<T>() where T : GameObject, new() {
            return new T();
        }

        private static GameObject EmptyFactory() {
            return new GameObject(Objects.Empty, false);
        }
    }
}
