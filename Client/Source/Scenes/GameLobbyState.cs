using CaptainCombat.network;
using CaptainCombat.Client.GameLayers;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.Threading;
using CaptainCombat.Client.Scenes;
using CaptainCombat.Common;
using CaptainCombat.Client.Source.Layers;
using Microsoft.Xna.Framework.Input;

namespace CaptainCombat.Client.Source.Scenes
{
    class GameLobbyState : State
    {
        List<Layer> Layers = new List<Layer>();
        Game Game;
        
        public GameLobbyState(Game game)
        {
            Game = game;
            Layers.Add(new GameLobby(game, this));
        }


        public override void OnKeyDown(Keys key)
        {
            foreach (Layer layer in Layers)
                if (layer.OnKeyDown(key)) break;
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
