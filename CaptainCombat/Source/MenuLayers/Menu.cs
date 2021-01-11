using ECS;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaptainCombat.Source.MenuLayers
{
    class Menu : Layer
    {
        private Domain domain = new Domain();
        private Camera camera;

        public Menu()
        {
            camera = new Camera(domain);
            init();
        }

        public override void init()
        {
            // Game score static 
            EntityUtility.CreateMessage(domain, "Menu", 50, 50);
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
