using dotSpace.Interfaces.Space;
using RemoteServer.Mapmaker.EntityToAdd;
using StaticGameLogic_Library.Singletons;
using StaticGameLogic_Library.Source.ECS;
using System;
using System.Collections.Generic;

namespace RemoteServer.Mapmaker
{

    class Game
    {

        Domain domain = new Domain();
        public delegate void InitCmponent(object source, EventArgs e);
        public event InitCmponent ComputerInit;

        public void Init()
        {

            DomainState.Instance.Domain = domain;
            Join();
            

            IEntity entity = new Rocks(10);
            entity.OnComputerInit();
            //game.ComputerInit += entity.OnComputerInit;




        }

        public void Join()
        {
            string username = "Server";

            ISpace space = Connection.Instance.Space;

            Console.Write("User in server: ");

            IEnumerable<ITuple> users = space.QueryAll("users", typeof(string));

            foreach (ITuple user in users)
            {
                Console.Write(user[1]);
            }            

            space.Put("user", username);

            ITuple results = space.Get("connected", typeof(int), typeof(string));
            Console.WriteLine(results[2]);
            Connection.Instance.User = username;
            Connection.Instance.User_id = (int)results[1];

        }

        protected virtual void OnComputerInit()
        {
            if(ComputerInit != null)
            {
                ComputerInit(this, EventArgs.Empty);
            }
        }
    }
}
