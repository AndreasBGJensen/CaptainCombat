
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using dotSpace.Objects.Network;
using dotSpace.Objects.Space;
using Tuple = dotSpace.Objects.Space.Tuple;
using dotSpace.Interfaces.Space;
using CaptainCombat.singletons;

namespace CaptainCombat.states
{
    class Join : State
    {

        public Join()
        {
          
            Connection connecting = Connection.Instance;
            RemoteSpace space = connecting.Space;


            Console.Write("User in server: ");
          
            IEnumerable<ITuple> users = space.QueryAll("users", typeof(string));

            foreach (ITuple user in users)
            {
                Console.Write(user[1]);
            }
           
            Console.Write("\nEnter username: ");
            string username = Console.ReadLine();
            space.Put("user", username);

            Tuple results = (Tuple)space.Get("connected", username, typeof(string));
            Console.WriteLine(results[2]);
            Connection.Instance.User = username;

            this._context.TransitionTo(new Game());
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
