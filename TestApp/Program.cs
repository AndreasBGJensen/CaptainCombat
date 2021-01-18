using CaptainCombat.Common;
using CaptainCombat.Common.Singletons;
using dotSpace.Interfaces.Space;
using dotSpace.Objects.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace TestApp
{
    class Program
    {
        static void Main(string[] args)
        {
            string serverUrl = "tcp://" + ConnectionInfo.SERVER_ADDRESS + "/" + ConnectionInfo.SPACE_NAME + "?KEEP";
            Console.WriteLine($"Connecting to server at '{serverUrl}'...");

            ISpace space = new RemoteSpace(serverUrl);

            try
            {
                // Connection is made first time some request is being made
                space.QueryP("disregard this string content");
            }
            catch (SocketException e)
            {
                // TODO: Handle connection issues here
                Console.WriteLine("Connection to server failed\n");
                throw e;
            }

            Connection connecting = Connection.Instance;
            connecting.Space = space;

            Console.WriteLine("Connection to server succeeded\n");


            connecting.Space.Put("createLobby", "Andreas", 1);
            connecting.Space.Put("createLobby", "Per", 2);
            connecting.Space.Put("createLobby", "Hans",3);

            ITuple respons = connecting.Space.Get("lobbyResponse", 1, "Andreas", typeof(string));
            GetAllLobbys(connecting.Space);


            Console.ReadLine();

        }

        static void GetAllLobbys(ISpace space)
        {
            IEnumerable<ITuple> allLobbyes = space.QueryAll("createdLubby", typeof(string), typeof(string), typeof(string));
            foreach(ITuple x in allLobbyes)
            {
                Console.WriteLine((string)x[1] + (string)x[2], (string)x[3]);
            }


        }
    }
}
