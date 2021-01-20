using CaptainCombat.Common;
using CaptainCombat.Server.Mapmaker;
using CaptainCombat.Server.Source.threads;
using dotSpace.Interfaces.Space;
using System;

namespace CaptainCombat.Server.Source.LobbyContent
{

    class Lobby
    {
        public string Url { get; set; }
        public string Id { get; set; }
        public string CreatorName { get; set; }
        public uint CreatorId { get; set; }

        private ISpace space;

        private GameDataStreamer streemComponent;
        private FinishChecker finishChecker;

        /// <summary>
        /// Creates a new lobby in the given space
        /// </summary>
        public Lobby(ISpace space, string lobbyId, uint creatorId, string creatorName, string url)
        {
            this.space = space;
            Id = lobbyId;
            CreatorName = creatorName;
            CreatorId = creatorId;
            Url = url;

            // Add locks
            space.Put("winner-lock");
            space.Put("lock");
           
            // Add player slot tuples
            for (int i = 0; i < Settings.LOBBY_SLOTS; i++)
                space.Put("player", 0, "No user");

            // Create controller
            streemComponent = new GameDataStreamer(space);
            finishChecker = new FinishChecker(space, GameFinished);
        }


        public void Start()
        {
            Game game = new Game();
            game.Init(space);
            streemComponent.Start();
            finishChecker.Start();
        }


        private void GameFinished()
        {
            Console.WriteLine("Game finished");
            streemComponent.Stop();
        }
    }
}
