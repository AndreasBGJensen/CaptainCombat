using CaptainCombat.Client.Scenes;
using CaptainCombat.Client.Source.Layers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;


namespace CaptainCombat.Client.Source.Scenes
{
    class CreateJoinLobbyState : State
    {
        List<Layer> layers = new List<Layer>();

        private Game Game;
        
        public CreateJoinLobbyState(Game game)
        {
            Game = game;
            layers.Add(new Select(game, this));
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
