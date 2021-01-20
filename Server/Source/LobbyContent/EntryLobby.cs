using CaptainCombat.Common.Singletons;
using dotSpace.Interfaces.Space;
using System.Collections.Generic;
using System.Threading;

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
            
            ListenForLobbyRequests();
        }

        private void ListenForLobbyRequests()
        {
            while (true)
                {
                    IEnumerable<ITuple> lobbyReuests = Connection.Instance.Space.GetAll("createLobby", typeof(string), typeof(int));

                    if(lobbyReuests!= null)
                    {
                        
                        //Console.WriteLine("Handling lobby request");
                        foreach(ITuple request in lobbyReuests)
                        {
                            Lobby newLobby = new Lobby(Connection.Instance.repository);
                            
                            //Returning space information to the global space so that users can connect to the space.
                            Connection.Instance.Space.Put(newLobby.CreateLobby((string)request[1], (int)request[2]));
                            
                            //Begin the lobby thread
                            //The space will only begin, when the tuple {<start>} is in the space.
                            Thread lobbyThread = new Thread(new ThreadStart(newLobby.RunProtocol));
                            lobbyThread.Start();
                        }
                    }  
            }         
        }
    }
}
