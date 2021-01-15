using dotSpace.Interfaces.Space;
using StaticGameLogic_Library.Singletons;
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
                ITuple result = Connection.Instance.Space.Get("score", typeof(int), typeof(int));

                // Update score in space 
                ITuple clientScore = Connection.Instance.Space.GetP("allClientScores", (int)result[1], typeof(int));
                Connection.Instance.Space.Put("allClientScores", (int)result[1], (int)result[2]);
            }
        }
    }
}
