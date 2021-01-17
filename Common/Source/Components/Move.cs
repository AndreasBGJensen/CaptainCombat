

using static CaptainCombat.Common.Domain;

namespace CaptainCombat.Common.Components {

    public class Move : Component {

        public bool Enabled { get; set; } = true;

        public Vector Velocity { get; set; } = new Vector(0, 0);

        public Vector Acceleration { get; set; } = new Vector(0, 0);

        public double Resistance { get; set; } = 0;

        /// <summary>
        /// Whether or not to only use the magnitude of the velocity,
        /// and make the direction dependent on Transform rotation
        /// </summary>
        public bool ForwardVelocity { get; set; } = false;

        public double RotationVelocity { get; set; } 

        public double RotationAcceleration { get; set; }

        public double RotationResistance { get; set; }

        public override void OnUpdate(Component component) {
            var c = (Move)component;
            Enabled = c.Enabled;
            Velocity = c.Velocity;
            Acceleration = c.Acceleration;
            Resistance = c.Resistance;
            ForwardVelocity = c.ForwardVelocity;
            RotationVelocity = c.RotationVelocity;
            RotationAcceleration = c.RotationAcceleration;
            RotationResistance = c.RotationResistance;
        }
    }
}
