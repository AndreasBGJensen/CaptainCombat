using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


using MGVector = Microsoft.Xna.Framework.Vector2;
using MGMatrix = Microsoft.Xna.Framework.Matrix;
using CaptainCombat.Source.Utility;

namespace CaptainCombat.Source {
    class VectorTest {


        public static void Start() {


            double x = 1;
            double y = 4;

            Matrix mat = Matrix.CreateTranslation(2, 3) * Matrix.CreateRotation(40) * Matrix.CreateScale(2,1);

            MGMatrix mgMat = MGMatrix.CreateTranslation(2, 3, 0) * MGMatrix.CreateRotationZ((float)(40 * Math.PI / 180)) * MGMatrix.CreateScale(2, 1, 1);

            Console.WriteLine("\nVector:");
            Console.WriteLine(new Vector(x,y) * mat);

            Console.WriteLine("\nMGVector:");
            MGMatrix convertedMat = mat.ToMGMatrix();
            Console.WriteLine(MGVector.Transform(new MGVector((float)x, (float)y), convertedMat));
            Console.WriteLine(MGVector.Transform(new MGVector((float)x, (float)y), mgMat));

        }

    }
}
