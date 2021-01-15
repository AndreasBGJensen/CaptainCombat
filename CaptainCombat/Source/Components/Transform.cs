
using Newtonsoft.Json.Linq;
using static ECS.Domain;

namespace CaptainCombat.Source.Components {

    public class Transform : Component {

        public Vector Position;
        
        // Should be checked for negative scale
        private double scaleX = 1.0;
        public double ScaleX {
            get => scaleX;
            set {
                scaleX = value;
                if (scaleX <= 0.000001)
                    scaleX = 0.000001;
            }
        }
            
        public double ScaleY { get; set; } = 1.0;

        // Rotation in degrees
        private double rotation = 0.0;
        public double Rotation { 
            get => rotation;
            set {
                if( value >= 0) {
                    rotation = value % 360;
                } else {
                    rotation = 360 - (value * -1) % 360;
                }
            }
        }


        public override void OnUpdate(Component component) {
            var c = (Transform)component;
            Position = c.Position;
            ScaleX = c.ScaleX;
            ScaleY = c.ScaleY;
            Rotation = c.Rotation;
        }

    }

}
