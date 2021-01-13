
using CaptainCombat.Source.Components;
using CaptainCombat.Source.Utility;
using ECS;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using static ECS.Domain;


namespace CaptainCombat.Source {



    public struct CollisionType {
        ColliderType type1;
        ColliderType type2;

        public CollisionType(ColliderType type1, ColliderType type2) {
            this.type1 = type1;
            this.type2 = type2;
        }

        public static bool operator ==(CollisionType c1, CollisionType c2) {
            if ((object)c1 == null)
                return (object)c2 == null;
            return c1.Equals(c2);
        }

        public static bool operator !=(CollisionType c1, CollisionType c2) {
            return !(c1 == c2);
        }

        public override bool Equals(object obj) {
            if (obj == null || GetType() != obj.GetType())
                return false;

            var other = (CollisionType)obj;
            return (type1 == other.type1 && type2 == other.type2) || (type1 == other.type2 && type2 == other.type1);
        }

        public override int GetHashCode() {
            return type1.GetHashCode() ^ type2.GetHashCode();
        }
    }


    // Returns true if the collision was "consumed"
    public delegate bool CollisionListener(Entity e1, Entity e2);


    public class CollisionController {

        public Dictionary<CollisionType, List<CollisionListener>> listenerMap = new Dictionary<CollisionType, List<CollisionListener>>();

        public void AddListener(ColliderType type1, ColliderType type2, CollisionListener listener) {
            var collisionType = new CollisionType(type1, type2);
            if (!listenerMap.ContainsKey(collisionType))
                listenerMap[collisionType] = new List<CollisionListener>();
            listenerMap[collisionType].Add(listener);
        }


        public List<CollisionListener> GetCollisionListeners(ColliderType type1, ColliderType type2) {
            var collisionType = new CollisionType(type1, type2);
            if (listenerMap.ContainsKey(collisionType))
                return listenerMap[collisionType];
            return null;
        }

        struct ColliderWrapper {

            public Vector2 center;
            public float sortDistance;
            public ColliderType colliderType;

            public Collider collider;
            public Transform transform;
            public Entity entity;

            public void Set(Entity entity, Transform transform, BoxCollider collider) {
                this.collider = collider;
                this.entity = entity;
                this.transform = transform;

                colliderType = collider.ColliderType;

                center = new Vector2((float)(transform.X + collider.Offset.X), (float)(transform.Y + collider.Offset.Y));
                collider.CalculatePoints(transform);

                var diagonal1 = Vector2.Distance(collider.Points.a, collider.Points.c);
                var diagonal2 = Vector2.Distance(collider.Points.b, collider.Points.d);
                double maxDiagonal = diagonal1 > diagonal2 ? diagonal1 : diagonal2;
                sortDistance = (float)(maxDiagonal/1.95);
            
            }

            public void Set(Entity entity, Transform transform, CircleCollider collider) {
                this.collider = collider;
                this.entity = entity;
                this.transform = transform;

                center = new Vector2((float)(transform.X + collider.Offset.X), (float)(transform.Y + collider.Offset.Y));
                sortDistance = (float)collider.Radius;
                colliderType = collider.ColliderType;
            }



            public void Unset() {
                collider = null;
                transform = null;
                entity = null;
            }
        }


        private static bool IsPointInRectangle(ref Vector2 point, ref BoxColliderPoints rect) {
            // 0 ≤ AP·AB ≤ AB·AB and 0 ≤ AP·AD ≤ AD·AD

            var ap = point - rect.a;
            var ab = rect.b - rect.a;
            var ad = rect.d - rect.a;

            var apab = Vector2.Dot(ap, ab);
            var abab = Vector2.Dot(ab, ab);
            var apad = Vector2.Dot(ap, ad);
            var adad = Vector2.Dot(ad, ad);


            return  0 <= apab && apab <= abab && 0 <= apad && apad <= adad;
        }


        private static bool IsPointOnLineSegment(ref Vector2 p, ref Vector2 a, ref Vector2 b) {
            // Check https://stackoverflow.com/questions/328107/how-can-you-determine-a-point-is-between-two-other-points-on-a-line-segment
            var ba = b - a;
            var pa = p - a;

            var crossProduct = ba.Cross(pa);

            const float err = 0.001f;
            if ( crossProduct > err )
                return false;

            var dotProduct = Vector2.Dot(ba, pa);
            if (dotProduct < 0)
                return false;

            var squaredLengthba = ba.LengthSquared();
            if (dotProduct > squaredLengthba)
                return false;

            return true;
        }

        private static bool IsLineInCircle(ref Vector2 c, float r, ref Vector2 a, ref Vector2 b) {
            // https://stackoverflow.com/questions/1073336/circle-line-segment-collision-detection-algorithm

            // Check if either ends are within the circle
            if (Vector2.Distance(c, a) < r) return true;
            if (Vector2.Distance(c, b) < r) return true;

            var ac = c - a;
            var ab = b - a;

            var d = Vector2.Dot(ac, ab) / ab.LengthSquared() * ab;

            if ( Vector2.Distance(d, ac) > r )
                return false;

            var temp = Vector2.Zero;
            if (!IsPointOnLineSegment(ref d, ref temp, ref ab))
                return false;

            return true;
        }


        private static bool BoxBoxCollision(ref ColliderWrapper collider1, ref ColliderWrapper collider2) {
            // TODO: Implement this
            return false;
        }

        private static bool BoxCircleCollision(ref ColliderWrapper box, ref ColliderWrapper circle) {
            var boxCollider = (BoxCollider) box.collider;

            if (IsPointInRectangle(ref circle.center, ref boxCollider.Points))
                return true;

            if (IsLineInCircle(ref circle.center, circle.sortDistance, ref boxCollider.Points.a, ref boxCollider.Points.b))
                return true;
            if (IsLineInCircle(ref circle.center, circle.sortDistance, ref boxCollider.Points.b, ref boxCollider.Points.c))
                return true;
            if (IsLineInCircle(ref circle.center, circle.sortDistance, ref boxCollider.Points.c, ref boxCollider.Points.d))
                return true;
            if (IsLineInCircle(ref circle.center, circle.sortDistance, ref boxCollider.Points.d, ref boxCollider.Points.a))
                return true;

            return false;
        }


        public class Counter {
            public int val = 0;
        }

        public void CheckCollision(Domain domain) {

            // Creating list with "plenty of space"
            ColliderWrapper[] colliders = new ColliderWrapper[domain.entities.Count*2];
            var numColliders = new Counter();

            domain.ForMatchingEntities<Transform, BoxCollider>((e) => {
                var collider = e.GetComponent<BoxCollider>();
                collider.Collided = false;
                if (!collider.Enabled) return;
                colliders[numColliders.val].Set(e, e.GetComponent<Transform>(), collider );
                numColliders.val++;
            });

            domain.ForMatchingEntities<Transform, CircleCollider>((e) => {
                var collider = e.GetComponent<CircleCollider>();
                collider.Collided = false;
                if (!collider.Enabled) return;
                colliders[numColliders.val].Set(e, e.GetComponent<Transform>(), collider);
                numColliders.val++;
            });


            for( int i=0; i<numColliders.val-1; i++) {
                var type1 = colliders[i].collider.GetType();
                for (int j = i + 1; j < numColliders.val; j++) {

                    var collisionType = new CollisionType(colliders[i].colliderType, colliders[j].colliderType);
                    if (!listenerMap.ContainsKey(collisionType)) continue;

                    var listeners = listenerMap[collisionType];

                    if (Vector2.Distance(colliders[i].center, colliders[j].center) > colliders[i].sortDistance + colliders[j].sortDistance)
                        continue;

                    var type2 = colliders[j].collider.GetType();

                    bool collision = false;

                    if (type1 == typeof(CircleCollider) && type2 == typeof(CircleCollider)) {
                        // Circle-circle collision has already been detected when sorting
                        collision = true;
                    }
                    else if (type1 == typeof(BoxCollider) && type2 == typeof(CircleCollider)) {
                        collision = BoxCircleCollision(ref colliders[i], ref colliders[j]);
                    }
                    else if (type1 == typeof(CircleCollider) && type2 == typeof(BoxCollider)) {
                        collision = BoxCircleCollision(ref colliders[j], ref colliders[i]);
                    }
                    else {
                        collision = BoxBoxCollision(ref colliders[i], ref colliders[j]);
                    }

                    if (!collision) continue;

                    // Fire listeners
                    foreach (var listener in listeners) {
                        var confirmed = listener(colliders[i].entity, colliders[j].entity);
                        if( confirmed ) {
                            colliders[i].collider.Collided = true;
                            colliders[j].collider.Collided = true;
                        }
                    }
                }
            }



            /*
            Data:
                Create some wrapper common wrapper class for colliders


            Collect colliders in two lists (circle and box)

            For each collider a
                For each collider b
                    If a == b
                        continue;
                        
                    dist(a, b) > a.sortDistance + b.sortDistance
                        continue;

                    if a is box and b is box
                        box-box collision

                    if a is circle and b is box
                        box-circle collision

                    if a is box and b is circle
                        box-circle collision

                    if a is circle and b is circle
                        circle-circle collision

            */
        }
    
    }
}
