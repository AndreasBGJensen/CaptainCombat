
using StaticGameLogic_Library.Source;
using StaticGameLogic_Library.Source.Components;
using StaticGameLogic_Library.Source.ECS;
using static StaticGameLogic_Library.Source.ECS.Domain;

namespace CaptainCombat.Source {

    /// <summary>
    /// Wrapper class for an Entity representing a Camera,
    /// and provides some more sensible naming for its values
    /// </summary>
    public class Camera {

        private Entity entity;
        private Transform transform;
        // May add movement component here

        public Vector Position { get => transform.Position; set => transform.Position = value; }
        public double Zoom { get => transform.ScaleX; set { transform.ScaleX = value; transform.ScaleY = value; } }
        public double Rotation { get => transform.Rotation; set => transform.Rotation = value; }

        public Camera(Domain domain) {
            entity = new Entity(domain);
            transform = entity.AddComponent(new Transform());
        }


        public Matrix GetMatrix() {
            var translation = Matrix.CreateTranslation(-transform.Position);
            var rotation = Matrix.CreateRotation(transform.Rotation);
            var zoom = Matrix.CreateScale(transform.ScaleX, transform.ScaleY);

            var center = Matrix.CreateTranslation(GameController.Graphics.PreferredBackBufferWidth / 2.0f, GameController.Graphics.PreferredBackBufferHeight / 2.0f);

            return translation * zoom * rotation * center;
        }


    }

}
