using Newtonsoft.Json;

namespace CaptainCombat.Client {

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
            Sender = Connection.LocalPlayer.Id;
            Receiver = receiver;
        }
    }

}
