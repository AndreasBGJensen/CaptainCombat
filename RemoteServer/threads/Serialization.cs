using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using dotSpace.Interfaces.Space;
using dotSpace.Objects.Network;
using dotSpace.Objects.Space;
using RemoteServer.singletons;
using Tuple = dotSpace.Objects.Space.Tuple;
namespace RemoteServer.threads
{
    class Serialization
    {
        public void RunProtocol()
        {
            while (true)
            {
                try
                {
                    Tuple result = (Tuple)Connection.Instance.Space.Get("global", typeof(string), typeof(string));

                    Console.WriteLine("User data: " + result[1] + result[2]);

                    IEnumerable<ITuple> users = Connection.Instance.Space.QueryAll("users", typeof(string));

                    foreach (ITuple user in users)
                    {
                        Connection.Instance.Space.Put(user[1], result[2]);
                    }
                }
                catch (Exception e)
                {
                }
            }
        }
    }
}
