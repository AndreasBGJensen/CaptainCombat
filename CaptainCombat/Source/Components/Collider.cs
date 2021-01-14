using Microsoft.Xna.Framework;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using static ECS.Domain;

namespace CaptainCombat.Source.Components {

    /// <summary>
    /// ColliderTags are used to group Collider instances together
    /// and to limit the number of collision checks, by only checking
    /// for collision between paricular ColliderTypes
    /// </summary>
    public class ColliderTag {
        private static Dictionary<string, ColliderTag> all = new Dictionary<string, ColliderTag>();

        public string Tag { get; }

        public ColliderTag(string tag) {
            if (all.ContainsKey(tag))
                throw new ArgumentException($"ColliderType with tag '{tag} already exists");
            Tag = tag;
            all.Add(tag, this);
        }

        public static ColliderTag Get(string tag) {
            if (tag == "") return null;
            if (!all.ContainsKey(tag))
                throw new NullReferenceException($"No ColliderType with tag '{tag}' exists");
            return all[tag];
        }

        public static bool operator ==(ColliderTag c1, ColliderTag c2) {
            if ((object)c1 == null)
                return (object)c2 == null;
            return c1.Equals(c2);
        }

        public static bool operator !=(ColliderTag c1, ColliderTag c2) {
            return !(c1 == c2);
        }

        public override bool Equals(object obj) {
            if (obj == null || GetType() != obj.GetType())
                return false;
            var c1 = (ColliderTag)obj;
            return Tag == c1.Tag;
        }

        public override int GetHashCode() {
            return Tag.GetHashCode();
        }
    }


    public abstract class Collider : Component {

        /// <summary>
        /// Only enabled Colliders will trigger collisions 
        /// </summary>
        public bool Enabled { get; set; } = true;

        /// <summary>
        /// The type of this Collider (NOT the class type, i.e. BoxCollider)
        /// If null, the Collider will be ignored
        /// </summary>
        public ColliderTag Tag { get; set; } = null;

        /// <summary>
        /// Flag to denote that this collider collided with something
        /// in the previous frame. Only used for Rendering the Colliders
        /// </summary>
        public bool Collided { get; set; }
    }


    /// <summary>
    /// A rectanglecollider, which may rotated
    /// </summary>
    public class BoxCollider : Collider {

        public double Width { get; set; }
        public double Height { get; set; }

        public double Rotation { get; set; }

        /// <summary>
        /// The 4 corners of the Collider in world coordinates
        /// They should not be set from outside this class
        /// </summary>
        public BoxColliderPoints Points;

        /// <summary>
        /// Constructs the Points
        /// </summary>
        public void CalculatePoints(Transform transform) {
            Matrix matrix =
                   Matrix.CreateRotationZ((float)(MathHelper.ToRadians((float)(Rotation + transform.Rotation))))
                 * Matrix.CreateTranslation((float)transform.X, (float)transform.Y, 0);

            float halfWidth = (float)(Width / 2.0);
            float halfHeight = (float)(Height / 2.0);

            Points.a = Vector2.Transform(new Vector2(-halfWidth, -halfHeight), matrix);
            Points.b = Vector2.Transform(new Vector2(halfWidth, -halfHeight), matrix);
            Points.c = Vector2.Transform(new Vector2(halfWidth, halfHeight), matrix);
            Points.d = Vector2.Transform(new Vector2(-halfWidth, halfHeight), matrix);
        }


        public override object getData() {
            var obj = new {
                enabled = Enabled,
                tag = Tag == null ? "" : Tag.Tag,
                collided = Collided,
                width = Width,
                height = Height,
                rotation = Rotation
            };
            return obj;
        }

        public override void update(JObject json) {
            Enabled = (bool)json.SelectToken("enabled");
            Tag = ColliderTag.Get((string)json.SelectToken("tag"));
            Collided = (bool)json.SelectToken("collided");
            Width = (double)json.SelectToken("width");
            Height = (double)json.SelectToken("height");
            Rotation = (double)json.SelectToken("rotation");
        }
    }

    public struct BoxColliderPoints {
        public Vector2 a;
        public Vector2 b;
        public Vector2 c;
        public Vector2 d;
    }


    /// <summary>
    /// Collider which is represented by a Radius only
    /// </summary>
    public class CircleCollider : Collider {

        public double Radius { get; set; } = 1.0;

        public CircleCollider() { }

        public CircleCollider(ColliderTag tag, double radius) {
            Tag = Tag;
            Radius = radius;
        }

        public override object getData() {
            var obj = new {
                enabled = Enabled,
                tag = Tag == null ? "" : Tag.Tag,
                collided = Collided,
                radius = Radius,
            };
            return obj;
        }

        public override void update(JObject json) {
            Enabled = (bool)json.SelectToken("enabled");
            Tag = ColliderTag.Get((string)json.SelectToken("tag"));
            Collided = (bool)json.SelectToken("collided");
            Radius = (double)json.SelectToken("radius");
        }
    }

}
