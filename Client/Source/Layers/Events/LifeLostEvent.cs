
using CaptainCombat.Client.NetworkEvent;

namespace CaptainCombat.Client.Source.Layers.Events {
    
    
    class LifeLostEvent : Event {
        public LifeLostEvent() { }
        public LifeLostEvent(uint receiver) : base(receiver) {}
    }

}
