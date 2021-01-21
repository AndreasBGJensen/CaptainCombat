using dotSpace.Interfaces.Space;
using dotSpace.Objects.Network;
using System;
using System.Collections.Generic;
using Tuple = dotSpace.Objects.Space.Tuple;
using CaptainCombat.Common;
using System.Net.Sockets;
using CaptainCombat.Client.Source.Layers;


namespace CaptainCombat.Client.protocols
{

    class ClientProtocol
    {

        public static void ConnectToServer()
        {
            string serverUrl = "tcp://" + ConnectionInfo.SERVER_ADDRESS + "/" + ConnectionInfo.SPACE_NAME + "?KEEP";
            Console.WriteLine($"Connecting to server at '{serverUrl}'...");

            ISpace space = new RemoteSpace(serverUrl);

            try {
                // Connection is made first time some request is being made
                space.QueryP("disregard this string content");
            }
            catch (SocketException e) {
                Console.WriteLine("Connection to server failed\n");
                throw e;
            }

            Connection.GlobalSpace = space;

            Console.WriteLine("Connection to server succeeded\n");
        }


        public static IEnumerable<ITuple> GetClientsInLobby()
        {
            return Connection.Lobby.Space.QueryAll("player", typeof(int), typeof(string));
        }


        public static List<string> GetAllUsers()
        {
            List<string> allUsers = new List<string>(); 
            
            IEnumerable<ITuple> usersInServer = Connection.GlobalSpace.QueryAll("users", typeof(string));

            foreach (ITuple user in usersInServer)
            {
                allUsers.Add((string)user[1]); 
            }
            return allUsers; 
        }


        /// <summary>
        /// Fetches information about all existing lobbies
        /// May perform many server calls
        /// </summary>
        public static List<LobbyInfo> GetLobbies()
        {
            var lobbies = new List<LobbyInfo>();
            var lobbyTuples = Connection.GlobalSpace.QueryAll("existingLobby", typeof(string), typeof(string), typeof(string));

            foreach(var lobbyTuple in lobbyTuples)
            {
                // Setup basic info
                LobbyInfo lobby;
                lobby.Id = (string)lobbyTuple[1];
                lobby.OwnerName = (string)lobbyTuple[2];
                lobby.Url = (string)lobbyTuple[3];

                // Connect to lobby space, and fetch players
                // Note: It's super slow to establish this connection everytime
                RemoteSpace lobbySpace = new RemoteSpace(lobby.Url);

                lobby.NumPlayers = 0;
                var playerTuples = lobbySpace.QueryAll("player", typeof(int), typeof(string));
                foreach (var playerTuple in playerTuples)
                {
                    // Check that slot is filled
                    if ((int)playerTuple[1] > 1 && (string)playerTuple[2] != "No user")
                        lobby.NumPlayers++;
                }

                lobbies.Add(lobby);
            }

            return lobbies;
        }



        public static bool CreateLobby()
        {
            var playerName = Connection.LocalPlayer.Name;
            var playerId = (int)Connection.LocalPlayer.Id;

            Connection.GlobalSpace.Put("createLobby", playerName, playerId);

            //Blocking because we want to know if the lobby have been created
            var response = Connection.GlobalSpace.Get("lobbyCreationResponse", playerId, typeof(string), typeof(string));
            string url = (string)response[3];
            if (!url.Contains("tcp"))
            {
                return false;
            }

            // Get lobby id (wasn't given with responsee)
            var lobbyTuple = Connection.GlobalSpace.Query("existingLobby", typeof(string), typeof(string), url);

            Connection.Lobby = new Lobby();
            Connection.Lobby.Space = new RemoteSpace(url);
            Connection.Lobby.Url = url;
            Connection.Lobby.Owner = Connection.LocalPlayer;
            Connection.Lobby.Id = (string)lobbyTuple[1];

            Connection.Lobby.Space.Get("lobby_lock");
            Connection.Lobby.Space.GetP("player",typeof(int),typeof(string));
            Connection.Lobby.Space.Put("player", playerId, playerName);
            Connection.Lobby.Space.Put("lobby_lock");

            return true;
        }


        public static bool StartGame()
        {
            Connection.Lobby.Space.Get("lobby_lock");

            var playersInLobby = GetPlayersInLobby();

            //Check if enough players in the lobby (at least two is needed)
            if (playersInLobby.Count > 1)
            {
                Connection.Lobby.Players = playersInLobby;

                Connection.Lobby.Space.Put("start");
                //Removing the lobby from the global space
                ITuple tuple = Connection.GlobalSpace.Get("existingLobby", typeof(string), typeof(string), Connection.Lobby.Url);

                Console.WriteLine("Match is starting");
                Connection.Lobby.Space.Put("lobby_lock");
                return true;
            }
            else
            { // Not enough players
                Connection.Lobby.Space.Put("lobby_lock");
                return false;
            }           
        }


        public static bool IsGameStarted()
        {
            var response = Connection.Lobby.Space.QueryP("start");
            if( response != null )
            {
                Connection.Lobby.Players = GetPlayersInLobby();
                return true;
            }
            return false;
        }


        /// <summary>
        /// Retrieves the Id for the user with the given name, or
        /// 0 if the user does not exist
        /// </summary>
        public static uint GetPlayerId(string playerName)
        {
            var playerTuple = Connection.GlobalSpace.QueryP("usersInGame", typeof(int), playerName);
            if (playerTuple == null) return 0;
            return (uint)(int)playerTuple[1];
        }


        public static bool SubscribeForLobby(string lobbyUrl)
        {
            string username = Connection.LocalPlayer.Name;
            int user_id = (int)Connection.LocalPlayer.Id;

            RemoteSpace lobbySpace = new RemoteSpace(lobbyUrl);
            lobbySpace.Get("lobby_lock");
            ITuple playerTuple = lobbySpace.GetP("player", 0, "No user");
            
            //Means that that there is no slots left in the lobby
            if (playerTuple == null)
            {
                lobbySpace.Put("lobby_lock");
                return false;
            }

            // Get lobby info (wasn't given with responsee)
            var lobbyTuple = Connection.GlobalSpace.Query("existingLobby", typeof(string), typeof(string), lobbyUrl);

            var ownerName = (string)lobbyTuple[2];
            var ownerId = GetPlayerId(ownerName);
            
            Connection.Lobby = new Lobby();
            Connection.Lobby.Space = lobbySpace;
            Connection.Lobby.Url = lobbyUrl;
            Connection.Lobby.Owner = new Player(ownerId, ownerName);
            Connection.Lobby.Id = (string)lobbyTuple[1];

            lobbySpace.Put("player", user_id, username);
            lobbySpace.Put("lobby_lock");

            return true;
        }


        public static List<Player> GetPlayersInLobby()
        {
            // Fetch lobby players
            var playersInLobby = new List<Player>();
            var playerTuples = Connection.Lobby.Space.QueryAll("player", typeof(int), typeof(string));
            foreach (var playerTuple in playerTuples)
            {
                if ((int)playerTuple[1] != 0)
                    playersInLobby.Add(new Player((uint)(int)playerTuple[1], (string)playerTuple[2]));
            }
            return playersInLobby;
        }


        public static int GetNumberOfSubscribersInALobby(string lobbyUrl)
        {
            RemoteSpace lobbySpace = new RemoteSpace(lobbyUrl);
            IEnumerable<ITuple> subscriberTuple = lobbySpace.QueryAll("player", typeof(int), typeof(string));

            int count = 0;
            foreach(ITuple subscriber in subscriberTuple)
                if((int)subscriber[1] > 0 && (string)subscriber[2] != "No user")
                    count++;

            return count;
        }


        public static IEnumerable<ITuple> GetSubscribersInALobby(string lobbyUrl)
        {
            RemoteSpace lobbySpace = new RemoteSpace(lobbyUrl);
            IEnumerable<ITuple> subscriberTuple = lobbySpace.QueryAll("player", typeof(int), typeof(string));

            return subscriberTuple;

        }


        public static bool Join(string username)
        {
            Console.WriteLine("Validate username");
            ISpace space = Connection.GlobalSpace;

            // Get lock 
            space.Get("newuser_lock"); 
            space.Put("user", username);
            Tuple result = (Tuple)space.Get("connected", typeof(bool), typeof(int));

            if ((bool)result[1])
            {
                Console.WriteLine("Join succesfull");
                Connection.LocalPlayer = new Player((uint)(int)result[2], username);
                space.Put("newuser_lock");
                return true;
            }
            else
            {
                space.Put("newuser_lock");
                return false; 
            }
        }


        public static void AddClientScoreToServer(int clientScore)
        {
            ISpace space = Connection.Lobby.Space;
            space.Put("score", (int)Connection.LocalPlayer.Id, clientScore);
        }

    }
}
