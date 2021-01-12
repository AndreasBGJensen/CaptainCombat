
using CaptainCombat.Source.Components;
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
                double width = Math.Abs(collider.Width);
                double height = Math.Abs(collider.Height);
                double maxDimension = width > height ? width : height;
                sortDistance = (float)(collider.Offset.Length() + maxDimension / 2.0);
            
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


        private static bool BoxBoxCollision(ref ColliderWrapper collider1, ref ColliderWrapper collider2) {
            throw new NotImplementedException();
        }


        private static bool BoxCircleCollision(ref ColliderWrapper collider1, ref ColliderWrapper collider2) {
            throw new NotImplementedException();
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
