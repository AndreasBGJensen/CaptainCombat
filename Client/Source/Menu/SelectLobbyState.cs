using CaptainCombat.Client.MenuLayers;
using CaptainCombat.Client.Scenes;
using CaptainCombat.Client.Source.Layers;
using CaptainCombat.Common;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;


namespace CaptainCombat.Client.Source.Scenes
{
    class SelectLobbyState : State
    {
        private List<Layer> layers = new List<Layer>();
        private Game game;
        
        public SelectLobbyState(Game game)
        {
            this.game = game;
            layers.Add(new LobbyList(game, this));
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
            game.GraphicsDevice.Clear(Microsoft.Xna.Framework.Color.Black);
            foreach (Layer layer in layers)
            {
                layer.draw(gameTime);
            }
        }
    }
}
