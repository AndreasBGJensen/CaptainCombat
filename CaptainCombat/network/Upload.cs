
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
        public Upload()
        {


        }

        public void RunProtocol()
        {
            while (true)
            {
                while (true)
                {
                    Console.WriteLine("Uploading");
                    if (DomainState.Instance.Upload != null)
                    {
                        Connection.Instance.Space.Put("global", Connection.Instance.User, DomainState.Instance.Upload);
                    }
                    Thread.Sleep(5000);
                }
            }
        }
    }
}
