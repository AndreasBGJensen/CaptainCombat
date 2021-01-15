using CaptainCombat.Source.Components;
using ECS;
using System.Collections.Generic;
using static ECS.Domain;


namespace CaptainCombat.Source {

    using CollisionFilter = System.Tuple<ColliderTag, ColliderTag>;

    /// <summary>
    /// Listener which is notified when a collision happens between two entities
    /// </summary>
    public delegate bool CollisionListener(Entity e1, Entity e2);


    /// <summary>
    /// Handles collisions and calls collision listeners mapped to collision filters,
    /// that are local to the controller
    /// </summary>
    public class CollisionController {

        // Map from collision filters (pair of collider types types)
        // to list of collision listeners (submitted by AddListener)
        private Dictionary<CollisionFilter, List<CollisionListener>> listenerMap = new Dictionary<CollisionFilter, List<CollisionListener>>();

        /// <summary>
        /// Add a listener which is notified when any collision happens between two
        /// colliders with the given ColliderTags (the collision filter)
        /// </summary>
        public void AddListener(ColliderTag tag1, ColliderTag tag2, CollisionListener listener) {
            var collisionType = new CollisionFilter(tag1, tag2);
            if (!listenerMap.ContainsKey(collisionType))
                listenerMap[collisionType] = new List<CollisionListener>();
            listenerMap[collisionType].Add(listener);
        }

        /// <summary>
        /// Check collisions between Entities in the given Domain
        /// Collisions are only checked between colliders with types, that have
        /// at least one listener associated with them (added with AddListener)
        /// </summary>
        public void CheckCollisions(Domain domain) {

            // Reset wrapper lists
            foreach (var list in colliderMap.Values)
                list.Count = 0;

            // Add BoxColliders to list
            domain.ForMatchingEntities<Transform, BoxCollider>((e) => {
                var collider = e.GetComponent<BoxCollider>();
                collider.Collided = false;
                if (!collider.Enabled || collider.Tag == null) return;

                if (!colliderMap.ContainsKey(collider.Tag))
                    colliderMap[collider.Tag] = new ColliderDataList();

                var list = colliderMap[collider.Tag];
                list.Expand();
                list.Elements[list.Count].Set(e, e.GetComponent<Transform>(), collider);
                list.Count++;
            });

            // Add CircleColliders to list
            domain.ForMatchingEntities<Transform, CircleCollider>((e) => {
                var collider = e.GetComponent<CircleCollider>();
                collider.Collided = false;
                if (!collider.Enabled || collider.Tag == null) return;

                if (!colliderMap.ContainsKey(collider.Tag))
                    colliderMap[collider.Tag] = new ColliderDataList();

                var list = colliderMap[collider.Tag];
                list.Expand();
                list.Elements[list.Count].Set(e, e.GetComponent<Transform>(), collider);
                list.Count++;
            });

            // Check registered collision pairs
            foreach (var pair in listenerMap) {
                if (!colliderMap.ContainsKey(pair.Key.Item1)) continue;
                if (!colliderMap.ContainsKey(pair.Key.Item2)) continue;
                CheckTypeCollision(colliderMap[pair.Key.Item1], colliderMap[pair.Key.Item2], pair.Value);
            }
        }


        /// <summary>
        /// Checks collision between two list of colliders, 
        /// and fire the given list of Listeners if a collision
        /// is registed
        /// </summary>
        private void CheckTypeCollision(ColliderDataList sourceColliders, ColliderDataList targetColliders, List<CollisionListener> listeners) {
            for (int i = 0; i < sourceColliders.Count; i++) {
                var source = sourceColliders.Elements[i];
                var sourceType = source.collider.GetType();
                for (int j = 0; j < targetColliders.Count; j++) {
                    var target = targetColliders.Elements[j];

                    // Initial radius sorting
                    if ( (target.center - source.center).Length() > source.sortDistance + target.sortDistance)
                        continue;

                    var targetType = target.collider.GetType();

                    // Run approriate collision function
                    bool collision;
                    // Circle-Circle
                    if (sourceType == typeof(CircleCollider) && targetType == typeof(CircleCollider)) {
                        // Circle-circle collision has already been detected when sorting
                        collision = true;
                    }
                    // Box-Circle
                    else if (sourceType == typeof(BoxCollider) && targetType == typeof(CircleCollider)) {
                        collision = BoxCircleCollision(ref source, ref target);
                    }
                    // Circle-Box
                    else if (sourceType == typeof(CircleCollider) && targetType == typeof(BoxCollider)) {
                        collision = BoxCircleCollision(ref target, ref source);
                    }
                    // Box-box
                    else {
                        collision = BoxBoxCollision(ref source, ref target);
                    }

                    if (!collision) continue;

                    // Fire listeners
                    foreach (var listener in listeners) {
                        var confirmed = listener(source.entity, target.entity);
                        // The listener can implement its own logic to "decline" collision
                        if (confirmed) {
                            // Set flag for rendering collisions
                            source.collider.Collided = true;
                            target.collider.Collided = true;
                        }
                    }
                }
            }
        }


        // Maps a ColliderTag to a a list of Collider data
        // Stored an attribute to prevent reconstruction on every frame (for performance)
        private Dictionary<ColliderTag, ColliderDataList> colliderMap = new Dictionary<ColliderTag, ColliderDataList>();


        /// <summary>
        /// Struct to group necessary data about an Entity's collider
        /// which allows for faster computations
        /// </summary>
        private struct ColliderData {
            public Vector center;
            public float sortDistance;
            public ColliderTag colliderTag;

            public Collider collider;
            public Transform transform;
            public Entity entity;

            public void Set(Entity entity, Transform transform, BoxCollider collider) {
                this.collider = collider;
                this.entity = entity;
                this.transform = transform;
                colliderTag = collider.Tag;

                center = transform.Position;
                collider.CalculatePoints(transform);

                // Diagonal distances
                var diagonal1 = (collider.Points.c - collider.Points.a).Length();
                var diagonal2 = (collider.Points.d - collider.Points.b).Length();
                double maxDiagonal = diagonal1 > diagonal2 ? diagonal1 : diagonal2;
                sortDistance = (float)(maxDiagonal/1.95);
            
            }

            public void Set(Entity entity, Transform transform, CircleCollider collider) {
                this.collider = collider;
                this.entity = entity;
                this.transform = transform;
                colliderTag = collider.Tag;

                center = transform.Position;
                sortDistance = (float)collider.Radius;
            }
        }


        /// <summary>
        /// Wrapper for an array of ColliderData to allow
        /// for faster iteration and construction of ColliderData
        /// structs (less copying)
        /// </summary>
        private class ColliderDataList {
            public ColliderData[] Elements = new ColliderData[64];
            public int Count = 0;

            // Double the size of the array, if the array is full
            public void Expand() {
                if (Count < Elements.Length) return;
                var newList = new ColliderData[Elements.Length * 2];
                for(int i=0; i<Elements.Length; i++) {
                    newList[i] = Elements[i];
                }
                Elements = newList;
            }
        }


// ===========================================================================================================================================================
// Intersection functions

         static bool IsPointInBoxCollider(ref Vector point, ref BoxColliderPoints rect) {
            var ap = point - rect.a;
            var ab = rect.b - rect.a;
            var ad = rect.d - rect.a;

            var apab = ap.Dot(ab);
            var abab = ab.Dot(ab);
            var apad = ap.Dot(ad);
            var adad = ad.Dot(ad);

            return 0 <= apab && apab <= abab && 0 <= apad && apad <= adad;
        }


        private static bool IsPointOnLineSegment(ref Vector p, ref Vector a, ref Vector b) {
            // Check https://stackoverflow.com/questions/328107/how-can-you-determine-a-point-is-between-two-other-points-on-a-line-segment
            var ba = b - a;
            var pa = p - a;

            var crossProduct = ba.Cross(pa);

            const float err = 0.001f;
            if (crossProduct > err)
                return false;

            var dotProduct = ba.Dot(pa);
            if (dotProduct < 0)
                return false;

            var squaredLengthba = ba.LengthSquared();
            if (dotProduct > squaredLengthba)
                return false;

            return true;
        }

        private static bool IsLineInCircle(ref Vector c, float r, ref Vector a, ref Vector b) {
            // https://stackoverflow.com/questions/1073336/circle-line-segment-collision-detection-algorithm

            // Check if either ends are within the circle
            if ( c.DistanceTo(a) < r) return true;
            if ( c.DistanceTo(b) < r) return true;

            var ac = c - a;
            var ab = b - a;

            var d = ac.Dot(ab) / ab.LengthSquared() * ab;

            if ( d.DistanceTo(ac) > r)
                return false;

            var temp = Vector.Zero;
            if (!IsPointOnLineSegment(ref d, ref temp, ref ab))
                return false;

            return true;
        }


        private static bool BoxBoxCollision(ref ColliderData collider1, ref ColliderData collider2) {
            var box1 = (BoxCollider)collider1.collider;
            var box2 = (BoxCollider)collider2.collider;

            if (IsPointInBoxCollider(ref box1.Points.a, ref box2.Points)) return true;
            if (IsPointInBoxCollider(ref box1.Points.b, ref box2.Points)) return true;
            if (IsPointInBoxCollider(ref box1.Points.c, ref box2.Points)) return true;
            if (IsPointInBoxCollider(ref box1.Points.d, ref box2.Points)) return true;

            if (IsPointInBoxCollider(ref box2.Points.a, ref box1.Points)) return true;
            if (IsPointInBoxCollider(ref box2.Points.b, ref box1.Points)) return true;
            if (IsPointInBoxCollider(ref box2.Points.c, ref box1.Points)) return true;
            if (IsPointInBoxCollider(ref box2.Points.d, ref box1.Points)) return true;

            return false;
        }


        private static bool BoxCircleCollision(ref ColliderData box, ref ColliderData circle) {
            var boxCollider = (BoxCollider)box.collider;

            if (IsPointInBoxCollider(ref circle.center, ref boxCollider.Points))
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
    }
}
