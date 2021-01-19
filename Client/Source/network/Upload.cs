
using CaptainCombat.Diagnostics;
using CaptainCombat.Common.Singletons;

namespace CaptainCombat.network {

    class Upload {

        // Also known as UpdateId
        public ulong UpdateCount = 0; 

        public void RunProtocol() {

            StopWatch watch = new StopWatch("Uploading", 50);

            while (true) {

                if (DomainState.Instance.Upload != null) {
                    // TODO: Set Upload to null
                    UpdateCount++;
                    watch.Start();
                    Connection.Instance.lobbySpace.Put("components", UpdateCount.ToString(), DomainState.Instance.Upload);
                    watch.Stop();
                    watch.PrintTimer();
                }
            }
        }
    }
}
