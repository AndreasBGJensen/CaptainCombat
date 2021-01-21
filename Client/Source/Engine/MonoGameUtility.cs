using CaptainCombat.Common;

using MGVector = Microsoft.Xna.Framework.Vector2;
using MGMatrix = Microsoft.Xna.Framework.Matrix;
using MGColor = Microsoft.Xna.Framework.Color;

namespace CaptainCombat.Client {

    static class MonoGameUtility {

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

        public static MGColor ToMGColor(this Color c) {
            return new MGColor(c.Red, c.Green, c.Blue, c.Alpha);

        }

    }

}
