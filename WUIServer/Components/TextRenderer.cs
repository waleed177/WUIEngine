using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WUIShared.Objects;

namespace WUIServer.Components {
    class TextRenderer : GameObject {
        private string text;
        private Color color;

        public TextRenderer(string text, Color color) : base(Objects.TextRenderer, false) {
            this.text = text;
            this.color = color;
        }

        
    }
}
