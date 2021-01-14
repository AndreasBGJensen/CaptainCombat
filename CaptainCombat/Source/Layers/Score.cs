using CaptainCombat.Source.Scenes;
using ECS;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaptainCombat.Source.GameLayers
{
    class Score : Layer
    {

        private Domain Domain = new Domain(); 
        private Camera Camera;
        private Game Game;
        private State ParentState; 
        public Score(Game game, State state)
        {
            ParentState = state; 
            Game = game; 
            Camera = new Camera(Domain);
            init();
        }

        public override void init()
        {
           
        }

        public override void update(GameTime gameTime)
        {
            Domain.Clean();
        }

        public override void draw(GameTime gameTime)
        {
            Renderer.RenderSprites(Domain, Camera);
            Renderer.RenderText(Domain, Camera);
        }
    }
}
