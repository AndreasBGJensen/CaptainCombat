

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using StaticGameLogic_Library.Singletons;
using System;
using System.Collections.Generic;

namespace CaptainCombat.Source.Event {

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


        ///// <summary>
        ///// Saves the data of the inheriting Event into the JSON object
        ///// </summary>
        ///// <param name="json"></param>
        //public abstract JObject ToJSON();

        ///// <summary>
        ///// Updates the data of this Event to the match the data
        ///// in the JSON object
        ///// </summary>
        ///// <param name="json"></param>
        //public abstract void UpdateFromJSON(JObject json);

    }

}
