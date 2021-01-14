using System;
using CaptainCombat.singletons;
using dotSpace.Objects.Network;

namespace CaptainCombat.states
{
    class Connect : State
    {
        public Connect()
        {}

        public override void Run()
        {
            Console.WriteLine("Start GameClient");

            //string uri = "tcp://127.0.0.1:5000/space?KEEP";
            string uri = "tcp://49.12.75.251:5000/space?CONN";
            RemoteSpace space = new RemoteSpace(uri);
            Connection connecting = Connection.Instance;
            connecting.Space = space;
            this._context.TransitionTo(new Join());
        }

    }
}
