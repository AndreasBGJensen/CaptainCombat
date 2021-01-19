
using CaptainCombat.Diagnostics;
using CaptainCombat.Common.Singletons;
using System.Threading;

namespace CaptainCombat.network {

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
                    Connection.Instance.LobbySpace.Put("components", UpdateCount.ToString(), data);
                    watch.Stop();
                    watch.PrintTimer();
                }

                callerWaitHandle.Set();
            }
        }
    }
}
