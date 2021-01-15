
using System;
using dotSpace.Interfaces.Space;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CaptainCombat.singletons;
using CaptainCombat.Diagnostics;

namespace CaptainCombat.network
{
    class DownLoad
    {
        StopWatch watch = new StopWatch("Downloading:",50);
            public DownLoad()
        {


        }
        public void RunProtocol()
        {
            
            while (true)
            {
                watch.Start();
                    //Console.WriteLine("DownLoading");
                    //ITuple result = Connection.Instance.Space.GetP(comp, client_id, component_id, entity_id, typeof(string));
                IEnumerable<ITuple> gameData = Connection.Instance.Space.QueryAll(typeof(string), typeof(int), typeof(int), typeof(int), typeof(string));
                IEnumerable<ITuple> messageData = Connection.Instance.Space.QueryAll("chat", typeof(int),typeof(string));
                IEnumerable<ITuple> clientsInGame = Connection.Instance.Space.QueryAll("usersInGame", typeof(int), typeof(string));
                IEnumerable<ITuple> allClientScores = Connection.Instance.Space.QueryAll("allClientScores", typeof(int), typeof(int));


                DomainState.Instance.Messages = messageData;
                DomainState.Instance.Clients = clientsInGame;
                DomainState.Instance.ClientScores = allClientScores;

                DomainState.Instance.Domain.update(gameData);
                watch.Stop();
                watch.PrintTimer();
                //Console.WriteLine("Done");
            }
           
        }
    }
}
