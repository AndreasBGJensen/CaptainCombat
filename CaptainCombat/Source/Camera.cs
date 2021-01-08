

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

        public double X { get => transform.X; set => transform.X = value; }
        public double Y { get => transform.Y; set => transform.Y = value; }
        public double Zoom { get => transform.ScaleX; set { transform.ScaleX = value; transform.ScaleY = value; } }
        public double Rotation { get => transform.Rotation; set => transform.Rotation = value; }

        public Camera(Domain domain) {
            entity = new Entity(domain);
            transform = entity.AddComponent<Transform>();
        }


        /// <summary>
        /// Moves the camera's transform component relative to its
        /// current direction, the world moves along the screen
        /// x and y axis.
        /// </summary>
        public void MoveInRotationDirection(double x, double y) {
            // TODO: Implemen this (if necessary)
            throw new NotImplementedException();
        }


        public Matrix GetMatrix() {
            var translation = Matrix.CreateTranslation(-(float)transform.X, -(float)transform.Y, 0);
            var rotation = Matrix.CreateRotationZ((float)(transform.Rotation * Math.PI / 180.0));
            var zoom = Matrix.CreateScale((float)transform.ScaleX, (float)transform.ScaleX, 1.0f);

            // TODO: Correct the screen size when that is in place

            var center = Matrix.CreateTranslation((float)GameController.Graphics.PreferredBackBufferWidth / 2.0f, (float)GameController.Graphics.PreferredBackBufferHeight / 2.0f, 0);
            
            // Apparently Matrix multiplication order is reversed in MonoGame,
            // so that the transformation takes place from left to right
            // (translation -> rotation -> centering)
            return translation * zoom * rotation * center;
        }


    }

}
