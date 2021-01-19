﻿using CaptainCombat.Client.MenuLayers;
using CaptainCombat.Common;
using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace CaptainCombat.Client.Scenes
{
    class MenuState : State
    {
        List<Layer> layers = new List<Layer>();

        private Domain Domain = new Domain();
        Game Game; 
        public MenuState(Game game)
        {
            Game = game; 
            layers.Add(new Menu(game, this)); 
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