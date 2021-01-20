using CaptainCombat.Diagnostics;
using CaptainCombat.Common.Singletons;
using System.Threading;
using System.Collections.Generic;
using dotSpace.Interfaces.Space;

namespace CaptainCombat.network
{
    
    /// <summary>
    /// Class running repeated queries to lobby space in order to get 
    /// the game data (components).
    /// 
    /// The scheduling of the thread is balanced by using WaitHandles
    /// (barriers that block, until signaled). The thread will be blocked
    /// after each successful fetch from the space, and has to be woken up
    /// by some other class (game loop), retrieving the downloaded data.
    /// 
    /// To prevent the caller from dominating the schedule, the caller will
    /// be blocked if it has requested data more than 100 times, without the
    /// downloader having downloaded once.
    /// </summary>
    class GameDataDownloader
    {
        
        private readonly object dataLock = new object();

        private IEnumerable<ITuple> downloadedData = null;

        private Thread thread;

        private bool started = false;

        private readonly EventWaitHandle waitHandle = new EventWaitHandle(false, EventResetMode.AutoReset);
        
        private EventWaitHandle callerWaitHandle = new EventWaitHandle(false, EventResetMode.AutoReset);
        private int blockCountdown = 0;
                
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
                data = downloadedData;
                downloadedData = null;
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
                callerWaitHandle.Set();
                var data = Connection.Instance.LobbySpace.QueryAll(typeof(string), typeof(int), typeof(int), typeof(int), typeof(string), typeof(string));
                lock(dataLock) {
                    blockCountdown = 100;
                    downloadedData = data;
                }
                watch.Stop();
                watch.PrintTimer();

                waitHandle.WaitOne();
                callerWaitHandle.Set();
            }
        }
    }
}
