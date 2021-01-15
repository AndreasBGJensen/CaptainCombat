

using CaptainCombat.Source.Components;
using ECS;

namespace CaptainCombat.Source {




    public class Movement {
        

        public static void Update(Domain domain, double deltaTime) {

            domain.ForMatchingEntities<Transform, Move>((entity) => {

                var transform = entity.GetComponent<Transform>();
                var move = entity.GetComponent<Move>();

                if (!move.Enabled) return;

                // Update rotation velocity
                move.RotationVelocity *= 1.0f - move.RotationResistance * deltaTime;
                move.RotationVelocity += move.RotationAcceleration * deltaTime;

                // Update rotation
                transform.Rotation += move.RotationVelocity * deltaTime;

                // Update velocity
                double resistance = 1.0 - move.Resistance * deltaTime;
                move.Velocity *= resistance;
                move.Velocity += move.Acceleration * deltaTime;
                
                // Update position
                var finalVelocity = move.ForwardVelocity ? Vector.CreateDirection(transform.Rotation) * move.Velocity.Length() : move.Velocity;
                transform.Position += finalVelocity * deltaTime;

            });


        }


    }

}
