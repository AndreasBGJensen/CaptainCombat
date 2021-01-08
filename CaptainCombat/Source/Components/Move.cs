

using Microsoft.Xna.Framework;
using static ECS.Domain;

namespace CaptainCombat.Source.Components {

    public class Move : Component {

        public Vector2 Velocity { get; set; } = new Vector2(0, 0);

        public Vector2 Acceleration { get; set; } = new Vector2(0, 0);

        public double Resistance { get; set; } = 0;

        /// <summary>
        /// Whether or not to only use the magnitude of the velocity,
        /// and make the direction dependent on Transform rotation
        /// </summary>
        public bool ForwardVelocity { get; set; } = false;

        public double RotationVelocity { get; set; } 

        public double RotationAcceleration { get; set; }

        public double RotationResistance { get; set; }

        public override object getData() {
            throw new System.NotImplementedException();
        }
    }

}
