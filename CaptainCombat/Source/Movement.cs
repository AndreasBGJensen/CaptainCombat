

using CaptainCombat.Source.Components;
using CaptainCombat.Source.Utility;
using ECS;
using Microsoft.Xna.Framework;

namespace CaptainCombat.Source {




    public class Movement {
        

        public static void Update(Domain domain, double deltaTime) {

            domain.ForMatchingEntities<Transform, Move>((entity) => {

                var transform = entity.GetComponent<Transform>();
                var move = entity.GetComponent<Move>();

                if (!move.Enabled) return;

                // Update rotation velocity
                move.RotationVelocity *= 1.0f - (float)(move.RotationResistance * deltaTime);
                move.RotationVelocity += move.RotationAcceleration * (float)deltaTime;

                // Update rotation
                transform.Rotation += move.RotationVelocity * deltaTime;

                // Update velocity
                double resistance = 1.0 - move.Resistance * deltaTime;
                move.Velocity = new Vector2( (float)(move.Velocity.X * resistance), (float)(move.Velocity.Y * resistance) );
                move.Velocity = new Vector2(
                    (float)(move.Velocity.X + move.Acceleration.X * deltaTime),
                    (float)(move.Velocity.Y + move.Acceleration.Y * deltaTime)
                    );
                
                // Update position
                var finalVelocity = move.ForwardVelocity ? move.Velocity.WithDirection((float)transform.Rotation) : move.Velocity;
                transform.X += finalVelocity.X * deltaTime;
                transform.Y += finalVelocity.Y * deltaTime;

            });


        }


    }

}
