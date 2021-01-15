

using CaptainCombat.Source.Components;
using ECS;
using Microsoft.Xna.Framework;
using System;
using static ECS.Domain;

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
            // TODO: Cleanup this method
            //var translation = Matrix.CreateTranslation(-(float)transform.Position.X, -(float)transform.Position.Y, 0);
            //var rotation = Matrix.CreateRotationZ((float)(transform.Rotation * Math.PI / 180.0));
            //var zoom = Matrix.CreateScale((float)transform.ScaleX, (float)transform.ScaleX, 1.0f);
            var translation = Matrix.CreateTranslation(-transform.Position);
            var rotation = Matrix.CreateRotation(transform.Rotation);
            var zoom = Matrix.CreateScale(transform.ScaleX, transform.ScaleY);


            //var center = Matrix.CreateTranslation((float)GameController.Graphics.PreferredBackBufferWidth / 2.0f, (float)GameController.Graphics.PreferredBackBufferHeight / 2.0f, 0);

            var center = Matrix.CreateTranslation(GameController.Graphics.PreferredBackBufferWidth / 2.0f, GameController.Graphics.PreferredBackBufferHeight / 2.0f);

            // Matrix multiplication order is reversed
            return translation * zoom * rotation * center;
        }


    }

}
