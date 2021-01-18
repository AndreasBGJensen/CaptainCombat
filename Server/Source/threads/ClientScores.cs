using CaptainCombat.Common.Singletons;
using dotSpace.Interfaces.Space;
using dotSpace.Objects.Space;
using System.Threading;

namespace CaptainCombat.Server.threads
{
    class ClientScores
    {
        private SequentialSpace space;

        public ClientScores(SequentialSpace lobbySpace)
        {
            this.space = lobbySpace;
        }

        public void RunProtocol()
        {
            new Thread(() =>
            {
                while (true)
                {
                    // From client 
                    ITuple result = space.Get("score", typeof(int), typeof(int));

                    // Update score in space 
                    ITuple clientScore = space.GetP("allClientScores", (int)result[1], typeof(int));
                    space.Put("allClientScores", (int)result[1], (int)result[2]);
                }
            });
           
        }
    }
}
