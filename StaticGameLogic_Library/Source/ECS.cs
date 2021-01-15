
using dotSpace.Interfaces.Space;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using StaticGameLogic_Library.Singletons;
using StaticGameLogic_Library.Source.Utility;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Reflection;

namespace StaticGameLogic_Library.Source.ECS {

    public class Domain {

        // Id generators
        private readonly IdGenerator entityIdGenerator = new IdGenerator();
        private readonly IdGenerator componentIdGenerator = new IdGenerator();

        // List of entities registered and finalized in this domain
        public readonly List<Entity> entities = new List<Entity>();

        // List of entities that has been registered, but not commited yet
        // They will be finalized (added to entities list), when domain is
        // cleaned
        private readonly List<Entity> entitiesToAdd = new List<Entity>();

        private readonly ConcurrentDictionary<GlobalId, Entity> registeredEntities = new ConcurrentDictionary<GlobalId, Entity>();

        // Mapping of component ids to components
        private readonly ConcurrentDictionary<GlobalId, Component> registeredComponents = new ConcurrentDictionary<GlobalId, Component>();

        public delegate void EntityCallback(Entity e);


        public Entity GetEntity(GlobalId id) {
            Entity entity;
            var found = registeredEntities.TryGetValue(id, out entity);
            return found ? entity : null;
        }

        
        /// <summary>
        /// Runs the given callback for all entities in the Domain, which
        /// has been committed
        /// </summary>
        /// <param name="callback"></param>
        public void ForAllEntities(EntityCallback callback) {
            foreach( var entity in entities ) {
                callback(entity);
            }
        }


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
                    if (component == null || !component.Commited) {
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


        public Dictionary<uint, ulong> updateIdMap = new Dictionary<uint, ulong>();

        public void update(IEnumerable<ITuple> gameData) {

            // TODO: Clean up this method

            HashSet<GlobalId> updatedComponents = new HashSet<GlobalId>();

            Dictionary<uint, ulong> newIdMap = new Dictionary<uint, ulong>();
            foreach (var pair in updateIdMap)
                newIdMap.Add(pair.Key, pair.Value);

            List<ITuple> sortedComponents = new List<ITuple>();
            foreach (var component in gameData) {
                var clientId = (uint)(int)component[1];

                if (clientId == Connection.Instance.User_id) continue;

                var componentId = (uint)(int)component[2];
                var updateId = ulong.Parse((string)component[5]);

                updatedComponents.Add(new GlobalId(clientId, componentId));

                if (!updateIdMap.TryGetValue(clientId, out ulong currentUpdateId)) {
                    currentUpdateId = 0;
                }

                if (updateId <= currentUpdateId) continue;

                sortedComponents.Add(component);
                newIdMap[clientId] = updateId;
            }

            updateIdMap = newIdMap;

            foreach (ITuple data in sortedComponents) {
                var client_id = (uint)(int)data[1];
                var component_id = (uint)(int)data[2];
                var entity_id = (uint)(int)data[3];

                if (Connection.Instance.User_id == client_id)
                    continue;

                GlobalId global_entity_id = new GlobalId(client_id, entity_id); 

                Entity current_entity;
                if (registeredEntities.ContainsKey(global_entity_id))
                {
                    current_entity = registeredEntities[global_entity_id]; 
                }
                else
                {
                    current_entity = new Entity(this, global_entity_id); 
                }

                GlobalId global_compotent_id = new GlobalId(client_id, component_id);

                var componentTypeIdentifier = (string)data[0];
                var newComponentData = Component.CreateComponentFromJson(componentTypeIdentifier, (string)data[4]);
                
                if (registeredComponents.ContainsKey(global_compotent_id)) {
                    if( newComponentData.SyncMode == Component.SynchronizationMode.UPDATE ) {
                        var currentComponent = registeredComponents[global_compotent_id];
                        currentComponent.Update(newComponentData);
                    }
                }
                else
                {
                    // Create new component
                    current_entity.AddComponent(newComponentData);
                }

            }


            // Clean up remote components that no longer exists
            foreach(var pair in registeredComponents) {
                var id = pair.Key;

                // Skip local components
                if (id.clientId == Connection.Instance.User_id) continue;

                // Component was updated, so it still exists
                if (updatedComponents.Contains(id)) continue;
                
                // Component no longer exists remotely, so we delete it
                var component = pair.Value;
                component.Delete();                
            }
        }




        public void Clean() {

            // Add new entities
            foreach (var entity in entitiesToAdd)
                entities.Add(entity);

            entitiesToAdd.Clear();

            // Clean entities
            List<Entity> entitiesToRemove = new List<Entity>();
            foreach (var entity in entities) {
                bool shouldBeRemoved = entity.Clean();
                if (shouldBeRemoved) {
                    entitiesToRemove.Add(entity);
                }
            }

            // Remove entities
            foreach (var entity in entitiesToRemove) {
                entityIdGenerator.Release(entity.Id.objectId);
                entities.Remove(entity);
                registeredEntities.TryRemove(entity.Id, out _);
            }

        }


        /// <summary>
        /// Executes the given callback for all Components in the Domain
        /// that belongs to the local Client, and that are matchable (meaning
        /// that they are finalized)
        /// </summary>
        public void ForLocalComponents(ComponentCallback callback) {
            foreach( var pair in registeredComponents ) {
                var component = pair.Value;
                if (component.Commited && component.IsLocal )
                    callback(component);
            }
        }
        public delegate void ComponentCallback(Component component);


        private GlobalId registerComponent<C>(C component, GlobalId id) where C : Component {
            if (registeredComponents.ContainsKey(id))
                throw new ArgumentException("Component already exists with given ID");
            registeredComponents[id] = component;
            return id;
        }


        private GlobalId registerComponent<C>(C component, uint clientId) where C : Component {
            return registerComponent(component, new GlobalId(clientId, componentIdGenerator.Get()));
        }

        
        private void unregisterComponent(Component component) {
            componentIdGenerator.Release(component.Id.objectId);   
            if (!registeredComponents.TryRemove(component.Id, out _)){
                throw new InvalidOperationException("Component id does not exit"); 
            }
        }


        private GlobalId registerEntity(Entity entity, GlobalId id) {
            if (registeredEntities.ContainsKey(id))
                throw new ArgumentException("Entity already exists with given ID");
            entitiesToAdd.Add(entity);
            registeredEntities.TryAdd(id, entity);
            return id;
        }


        private GlobalId registerEntity(Entity entity, uint clientId) {
            return registerEntity(entity, new GlobalId(clientId, entityIdGenerator.Get()));
        }



        // ========================================================================================================================================================= 

        public class Entity {

            public Domain Domain { get; }

            public GlobalId Id { get; }

            public uint ClientId { get => Id.clientId; }

            public bool Deleted { get; private set; }

            public bool IsLocal { get => Id.clientId == Connection.Instance.User_id; }

            // If true, it signals that the component should delete itself
            // the next time it is cleaned
            private bool shouldBeDeleted = false;

            /// <summary>
            /// Dictionary of ALL components within this Entity, including components
            /// that are about to be added and removed
            /// </summary>
            private Dictionary<Type, Component> components = new Dictionary<Type, Component>();

            // List of components that were added since last clean
            public readonly List<Component> componentsToAdd = new List<Component>();

            private readonly HashSet<Type> componentsToRemove = new HashSet<Type>();


            /// <summary>
            /// Create an Entity within the Domain with the
            /// local Client's ID and a new, unique object id
            /// </summary>
            /// <param name="domain"></param>
            public Entity(Domain domain) {
                Domain = domain;
                // Get local client id if clientId is set to 0
                Id = domain.registerEntity(this, (uint)Connection.Instance.User_id);
            }


            /// <summary>
            /// Creates am new Entity within the given Domain, that has a predefined ID
            /// This may eventually throw an exception in case an Entity with
            /// the given ID already exists, when the Entity is commited
            /// </summary>
            /// <param name="domain"></param>
            public Entity(Domain domain, GlobalId id) {
                Domain = domain;
                Id = domain.registerEntity(this, id);
            }


            /// <summary>
            /// Adds the given Component to this Entity
            /// The Entity will not partake in matches, before the Entity
            /// has been cleaned.
            /// </summary>
            /// <param name="component">Component to add to this Entity</param>
            /// <returns>The same object that was passed to the function</returns>
            public T AddComponent<T>(T component) where T : Component {
                // This method is just a wrapper, to allow the return
                // of the component in its actual type rather than the
                // base Component class
                return (T) AddComponent((Component)component);
            }


            /// <summary>
            /// Adds the given Component to this Entity
            /// The Entity will not partake in matches, before the Entity
            /// has been cleaned.
            /// </summary>
            /// <param name="component">Component to add to this Entity</param>
            /// <returns>The same object that was passed to the function</returns>
            public Component AddComponent(Component component) {
                if (Deleted) throw new InvalidOperationException("Entity has been deleted");

                var type = component.GetType();

                if (components.ContainsKey(type))
                    throw new InvalidOperationException($"Entity already has component {type.Name}");

                component.Entity = this;

                componentsToAdd.Add(component);
                components.Add(type, component);

                return component;
            }


            public void RemoveComponent(Component component) {
                var type = component.GetType();
                Component matched;
                var found = components.TryGetValue(type, out matched);
                if( !found )
                    throw new NullReferenceException($"Entity does not have component of type '{type.Name}'");
                if( matched != component )
                    throw new NullReferenceException($"Entity's component of type '{type.Name}' does not match provided argument");
                RemoveComponent(type);
            }


            public void RemoveComponent<C>() where C : Component {
                RemoveComponent(typeof(C));
            }


            public void RemoveComponent(Type type) {
                if (Deleted) throw new InvalidOperationException("Entity has been deleted");

                if ( !components.ContainsKey(type) )
                    throw new NullReferenceException($"Entity does not have component of type '{type.Name}'");
                              
                componentsToRemove.Add(type);
            }


            /// <summary>
            /// Sets the SyncMode for all Components that has been added to
            /// this Entity. The Entity does not have to be cleaned, after
            /// adding components for this to take effect on the newly added
            /// components.
            /// </summary>
            public void SetSyncMode(Component.SynchronizationMode syncMode) {
                foreach (var component in components)
                    component.Value.SyncMode = syncMode;
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
            /// <returns>True if the Entity was deleted</returns>
            public bool Clean() {
                if (Deleted) throw new InvalidOperationException("Entity has been deleted");

                bool deleted;
                if (shouldBeDeleted) {
                    // Unregister all components before deletion
                    foreach (var pair in components)
                        Domain.unregisterComponent(pair.Value);
                    Deleted = true;
                    deleted = true;
                }
                else {
                    // Create new components
                    foreach (var component in componentsToAdd)
                        component.Commited = true;

                    // Delete components
                    foreach (var componentType in componentsToRemove) {
                        var component = components[componentType];
                        components.Remove(componentType);
                        Domain.unregisterComponent(component);
                    }
                    deleted = false;
                }

                componentsToAdd.Clear();
                componentsToRemove.Clear();

                return deleted;
            }

        }


        //public void MakeGlobal() {
        //    foreach (var pair in components) {
        //        pair.Value.ShouldSynchronize = false;
        //    }
        //}


        //public void MakeLocal() {
        //    foreach( var pair in components ) {
        //        pair.Value.ShouldSynchronize = true;
        //    }
        //}


        // ========================================================================================================================================================= 
        public abstract class Component {

            [JsonIgnore]
            private Entity entity = null;
            [JsonIgnore]
            public Entity Entity { 
                get => entity;
                
                // Assign the component to the Entity
                set {
                    if (entity != null)
                        throw new ArgumentException("Component already been assigned to an Entity");
                    
                    // Connect component with Entity and its Domain
                    entity = value;
                    Domain = entity.Domain;

                    // Register component in the Domain
                    if (id != GlobalId.NULL) {
                        if (entity.ClientId != Id.clientId)
                            throw new ArgumentException("Component's client ID does not match with the Entity's client id");
                        Domain.registerComponent(this, Id);
                    }
                    else {
                        id = Domain.registerComponent(this, entity.ClientId);
                    }
                }
            }

            [JsonIgnore]
            public Domain Domain { get; private set; }

            private GlobalId id;
            public GlobalId Id {
                get => id;
                set {
                    if (entity != null)
                        throw new ArgumentException("Component already been assigned to an Entity");
                    id = value;
                }
            }

            [JsonIgnore]
            public uint ClientId { get => Id.clientId; }

            public SynchronizationMode SyncMode { get; set; } = SynchronizationMode.UPDATE;

            /// <summary>
            /// Whether or not this Component is owned by this local Client
            /// </summary>
            [JsonIgnore]
            public bool IsLocal { get => ClientId == Connection.Instance.User_id; }

            // This flag is set to true, if it the component has been "finalized" in the domain
            // Should only be set by the domain
            [JsonIgnore]
            public bool Commited { get; set; } = false;


            public Component() {}
          
            public void Update(Component component) {
                SyncMode = component.SyncMode;
                if( entity == null ) Id = component.Id;
                OnUpdate(component);
            }

            public abstract void OnUpdate(Component component);
            
            /// <summary>
            /// Deletes the component and remove it from its Entity
            /// </summary>
            public void Delete() {
                entity.RemoveComponent(this);
            }


            // - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -
            // Setup type mapping (mapping of some identifier to Component child classes)

            private static readonly Dictionary<string, Type> typeMap = new Dictionary<string, Type>();

            // Constructs a new Component of the type identified by the
            // given identifier
            public static Component CreateComponentFromJson(string identifier, string json) {
                if (!typeMap.ContainsKey(identifier))
                    throw new ArgumentException($"Component type with identifier '{identifier}' does not exist");   
                return (Component)JsonConvert.DeserializeObject(json, typeMap[identifier]);
            }

            // Analyze which classes inherit from Component, and
            // add them to the 'typeMap' with their full name as
            // an identifier
            static Component() {
                foreach (var type in Assembly.GetAssembly(typeof(Component)).GetTypes()) {
                    if (type.IsSubclassOf(typeof(Component)) && !type.IsAbstract ) {
                        var identifier = type.FullName;
                        if (type.GetConstructor(Type.EmptyTypes) == null)
                            throw new Exception($"Component type '{type.FullName}' does not have a constructor with no parameters");
                        if (typeMap.ContainsKey(identifier))
                            throw new DuplicateNameException($"Component type '{identifier}' has already been registered");
                        typeMap.Add(identifier, type);
                    }
                }
            }

            public string GetTypeIdentifier() {
                return GetType().FullName;
            }

            // - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -
            // Networking state
            public enum SynchronizationMode {
                // The component will not be uploaded to the network
                NONE,
                
                // The component will be continously updated
                // by client
                UPDATE,

                // The component is uploaded by creator, but will be
                // simulated locally on each client
                CREATE
            }
        }

    }
}
