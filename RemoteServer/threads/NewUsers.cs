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
        public NewUsers()
        {

        }

        public void RunProtocol()
        {
            while (true)
            {
                try
                {
                    Tuple result = (Tuple)Connection.Instance.Space.Get("user", typeof(string));
                    Console.WriteLine("User joined: " + result[1]);

                    Connection.Instance.Space.Put("users", result[1]);
                    Connection.Instance.Space.Put("connected", result[1], "Joined successfully");
                }
                catch (Exception e)
                {
                }
            }
        }
    }
}
