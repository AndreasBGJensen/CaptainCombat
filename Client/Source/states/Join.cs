
using System;
using System.Collections.Generic;
using Tuple = dotSpace.Objects.Space.Tuple;
using dotSpace.Interfaces.Space;
using CaptainCombat.Common.Singletons;

namespace CaptainCombat.states
{
    class Join : State
    {

        public Join(){}

        public override void Run()
        {
            Connection connecting = Connection.Instance;
            ISpace space = connecting.Space;


            Console.Write("User in server: ");

            IEnumerable<ITuple> users = space.QueryAll("users", typeof(string));

            foreach (ITuple user in users)
            {
                Console.Write(user[1]);
            }


            Console.Write("\nEnter username: ");
            string username = Console.ReadLine();

            space.Put("user", username);

            Tuple results = (Tuple)space.Get("connected", typeof(int), typeof(string));
            Console.WriteLine(results[2]);
            Connection.Instance.User = username;
            Connection.Instance.User_id = (int)results[1];

            _context.TransitionTo(new Game());
        }


    }
}
