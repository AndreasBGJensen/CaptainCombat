using CaptainCombat.Common;
using CaptainCombat.Server.Mapmaker;
using dotSpace.Interfaces.Space;
using System;

namespace CaptainCombat.Server
{

    /// <summary>
    /// Represents a Lobby's data and controllers that manages the lobby "flow"
    /// </summary>
    class Lobby
    {
        public string Url { get; set; }
        public string Id { get; set; }
        public string CreatorName { get; set; }
        public uint CreatorId { get; set; }

        private ISpace space;

        private GameDataStreamer gameDataStreamer;
        private FinishListener finishListener;

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
            space.Put("lobby_lock");
           
            // Add player slot tuples
            for (int i = 0; i < Settings.LOBBY_SLOTS; i++)
                space.Put("player", 0, "No user");

            // Create controller
            gameDataStreamer = new GameDataStreamer(space);
            finishListener = new FinishListener(space, GameFinished);
        }


        public void Start()
        {
            Game game = new Game(space);
            game.Start();
            gameDataStreamer.Start();
            finishListener.Start();
        }


        // Clean up the lobby
        private void GameFinished()
        {
            Console.WriteLine("Game finished");
            gameDataStreamer.Stop();

            // Could remove lobby from list here 
        }
    }
}
