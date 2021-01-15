using System;
using RemoteServer.Collector;
using RemoteServer.Collector.Helpers;
using StaticGameLogic_Library.Singletons;

namespace RemoteServer.threads
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
