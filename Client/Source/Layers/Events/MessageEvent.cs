
using CaptainCombat.Client.NetworkEvent;

namespace CaptainCombat.Client.Layers.Events
{

    /// <summary>
    /// Event signalling that a Chat message has been received
    /// </summary>
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
