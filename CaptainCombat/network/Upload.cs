
using CaptainCombat.Diagnostics;
using CaptainCombat.singletons;
using System.Diagnostics;

namespace CaptainCombat.network {
    class Upload
    {
        StopWatch watch = new StopWatch("Uploading", 50);
        
        public Upload()
        {

        }

        public void RunProtocol()
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
