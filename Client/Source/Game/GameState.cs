using CaptainCombat.Client.GameLayers;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System;
using CaptainCombat.Client.Layers;
using CaptainCombat.Client.protocols;
using CaptainCombat.Client.NetworkEvent;
using CaptainCombat.Client.Source.Scenes;
using Microsoft.Xna.Framework.Input;
using CaptainCombat.Common;

using MGColor = Microsoft.Xna.Framework.Color;

namespace CaptainCombat.Client.Scenes
{

    class GameState : State
    {
        private List<Layer> layers = new List<Layer>();
        private Game game;
        private LogicLayer background;
        private ScoreLayer score;
        private EventController eventController;
        private LifeController lifeController;

        private bool gameReady = false;
        private bool gameStarted = false;

        private bool winScreenDisplayed = false;

        private Loader<bool> loader;

        public GameState(Game game)
        {
            this.game = game;

            eventController = new EventController();
            lifeController = new LifeController(eventController);

            background = new LogicLayer(lifeController, eventController);
            layers.Add(background);

            score = new ScoreLayer(lifeController);
            layers.Add(score);
            layers.Add(new ChatLayer(eventController));

            Console.WriteLine("Initialized game state!");

            // Start loader
            loader = new Loader<bool>("Starting game...",
                () => {
                    InitializeGame();
                    return true;
                },
                (_) => {
                    loader = null;
                    gameReady = true;
                }
            );
        }


        private void InitializeGame() {



            lifeController.Start();
            eventController.Start();
            score.Start();
            background.init();

            Connection.Lobby.Space.Put("ready", (int)Connection.LocalPlayer.Id);
            Console.WriteLine("Local client is ready");
            
            // Get ready signal from other clients
            foreach(var player in Connection.Lobby.Players) {
                if (player != Connection.LocalPlayer) {
                    Connection.Lobby.Space.Query("ready", (int)player.Id);
                    Console.WriteLine(player.Name + " is ready");
                }
            }
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


            foreach (Layer layer in layers)
            {
                layer.update(gameTime);
            }

            loader?.update(gameTime);
        }


        public override void OnKeyDown(Keys key)
        {
            for( int i=layers.Count-1; i>=0; i-- )
                if (layers[i].OnKeyDown(key)) break;
        }


        public override void draw(GameTime gameTime)
        {
            game.GraphicsDevice.Clear(MGColor.CornflowerBlue);
            foreach (Layer layer in layers)
            {
                layer.draw(gameTime);
            }
            loader?.draw(gameTime);
        }


        // Display the win screen
        private void GameFinished(Player winner) {
            if (winScreenDisplayed) return;
            winScreenDisplayed = true;
            background.UpdateEnabled = false;
            layers.Add(new FinishLayer(winner, ExitGame));
        }


        private void ExitGame()
        {
            Console.WriteLine("Exitting game");
            eventController.Stop();
            lifeController.Stop();
            background.Terminate();
            _context.TransitionTo(new CreateJoinLobbyState(game));
            
        }
    }
}
