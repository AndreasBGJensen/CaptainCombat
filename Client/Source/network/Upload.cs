

using CaptainCombat.Diagnostics;
using CaptainCombat.Common.Singletons;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

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
