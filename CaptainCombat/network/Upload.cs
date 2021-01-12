
using CaptainCombat.Diagnostics;
using CaptainCombat.singletons;


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
