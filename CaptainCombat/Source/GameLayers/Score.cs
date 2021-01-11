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

        private Domain domain = new Domain();
        private Camera camera;

        public Score()
        {
            camera = new Camera(domain);
            init();
        }

        
        public override void init()
        {
            EntityUtility.CreateMessage(domain, "Score: 1", 50, 50);
        }

        public override void update(GameTime gameTime)
        {
            // Clean the Domain before each frame
            domain.Clean();
        }

        public override void draw(GameTime gameTime)
        {
            Renderer.RenderSprites(domain, camera);
            Renderer.RenderText(domain, camera);
        }
    }
}
