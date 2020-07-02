using WUIClient.Gizmos;

namespace WUIClient.Tools {
    public class ScaleTool : Tool {
        private ScaleGizmo gizmo;
        private GameObject selected;

        public ScaleTool() {
            gizmo = new ScaleGizmo();

        }

        protected override void OnSelect() {

        }

        protected override void OnUpdate() {
            if (WMouse.LeftMouseClick()) {
                GameObject obj = WMouse.GetGameObjectUnderneathMouse(Game1.instance.world, false);

                if (obj != null) {
                    selected = obj;
                    if (gizmo.Parent == null)
                        Game1.gizmoWorld.AddChild(gizmo);
                    gizmo.UseOnBounds(selected.transform.Bounds);
                }
            }

            if (WMouse.RightMouseClick()) {
                selected = null;
                gizmo.Remove();
            }

            if (selected != null) {
                selected.transform.Size = gizmo.GetSize();
                selected.transform.Position = gizmo.GetPosition();
                gizmo.UseOnBounds(selected.transform.Bounds);
            }
        }

        protected override void OnDeselect() {
            selected = null;
            gizmo.Remove();
        }
    }
}
