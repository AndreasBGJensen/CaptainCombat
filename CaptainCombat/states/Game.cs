
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
        }

        public object JsonSerializer { get; private set; }

        public override void Run(){

            Domain domain = new Domain();
            DomainState.Instance.Domain = domain;

            Upload upload = new Upload();
            Thread uploadThread = new Thread(new ThreadStart(upload.RunProtocol));
            uploadThread.Start();

            DownLoad download = new DownLoad();
            Thread downloadThread = new Thread(new ThreadStart(download.RunProtocol));
            downloadThread.Start();

            RunGameLoop();
        }


        private void RunGameLoop()
        {
            
            Entity player = new Entity(DomainState.Instance.Domain, (uint)Connection.Instance.User_id);
            player.AddComponent<Transform>();

            DomainState.Instance.Domain.Clean(); 

            JsonBuilder builder = new JsonBuilder(); 
            while (true)
            {
                Console.WriteLine("Game running");
                DomainState.Instance.Upload = builder.createJsonString(); 
                Thread.Sleep(2000);
            }
        }

        private object Transform()
        {
            throw new NotImplementedException();
        }
    }
}
