using CaptainCombat.singletons;
using CaptainCombat.Source.Utility;
using dotSpace.Interfaces.Space;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Reflection;


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


        private readonly ConcurrentDictionary<GlobalId, Entity> registeredEntities = new ConcurrentDictionary<GlobalId, Entity>();

        // Mapping of component ids to components
        private readonly ConcurrentDictionary<GlobalId, Component> components = new ConcurrentDictionary<GlobalId, Component>();

        public delegate void EntityCallback(Entity e);

        
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


        public void update(IEnumerable<ITuple> gameData)
        {
            //Console.WriteLine("Run update");
            // ITuple data format (string)comp, (int)client_id, (int)component_id, (int)entity_id, (string)data);

            HashSet<GlobalId> updatedComponents = new HashSet<GlobalId>();

            foreach (ITuple data in gameData)
            {
                var client_id = (uint)(int)data[1];
                var component_id = (uint)(int)data[2];
                var entity_id = (uint)(int)data[3];

                //Console.WriteLine("Test");
                if (Connection.Instance.User_id == client_id)
                {
                    //Console.WriteLine("Skip"); 
                    continue; 
                }

                //Console.WriteLine(data);

                Entity current_entity = null;
               
                GlobalId global_entity_id = new GlobalId(client_id, entity_id); 

                if (registeredEntities.ContainsKey(global_entity_id))
                {
                    //Console.WriteLine("Entity found in domain");
                    current_entity = registeredEntities[global_entity_id]; 
                }
                else
                {
                    //Console.WriteLine("Entity not found in domain create new entity");
                    current_entity = new Entity(this, global_entity_id); 
                }

                GlobalId global_compotent_id = new GlobalId(client_id, component_id);
                Component current_compotent = null;

                if (components.ContainsKey(global_compotent_id))
                {
                    //Console.WriteLine("Component already exists in domain");
                    current_compotent = components[global_compotent_id];
                    current_compotent.update((JObject)JsonConvert.DeserializeObject((string)data[4])); 
                }
                else
                {
                    //Console.WriteLine("New component added to entity in domain");
                    // Create new component
                    var componentTypeIdentifier = (string)data[0];
                    current_compotent = Component.CreateComponent(componentTypeIdentifier);

                    current_compotent.Id = global_compotent_id;

                    // TODO: Not sure if this actually works (GetType() may return Component instead of child class)
                    current_entity.AddComponent(current_compotent, current_compotent.GetType());
                }

                // Update Component data to new data
                var componentJsonData = (string)data[4];
                current_compotent.update((JObject)JsonConvert.DeserializeObject(componentJsonData));

                updatedComponents.Add(global_compotent_id);
            }

            // Clean up remote components that no longer exists
            foreach(var pair in components) {
                var id = pair.Key;
                if (id.clientId == Connection.Instance.User_id) continue;

                var found = updatedComponents.Remove(id);
                if( found ) continue;
                
                // Component no longer exists remotely, so we delete it
                
                
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
            foreach( var pair in components ) {
                var component = pair.Value;
                if (component.Matchable && component.ShouldSynchronize && component.IsLocal )
                    callback(component);
            }
        }
        public delegate void ComponentCallback(Component component);


        private GlobalId registerComponent<C>(C component, GlobalId id) where C : Component {
            if (components.ContainsKey(id))
                throw new ArgumentException("Component already exists with given ID");
            components[id] = component;
            return id;
        }


        private GlobalId registerComponent<C>(C component, uint clientId) where C : Component {
            return registerComponent(component, new GlobalId(clientId, componentIdGenerator.Get()));
        }

        
        private void unregisterComponent(Component component) {
            componentIdGenerator.Release(component.Id.objectId);
            
            if (!components.TryRemove(component.Id, out _)){
                throw new InvalidOperationException("Component id doesn`t exit"); 
            }
            //components.Remove(component.Id);
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

            private bool shouldBeDeleted = false;

            private Dictionary<Type, Component> components = new Dictionary<Type, Component>();

            // List of components that were added since last clean
            public readonly List<Component> newComponents = new List<Component>();

            private readonly HashSet<Type> componentsToRemove = new HashSet<Type>();


            /// <summary>
            /// Construct a new Entity within the given Domain
            /// </summary>
            /// <param name="domain"></param>
            public Entity(Domain domain, uint clientId = 0) {
                Domain = domain;

                // Get local client id if clientId is set to 0
                clientId = clientId == 0 ? (uint) Connection.Instance.User_id : clientId;
                Id = domain.registerEntity(this, clientId);
            }


            /// <summary>
            /// Construct a new Entity within the given Domain
            /// </summary>
            /// <param name="domain"></param>
            public Entity(Domain domain, GlobalId id) {
                Domain = domain;
                Id = domain.registerEntity(this, id);
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
            public C AddComponent<C>(C component) where C : Component {
                AddComponent(component, typeof(C));
                return component;
            }


            public Component AddComponent(Component component, Type type) {
                if (Deleted) throw new InvalidOperationException("Entity has been deleted");

                if (components.ContainsKey(type))
                    throw new InvalidOperationException($"Entity already has component {type.Name}");

                component.Entity = this;

                newComponents.Add(component);
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

                if (shouldBeDeleted) {
                    // Unregister all components before deletion
                    foreach (var pair in components)
                        Domain.unregisterComponent(pair.Value);
                    Deleted = true;
                    return true;
                }
                else {
                    // Create new components
                    foreach (var component in newComponents)
                        component.Matchable = true;

                    // Delete components
                    foreach (var componentId in componentsToRemove) {
                        var component = components[componentId];
                        components.Remove(componentId);
                        Domain.unregisterComponent(component);
                    }
                    return false;
                }
            }

        }


        public void MakeGlobal() {
            foreach (var pair in components) {
                pair.Value.ShouldSynchronize = false;
            }
        }


        public void MakeLocal() {
            foreach( var pair in components ) {
                pair.Value.ShouldSynchronize = true;
            }
        }


        // ========================================================================================================================================================= 

        public abstract class Component {



            private Entity entity = null;
            public Entity Entity { 
                get => entity;
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

            public uint ClientId { get => Id.clientId; }


            /// <summary>
            /// Whether or not this Component is owned by this local Client
            /// </summary>
            public bool IsLocal { get => ClientId == Connection.Instance.User_id; }


            /// <summary>
            /// True if this Component should be synchronized with other clients, or false
            /// if it should only exist locally.
            /// Defaults to true
            /// </summary>
            public bool ShouldSynchronize { get; set; } = true;

            // This flag is set to true, if it the component has been "finalized" in the domain
            // Should only be set by the domain
            public bool Matchable { get; set; } = false;


            public Component() {}
          

            public abstract Object getData();

            public abstract void update(JObject json);



            // - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -
            // Setup type mapping (mapping of some identifier to Component child classes)

            private static readonly Dictionary<string, Type> typeMap = new Dictionary<string, Type>();

            // Constructs a new Component of the type identified by the
            // given identifier
            public static Component CreateComponent(string identifier) {
                if (!typeMap.ContainsKey(identifier))
                    throw new ArgumentException($"Component type with identifier '{identifier}' does not exist");
                return (Component)Activator.CreateInstance(typeMap[identifier]);
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
        }

    }
}
