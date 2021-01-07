
using CaptainCombat.game;
using CaptainCombat.singletons;
using CaptainCombat.network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;



namespace CaptainCombat.states
{
    class Game : State
    {
        public Game()
        {
            Upload upload = new Upload();
            Thread uploadThread = new Thread(new ThreadStart(upload.RunProtocol));
            uploadThread.Start();

            DownLoad download = new DownLoad();
            Thread downloadThread = new Thread(new ThreadStart(download.RunProtocol));
            downloadThread.Start();

            RunGameLoop();

        }

        public object JsonSerializer { get; private set; }

        public override void Handle1()
        {
            throw new NotImplementedException();
        }

        public override void Handle2()
        {
            throw new NotImplementedException();
        }

        private void RunGameLoop()
        {
            int index = 0;
            while (true)
            {
                Console.WriteLine("Game running");
                index++;
                DataObject data = new DataObject(Connection.Instance.User + ": " + index.ToString());
                string jsonString = Newtonsoft.Json.JsonConvert.SerializeObject(data);
                DomainState.Instance.Upload = jsonString;
                Thread.Sleep(2000);
            }
        }
    }
}
