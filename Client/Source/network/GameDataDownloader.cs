using CaptainCombat.Diagnostics;
using CaptainCombat.Common.Singletons;
using System.Threading;
using System.Collections.Generic;
using dotSpace.Interfaces.Space;

namespace CaptainCombat.network
{
    
    class GameDataDownloader
    {
        
        private readonly object dataLock = new object();

        private IEnumerable<ITuple> downloadData = null;

        private readonly EventWaitHandle waitHandle = new EventWaitHandle(false, EventResetMode.AutoReset);
        private EventWaitHandle callerWaitHandle = new EventWaitHandle(false, EventResetMode.AutoReset);

        private int blockCountdown = 0;

        private Thread thread;

        private bool started = false;

        private StopWatch watch = new StopWatch("Downloading: ", 50);


        public void Start() {
            thread = new Thread(Download);
            //thread.Priority = ThreadPriority.Highest;
            thread.Start();
        }

        
        public IEnumerable<ITuple> GetData() {
            IEnumerable<ITuple> data;
            lock(dataLock) {
                blockCountdown--;
                data = downloadData;
                downloadData = null;
            }
            waitHandle.Set();
            if (started &&  blockCountdown <= 0)
                callerWaitHandle.WaitOne();
            //System.Console.WriteLine("Activate download");
            return data;
        }

           
        private void Download() {
            started = true;
            while (true) {
                System.Console.WriteLine("Download");

                watch.Start();
                var data = Connection.Instance.LobbySpace.QueryAll(typeof(string), typeof(int), typeof(int), typeof(int), typeof(string), typeof(string));
                lock(dataLock) {
                    blockCountdown = 100;
                    downloadData = data;
                }
                watch.Stop();
                watch.PrintTimer();

                waitHandle.WaitOne();
                callerWaitHandle.Set();



                // TODO: Fix chat messages
                //DomainState.Instance.Messages = Connection.Instance.LobbySpace.QueryAll("chat", typeof(int),typeof(string));
                //IEnumerable<ITuple> clientsInGame = Connection.Instance.lobbySpace.QueryAll("usersInGame", typeof(int), typeof(string));
                //DomainState.Instance.Clients = clientsInGame;

            }
        }
    }
}
