using CaptainCombat.Client.Layers;
using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace CaptainCombat.Client.Scenes
{
    class Loading : State
    {
        List<Layer> layers = new List<Layer>();
        Game Game; 
        public Loading(Game game)
        {
            Game = game;
            layers.Add(new Load(game, this));
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
            Game.GraphicsDevice.Clear(Color.White);
            foreach (Layer layer in layers)
            {
                layer.draw(gameTime);
            }
        }
    }
}
