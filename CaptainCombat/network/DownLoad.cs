
using System;
using dotSpace.Interfaces.Space;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CaptainCombat.singletons;

namespace CaptainCombat.network
{
    class DownLoad
    {
            public DownLoad()
        {


        }
        public void RunProtocol()
        {
            while (true)
            {
                while (true)
                {
                    Console.WriteLine("DownLoading");


                    IEnumerable<ITuple> gameData = Connection.Instance.Space.GetAll(Connection.Instance.User, typeof(string));

                    foreach (ITuple data in gameData)
                    {
                        Console.WriteLine(data[1]);
                    }

                    Thread.Sleep(5000);
                }
            }
        }
    }
}
