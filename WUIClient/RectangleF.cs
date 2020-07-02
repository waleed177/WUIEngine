using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WUIClient {
    public struct RectangleF {
        public float x, y, width, height;

        public RectangleF(float x, float y, float width, float height) : this() {
            this.x = x;
            this.y = y;
            this.width = width;
            this.height = height;
        }

        public float Right => x + width;
        public float Left => x;
        public float Top => y;
        public float Bottom => y + height;
        public Vector2 Center => new Vector2(x + width / 2, y + height / 2);

        public bool Contains(Vector2 point) {
            return x < point.X && point.X < x + width && y < point.Y && point.Y < y + height;
        }

        public bool Contains(Point point) {
            return x < point.X && point.X < x + width && y < point.Y && point.Y < y + height;
        }

        public bool OverlapsWith(RectangleF rect) {
            return !(x > rect.x + rect.width || y > rect.y + rect.height || rect.x > x + width || rect.y > y + height);
        }
    }
}
