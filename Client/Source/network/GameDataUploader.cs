
using CaptainCombat.Diagnostics;
using CaptainCombat.Common.Singletons;
using System.Threading;

namespace CaptainCombat.network
{


    /// <summary>
    /// Class running repeated puts to lobby space, in order to upload
    /// game data (components in json string format)
    /// 
    /// The scheduling of the thread is balanced by using WaitHandles
    /// (barriers that block, until signaled). The thread will be blocked
    /// after each successful upload to the space, and has to be woken up
    /// by some other class (game loop), supplying new data to upload.
    /// 
    /// To prevent the caller from dominating the schedule, the caller will
    /// be blocked if it has requested for data to uploaded more than 100
    /// times, without the Uploader having uploaded once.
    /// </summary>
    class GameDataUploader {

        // Also known as UpdateId
        public ulong UpdateCount = 0;

        private object updateLock = new object();

        private string updateData = null;

        private EventWaitHandle waitHandle = new EventWaitHandle(false, EventResetMode.AutoReset);

        private EventWaitHandle callerWaitHandle = new EventWaitHandle(false, EventResetMode.AutoReset);

        private int blockCountdown = 0;
        private bool started = false;

        private Thread thread;
        
        public void Start() {
            thread = new Thread(Upload);
            //thread.Priority = ThreadPriority.Highest;
            thread.Start();
        }

        
        public void Stop()
        {
            thread.Abort();
            callerWaitHandle.Set();
        }


        public void UploadData(string data) {
            lock(updateLock) {
                blockCountdown--;
                updateData = data;
            }
            waitHandle.Set();
            if (started && blockCountdown <= 0)
                callerWaitHandle.WaitOne();
            //System.Console.WriteLine("Acivate upload!");
        }


        private void Upload() {
            started = true;
            StopWatch watch = new StopWatch("Uploading", 50);

            while (true) {
                System.Console.WriteLine("Upload");

                // Set to wait, to prevent the thread from dominating
                waitHandle.WaitOne();

                // Copy data to upload
                string data;
                lock (updateLock) {
                    blockCountdown = 100;
                    data = updateData;
                    updateData = null;
                }

                if( data != null ){
                    // Upload the data
                    UpdateCount++;
                    watch.Start();
                    // Settting the wait handler here (better once too many
                    // than once too few)
                    callerWaitHandle.Set();
                    Connection.Instance.LobbySpace.Put("components", UpdateCount.ToString(), data);
                    watch.Stop();
                    watch.PrintTimer();
                }

                callerWaitHandle.Set();
            }
        }
    }
}
