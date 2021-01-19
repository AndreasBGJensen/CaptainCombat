using System;
using CaptainCombat.Server.Collector;
using CaptainCombat.Server.Collector.Helpers;
using CaptainCombat.Common.Singletons;

namespace CaptainCombat.Server.threads
{
   class Serialization
    {
        
        public void RunProtocol()
        {
           CollectorClass collector = new CollectorClass();
            ArrayCreator creator = new ArrayCreator();
            //collector.SetCollector(new TupleCollectorParallel(creator,Connection.Instance.Space));
            collector.SetCollector(new TupleCollector(creator, Connection.Instance.Space));

            Console.WriteLine("Starting game data upload");
            
            while (true)
            {
                    collector.BeginCollect();
                    //collector.PrintUpdateComponents();
                   
            }
        }
    }
}
