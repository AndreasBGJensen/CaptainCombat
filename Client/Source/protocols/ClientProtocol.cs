using dotSpace.Interfaces.Space;
using dotSpace.Objects.Network;
using CaptainCombat.Common.Singletons;
using System;
using System.Collections.Generic;
using Tuple = dotSpace.Objects.Space.Tuple;
using CaptainCombat.Common;
using System.Net.Sockets;

namespace CaptainCombat.Client.protocols
{
    public class ClientProtocol
    {

        public static void Connect()
        {
            string serverUrl = "tcp://" + ConnectionInfo.SERVER_ADDRESS + "/" + ConnectionInfo.SPACE_NAME + "?KEEP";
            Console.WriteLine($"Connecting to server at '{serverUrl}'...");

            ISpace space = new RemoteSpace(serverUrl);

            try {
                // Connection is made first time some request is being made
                space.QueryP("disregard this string content");
            }
            catch (SocketException e) {
                // TODO: Handle connection issues here
                Console.WriteLine("Connection to server failed\n");
                throw e;
            }

            Connection connecting = Connection.Instance;
            connecting.globalSpace = space;
            connecting.Space = connecting.globalSpace;

            Console.WriteLine("Connection to server succeeded\n");
        }


        public static IEnumerable<ITuple> GetClientsInLobby()
        {
            return Connection.Instance.LobbySpace.QueryAll("player", typeof(int), typeof(string));
        }


        public static List<string> GetAllUsers()
        {
            List<string> allUsers = new List<string>(); 
            
            Connection connecting = Connection.Instance;
            ISpace space = connecting.Space;

            IEnumerable<ITuple> usersInServer = space.QueryAll("users", typeof(string));

            foreach (ITuple user in usersInServer)
            {
                allUsers.Add((string)user[1]); 
            }
            return allUsers; 
        }

        public static IEnumerable<ITuple> GetAllLobbys()
        {
            return Connection.Instance.Space.QueryAll("existingLobby", typeof(string), typeof(string), typeof(string));
        }


        public static bool CreateLobby()
        {
            
            string username = Connection.Instance.User;
            int user_id = Connection.Instance.User_id;

            
            Connection.Instance.Space.Put("createLobby", username, user_id);
            //Blocking because we want to know if the lobby have been created
            ITuple response = Connection.Instance.Space.Get("lobbyCreationResponse", user_id, typeof(string), typeof(string));
            string url = (string)response[3];
            if (!url.Contains("tcp"))
            {
                return false;
            }
            Connection.Instance.LobbySpace = new RemoteSpace(url);
            Connection.Instance.LobbySpace.Get("lobby_lock");
            Connection.Instance.LobbySpace.GetP("player",typeof(int),typeof(string));
            Connection.Instance.LobbySpace.Put("player", user_id, username);
            Connection.Instance.lobbyUrl = url;
            Connection.Instance.LobbySpace.Put("lobby_lock");

            return true;
        }

        public static bool ListenForMatchBegin()
        {
            ITuple response = Connection.Instance.LobbySpace.QueryP("start");
            if (response != null)
            {
                return true; 
            }
            return false; 
        }


        public static void BeginMatch()
        {
            Connection.Instance.LobbySpace.Get("lobby_lock");
            Connection.Instance.LobbySpace.Put("start");
            //Removing the lobby from the global space
            ITuple tuple = Connection.Instance.Space.Get("existingLobby", typeof(string), typeof(string), Connection.Instance.lobbyUrl);
            Connection.Instance.LobbySpace.Put("lobby_lock");
        }

        public static bool SubscribeForLobby(string lobbyUrl)
        {
            string username = Connection.Instance.User;
            int user_id = Connection.Instance.User_id;

            RemoteSpace lobbySpace = new RemoteSpace(lobbyUrl);
            lobbySpace.Get("lobby_lock");
            ITuple playerTuple = lobbySpace.GetP("player", 0, "No user");
            
            //Means that that there is no sluts left in the lobby
            if (playerTuple == null)
            {
                lobbySpace.Put("lobby_lock");
                return false;
            }
            Connection.Instance.LobbySpace = lobbySpace;
            lobbySpace.Put("player", user_id, username);
            //Changing the space to the selected lobbyspace
            
            lobbySpace.Put("lobby_lock");

            return true;
        }

        public static int GetNumberOfSubscribersInALobby(string lobbyUrl)
        {
            RemoteSpace lobbySpace = new RemoteSpace(lobbyUrl);
            IEnumerable<ITuple> subscriberTuple = lobbySpace.QueryAll("player", typeof(int), typeof(string));

            //TODO: See if there is a better way
            int count = 0;

            foreach(ITuple subscriber in subscriberTuple)
            {
                if((int)subscriber[1] > 0 && (string)subscriber[2] != "No user")
                {
                    count++;
                }
                
            }

            //TODO: Find global state for max number of subscribers in a lobby
            //The default is 4 and is set in the lobbyClass on the Server
            return count;
        }

        public static IEnumerable<ITuple> GetSubscribersInALobby(string lobbyUrl)
        {
            RemoteSpace lobbySpace = new RemoteSpace(lobbyUrl);
            IEnumerable<ITuple> subscriberTuple = lobbySpace.QueryAll("player", typeof(int), typeof(string));

            return subscriberTuple;

        }


        public static IEnumerable<ITuple> GetAllClients()
        {
            return DomainState.Instance.Clients;
        }

        public static bool Join(string username)
        {
            Console.WriteLine("Validate username");
            ISpace space = Connection.Instance.Space;

            // Get lock 
            space.Get("newuser_lock"); 
            space.Put("user", username);
            Tuple result = (Tuple)space.Get("connected", typeof(bool), typeof(int));

            if ((bool)result[1])
            {
                Console.WriteLine("Join succesfull");
                Connection.Instance.User = username;
                Connection.Instance.User_id = (int)result[2];
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
            ISpace space = Connection.Instance.LobbySpace;
            space.Put("score", Connection.Instance.User_id, clientScore);
        }

    }
}
