

using Microsoft.Xna.Framework;
using Newtonsoft.Json.Linq;
using static Source.ECS.Domain;

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

       
        public override object getData()
        {
            var obj = new { 
                VelocityX = (double)this.Velocity.X,
                VelocityY = (double)this.Velocity.Y,
                AccelerationX = (double)this.Acceleration.X,
                AccelerationY = (double)this.Acceleration.Y,
                Resistance = this.Resistance,
                ForwardVelocity = this.ForwardVelocity,
                RotationVelocity = this.RotationVelocity,
                RotationAcceleration = this.RotationAcceleration,
                RotationResistance = this.RotationResistance
            };
            return obj;
        }

        public override void update(JObject json)
        {
            this.Velocity = new Vector2((float)json.SelectToken("VelocityX"), (float)json.SelectToken("VelocityY"));
            this.Acceleration = new Vector2((float)json.SelectToken("AccelerationX"), (float)json.SelectToken("AccelerationY"));
            this.Resistance = (double)json.SelectToken("Resistance");
            this.ForwardVelocity = (bool)json.SelectToken("ForwardVelocity");
            this.RotationVelocity = (double)json.SelectToken("RotationVelocity");
            this.RotationAcceleration = (double)json.SelectToken("RotationAcceleration");
            this.RotationResistance = (double)json.SelectToken("RotationResistance");
        }
    }
}
