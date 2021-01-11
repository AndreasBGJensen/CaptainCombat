using CaptainCombat.Source.GameLayers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaptainCombat.Source.Scenes
{
    class GameState : State
    {
        List<Layer> layers = new List<Layer>(); 

        public GameState()
        {
            layers.Add(new Background());
            layers.Add(new Score());
            layers.Add(new Chat());
        }

        

        public override void onEnter()
        {
            
        }

        public override void onExit()
        {
            
        }

        public override void update(GameTime gameTime)
        {
            if (Keyboard.GetState().IsKeyDown(Keys.Escape))
            {
                this._context.TransitionTo(_context.menuState); 
            }
            foreach (Layer layer in layers)
            {
                layer.update(gameTime);
            }
        }

        public override void draw(GameTime gameTime)
        {
            foreach (Layer layer in layers)
            {
                layer.draw(gameTime);
            }
        }
    }
}
