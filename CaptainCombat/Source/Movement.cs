

using CaptainCombat.Source.Components;
using ECS;

namespace CaptainCombat.Source {




    public class Movement {
        

        public static void Update(Domain domain, double deltaTime) {

            domain.ForMatchingEntities<Transform, Move>((entity) => {

                var transform = entity.GetComponent<Transform>();
                var move = entity.GetComponent<Move>();


                // Update rotation velocity
                move.RotationVelocity *= 1.0f - (float)(move.RotationResistance * deltaTime);
                move.RotationVelocity += move.RotationAcceleration * (float)deltaTime;

                // Update rotation
                transform.Rotation += move.RotationVelocity * deltaTime;

                // Update velocity
                move.Velocity *= 1.0f - (float)(move.Resistance * deltaTime);
                move.Velocity += move.Acceleration * (float)deltaTime;

                // Update position
                var finalVelocity = move.ForwardVelocity ? move.Velocity.WithDirection((float)transform.Rotation) : move.Velocity;
                transform.X += finalVelocity.X * deltaTime;
                transform.Y += finalVelocity.Y * deltaTime;

            });


        }


    }

}
