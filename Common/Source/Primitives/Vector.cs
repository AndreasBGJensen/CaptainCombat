
using System;

namespace CaptainCombat.Common {

    public struct Vector {

        public static readonly Vector Zero = new Vector(0, 0);
        public static readonly Vector Unit = new Vector(1, 1);

        public double X;
        public double Y;

        public Vector(double value) {
            X = value;
            Y = value;
        }

        public Vector(double x, double y) {
            X = x;
            Y = y;
        }

        public double LengthSquared() {
            return X * X + Y * Y;
        }

        public double Length() {
            return Math.Sqrt(X*X+Y*Y);
        }

        public Vector Normalized() {
            var length = Length();
            return new Vector(X/length, Y/length);
        }

        public double Dot(Vector vec) {
            return X * vec.X + Y * vec.Y;
        }

        public double Cross(Vector vec) {
            return X * vec.Y - Y * vec.X;
        }

        public double DistanceTo(Vector vec) {
            return (vec - this).Length();
        }


        /// <summary>
        /// Returns raw angle in degrees between 0 and 360, where
        /// 0 is along the positive x-axis and 90 is along positive
        /// y-axis.
        /// </summary>
        /// <returns></returns>
        public double Direction() {
            var rawAngle = Math.Atan2(X, -Y) * 180/Math.PI;
            if (rawAngle < 0) return rawAngle + 360;
            return rawAngle;
        }


        public static Vector CreateDirection(double angle) {
            var radianAngle = angle * Math.PI/180;
            return new Vector(Math.Sin(radianAngle), -Math.Cos(radianAngle));
        }

        // Operator overloading
        
        public static Vector operator +(Vector v1) => new Vector(v1.X, v1.Y);

        public static Vector operator -(Vector v1) => new Vector(-v1.X, -v1.Y);

        public static Vector operator +(Vector v1, Vector v2) => new Vector(v1.X + v2.X, v1.Y + v2.Y);
        
        public static Vector operator -(Vector v1, Vector v2) => new Vector(v1.X - v2.X, v1.Y - v2.Y);

        public static Vector operator +(Vector v1, double d) => new Vector(v1.X + d, v1.Y + d);

        public static Vector operator -(Vector v1, double d) => new Vector(v1.X - d, v1.Y - d);

        public static Vector operator *(Vector v, double d) => new Vector(v.X * d, v.Y * d);
        
        public static Vector operator *(double d, Vector v) => new Vector(v.X * d, v.Y * d);

        public static Vector operator /(Vector v1, double d) => new Vector(v1.X / d, v1.Y / d);


        // Equality overloading
        public static bool operator ==(Vector v1, Vector v2) {
            if ((object)v1 == null)
                return (object)v2 == null;
            return v1.Equals(v2);
        }

        public static bool operator !=(Vector v1, Vector v2) {
            return !(v1 == v2);
        }

        public override bool Equals(object obj) {
            if (obj == null || GetType() != obj.GetType())
                return false;
            var v2 = (Vector) obj;
            return X == v2.X && Y == v2.Y;
        }

        public override int GetHashCode() {
            return X.GetHashCode() ^ Y.GetHashCode();
        }


        public override string ToString() {
            return $"({X},{Y})";
        }
    }

}
