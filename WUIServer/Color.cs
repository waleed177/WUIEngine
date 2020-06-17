using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WUIServer {
    public struct Color {
        public float r, g, b, a;

        public Color(float r, float g, float b, float a = 1) {
            this.r = r;
            this.g = g;
            this.b = b;
            this.a = a;
        }

        public static Color White = new Color(1, 1, 1, 1);
    }
}
