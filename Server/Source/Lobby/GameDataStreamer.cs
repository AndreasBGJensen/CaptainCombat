using CaptainCombat.Server.Collector;
using CaptainCombat.Server.Collector.Helpers;
using dotSpace.Interfaces.Space;
using System;
using System.Threading;

namespace CaptainCombat.Server
{
    /// <summary>
    /// Reads the list of Component tuples to upload to the Lobby space
    /// from the Lobby's Players
    /// </summary>
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
            thread.Priority = ThreadPriority.AboveNormal;
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
            space.Get("lobby_lock");

            Console.WriteLine("Starting game data streaming");
            while (true)
            {
                collector.BeginCollect();
            }
        }
    }
}
