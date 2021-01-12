using CaptainCombat.Source.Components;
using ECS;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static ECS.Domain;

namespace CaptainCombat.Source.Layers
{
    class Load : Layer
    {

        private Camera Camera;
        private Domain Domain = new Domain();

        private Entity ship;
        public Load()
        {
            Camera = new Camera(Domain);
            init();
        }

        public override void init()
        {
            // Game score static 
            {
                ship = new Entity(Domain);
                ship.AddComponent(new Transform());
                ship.AddComponent(new Sprite(Assets.Textures.Loader, 880, 464));

                var move = ship.AddComponent(new Move());
                move.Resistance = 0.25;
                move.RotationResistance = 3;
                move.RotationAcceleration = 270;
            }
        }

        public override void update(GameTime gameTime)
        {
            Domain.Clean();
            double seconds = gameTime.ElapsedGameTime.TotalSeconds;
            Movement.Update(Domain, seconds);
        }

        public override void draw(GameTime gameTime)
        {
            Renderer.RenderSprites(Domain, Camera);
            Renderer.RenderText(Domain, Camera);
        }
    }
}
