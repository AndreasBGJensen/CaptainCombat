
using CaptainCombat.Client.NetworkEvent;

namespace CaptainCombat.Client.Layers.Events
{


    class MessageEvent : Event
    {
        public string Message { get; set; }

        public MessageEvent() { }

        public MessageEvent(string message)
        {
            Message = message;
        }
    }
}
