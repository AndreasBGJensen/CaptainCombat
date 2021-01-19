using CaptainCombat.Diagnostics;
using CaptainCombat.Common.Singletons;


namespace CaptainCombat.network
{
    
    
    class DownLoad
    {

        StopWatch watch = new StopWatch("Downloading: ",50);
           
        public void RunProtocol() {
            while (true) {

                watch.Start();
                DomainState.Instance.Download = Connection.Instance.lobbySpace.QueryAll(typeof(string), typeof(int), typeof(int), typeof(int), typeof(string), typeof(string));
                //IEnumerable<ITuple> messageData = Connection.Instance.lobbySpace.QueryAll("chat", typeof(int),typeof(string));
                //IEnumerable<ITuple> clientsInGame = Connection.Instance.lobbySpace.QueryAll("usersInGame", typeof(int), typeof(string));

                //DomainState.Instance.Messages = messageData;
                //DomainState.Instance.Clients = clientsInGame;

                watch.Stop();
                watch.PrintTimer();
            }
        }
    }
}
