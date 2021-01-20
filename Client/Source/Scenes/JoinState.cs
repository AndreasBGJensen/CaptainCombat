using CaptainCombat.Client.MenuLayers;
using CaptainCombat.Common;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;

namespace CaptainCombat.Client.Scenes
{
    class JoinState : State
    {
        List<Layer> layers = new List<Layer>();

        private Domain Domain = new Domain();
        Game Game; 
        public JoinState(Game game)
        {
            Game = game; 
            layers.Add(new Menu(game, this));
        }

        public override void OnKeyDown(Keys key)
        {
            foreach (Layer layer in layers)
                if (layer.OnKeyDown(key)) break;
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
            Game.GraphicsDevice.Clear(Microsoft.Xna.Framework.Color.Black);
            foreach (Layer layer in layers)
            {
                layer.draw(gameTime);
            }
        }
    }
}
