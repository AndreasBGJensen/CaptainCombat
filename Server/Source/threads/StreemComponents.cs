using CaptainCombat.Server.Collector;
using CaptainCombat.Server.Collector.Helpers;
using dotSpace.Objects.Space;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CaptainCombat.Server.Source.threads
{
    class StreemComponents
    {
        private SequentialSpace space;

        public StreemComponents(SequentialSpace lobbySpace)
        {
            this.space = lobbySpace;
        }

        public void RunProtocol()
        {
            CollectorClass collector = new CollectorClass();
            ArrayCreator creator = new ArrayCreator();
            //collector.SetCollector(new TupleCollectorParallel(creator,Connection.Instance.Space));
            collector.SetCollector(new TupleCollector(creator, space));

            Console.WriteLine("Starting game data upload");
            new Thread(() =>
            {
                space.Query("start");
                space.Get("lock");
                while (true)
                {
                    collector.BeginCollect();
                    //collector.PrintUpdateComponents();
                }
            }).Start();
           
        }
    }
}
