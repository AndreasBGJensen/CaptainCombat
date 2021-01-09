
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
                
                    Console.WriteLine("DownLoading");
                    //ITuple result = Connection.Instance.Space.GetP(comp, client_id, component_id, entity_id, typeof(string));
                    IEnumerable<ITuple> gameData = Connection.Instance.Space.QueryAll(typeof(string), typeof(int), typeof(int), typeof(int), typeof(string));
                    /*
                    foreach (ITuple data in gameData)
                    {
                        Console.WriteLine(data);

                    }
                    */
                    DomainState.Instance.Domain.update(gameData);
                    Console.WriteLine("Done");
                
                Thread.Sleep(5000);
            }
           
        }
    }
}
