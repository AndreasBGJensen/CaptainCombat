using dotSpace.Interfaces.Space;
using dotSpace.Objects.Network;
using Source.ECS;
using Source.EntityUtility;
using StaticGameLogic_Library.JsonBuilder;
using StaticGameLogic_Library.Singletons;
using StaticGameLogic_Library.Source.Assets;
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
        
        
        public void CreateRocks()
        {
            Connect();
            Join();
            DomainState.Instance.Domain = domain;


            EntityUtility.CreateRock(domain, 150, 100, 0.7, 120);
            EntityUtility.CreateRock(domain, 400, -200, 1.0, 40);
            EntityUtility.CreateRock(domain, 0, 50, 1.2, 300);
            EntityUtility.CreateRock(domain, -300, 75, 1.4, 170);
            EntityUtility.CreateRock(domain, -100, -200, 1.2, 30);
            
            
            domain.Clean();
            DomainState.Instance.Upload = JsonBuilder.createJsonString();
            Connection.Instance.Space.Put("components", DomainState.Instance.Upload);
        }

        public void Join()
        {
            string username = "Computer_AI";

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
    }
}
