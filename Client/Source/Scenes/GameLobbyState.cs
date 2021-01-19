using CaptainCombat.network;
using CaptainCombat.Client.GameLayers;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.Threading;
using CaptainCombat.Client.Scenes;
using CaptainCombat.Common;
using CaptainCombat.Client.Source.Layers;

namespace CaptainCombat.Client.Source.Scenes
{
    class GameLobbyState : State
    {
        List<Layer> Layers = new List<Layer>();

        private Domain Domain = new Domain();
        Game Game;
        public GameLobbyState(Game game)
        {
            Game = game;
            Layers.Add(new GameLobby(game, this));
        }

        public override void onEnter()
        {

        }

        public override void onExit()
        {

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
            Game.GraphicsDevice.Clear(Microsoft.Xna.Framework.Color.Black);
            foreach (Layer layer in Layers)
            {
                layer.draw(gameTime);
            }
        }
    }
}
