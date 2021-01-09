
using CaptainCombat.Diagnostics;
using CaptainCombat.singletons;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CaptainCombat.network
{
    class Upload
    {
        StopWatch watch = new StopWatch("Uploading", 100);
        public Upload()
        {


        }

        public void RunProtocol()
        {
            while (true)
            {
                while (true)
                {
                    
                    //Console.WriteLine("Uploading");
                    if (DomainState.Instance.Upload != null)
                    {
                        watch.Start();
                        Connection.Instance.Space.Put("components", DomainState.Instance.Upload);
                        watch.Stop();
                        watch.PrintTimer();
                    }
                    
                }
            }
        }
    }
}
