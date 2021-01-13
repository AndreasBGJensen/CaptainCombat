using Microsoft.Xna.Framework;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using static ECS.Domain;

namespace CaptainCombat.Source.Components {

    public abstract class Collider : Component {
        public bool Enabled { get; set; } = true;

        public ColliderType ColliderType { get; set; }

        public Vector2 Offset { get; set; }
        /// <summary>
        /// Flag to denote that this collider collided with something
        /// in the previous frame. Only used for rendering
        /// </summary>
        public bool Collided { get; set; }
    }


    public class ColliderType {
        private static Dictionary<string, ColliderType> all = new Dictionary<string, ColliderType>();

        public string Tag { get; }

        public ColliderType(string tag) {
            if (all.ContainsKey(tag))
                throw new ArgumentException($"ColliderType with tag '{tag} already exists");
            Tag = tag;
            all.Add(tag, this);
        }

        public static bool operator ==(ColliderType c1, ColliderType c2) {
            if ((object)c1 == null)
                return (object)c2 == null;
            return c1.Equals(c2);
        }

        public static bool operator !=(ColliderType c1, ColliderType c2) {
            return !(c1 == c2);
        }

        public override bool Equals(object obj) {
            if (obj == null || GetType() != obj.GetType())
                return false;
            var c1 = (ColliderType)obj;
            return Tag == c1.Tag;
        }

        public override int GetHashCode() {
            return Tag.GetHashCode();
        }
    }





    public class BoxCollider : Collider {

        public double Width { get; set; }
        public double Height { get; set; }

        public double Rotation { get; set; }

        public BoxColliderPoints Points;

        public BoxCollider() { }



        public override object getData() {
            // TODO: Implement this
            var obj = new {
            };
            return obj;
        }

        public override void update(JObject json) {
            // TODO: Implement this
        }


        public void CalculatePoints(Transform transform) {
            Matrix matrix =
                 Matrix.CreateTranslation(Offset.X + (float)transform.X, Offset.Y + (float)transform.Y, 0) 
                * Matrix.CreateRotationZ((float)(MathHelper.ToRadians((float)(Rotation + transform.Rotation))));
             

            float halfWidth = (float)(Width / 2.0);
            float halfHeight = (float)(Height / 2.0);

            Points.a = Vector2.Transform(new Vector2( -halfWidth, -halfHeight), matrix);
            Points.b = Vector2.Transform(new Vector2(  halfWidth, -halfHeight), matrix);
            Points.c = Vector2.Transform(new Vector2(  halfWidth,  halfHeight), matrix);
            Points.d = Vector2.Transform(new Vector2( -halfWidth, halfHeight), matrix);
        }



    }

    public struct BoxColliderPoints {
        public Vector2 a;
        public Vector2 b;
        public Vector2 c;
        public Vector2 d;
    }


    public class CircleCollider : Collider {

        public double Radius { get; set; } = 1.0;


        public CircleCollider() { }



        public override object getData() {
            // TODO: Implement this
            var obj = new {
            };
            return obj;
        }

        public override void update(JObject json) {
            // TODO: Implement this
        }
    }

}
