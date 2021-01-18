using CaptainCombat.network;
using CaptainCombat.Client.GameLayers;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.Threading;
using CaptainCombat.Client.Source.Layers;

namespace CaptainCombat.Client.Scenes
{

    class GameState : State
    {
        private List<Layer> Layers = new List<Layer>();
        private Game Game;

        Upload Upload;
        Thread UploadThread;
        DownLoad Download;
        Thread DownloadThread; 

        public GameState(Game game)
        {
            Game = game;
            LifeController lifeController = new LifeController();
            Layers.Add(new Background(game, this, lifeController));
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
    }
}
