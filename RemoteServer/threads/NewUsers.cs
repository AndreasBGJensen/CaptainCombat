
using dotSpace.Interfaces.Space;
using StaticGameLogic_Library.Singletons;
using System;

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
                ITuple result = Connection.Instance.Space.Get("user", typeof(string));
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
