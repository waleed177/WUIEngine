﻿using System.Collections.Generic;
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
            factories = new Dictionary<Objects, ObjectFactory> {
                [Objects.ButtonComponent] = GeneralFactory<ButtonComponent>,
                [Objects.DragComponent] = GeneralFactory<DragComponent>,
                [Objects.Empty] = EmptyFactory,
                [Objects.FollowMouse] = GeneralFactory<FollowMouse>,
                [Objects.MouseClickableComponent] = GeneralFactory<MouseClickableComponent>,
                [Objects.RawTextureRenderer] = GeneralFactory<RawTextureRenderer>,
                [Objects.Transform] = GeneralFactory<Transform>,
                [Objects.PlayerController] = GeneralFactory<PlayerController>,
                [Objects.BoxCollider] = GeneralFactory<BoxCollider>,
                [Objects.Camera] = GeneralFactory<CameraComponent>,
                [Objects.LocalScriptsComponent] = GeneralFactory<LocalScriptsComponent>,
            };
        }

        internal static GameObject Instantiate(Objects objType) {
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