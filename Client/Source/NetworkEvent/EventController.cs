

using Newtonsoft.Json;
using CaptainCombat.Common.Singletons;
using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using System.Threading;
using dotSpace.Objects.Space;

namespace CaptainCombat.Client.NetworkEvent {

    /// <summary>
    /// Controller which listens for incoming event tuples in the remote space, and
    /// sends outgoing events
    /// </summary>
    // TODO: Make event controller to non-static class
    public class EventController {

        private static Dictionary<Type, List<EventListenerConverter>> listenerMap = new Dictionary<Type, List<EventListenerConverter>>();

        // private static List<Event> outgoingEvents = new List<Event>();
        private static SequentialSpace outgoingEvents = new SequentialSpace();

        private static bool sendEvents = false;
        private static AutoResetEvent senderWaitHandle = new AutoResetEvent(false);
        
        //private static List<Event> incomingEvents = new List<Event>();
        private static bool receiveEvents = false;

        private static SequentialSpace incomingEvents = new SequentialSpace();

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
                
                { // First get is blocking, and signals that some event exists
                  // which prevents busy waiting
                    var eventTuple = Connection.Instance.lobbySpace.Get("event", typeof(string), typeof(int), Connection.Instance.User_id, typeof(string));
                    incomingEvents.Put((string)eventTuple[1], (int)eventTuple[2], (string)eventTuple[4]);
                }
                { // Second Get(All) is flushes the existing events to receive
                    var remainingTuples = Connection.Instance.lobbySpace.GetAll("event", typeof(string), typeof(int), Connection.Instance.User_id, typeof(string));
                    foreach (var eventTuple in remainingTuples)
                        incomingEvents.Put((string)eventTuple[1], (int)eventTuple[2], (string)eventTuple[4]);
                }
            }
        }


        private static void SendEvents() {
            sendEvents = true;
            while (sendEvents) {
                // Wait for send signal
                outgoingEvents.Get("send-signal");
                // Get the batch of events to send
                var eventTuples = outgoingEvents.GetAll(typeof(string), typeof(int), typeof(string));

                foreach(var eventTuple in eventTuples) {
                    Connection.Instance.lobbySpace.Put("event",
                        (string)eventTuple[0], // Event type identifier
                        Connection.Instance.User_id, // Sender (local id)
                        (int)eventTuple[1], // Receiver
                        (string)eventTuple[2] // JSON data
                    );
                    //// TODO: Create some sort of thread pooling
                    //new Thread(() => {
                        
                    //}).Start();
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
            if (e.Receiver == 0)
                throw new ArgumentException("Receiver was not set for event");

            outgoingEvents.Put(
                e.GetType().FullName,
                (int)e.Receiver,
                JsonConvert.SerializeObject(e)
            );
        }

        /// <summary>
        /// Tells the controller to "dispatch" all Events,
        /// causing added listenes to be fired approriately
        /// </summary>
        public static void Flush() {

            // Signal events to be sent --------------
            outgoingEvents.Put("send-signal");

            // Trigger new events --------------------
            var events = incomingEvents.GetAll(typeof(string), typeof(int), typeof(string));
            foreach (var eventTuple in events) {

                // Decode the event 
                var typeIdentifier = (string)eventTuple.Fields[0];
                var sender = (uint)(int)eventTuple.Fields[1];
                var jsonData = (string)eventTuple.Fields[2];

                var e = CreateEvent(typeIdentifier, jsonData);
                e.Sender = sender;
                e.Receiver = (uint)Connection.Instance.User_id;

                // Fire the event
                Type type = e.GetType();
                if (listenerMap.ContainsKey(type)) {
                    foreach (var listener in listenerMap[type])
                        if (listener(e)) break;
                }
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
