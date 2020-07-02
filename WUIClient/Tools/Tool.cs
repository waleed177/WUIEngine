using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WUIClient.Gizmos;

namespace WUIClient.Tools {
    public abstract class Tool {
        
        public void Select() {
            OnSelect();
            
        }

        public void Update() {
            OnUpdate();

        }

        public void Deselect() {
            OnDeselect();

        }

        protected abstract void OnSelect();
        protected abstract void OnUpdate();
        protected abstract void OnDeselect();

    }
}
