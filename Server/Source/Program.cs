using System;
using System.Threading;
using dotSpace.Objects.Network;
using dotSpace.Objects.Space;
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
            Connection.Instance.repository = repository;

            PlayerController newUserProtocol = new PlayerController();
            Thread newUserThread = new Thread(new ThreadStart(newUserProtocol.RunProtocol));
            newUserThread.Start();

            LobbyController lobbyController = new LobbyController(repository);
            lobbyController.Start();

            Console.WriteLine("Server started");
        }
    }
}
