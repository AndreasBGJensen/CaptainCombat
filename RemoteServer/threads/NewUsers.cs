using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using dotSpace.Objects.Network;
using dotSpace.Objects.Space;
using RemoteServer.singletons;
using Tuple = dotSpace.Objects.Space.Tuple;

namespace RemoteServer.threads
{
    class NewUsers
    {
        static int newId = 1;
        //static int computer_AI_ID = 1;
        public NewUsers()
        {

        }

        public void RunProtocol()
        {
            while (true)
            {
                Tuple result = (Tuple)Connection.Instance.Space.Get("user", typeof(string));
                Console.WriteLine("User joined: " + result[1]);
                int client_id = newId;
                newId++;
                Connection.Instance.Space.Put("usersInGame", client_id, result[1]);
                Connection.Instance.Space.Put("users", result[1]);
                Connection.Instance.Space.Put("connected", client_id, "Joined successfully");
                
            }
        }
    }
}
