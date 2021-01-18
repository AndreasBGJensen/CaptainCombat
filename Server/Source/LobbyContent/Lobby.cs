using CaptainCombat.Common;
using dotSpace.Interfaces.Space;
using dotSpace.Objects.Network;
using dotSpace.Objects.Space;
using System;
using System.Text;
using Tuple = dotSpace.Objects.Space.Tuple;

namespace CaptainCombat.Server.Source.LobbyContent
{
    public class Lobby
    {

        private SpaceRepository repository;
        private SequentialSpace lobby;
        public string lobbyUrl { get; set; }
        public string spaceID { get; set; }
        public string creator { get; set; }

        private readonly int MAX_NUM_PLAYERS = 3;

        public Lobby(SpaceRepository repository)
        {
            this.repository = repository;
        }

        public ITuple CreateLobby(string username, int user_id)
        {
            creator = username;
            CreateUrl(user_id);
            if(lobbyUrl != null)
            {
                AddGameLobby();
                
                return new Tuple("lobbyCreationResponse", user_id, username, lobbyUrl);
            }
            return new Tuple("lobbyCreationResponse", user_id, username, "You have alreadey created a lobby");
        }

        private void CreateUrl(int user_id)
        {
           
            //TODO: Because remoteSpace are only able to connect to a spase, contaning letters we converet the user_id to a char.
            //This only alows us to have 20 players
            int unicode = user_id * 100;
            char character = (char)unicode;
            spaceID = character.ToString();
            
            lobbyUrl = "tcp://" + ConnectionInfo.SERVER_ADDRESS + "/"+ spaceID+ "?KEEP";
            
            if (!LobbyBookKeeping.AddLobby(this))
            {
                throw new ArgumentException("Lobby Could not be saved in database");
            }  
        }


        private void AddGameLobby()
        {
            lobby = new SequentialSpace();
           ReservePlayersToSpace(lobby, MAX_NUM_PLAYERS);
            repository.AddSpace(spaceID, lobby);
        }

        private void ReservePlayersToSpace(SequentialSpace space, int players_to_reserve)
        {
            for(int i = 0;i < players_to_reserve; i++)
            {
                space.Put("Player", typeof(int), typeof(string));
            } 
        }
    }
}
