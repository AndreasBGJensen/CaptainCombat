using RemoteServer.singletons;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tuple = dotSpace.Objects.Space.Tuple;

namespace RemoteServer.threads
{
    class ClientScores
    {
        public ClientScores()
        {

        }

        public void RunProtocol()
        {
            while (true)
            {
                // From client 
                Tuple result = (Tuple)Connection.Instance.Space.Get("score", typeof(int), typeof(int));

                // Update score in space 
                Tuple clientScore = (Tuple)Connection.Instance.Space.GetP("allClientScores", (int)result[1], typeof(int));
                Connection.Instance.Space.Put("allClientScores", (int)result[1], (int)result[2]);
            }
        }
    }
}
