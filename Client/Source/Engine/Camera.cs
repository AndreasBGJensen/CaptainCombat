
using CaptainCombat.Common;
using CaptainCombat.Common.Components;
using static CaptainCombat.Common.Domain;

namespace CaptainCombat.Client {

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

            var center = Matrix.CreateTranslation(ApplicationController.Graphics.PreferredBackBufferWidth / 2.0f, ApplicationController.Graphics.PreferredBackBufferHeight / 2.0f);

            return translation * zoom * rotation * center;
        }


    }

}
