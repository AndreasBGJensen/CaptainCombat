using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CaptainCombat.singletons;
using dotSpace.Objects.Network;
using dotSpace.Objects.Space;
using Tuple = dotSpace.Objects.Space.Tuple;

namespace CaptainCombat.states
{
    class Connect : State
    {
        public Connect()
        {
            Console.WriteLine("Start GameClient");
            
            try
            {
                string uri = "tcp://127.0.0.1:5000/space?CONN";
                RemoteSpace space = new RemoteSpace(uri);
                Connection connecting = Connection.Instance;
                connecting.Space = space;
                this._context.TransitionTo(new Join());
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        public override void Handle1()
        {
            throw new NotImplementedException();
        }

        public override void Handle2()
        {
            throw new NotImplementedException();
        }
    }
}
