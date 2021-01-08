using System;
using System.Collections.Generic;
using System.Dynamic;

namespace ECS {


    public class Domain {

        // Id generators
        private readonly IdGenerator entityIdGenerator = new IdGenerator();
        private readonly IdGenerator componentIdGenerator = new IdGenerator();

        // List of entities registered and finalized in this domain
        public readonly List<Entity> entities = new List<Entity>();

        // List of entities that has been registered, but not finalized yet
        // They will be finalized (added to entities list), when domain is
        // cleaned
        private readonly List<Entity> entitiesToAdd = new List<Entity>();

        // Mapping of component ids to components
        private readonly Dictionary<uint, Component> components = new Dictionary<uint, Component>();

        public delegate void EntityCallback(Entity e);


        /// <summary>
        /// Runs the given callback for each entity, which has the given
        /// Components
        /// </summary>
        /// <param name="componentTypes">List of Types to check for (must inherit from Component)</param>
        /// <param name="callback">Entity callback</param>
        public void ForMatchingEntities(Type[] componentTypes, EntityCallback callback) {
            foreach (var entity in entities) {

                // Check if entity matches the component type pattern
                bool match = true;
                foreach (var type in componentTypes) {
                    var component = entity.GetComponent(type);
                    if (component == null || !component.Matchable) {
                        match = false;
                        break;
                    }
                }

                if (match) callback(entity);
            }
        }

        // Below are some "syntactic sugar" for the forMatchingEntities call

        public void ForMatchingEntities<C1>(EntityCallback callback) where C1 : Component {
            ForMatchingEntities(new Type[] { typeof(C1) }, callback);
        }

        public void ForMatchingEntities<C1, C2>(EntityCallback callback) where C1 : Component where C2 : Component {
            ForMatchingEntities(new Type[] { typeof(C1), typeof(C2) }, callback);
        }

        public void ForMatchingEntities<C1, C2, C3>(EntityCallback callback) where C1 : Component where C2 : Component where C3 : Component {
            ForMatchingEntities(new Type[] { typeof(C1), typeof(C2), typeof(C3) }, callback);
        }

        public void ForMatchingEntities<C1, C2, C3, C4>(EntityCallback callback) where C1 : Component where C2 : Component where C3 : Component where C4 : Component {
            ForMatchingEntities(new Type[] { typeof(C1), typeof(C2), typeof(C3), typeof(C4) }, callback);
        }

        public void Clean() {

            // Add new entities
            foreach (var entity in entitiesToAdd)
                entities.Add(entity);

            entitiesToAdd.Clear();

            // Clean entities
            List<Entity> entitiesToRemove = new List<Entity>();
            foreach (var entity in entities) {
                bool shouldBeRemoved = entity.clean();
                if (shouldBeRemoved)
                    entitiesToRemove.Add(entity);
            }

            // Remove entities
            foreach (var entity in entitiesToRemove) {
                entityIdGenerator.releaseId(entity.Id);
                entities.Remove(entity);
            }

        }

        
        public List<Component> getAllComponents()
        {
            List<Component> allComponents = new List<Component>();

            foreach(Entity entity in entities)
            {
                foreach (Component component in entity.newComponents)
                {
                    allComponents.Add(component); 
                }
            }

            return allComponents; 
        }


        private uint registerComponent<C>(C component) where C : Component {
            uint id = componentIdGenerator.getId();
            components[id] = component;

            return id;
        }

        private void unregisterComponent(Component component) {
            componentIdGenerator.releaseId(component.Id);
            components.Remove(component.Id);
        }


        private uint registerEntity(Entity entity) {
            entitiesToAdd.Add(entity);
            return entityIdGenerator.getId();
        }



        // ========================================================================================================================================================= 

        public class Entity {

            public Domain Domain { get; }
            public uint Id { get; }
            public bool Deleted { get; private set; }

            private bool shouldBeDeleted = false;

            private Dictionary<Type, Component> components = new Dictionary<Type, Component>();

            // List of components that were added since last clean
            public readonly List<Component> newComponents = new List<Component>();

            private readonly Dictionary<Type, Component> componentsToRemove = new Dictionary<Type, Component>();

            /// <summary>
            /// Construct a new Entity within the given Domain
            /// </summary>
            /// <param name="domain"></param>
            public Entity(Domain domain) {
                Domain = domain;
                Id = domain.registerEntity(this);
            }


            /// <summary>
            /// Construct a new Component of the given Type, and bind it to the Entity.
            /// Only one Component of each type can be bound to an Entity
            /// The new Component will not partake in Entity matching in the Domain,
            /// before the Domain has been cleaned.
            /// </summary>
            /// <typeparam name="C">Class which inherits the Component class</typeparam>
            /// <param name="constructionArguments">List of arguments for constructor (excluding the first Entity argument)</param>
            /// <returns>The newly construct Component</returns>
            public C AddComponent<C>(params object[] constructionArguments) where C : Component {
                if (Deleted) throw new InvalidOperationException("Entity has been deleted");

                Type type = typeof(C);

                if (components.ContainsKey(type))
                    throw new InvalidOperationException($"Entity already has component {type.Name}");

                // Combine passed args with Entity arguments
                object[] args = new object[constructionArguments.Length + 1];
                args[0] = this; // Set Entity as first parameter
                for (int i = 0; i < constructionArguments.Length; i++)
                    args[i + 1] = constructionArguments[i];

                // Create Component instance
                C component = (C)Activator.CreateInstance(typeof(C), args);

                newComponents.Add(component);
                components.Add(type, component);

                return component;
            }


            /// <summary>
            /// Retrieve the Component of the given Type, or null if the
            /// Entity does not have the given Component
            /// </summary>
            /// <typeparam name="C">Type of the Component to retrieve</typeparam>
            public C GetComponent<C>() where C : Component {
                // Convert to Type object and forward to other method
                return (C)GetComponent(typeof(C));
            }


            /// <summary>
            /// Retrieve the Component of the given Type, or null if the
            /// Entity does not have the given Component
            /// </summary>
            public Component GetComponent(Type type) {
                if (Deleted) throw new InvalidOperationException("Entity has been deleted");
                if (components.ContainsKey(type))
                    return components[type];
                return null;
            }


            /// <summary>
            /// Signals that this Entity should be deleted
            /// The Entity will not actually be deleted before the Domain is cleaned,
            /// and will match queries until that happens
            /// </summary>
            public void Delete() {
                if (Deleted) throw new InvalidOperationException("Entity has been deleted");
                shouldBeDeleted = true;
            }


            /// <summary>
            /// Not recommend to call this function manually (Domain calls it
            /// when its cleaned.
            /// </summary>
            public bool clean() {
                if (Deleted) throw new InvalidOperationException("Entity has been deleted");

                if (shouldBeDeleted) {
                    foreach (var pair in components)
                        Domain.unregisterComponent(pair.Value);
                    Deleted = true;
                    return true;
                }
                else {
                    foreach (var component in newComponents)
                        component.Matchable = true;

                    foreach (var pair in componentsToRemove) {
                        components.Remove(pair.Key);
                        Domain.unregisterComponent(pair.Value);
                    }
                    return false;
                }
            }
        }


        // ========================================================================================================================================================= 

        public abstract class Component {

            public Entity Entity { get; }
            public Domain Domain { get; }
            public uint Id { get; }

            // This flag is set to true, if it the component has been "finalized" in the domain
            // Should only be set by the domain
            public bool Matchable { get; set; } = false;

            public Component(Entity entity) {
                Entity = entity;
                Domain = entity.Domain;
                Id = Domain.registerComponent(this);
            }

            public abstract Object getData(); 

        }


        // ========================================================================================================================================================= 

        private class IdGenerator {
            // 0 is "unknown" id
            private uint nextId = 1;

            // List of ids that has been generated, but freed again
            private Queue<uint> readyIds = new Queue<uint>();


            public uint getId() {
                if (readyIds.Count > 0)
                    return readyIds.Dequeue();
                return nextId++;
            }

            public void releaseId(uint id) {
                readyIds.Enqueue(id);
            }

        }

    }
}
