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
        
        private Camera Camera;
        private Domain Domain = new Domain();  
        public Menu(Domain domain)
        {
            Camera = new Camera(Domain);
            init();
        }

        public override void init()
        {
            // Game score static 
            EntityUtility.CreateMessage(Domain, "Menu", 50, 50);
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
