using CaptainCombat.Common;
using dotSpace.Interfaces.Space;
using dotSpace.Objects.Network;
using dotSpace.Objects.Space;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
        public Lobby(SpaceRepository repository)
        {
            this.repository = repository;
        }

        public ITuple CreateLobby(string username, int user_id)
        {
            creator = username;
            CreateUri(user_id);
            if(lobbyUrl != null)
            {
                AddGameLobby();
                
                return new Tuple("lobbyCreationResponse", user_id, username, lobbyUrl);
            }
            return new Tuple("lobbyCreationResponse", user_id, username, "You have alreadey created a lobby");

        }

        private void CreateUri(int user_id)
        {
            spaceID = "game" + user_id;
            lobbyUrl = "tcp://" + ConnectionInfo.SERVER_ADDRESS + "/"+ spaceID+ "?KEEP";
            
            if (!LobbyBookKeeping.AddLobby(this))
            {
                throw new ArgumentException("Lobby Could not be saved in database");
            }
        }


        private void AddGameLobby()
        {
            lobby = new SequentialSpace();
            repository.AddSpace(spaceID, lobby);

        }

    }
}
