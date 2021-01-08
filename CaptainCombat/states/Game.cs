
using CaptainCombat.game;
using CaptainCombat.singletons;
using CaptainCombat.network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ECS;
using static ECS.Domain;
using CaptainCombat.json;

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
            Domain domain = new Domain();
            DomainState.Instance.Domain = domain; 
            Entity player = new Entity(domain, Connection.Instance.User_id);
            player.AddComponent<Transform>();
            domain.Clean(); 

            JsonBuilder builder = new JsonBuilder(); 
           
            while (true)
            {
                Console.WriteLine("Game running");
                DomainState.Instance.Upload = builder.createJsonString(domain); 
                Thread.Sleep(2000);
            }
        }

        private object Transform()
        {
            throw new NotImplementedException();
        }
    }
}
