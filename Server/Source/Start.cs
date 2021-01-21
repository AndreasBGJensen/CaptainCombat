using System;
using System.Threading;
using dotSpace.Objects.Network;
using dotSpace.Objects.Space;
using CaptainCombat.Common;

namespace CaptainCombat.Server
{

    static class Start
    {
        static void Main(string[] args)
        {
            string serverUrl = "tcp://" + ConnectionInfo.SERVER_ADDRESS + "/space?KEEP";

            Console.WriteLine($"Launching server at '{serverUrl}'");

            SpaceRepository repository = new SpaceRepository();
            repository.AddGate(serverUrl);
            SequentialSpace space = new SequentialSpace();
            repository.AddSpace(ConnectionInfo.SPACE_NAME, space);

            Connection.Space = space;
            Connection.repository = repository;

            PlayerController newUserProtocol = new PlayerController();
            Thread newUserThread = new Thread(new ThreadStart(newUserProtocol.RunProtocol));
            newUserThread.Start();

            LobbyController lobbyController = new LobbyController(repository);
            lobbyController.Start();

            Console.WriteLine("Server started");
        }
    }
}
