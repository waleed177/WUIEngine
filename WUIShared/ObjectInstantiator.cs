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

        static ObjectInstantiator() {
            factories = new Dictionary<Objects, ObjectFactory>();

            AddFactory(Objects.Empty, EmptyFactory);

            AddGeneralFactory<ButtonComponent>(Objects.UIButton);
            AddGeneralFactory<UIText>(Objects.UIText);
            AddGeneralFactory<MouseClickableComponent>(Objects.clickable);

            AddGeneralFactory<DragComponent>(Objects.draggable);
            AddGeneralFactory<FollowMouse>(Objects.followMouse);
            AddGeneralFactory<RawTextureRenderer>(Objects.texture);
            AddGeneralFactory<Transform>(Objects.transform);
            AddGeneralFactory<CameraComponent>(Objects.camera);
            AddGeneralFactory<LocalScriptsComponent>(Objects.LocalScriptsComponent);
            AddGeneralFactory<ClientDontReplicate>(Objects.clientDontReplicate);

            AddGeneralFactory<PlayerController>(Objects.player);

            AddGeneralFactory<BoxCollider>(Objects.boxCollider);
        }

        private static void AddFactory(Objects objType, ObjectFactory factory) {
            factories.Add(objType, factory);
        }

        private static void AddGeneralFactory<T>(Objects objType) where T : GameObject, new() {
            AddFactory(objType, GeneralFactory<T>);
        }

        internal static GameObject Instantiate(Objects objType) {
            return factories[objType].Invoke();
        }

        internal static GameObject Instantiate(string objName) {
            System.Enum.TryParse<Objects>(objName, out Objects objType);
            return factories[objType].Invoke();
        }

        private static GameObject GeneralFactory<T>() where T : GameObject, new() {
            return new T();
        }

        private static GameObject EmptyFactory() {
            return new GameObject(Objects.Empty, false);
        }
    }
}
