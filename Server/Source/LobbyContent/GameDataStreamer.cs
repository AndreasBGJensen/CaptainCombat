using CaptainCombat.Server.Collector;
using CaptainCombat.Server.Collector.Helpers;
using dotSpace.Interfaces.Space;
using dotSpace.Objects.Space;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CaptainCombat.Server.Source.threads
{
    class GameDataStreamer
    {
        private ISpace space;
        private Thread thread;

        public GameDataStreamer(ISpace lobbySpace)
        {
            space = lobbySpace;
        }

        public void Start()
        {
            thread = new Thread(Stream);
            thread.Priority = ThreadPriority.Highest;
            thread.Start();
        }

        
        public void Stop()
        {
            thread?.Abort();
            Console.WriteLine("Stopped game data streamning");
        }


        private void Stream()
        {
            CollectorClass collector = new CollectorClass();
            ArrayCreator creator = new ArrayCreator();
            collector.SetCollector(new TupleCollector(creator, space));

            space.Query("start");
            space.Get("lock");

            Console.WriteLine("Starting game data streaming");
            while (true)
            {
                collector.BeginCollect();
            }
        }
    }
}
