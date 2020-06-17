using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WUIShared.Objects;

namespace WUIServer.Components {
    class ButtonComponent : GameObject {
        public Color normalColor = Color.White;
        public Color mouseHoverColor = new Color(0.9f, 0.9f, 0.9f);
        public Color mouseDownColor = new Color(0.75f, 0.75f, 0.75f);

        public ButtonComponent() : base(Objects.ButtonComponent, false) { }
    }
}
