using CaptainCombat.singletons;
using dotSpace.Interfaces.Space;
using dotSpace.Objects.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tuple = dotSpace.Objects.Space.Tuple;

namespace CaptainCombat.Source.protocols
{
    public class ClientProtocol
    {

        public static void Connect()
        {
            Console.WriteLine("Connected to server");
            try
            {
                string uri = "tcp://127.0.0.1:5000/space?CONN";
                //string uri = "tcp://49.12.75.251:5000/space?CONN";
                RemoteSpace space = new RemoteSpace(uri);
                Connection connecting = Connection.Instance;
                connecting.Space = space;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        public static List<string> GetAllUsers()
        {
            List<string> allUsers = new List<string>(); 
            
            Connection connecting = Connection.Instance;
            RemoteSpace space = connecting.Space;

            IEnumerable<ITuple> usersInServer = space.QueryAll("users", typeof(string));

            foreach (ITuple user in usersInServer)
            {
                allUsers.Add((string)user[1]); 
            }
            return allUsers; 
        }


        public static IEnumerable<ITuple> GetAllClients()
        {
            return DomainState.Instance.Clients;
        }


        public static IEnumerable<ITuple> GetAllUsersMessages()
        {
            return DomainState.Instance.Messages; 
        }

        public static void AddMessageToServer(string message)
        {
            Connection connecting = Connection.Instance;
            RemoteSpace space = connecting.Space;
            space.Put("chat", Connection.Instance.User_id, message);
        }


        public static void Join(string username)
        {
            Console.WriteLine("Enter game");
            try
            {
                Connection connecting = Connection.Instance;
                RemoteSpace space = connecting.Space;
                space.Put("user", username);
                Tuple results = (Tuple)space.Get("connected", typeof(int), typeof(string));
                Console.WriteLine(results[2]);
                Connection.Instance.User = username;
                Connection.Instance.User_id = (int)results[1];
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }


    }
}
