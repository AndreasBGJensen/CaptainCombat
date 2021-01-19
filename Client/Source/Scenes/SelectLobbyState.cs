using CaptainCombat.Client.MenuLayers;
using CaptainCombat.Client.Scenes;
using CaptainCombat.Client.Source.Layers;
using CaptainCombat.Common;
using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace CaptainCombat.Client.Source.Scenes
{
    class SelectLobbyState : State
    {
        List<Layer> layers = new List<Layer>();

        private Domain Domain = new Domain();
        Game Game;
        public SelectLobbyState(Game game)
        {
            Game = game;
            layers.Add(new Select(game, this));
        }

        public override void onEnter()
        {

        }

        public override void onExit()
        {

        }

        public override void update(GameTime gameTime)
        {
            foreach (Layer layer in layers)
            {
                layer.update(gameTime);
            }
        }

        public override void draw(GameTime gameTime)
        {
            Game.GraphicsDevice.Clear(Color.Black);
            foreach (Layer layer in layers)
            {
                layer.draw(gameTime);
            }
        }
    }
}
