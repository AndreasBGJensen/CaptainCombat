
namespace CaptainCombat.Common {

    public struct Color {

        public static Color White { get; } = new Color(1.0f, 1.0f, 1.0f, 1.0f);

        public float Red { get; set; }
        public float Green { get; set; }
        public float Blue { get; set; }
        public float Alpha { get; set; }

        public Color(float red, float green, float blue, float alpha) {
            Red = red;
            Green = green;
            Blue = blue;
            Alpha = alpha;
        }


    }

}
