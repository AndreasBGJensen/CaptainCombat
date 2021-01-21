using CaptainCombat.Common;
using CaptainCombat.Common.Singletons;
using dotSpace.Objects.Network;
using dotSpace.Objects.Space;
using System;
using System.Collections.Concurrent;
using System.Threading;


namespace CaptainCombat.Server
{

    class LobbyController
    {
        private SpaceRepository repository;
        
        // Mapping of lobby id to lobby
        private ConcurrentDictionary<string, Lobby> lobbies = new ConcurrentDictionary<string, Lobby>();

        private Thread thread;


        public LobbyController(SpaceRepository repository)
        {
            this.repository = repository;
        }


        public void Start()
        {
            thread = new Thread(ListenForLobbyRequests);
            thread.Start();
        }


        private void ListenForLobbyRequests()
        {
            while (true)
            {
                var request = Connection.Instance.Space.Get("createLobby", typeof(string), typeof(int));

                var userName = (string)request[1];
                var userId = (uint)(int)request[2];

                HandleCreationRequest(userId, userName);
            }         
        }


        // Create lobby (including tuple space), and send response
        // to player
        private void HandleCreationRequest(uint playerId, string playerName)
        {
            string lobbyId = GenerateLobbyId();

            string lobbyUrl = "tcp://" + ConnectionInfo.SERVER_ADDRESS + "/" + lobbyId + "?KEEP";

            var lobbySpace = new SequentialSpace();
            repository.AddSpace(lobbyId, lobbySpace);

            Lobby lobby = new Lobby(lobbySpace, lobbyId, playerId, playerName, lobbyUrl);

            lobbies.TryAdd(lobbyId, lobby);

            // Create lobby tuple
            Connection.Instance.Space.Put("existingLobby", lobbyId, playerName, lobbyUrl);
            lobby.Start();

            Console.WriteLine($"Lobby created: '{lobbyUrl}' (creator: {playerName})");

            // Returning space information to the global space so that users can connect to the space.
            Connection.Instance.Space.Put("lobbyCreationResponse", (int)playerId, playerName, lobbyUrl);
        }


        // Creates a random and unique lobby id
        private string GenerateLobbyId()
        {
            string lobbyId;
            do
            {
                lobbyId = "";
                for( int i=0; i<16; i++ )
                    lobbyId += (char)RandomGenerator.Integer(97, 122);
                
            } while (lobbies.ContainsKey(lobbyId));
            return lobbyId;
        }
    }
}
