using CaptainCombat.singletons;
using CaptainCombat.Diagnostics;
using System.Collections.Generic;
using dotSpace.Interfaces.Space;

namespace CaptainCombat.network
{
    class DownLoad
    {


        StopWatch watch = new StopWatch("Downloading: ",50);
           
        public void RunProtocol() { 
            while (true) {
                watch.Start();
                DomainState.Instance.Download = Connection.Instance.Space.QueryAll(typeof(string), typeof(int), typeof(int), typeof(int), typeof(string), typeof(string));
                IEnumerable<ITuple> messageData = Connection.Instance.Space.QueryAll("chat", typeof(int),typeof(string));
                IEnumerable<ITuple> clientsInGame = Connection.Instance.Space.QueryAll("usersInGame", typeof(int), typeof(string));
                IEnumerable<ITuple> allClientScores = Connection.Instance.Space.QueryAll("allClientScores", typeof(int), typeof(int));

                DomainState.Instance.Messages = messageData;
                DomainState.Instance.Clients = clientsInGame;
                DomainState.Instance.ClientScores = allClientScores;

                watch.Stop();
                watch.PrintTimer();
            }
           
        }
    }
}
