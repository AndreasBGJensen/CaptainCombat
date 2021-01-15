﻿
namespace CaptainCombat.states
{
    class Game : State
    {
        public Game()
        {
        }

        public object JsonSerializer { get; private set; }

        public override void Run(){
            
            using (var game = new GameController())
                game.Run();

            //RunGameLoop();
        }


        //private void RunGameLoop()
        //{
            
        //    Entity player = new Entity(DomainState.Instance.Domain, (uint)Connection.Instance.User_id);
        //    player.AddComponent(new Transform());

        //    JsonBuilder builder = new JsonBuilder(); 
        //    while (true)
        //    {
        //        DomainState.Instance.Domain.Clean();
        //        Console.WriteLine("Game running");
        //        DomainState.Instance.Upload = builder.createJsonString(); 
        //        Thread.Sleep(2000);
        //    }
        //}

    }
}
