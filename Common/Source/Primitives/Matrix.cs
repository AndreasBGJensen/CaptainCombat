using System;

namespace CaptainCombat.Common {


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
                    sin, cos, 0,
                      0, 0, 1
                );
        }


        public static Matrix operator *(Matrix m2, Matrix m1) {
            // THe multiplication is reversed just like monogame
            return new Matrix(
                    m1.M00 * m2.M00 + m1.M01 * m2.M10 + m1.M02 * m2.M20, m1.M00 * m2.M01 + m1.M01 * m2.M11 + m1.M02 * m2.M21, m1.M00 * m2.M02 + m1.M01 * m2.M12 + m1.M02 * m2.M22,
                    m1.M10 * m2.M00 + m1.M11 * m2.M10 + m1.M12 * m2.M20, m1.M10 * m2.M01 + m1.M11 * m2.M11 + m1.M12 * m2.M21, m1.M10 * m2.M02 + m1.M11 * m2.M12 + m1.M12 * m2.M22,
                    m1.M20 * m2.M00 + m1.M21 * m2.M10 + m1.M22 * m2.M20, m1.M20 * m2.M01 + m1.M21 * m2.M11 + m1.M22 * m2.M21, m1.M20 * m2.M02 + m1.M21 * m2.M12 + m1.M22 * m2.M22
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