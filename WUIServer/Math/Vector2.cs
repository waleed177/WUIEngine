using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WUIServer.Math {
    public struct Vector2 {
        public float X;
        public float Y;
        
        public Vector2(float X=0, float Y=0) {
            this.X = X;
            this.Y = Y;
        }

        public static Vector2 operator +(Vector2 a) => a;
        public static Vector2 operator -(Vector2 a) => new Vector2(-a.X,-a.Y);

        public static Vector2 operator +(Vector2 a, Vector2 b) => new Vector2(a.X + b.X, a.Y + b.Y);
        public static Vector2 operator -(Vector2 a, Vector2 b) => new Vector2(a.X - b.X, a.Y - b.Y);
        public static Vector2 operator *(float k, Vector2 a) => new Vector2(k * a.X, k * a.Y);
        public static Vector2 operator *(Vector2 a, float k) => new Vector2(k * a.X, k * a.Y);
        public static Vector2 operator /(Vector2 a, float k) => new Vector2(k / a.X, k / a.Y);

        public override string ToString() {
            return "Vector2(" + X + "," + Y + ")";
        }

    }
}
