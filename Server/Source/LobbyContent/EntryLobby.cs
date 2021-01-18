using CaptainCombat.Common;
using CaptainCombat.Common.Singletons;
using dotSpace.Interfaces.Space;
using dotSpace.Objects.Network;
using dotSpace.Objects.Space;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CaptainCombat.Server.Source.LobbyContent
{
    class EntryLobby
    {
        public EntryLobby()
        {
            InitEntry();
        }

        public void InitEntry()
        {
          /*  string serverUrl = "tcp://" + ConnectionInfo.SERVER_ADDRESS + "?KEEP";

            Console.WriteLine($"Launching server at '{serverUrl}'");
            Console.WriteLine("Lobby is listening...");
            repository = new SpaceRepository();
            repository.AddGate(serverUrl);
            SequentialSpace space = new SequentialSpace();
            repository.AddSpace(ConnectionInfo.SPACE_NAME, space);
            Connection.Instance.Space = space;

            Console.WriteLine("Server started");*/
            
            ListenForLobbyRequests();
        }

        private void ListenForLobbyRequests()
        {
            //new Thread(() =>
            //{
                while (true)
                {
                    IEnumerable<ITuple> lobbyReuests = Connection.Instance.Space.GetAll("createLobby", typeof(string), typeof(int));

                    if(lobbyReuests!= null)
                    {
                        
                        Console.WriteLine("Handling lobby request");
                        foreach(ITuple request in lobbyReuests)
                        {
                            Lobby newLobby = new Lobby(Connection.Instance.repository);
                            Connection.Instance.Space.Put(newLobby.CreateLobby((string)request[1], (int)request[2]));



                        }
                    }
                }
            //}).Start();
        }

        

    }
}
