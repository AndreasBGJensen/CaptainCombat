using CaptainCombat.Source.MenuLayers;
using StaticGameLogic_Library.Source.ECS;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaptainCombat.Source.Scenes
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
