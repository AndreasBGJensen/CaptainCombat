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
            connecting.Space = space;

            Console.WriteLine("Connection to server succeeded\n");
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


            return Connection.Instance.Space.QueryAll("createdLubby", typeof(string), typeof(string), typeof(string));

        }
        public static bool CreateLobby()
        {
            string username = Connection.Instance.User;
            int user_id = Connection.Instance.User_id;

            
            Connection.Instance.Space.Put("createLobby", username, user_id);
            //Blocking because we want to know if the lobby have been created
            ITuple response = Connection.Instance.Space.Get("createdLubby", user_id, typeof(string), typeof(string));
            string url = (string)response[3];
            if (!url.Contains("tcp"))
            {
                return false;
            }
            Connection.Instance.lobbySpace = new RemoteSpace(url);
            return true;
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
