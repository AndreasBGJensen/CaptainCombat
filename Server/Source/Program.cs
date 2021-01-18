using System;
using System.Threading;
using dotSpace.Objects.Network;
using dotSpace.Objects.Space;
using CaptainCombat.Server.Mapmaker;
using CaptainCombat.Server.threads;
using CaptainCombat.Common.Singletons;
using CaptainCombat.Common;

namespace CaptainCombat.Server
{

    class Program
    {
        static void Main(string[] args)
        {
            // TODO: Try to remove the "space"
            string serverUrl = "tcp://" + ConnectionInfo.SERVER_ADDRESS + "/space?KEEP";

            Console.WriteLine($"Launching server at '{serverUrl}'");

            SpaceRepository repository = new SpaceRepository();
            repository.AddGate(serverUrl);
            SequentialSpace space = new SequentialSpace();
            repository.AddSpace(ConnectionInfo.SPACE_NAME, space);
            Connection.Instance.Space = space;

            // TODO: Move this adding of lock to approriate place
            space.Put("life-lock");

            Console.WriteLine("Server started");

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
