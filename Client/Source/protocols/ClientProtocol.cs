using dotSpace.Interfaces.Space;
using dotSpace.Objects.Network;
using CaptainCombat.Common.Singletons;
using System;
using System.Collections.Generic;
using Tuple = dotSpace.Objects.Space.Tuple;
using CaptainCombat.Common;
using System.Net.Sockets;
using System.Threading;

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

        public static List<string> GetAllClientInLobby()
        {
            List<string> allUsers = new List<string>();

            Connection connecting = Connection.Instance;
            ISpace space = connecting.lobbySpace;

            IEnumerable<ITuple> usersInServer = space.QueryAll("users", typeof(string));

            foreach (ITuple user in usersInServer)
            {
                allUsers.Add((string)user[1]);
            }
            return allUsers;
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
            Connection.Instance.lobbySpace = new RemoteSpace(url);
            Connection.Instance.Space = Connection.Instance.lobbySpace;
            return true;
        }


        public static void BeginMatch()
        {
            Connection.Instance.Space.Get("lock");
            Connection.Instance.Space.Put("start");
            Connection.Instance.Space.Put("lock");
        }

        public static bool SubscribeForLobby(string lobbyUrl)
        {
            RemoteSpace lobbySpace = new RemoteSpace(lobbyUrl);
            lobbySpace.Get("lock");
            ITuple playerTuple = lobbySpace.GetP("player", typeof(int), typeof(string));

            //Means that that there is no sluts left in the lobby
            if(playerTuple == null)
            {
                lobbySpace.Put("lock");
                return false;
            }

            //Changing the space to the selected lobbyspace
            Connection.Instance.lobbySpace = lobbySpace;
            Connection.Instance.Space = Connection.Instance.lobbySpace;
            lobbySpace.Put("lock");

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
                count++;
            }

            //TODO: Find global state for max number of subscribers in a lobby
            //The default is 4 and is set in the lobbyClass on the Server
            return count;
        }

        public static IEnumerable<ITuple> GetAllClients()
        {
            return DomainState.Instance.Clients;
        }

        public static IEnumerable<ITuple> GetAllClientScores()
        {
            return DomainState.Instance.ClientScores;
        }

        public static IEnumerable<ITuple> GetAllUsersMessages()
        {
            return DomainState.Instance.Messages; 
        }

        public static void AddMessageToServer(string message)
        {
            Connection connecting = Connection.Instance;
            ISpace space = connecting.Space;
            space.Put("chat", Connection.Instance.User_id, message);
        }


        public static void Join(string username)
        {
            Console.WriteLine("Enter game");
            Connection connecting = Connection.Instance;
            ISpace space = connecting.Space;
            space.Put("user", username);
            Tuple results = (Tuple)space.Get("connected", typeof(int), typeof(string));
            Console.WriteLine(results[2]);
            Connection.Instance.User = username;
            Connection.Instance.User_id = (int)results[1];
        }

        public static void AddClientScoreToServer(int clientScore)
        {
            Connection connecting = Connection.Instance;
            ISpace space = connecting.Space;
            space.Put("score", Connection.Instance.User_id, clientScore);
        }


        public static bool isValidName(string username)
        {
            Connection connecting = Connection.Instance;
            ISpace space = connecting.Space;

            IEnumerable<ITuple> usersInServer = space.QueryAll("users", typeof(string));

            foreach (ITuple user in usersInServer)
            {
                if (((string)user[1]).Equals(username))
                {
                    return false; 
                }
            }
            return true; 
        }
    }
}
