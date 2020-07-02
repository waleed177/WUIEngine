using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WUIClient.Components;
using static WUIClient.Components.DragComponent;

namespace WUIClient.Gizmos {
    public class ScaleGizmo : Gizmo{
        private GameObject GizmoRight { get; }
        private GameObject GizmoLeft { get; }
        private GameObject GizmoUp { get; }
        private GameObject GizmoDown { get; }

        public ScaleGizmo() {
            GizmoRight = MakeGizmo(0, 16, 0, Axis.X);
            GizmoLeft = MakeGizmo(1, -16, 0, Axis.X);
            GizmoUp = MakeGizmo(2, 0, -16, Axis.Y);
            GizmoDown = MakeGizmo(3, 0, 16, Axis.Y);
        }

        private GameObject MakeGizmo(int i, float x, float y, Axis axis) {
            GameObject res = new GameObject();
            res.transform.Size = new Microsoft.Xna.Framework.Vector2(8, 8);
            res.AddChild(new RawTextureRenderer() { texture = Game1.instance.UIRect, color = Color.White });
            res.AddChild(new MouseClickableComponent());
            res.AddChild(new DragComponent() { axis = axis });
            AddChild(res);
            return res;
        }

        public void UseOnBounds(RectangleF rect) {
            GizmoRight.transform.Position = new Vector2(rect.Right, rect.Center.Y);
            GizmoLeft.transform.Position = new Vector2(rect.Left, rect.Center.Y);
            GizmoUp.transform.Position = new Vector2(rect.Center.X, rect.Top);
            GizmoDown.transform.Position = new Vector2(rect.Center.X, rect.Bottom);
        }

        public Vector2 GetSize() {
            return new Vector2(GizmoRight.transform.Position.X - GizmoLeft.transform.Position.X, GizmoDown.transform.Position.Y - GizmoUp.transform.Position.Y);
        }

        public Vector2 GetPosition() {
            return new Vector2(GizmoLeft.transform.Position.X, GizmoUp.transform.Position.Y);
        }
    }
}
