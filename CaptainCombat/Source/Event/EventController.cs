
using CaptainCombat.singletons;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using System.Threading;

namespace CaptainCombat.Source.Event {

    /// <summary>
    /// Controller which listens for incoming event tuples in the remote space, and
    /// sends outgoing events
    /// </summary>
    public class EventController {

        private static Dictionary<Type, List<EventListenerConverter>> listenerMap = new Dictionary<Type, List<EventListenerConverter>>();

        private static List<Event> outgoingEvents = new List<Event>();
        private static bool sendEvents = false;
        private static AutoResetEvent senderWaitHandle = new AutoResetEvent(false);
        
        private static List<Event> incomingEvents = new List<Event>();
        private static bool receiveEvents = false;

        /// <summary>
        /// Starts the EventController by starting the receiver
        /// and sender threads
        /// </summary>
        public static void Start() {
            new Thread(ReceiveEvents).Start();
            new Thread(SendEvents).Start();
        }

        /// <summary>
        /// Stops the EventController, by stopping the sender
        /// and receiver threads
        /// </summary>
        public static void Stop() {
            receiveEvents = false;
            sendEvents = false;
            senderWaitHandle.Set(); // Wake up sender thread to close it
        }

        private static void ReceiveEvents() {
            receiveEvents = true;
            while (receiveEvents) {
                var eventTuples = Connection.Instance.Space.GetAll("event", typeof(string), typeof(int), Connection.Instance.User_id, typeof(string));
                foreach (var eventTuple in eventTuples) {

                    var typeIdentifier = (string)eventTuple.Fields[1];
                    var sender = (uint)(int)eventTuple.Fields[2];
                    var receiver = (uint)(int)eventTuple.Fields[3];
                    var jsonData = (string)eventTuple.Fields[4];

                    var e = CreateEvent(typeIdentifier, jsonData);
                    e.Sender = sender;
                    e.Receiver = receiver;

                    // The lock ensures that no one is reading/updating
                    // from the list, when adding the new event
                    lock (incomingEvents) {
                        incomingEvents.Add(e);
                    }
                }
            }
        }


        private static void SendEvents() {
            sendEvents = true;
            while (sendEvents) {
                senderWaitHandle.WaitOne();
                lock (outgoingEvents) {
                    foreach (var e in outgoingEvents) {
                        Connection.Instance.Space.Put(
                                "event",
                                e.GetType().FullName,
                                (int) Connection.Instance.User_id,
                                (int) e.Receiver,
                                JsonConvert.SerializeObject(e)
                        );

                    }
                    outgoingEvents.Clear();
                }
            }
        }



        /// <summary>
        /// The listener delegate which is called upon receiving
        /// an Event of the given type
        /// </summary>
        /// <typeparam name="E">The specialized class (must inherit from Event)</typeparam>
        /// <param name="e">The event instanceto handle</param>
        /// <returns>True if the Event was consumed, and proceeding listeners should not be called</returns>
        public delegate bool EventListener<E>(E e) where E : Event;

        // Works as a converter between the base Event class
        // and the inherting class specified by the generic
        // type (see AddListener)
        private delegate bool EventListenerConverter(Event e);

        /// <summary>
        /// Adds a listener which is notified when an event of a particular
        /// type is received
        /// Multiple listeners may be added, and will be fired sequentially
        /// in the order they have been added
        /// </summary>
        public static EventListener<E> AddListener<E>(EventListener<E> e) where E : Event {
            var eventType = typeof(E);
            if (!listenerMap.ContainsKey(eventType))
                listenerMap[eventType] = new List<EventListenerConverter>();
            listenerMap[eventType].Add((ev) => e((E)ev) );
            return e;
        }

        /// <summary>
        /// Sends the given Event, which has been specified
        /// with a receiver and sender client id.
        /// The event is sent asynchronously
        /// </summary>
        public static void Send(Event e) {
            lock (outgoingEvents) {
                outgoingEvents.Add(e);
            }
            senderWaitHandle.Set();
        }

        /// <summary>
        /// Tells the controller to "dispatch" all Events,
        /// causing added listenes to be fired approriately
        /// </summary>
        public static void HandleEvents() {
            lock(incomingEvents) {
                foreach(var e in incomingEvents) {
                    Type type = e.GetType();
                    if (listenerMap.ContainsKey(type)) {
                        foreach (var listener in listenerMap[type])
                            if (listener(e)) break;
                    }
                }
                incomingEvents.Clear();
            }
        }


        // - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -
        // Setup type mapping (mapping of some identifier to Component child classes)

        private static readonly Dictionary<string, Type> typeMap = new Dictionary<string, Type>();

        // Constructs a new Component of the type identified by the
        // given identifier
        public static Event CreateEvent(string identifier, string jsonData) {
            if (!typeMap.ContainsKey(identifier))
                throw new ArgumentException($"Event type with identifier '{identifier}' does not exist");

            return (Event)JsonConvert.DeserializeObject(jsonData, typeMap[identifier]);
        }

        // Analyze which classes inherit from Component, and
        // add them to the 'typeMap' with their full name as
        // an identifier
        static EventController() {
            foreach (var type in Assembly.GetAssembly(typeof(Event)).GetTypes()) {
                if (type.IsSubclassOf(typeof(Event))) {
                    var identifier = type.FullName;
                    if (type.GetConstructor(Type.EmptyTypes) == null)
                        throw new Exception($"Event type '{type.FullName}' does not have a constructor with no parameters");
                    if (typeMap.ContainsKey(identifier))
                        throw new DuplicateNameException($"Event type '{identifier}' has already been registered");
                    typeMap.Add(identifier, type);
                }
            }
        }

    }
}
