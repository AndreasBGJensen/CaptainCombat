
using Microsoft.Xna.Framework;
using System;

namespace CaptainCombat.Source.Utility {

    public static class VectorUtility {

        /// <summary>
        /// Constructs a new Vector2 with the same magnitude as this,
        /// but with the given direction
        /// </summary>
        /// <param name="angle">New direction in degrees</param>
        public static Vector2 WithDirection(this Vector2 vec, float angle) {
            var radianAngle = MathHelper.ToRadians(angle);
            float length = vec.Length();
            return new Vector2((float)Math.Sin(radianAngle)*length, -(float)Math.Cos(radianAngle) * length) ;
        }


        public static Vector2 WithMagnitude(this Vector2 vec, float magnitude) {
            if (vec.X == 0.0 && vec.Y == 0.0f)
                // If length is 0 then direction is 0, which is upwards (negative y)
                return new Vector2(0.0f, -magnitude);
            vec.Normalize();
            return vec * magnitude;
        }

        public static Vector2 Rotate(this Vector2 vec, float angle) {
            return vec.WithDirection(vec.Direction() + angle);
        }

        public static float Direction(this Vector2 vec) {
            return MathHelper.ToDegrees((float) Math.Atan2(vec.X, -vec.Y));
        }
    }
}
