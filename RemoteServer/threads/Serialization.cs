using System;
using RemoteServer.singletons;
using RemoteServer.TestData;
using RemoteServer.Collector;
using RemoteServer.Collector.Helpers;
using System.Threading;

namespace RemoteServer.threads
{
   class Serialization
    {
        TestComponentClasse test = new TestComponentClasse();
        
        public void RunProtocol()
        {
           CollectorClass collector = new CollectorClass();
           ArrayCreator creator = new ArrayCreator();
           collector.SetSpace(Connection.Instance.Space);
           //collector.SetCollector(new TupleCollectorParallel(creator,Connection.Instance.Space));
           collector.SetCollector(new TupleCollector(creator, Connection.Instance.Space));
           Console.WriteLine("Running...");
           while (true)
           {
                try
                {

                collector.BeginCollect();
                collector.PrintUpdateComponents();
                    
                }
                catch (Exception e)
                {
                      Console.WriteLine(e.Message);
                      Console.WriteLine(e.StackTrace);
                }
            }
        }
    }
}
