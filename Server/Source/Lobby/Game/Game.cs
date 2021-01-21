using dotSpace.Interfaces.Space;
using CaptainCombat.Server.Mapmaker.EntityToAdd;
using System;
using CaptainCombat.Common;
using CaptainCombat.Common.JsonBuilder;

namespace CaptainCombat.Server.Mapmaker
{

    /// <summary>
    /// The Server's collection of the game logic
    /// As of now it only uploads the static data (rocks and boundaries)
    /// </summary>
    class Game
    {
        private ISpace lobbySpace;
        private Domain domain;

        public Game(ISpace lobbySpace)
        {
            this.lobbySpace = lobbySpace;
            domain = new Domain(Player.Server.Id);
        }

        public void Start()
        {
            RockGenerator.Generate(domain, Settings.NUM_ROCKS);
            domain.Clean();

            // Upload the Server's domain
            // We're just using UpdateId 1, as this will be the only update from the server
            lobbySpace.Put("components", "1", JsonBuilder.BuildComponentsJSONString(domain));
        }
    }
}
