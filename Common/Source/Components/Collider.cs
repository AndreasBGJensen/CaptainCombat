using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using static CaptainCombat.Common.Domain;

namespace CaptainCombat.Common.Components {

    /// <summary>
    /// ColliderTags are used to group Collider instances together
    /// and to limit the number of collision checks, by only checking
    /// for collision between paricular ColliderTypes
    /// </summary>
    public class ColliderTag {
        private static Dictionary<string, ColliderTag> all = new Dictionary<string, ColliderTag>();

        public string Tag { get; }

        public ColliderTag(string tag) {
            // TODO: Fix this
            //if (all.ContainsKey(tag))
            //    throw new ArgumentException($"ColliderType with tag '{tag} already exists");
            Tag = tag;
            //all.Add(tag, this);
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


        public override void OnUpdate(Component component) {
            var c = (Collider)component;
            Enabled = c.Enabled;
            Tag = c.Tag;
            Collided = c.Collided;
        }
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
        [JsonIgnore]
        public BoxColliderPoints Points;

        public override void OnUpdate(Component component) {
            base.OnUpdate(component);

            var c = (BoxCollider)component;
            Width = c.Width;
            Height = c.Height;
            Rotation = c.Rotation;
        }

        /// <summary>
        /// Constructs the Points
        /// </summary>
        public void CalculatePoints(Transform transform) {
            Matrix matrix = Matrix.CreateRotation(Rotation + transform.Rotation) * Matrix.CreateTranslation(transform.Position);

            float halfWidth = (float)(Width / 2.0);
            float halfHeight = (float)(Height / 2.0);

            Points.a = new Vector(-halfWidth, -halfHeight) * matrix;
            Points.b = new Vector( halfWidth,  -halfHeight) * matrix;
            Points.c = new Vector( halfWidth,   halfHeight) * matrix;
            Points.d = new Vector(-halfWidth,  halfHeight) * matrix;
        }
    }

    public struct BoxColliderPoints {
        public Vector a;
        public Vector b;
        public Vector c;
        public Vector d;
    }


    /// <summary>
    /// Collider which is represented by a Radius only
    /// </summary>
    public class CircleCollider : Collider {

        public double Radius { get; set; } = 1.0;

        public CircleCollider() { }

        public CircleCollider(ColliderTag tag, double radius) {
            Tag = tag;
            Radius = radius;
        }

        public override void OnUpdate(Component component) {
            var c = (CircleCollider)component;
            Radius = c.Radius;
        }
    }

}
