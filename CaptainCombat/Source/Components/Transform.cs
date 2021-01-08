
using static ECS.Domain;

namespace CaptainCombat.Source.Components {

    public class Transform : Component {

        public double X { get; set; } = 0;
        public double Y { get; set; } = 0;

        // Should be checked for negative scale
        public double ScaleX { get; set; } = 1.0;
        public double ScaleY { get; set; } = 1.0;

        // Rotation in degrees
        public double Rotation { get; set; } = 0.0;
        
        public Transform(Entity entity) : base(entity) {}


    }

}
