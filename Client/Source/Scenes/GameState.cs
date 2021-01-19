using CaptainCombat.network;
using CaptainCombat.Client.GameLayers;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.Threading;
using System;
using CaptainCombat.Client.Layers;
using CaptainCombat.Client.protocols;
using CaptainCombat.Client.Source.Scenes;

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

            // TODO: Make a proper init section
            GameInfo.Current = new GameInfo();
            foreach (var tuple in ClientProtocol.GetClientsInLobby()) {
                var clientId = (uint)(int)tuple[1];
                var clientName = (string)tuple[2];
                if( clientId != 0 )
                    GameInfo.Current.AddClient(new Player(clientId, clientName));
            }

            // TODO: Remove this
            Console.WriteLine("Clients in game:");
            foreach( var client in GameInfo.Current.Clients)
                Console.WriteLine($"  {client.Id}:{client.Name}");

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
            if ( lifeController.WinnerFound )
                GameFinished(lifeController.Winner);

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

        private void GameFinished(Player winner) {
            // TODO: Toggle off winner
            Console.WriteLine("Winner is " + winner.Name);
            background.UpdateEnabled = false;
            Layers.Add(new Finish(winner));
        }
    }
}
