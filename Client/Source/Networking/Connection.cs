using CaptainCombat.Common;
using dotSpace.Interfaces.Space;
using System.Collections.Generic;

namespace CaptainCombat.Client {

    /// <summary>
    /// Contains global information about the connection to the server
    /// </summary>
    static class Connection
    {
        public static ISpace GlobalSpace = null;

        public static Player LocalPlayer = Player.Unknown;

        public static Lobby Lobby = null;
    }


    sealed class Lobby
    {
        public Player Owner = Player.Unknown;
        public ISpace Space = null;
        public List<Player> Players = null;
        public string Url = null;
        public string Id = null;


        public Player GetPlayer(uint id)
        {
            if (id == 1) return Player.Server;
            foreach (var player in Players)
                if (player.Id == id) return player;
            return Player.Unknown;
        }
    }
}
