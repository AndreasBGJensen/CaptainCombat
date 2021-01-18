
using dotSpace.Interfaces.Space;
using CaptainCombat.Common.Singletons;
using System;

namespace CaptainCombat.Server.threads
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
                // TODO: Move this initial life setting
                Connection.Instance.Space.Put("lives", client_id, 2);
                Connection.Instance.Space.Put("usersInGame", client_id, result[1]);
                Connection.Instance.Space.Put("users", result[1]);
                Connection.Instance.Space.Put("connected", client_id, "Joined successfully");
                
            }
        }
    }
}
