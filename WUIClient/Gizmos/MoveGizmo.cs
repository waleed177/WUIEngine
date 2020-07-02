using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WUIClient.Components;
using static WUIClient.Components.DragComponent;

namespace WUIClient.Gizmos {
    class MoveGizmo : Gizmo {
        private GameObject GizmoRight { get; }
        private GameObject GizmoLeft { get; }
        private GameObject GizmoUp { get; }
        private GameObject GizmoDown { get; }

        public MoveGizmo() {
            GizmoRight = MakeGizmo(0, 16, 0, Axis.X);
            GizmoLeft = MakeGizmo(1, -16, 0, Axis.X);
            GizmoUp = MakeGizmo(2, 0, -16, Axis.Y);
            GizmoDown = MakeGizmo(3, 0, 16, Axis.Y);
        }

        private GameObject MakeGizmo(int i, float x, float y, Axis axis) {
            GameObject res = new GameObject();
            res.transform.Size = new Microsoft.Xna.Framework.Vector2(8, 8);
            res.transform.Position = new Vector2(x, y);
            res.AddChild(new RawTextureRenderer() { texture = Game1.instance.UIRect, color = Color.White });
            res.AddChild(new MouseClickableComponent());
            res.AddChild(new DragComponent() { dragTransform = transform, axis = axis});
            AddChild(res);
            return res;
        }
    }
}
