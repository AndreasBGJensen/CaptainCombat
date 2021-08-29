namespace CaptainCombat.Client
{

    class LifeLostEvent : Event {
        public LifeLostEvent() { }
        public LifeLostEvent(uint receiver) : base(receiver) {}
    }

}
