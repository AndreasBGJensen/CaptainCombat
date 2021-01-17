
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


    public struct Matrix {


        public static readonly Matrix UNIT = new Matrix(
                1, 0, 0,
                0, 1, 0,
                0, 0, 1
            );

        public double M00;
        public double M01;
        public double M02;
        public double M10;
        public double M11;
        public double M12;
        public double M20;
        public double M21;
        public double M22;

        public Matrix(double m00, double m01, double m02, double m10, double m11, double m12, double m20, double m21, double m22) {
            M00 = m00;
            M01 = m01;
            M02 = m02;
            M10 = m10;
            M11 = m11;
            M12 = m12;
            M20 = m20;
            M21 = m21;
            M22 = m22;
        }


        public static Matrix CreateScale(double x, double y) {
            return new Matrix(
                    x, 0, 0,
                    0, y, 0,
                    0, 0, 1
                );
        }


        public static Matrix CreateTranslation(Vector vec) {
            return new Matrix(
                    1, 0, vec.X,
                    0, 1, vec.Y,
                    0, 0, 1
                );
        }


        public static Matrix CreateTranslation(double x, double y) {
            return new Matrix(
                    1, 0, x,
                    0, 1, y,
                    0, 0, 1
                );
        }


        public static Matrix CreateRotation(double angle) {
            var radianAngle = angle * Math.PI / 180;
            var cos = Math.Cos(radianAngle);
            var sin = Math.Sin(radianAngle);
            return new Matrix(
                    cos, -sin, 0,
                    sin,  cos, 0,
                      0,    0, 1
                );
        }


        public static Matrix operator *(Matrix m2, Matrix m1) {
            // THe multiplication is reversed just like monogame
            return new Matrix(
                    m1.M00 * m2.M00 + m1.M01 * m2.M10 + m1.M02 * m2.M20,   m1.M00 * m2.M01 + m1.M01 * m2.M11 + m1.M02 * m2.M21,  m1.M00 * m2.M02 + m1.M01 * m2.M12 + m1.M02*m2.M22,
                    m1.M10 * m2.M00 + m1.M11 * m2.M10 + m1.M12 * m2.M20,   m1.M10 * m2.M01 + m1.M11 * m2.M11 + m1.M12 * m2.M21,  m1.M10 * m2.M02 + m1.M11 * m2.M12 + m1.M12*m2.M22,
                    m1.M20 * m2.M00 + m1.M21 * m2.M10 + m1.M22 * m2.M20,   m1.M20 * m2.M01 + m1.M21 * m2.M11 + m1.M22 * m2.M21,  m1.M20 * m2.M02 + m1.M21 * m2.M12 + m1.M22*m2.M22
                );

        }

        public static Vector operator *(Vector v, Matrix m) {
            return new Vector(
                    m.M00 * v.X + m.M01 * v.Y + m.M02 * 1,
                    m.M10 * v.X + m.M11 * v.Y + m.M12 * 1
                );
        }
    }

}
