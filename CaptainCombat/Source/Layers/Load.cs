using CaptainCombat.Source.Components;
using CaptainCombat.Source.protocols;
using CaptainCombat.Source.Scenes;
using ECS;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static ECS.Domain;

namespace CaptainCombat.Source.Layers
{
    class Load : Layer
    {

        private Camera Camera;
        private Domain Domain = new Domain();

        private Entity ship;
        private Game Game;
        private State ParentState; 
        public Load(Game game, State state)
        {
            ParentState = state; 
            Game = game; 
            Camera = new Camera(Domain);
            init();
        }

        public override void init()
        {
            // Background
            Entity backGround = new Entity(Domain);
            backGround.AddComponent(new Transform());
            backGround.AddComponent(new Sprite(Assets.Textures.Background, 1280, 720));

            {
                ship = new Entity(Domain);
                ship.AddComponent(new Transform());
                ship.AddComponent(new Sprite(Assets.Textures.Loader, 256, 256));

                var move = ship.AddComponent(new Move());
                move.Resistance = 0.25;
                move.RotationResistance = 3;
                move.RotationAcceleration = 270;
            }

            // Loading text 
            EntityUtility.CreateMessage(Domain, "Connecting to server", -70, 0, 14);

            ClientProtocol.Connect(); 

            Task.Factory.StartNew(async () =>
            {
                await Task.Delay(3000);
                changeState(); 
            });
        }

        public void changeState()
        {
            ParentState._context.TransitionTo(new MenuState(Game));
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
