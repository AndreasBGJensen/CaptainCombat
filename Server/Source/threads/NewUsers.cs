
using dotSpace.Interfaces.Space;
using CaptainCombat.Common.Singletons;
using System;
using System.Collections.Generic;

namespace CaptainCombat.Server.threads
{
    class NewUsers
    {
        static int newId = 2;

        public NewUsers()
        {
            Connection.Instance.Space.Put("newuser_lock");
        }

        public void RunProtocol()
        {
            while (true)
            {
                ITuple result = Connection.Instance.Space.Get("user", typeof(string));

                IEnumerable<ITuple> usersInServer = Connection.Instance.Space.QueryAll("users", typeof(string));
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
                    // TODO: Move this initial life setting
                    Connection.Instance.Space.Put("lives", client_id, 2);
                    Connection.Instance.Space.Put("usersInGame", client_id, result[1]);
                    Connection.Instance.Space.Put("users", result[1]);
                    Connection.Instance.Space.Put("connected", true, client_id);
                }
                else
                {
                    Console.WriteLine("Invalid username: " + result[1]);
                    Connection.Instance.Space.Put("connected",false, 0);
                }
               
            }
        }
    }
}
