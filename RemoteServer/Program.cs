using System;
using System.Threading;
using dotSpace.Interfaces.Space;
using dotSpace.Objects.Network;
using dotSpace.Objects.Space;
using RemoteServer.Mapmaker;
using RemoteServer.Mapmaker.EntityToAdd;
using RemoteServer.singletons;
using RemoteServer.threads;
using Tuple = dotSpace.Objects.Space.Tuple;

namespace RemoteServer
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
            IEntity entity = new Rocks(10);
            game.ComputerInit += entity.OnComputerInit;

            game.Init();


            Serialization serializationProtocol = new Serialization();
            Thread serializationThread = new Thread(new ThreadStart(serializationProtocol.RunProtocol));
            serializationThread.Start();

            //serializationProtocol.RunProtocol();
        }

        /*
        static void Main(string[] args)
        {
            //string uri = "tcp://127.0.0.1:5000?CONN";
            string uri = "tcp://49.12.75.251:5000?CONN";

            SpaceRepository repository = new SpaceRepository();
            repository.AddGate(uri);
            SequentialSpace space = new SequentialSpace();
            repository.AddSpace("space", space);
            Console.WriteLine("Start Gameserver");
            Connection.Instance.Space = space;

            NewUsers newUserProtocol = new NewUsers();
            Thread newUserThread = new Thread(new ThreadStart(newUserProtocol.RunProtocol));
            newUserThread.Start();

            Serialization serializationProtocol = new Serialization();
            Thread serializationThread = new Thread(new ThreadStart(serializationProtocol.RunProtocol));
            serializationThread.Start();

        }
        */

    }
}
