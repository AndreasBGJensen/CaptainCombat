

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using StaticGameLogic_Library.Singletons;
using System;
using System.Collections.Generic;

namespace CaptainCombat.Source.NetworkEvent {

    public abstract class Event {

        /// <summary>
        /// ID of client sending the event
        /// </summary>
        [JsonIgnore]
        public uint Sender { get; set; }
        
        /// <summary>
        /// ID of client receiving the event
        /// </summary>
        [JsonIgnore]
        public uint Receiver { get; set; }

        public Event() { }
        
        public Event(uint sender, uint receiver) {
            Sender = sender;
            Receiver = receiver;
        }

        public Event(uint receiver) {
            Sender = (uint) Connection.Instance.User_id;
            Receiver = receiver;
        }
    }

}
