using System;
using dotSpace.Objects.Network;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using dotSpace.Objects.Network;
using dotSpace.Objects.Space;
using StaticGameLogic_Library.Singletons;

using Tuple = dotSpace.Objects.Space.Tuple;

namespace CaptainCombat.states
{
    class Connect : State
    {
        public Connect()
        {}

        public override void Run()
        {
            Console.WriteLine("Start GameClient");

            string uri = "tcp://127.0.0.1:5000/space?KEEP";
            //string uri = "tcp://49.12.75.251:5000/space?KEEP";
            RemoteSpace space = new RemoteSpace(uri);
            Connection connecting = Connection.Instance;
            connecting.Space = space;
            this._context.TransitionTo(new Join());
        }

    }
}
