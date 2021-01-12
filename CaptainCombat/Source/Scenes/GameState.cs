using CaptainCombat.json;
using CaptainCombat.network;
using CaptainCombat.singletons;
using CaptainCombat.Source.GameLayers;
using ECS;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CaptainCombat.Source.Scenes
{
    class GameState : State
    {
        private List<Layer> layers = new List<Layer>();
        
        public GameState(Game game)
        {
            layers.Add(new Background());
            layers.Add(new Score());
            layers.Add(new Chat(game));
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
                this._context.TransitionTo(new MenuState()); 
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
