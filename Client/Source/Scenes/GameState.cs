using CaptainCombat.network;
using CaptainCombat.Client.GameLayers;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.Threading;
using System;
using CaptainCombat.Client.Layers;

namespace CaptainCombat.Client.Scenes
{

    class GameState : State
    {
        private List<Layer> Layers = new List<Layer>();
        private Game Game;
        private Background background;
        private LifeController lifeController;

        Upload Upload;
        Thread UploadThread;
        DownLoad Download;
        Thread DownloadThread; 

        public GameState(Game game)
        {
            Game = game;
            
            lifeController = new LifeController();

            background = new Background(game, this, lifeController);
            Layers.Add(background);
            Layers.Add(new Score(game, this, lifeController));
            Layers.Add(new Chat(game, this));

            Upload = new Upload();
            UploadThread = new Thread(new ThreadStart(Upload.RunProtocol));

            Download = new DownLoad();
            DownloadThread = new Thread(new ThreadStart(Download.RunProtocol));
        }

        public override void onEnter()
        {
            UploadThread.Start();
            DownloadThread.Start();
        }

        public override void onExit()
        {
            UploadThread.Abort(); 
            DownloadThread.Abort();
        }

        public override void update(GameTime gameTime)
        {
            var winner = lifeController.GetWinner();
            if ( winner != 0 )
                GameFinished(winner);

            foreach (Layer layer in Layers)
            {
                layer.update(gameTime);
            }
        }

        public override void draw(GameTime gameTime)
        {
            Game.GraphicsDevice.Clear(Color.CornflowerBlue);
            foreach (Layer layer in Layers)
            {
                layer.draw(gameTime);
            }
        }

        private void GameFinished(uint winnerId) {
            Console.WriteLine("Winner is " + winnerId);
            background.UpdateEnabled = false;
            Layers.Add(new Finish(winnerId));
        }
    }
}
