
using dotSpace.Interfaces.Space;
using System;
using System.Collections.Generic;

namespace CaptainCombat.Server
{
    class PlayerController
    {
        static int newId = 2;

        public PlayerController()
        {
            Connection.Space.Put("newuser_lock");
        }

        public void RunProtocol()
        {
            while (true)
            {
                ITuple result = Connection.Space.Get("user", typeof(string));

                IEnumerable<ITuple> usersInServer = Connection.Space.QueryAll("users", typeof(string));
                bool validUsername = true; 

                foreach (ITuple user in usersInServer)
                {
                    if (((string)user[1]).Equals((string)result[1]))
                    {
                        validUsername = false;
                    }
                }

                if(((string)result[1]).Length == 0)
                {
                    validUsername = false;
                }

                if (validUsername)
                {
                    Console.WriteLine("User joined: " + result[1]);
                    int client_id = newId;
                    newId++;
                    Connection.Space.Put("usersInGame", client_id, result[1]);
                    Connection.Space.Put("users", result[1]);
                    Connection.Space.Put("connected", true, client_id);
                }
                else
                {
                    Console.WriteLine("Invalid username: " + result[1]);
                    Connection.Space.Put("connected", false, 0);
                }
               
            }
        }
    }
}
