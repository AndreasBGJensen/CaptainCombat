using CaptainCombat.network;
using CaptainCombat.Client.GameLayers;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.Threading;
using System;
using CaptainCombat.Client.Layers;
using CaptainCombat.Client.protocols;
using CaptainCombat.Common.Singletons;
using CaptainCombat.Client.NetworkEvent;
using CaptainCombat.Client.Source.Scenes;

namespace CaptainCombat.Client.Scenes
{

    class GameState : State
    {
        private List<Layer> Layers = new List<Layer>();
        private Game game;
        private Background background;
        private Score score;
        private EventController eventController;
        private LifeController lifeController;

        private bool gameReady = false;
        private bool gameStarted = false;

        private bool winScreenDisplayed = false;
        

        public GameState(Game game)
        {
            this.game = game;

            eventController = new EventController();
            lifeController = new LifeController(eventController);

            // Initialize the game on seperate thread
            // to prevent UI from freezing
            new Thread(InitializeGame).Start();     

            background = new Background(lifeController, eventController);
            Layers.Add(background);

            score = new Score(lifeController);
            Layers.Add(score);
            Layers.Add(new Chat(eventController));

            Console.WriteLine("Initialized game state!");
        }


        public void InitializeGame() {
            // TODO: Make a proper init section
            GameInfo.Current = new GameInfo();
            foreach (var tuple in ClientProtocol.GetClientsInLobby()) {
                var clientId = (uint)(int)tuple[1];
                var clientName = (string)tuple[2];
                if (clientId != 0)
                    GameInfo.Current.AddClient(new Player(clientId, clientName));
            }

            lifeController.Start();
            eventController.Start();

            score.Start();

            background.init();

            Connection.Instance.LobbySpace.Put("ready", Connection.Instance.User_id);
            Console.WriteLine("Local client is ready");
            
            // Get ready signal from other clients
            foreach(var client in GameInfo.Current.Clients) {
                if (!client.IsLocal) {
                    Connection.Instance.LobbySpace.Query("ready", (int)client.Id);
                    Console.WriteLine(client.Name + " is ready");
                }
            }

            gameReady = true;
            Console.WriteLine("Game started!");

        }


        public override void onEnter()
        {
            
        }


        public override void onExit()
        {
        }

        public override void update(GameTime gameTime)
        {

            Console.WriteLine("Update!");


            // Check if game is ready to start
            if ( gameReady) {
                if( !gameStarted ) {
                    background.Start();
                    gameStarted = true;
                    Console.WriteLine("Started game state!");
                }
            }

            // Run game logic
            if( gameStarted ) {
                if (lifeController.WinnerFound)
                    GameFinished(lifeController.Winner);
            }


            foreach (Layer layer in Layers)
            {
                layer.update(gameTime);
            }
        }

        public override void draw(GameTime gameTime)
        {
            game.GraphicsDevice.Clear(Color.CornflowerBlue);
            foreach (Layer layer in Layers)
            {
                layer.draw(gameTime);
            }
        }


        // Display the win screen
        private void GameFinished(Player winner) {
            if (winScreenDisplayed) return;
            winScreenDisplayed = true;
            background.UpdateEnabled = false;
            Layers.Add(new Finish(winner, ExitGame));
        }


        private void ExitGame()
        {
            // TODO: Clean up game
            Console.WriteLine("Exitting game");
            eventController.Stop();
            lifeController.Stop();
            background.Terminate();
            _context.TransitionTo(new SelectLobbyState(game));
            
        }
    }
}
