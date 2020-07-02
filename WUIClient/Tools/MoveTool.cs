using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WUIClient.Gizmos;

namespace WUIClient.Tools {
    public class MoveTool : Tool {
        private MoveGizmo gizmo;
        private GameObject selected;

        public MoveTool() {
            gizmo = new MoveGizmo();
            
        }

        protected override void OnSelect() {
            
        }

        protected override void OnUpdate() {
            if(WMouse.LeftMouseClick()) {
                GameObject obj = WMouse.GetGameObjectUnderneathMouse(Game1.instance.world, false);
                
                if (obj != null) {
                    selected = obj;
                    if (gizmo.Parent == null)
                        Game1.gizmoWorld.AddChild(gizmo);
                    gizmo.transform.Position = obj.transform.Position + obj.transform.Size / 2;
                }
            }

            if (WMouse.RightMouseClick()) {
                selected = null;
                gizmo.Remove();
            }

            if(selected != null) {
                selected.transform.Position = gizmo.transform.Position - selected.transform.Size/2;
            }
        }

        protected override void OnDeselect() {
            selected = null;
            gizmo.Remove();
        }
    }
}
