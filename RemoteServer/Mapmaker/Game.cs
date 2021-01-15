using dotSpace.Interfaces.Space;
using dotSpace.Objects.Network;
using RemoteServer.Mapmaker.EntityToAdd;
using Source.EntityUtility;
using StaticGameLogic_Library.JsonBuilder;
using StaticGameLogic_Library.Singletons;
using StaticGameLogic_Library.Source;
using StaticGameLogic_Library.Source.ECS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RemoteServer.Mapmaker
{

    class Game
    {
        Domain domain = new Domain();
        public delegate void InitCmponent(object source, EventArgs e);
        public event InitCmponent ComputerInit;


        public void Init()
        {
            Connect();
            Join();
            DomainState.Instance.Domain = domain;
            //OnComputerInit();

            IEntity entity = new Rocks(10);
            entity.OnComputerInit();
            //game.ComputerInit += entity.OnComputerInit;




        }

        public void Join()
        {
            string username = "Server";

            Connection connecting = Connection.Instance;
            RemoteSpace space = connecting.Space;

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

        private void Connect()
        {
            string uri = "tcp://127.0.0.1:5000/space?CONN";
            //string uri = "tcp://49.12.75.251:5000/space?CONN";
            RemoteSpace space = new RemoteSpace(uri);
            Connection connecting = Connection.Instance;
            connecting.Space = space;
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
