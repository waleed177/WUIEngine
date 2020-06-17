namespace WUIServer.Math {
    public struct RectangleF {
        public float x, y, width, height;

        public RectangleF(float x, float y, float width, float height) : this() {
            this.x = x;
            this.y = y;
            this.width = width;
            this.height = height;
        }

        public bool Contains(Vector2 point) {
            return x < point.X && point.X < x + width && y < point.Y && point.Y < y + height;
        }

        public bool OverlapsWith(RectangleF rect) {
            return !(x > rect.x + rect.width || y > rect.y + rect.height || rect.x > x + width || rect.y > y + height);
        }
    }
}
