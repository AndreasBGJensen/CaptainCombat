using System;
using System.Threading;
using dotSpace.Objects.Network;
using dotSpace.Objects.Space;
using CaptainCombat.Server.Mapmaker;
using CaptainCombat.Server.threads;
using CaptainCombat.Common.Singletons;

namespace CaptainCombat.Server
{

    class Program
    {
        private static bool test_mode = true;
        static GlobalInfo info = new GlobalInfo();
        static void Main(string[] args)
        {

            string uri;
            if (test_mode) {
               
                uri = info.test_URI;
            }
            else {
                uri = info.server_URI;
            }
            Console.WriteLine(uri);

            SpaceRepository repository = new SpaceRepository();
            repository.AddGate(uri);
            SequentialSpace space = new SequentialSpace();
            repository.AddSpace("space", space);
            Console.WriteLine("Start Gameserver");
            Connection.Instance.Space = space;

            NewUsers newUserProtocol = new NewUsers();
            Thread newUserThread = new Thread(new ThreadStart(newUserProtocol.RunProtocol));
            newUserThread.Start();

            ClientScores newClientScoreProtocol = new ClientScores();
            Thread newClientScoreThread = new Thread(new ThreadStart(newClientScoreProtocol.RunProtocol));
            newClientScoreThread.Start();


            Game game = new Game();
            //IEntity entity = new Rocks(10);
            //game.ComputerInit += entity.OnComputerInit;

            game.Init();


            Serialization serializationProtocol = new Serialization();
            Thread serializationThread = new Thread(new ThreadStart(serializationProtocol.RunProtocol));
            serializationThread.Start();
        }
    }
}
