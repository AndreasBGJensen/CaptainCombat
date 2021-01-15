
using System;

using MGVector = Microsoft.Xna.Framework.Vector2;
using MGMatrix = Microsoft.Xna.Framework.Matrix;


namespace CaptainCombat.Source.Utility {

    public static class VectorUtility {

        /// <summary>
        /// Constructs a new Vector2 with the same magnitude as this,
        /// but with the given direction
        /// </summary>
        /// <param name="angle">New direction in degrees</param>
        public static MGVector WithDirection(this MGVector vec, float angle) {
            var radianAngle = angle * Math.PI/180;
            float length = vec.Length();
            return new MGVector((float)Math.Sin(radianAngle)*length, -(float)Math.Cos(radianAngle) * length) ;
        }


        public static MGVector WithMagnitude(this MGVector vec, float magnitude) {
            if (vec.X == 0.0 && vec.Y == 0.0f)
                // If length is 0 then direction is 0, which is upwards (negative y)
                return new MGVector(0.0f, -magnitude);
            vec.Normalize();
            return vec * magnitude;
        }

        public static MGVector Rotate(this MGVector vec, float angle) {
            return vec.WithDirection(vec.Direction() + angle);
        }

        public static float Direction(this MGVector vec) {
            return (float)(Math.Atan2(vec.X, -vec.Y) * 180 /Math.PI);
        }


        public static float AngleTo(this MGVector vec1, MGVector vec2) {
            var crossProduct = vec1.Cross(vec2);
            var dotProduct = vec1.Dot(vec2);
            return (float)(Math.Atan2(crossProduct, dotProduct)*180/Math.PI);
        }


        public static float Dot(this MGVector vec1, MGVector vec2) {
            return MGVector.Dot(vec1, vec2);
        }


        public static float Cross(this MGVector vec1, MGVector vec2) {
            return vec1.X * vec2.Y - vec1.Y * vec2.X;
        }


        public static MGVector ToMGVector(this Vector vec) {
            return new MGVector((float)vec.X, (float)vec.Y);
        }

        public static Vector ToVector(this MGVector vec) {
            return new Vector(vec.X, vec.Y);
        }





        public static MGMatrix ToMGMatrix(this Matrix matrix) {
            return new MGMatrix(
                (float)matrix.M00, (float)matrix.M10, 0, (float)matrix.M20,
                (float)matrix.M01, (float)matrix.M11, 0, (float)matrix.M21,
                 0, 0, 1, 0,
                 (float)matrix.M02, (float)matrix.M12, 0, (float)matrix.M22
                );
        }
    }
}
