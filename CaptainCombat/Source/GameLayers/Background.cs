using CaptainCombat.Source.Components;
using CaptainCombat.Source.Utility;
using ECS;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static ECS.Domain;

namespace CaptainCombat.Source.GameLayers
{
    class Background : Layer
    {
        private Domain domain = new Domain();
        private  Camera camera;

        public static Entity ship;

        public Background()
        {
            camera = new Camera(domain);
            init(); 
        }

        public override void init()
        {
            int xOffset = -40;
            int yOffset = -70;

            // Create some test rocks
            EntityUtility.CreateMessage(domain, "Tortuga", 150 + xOffset, 100 + yOffset);
            EntityUtility.CreateRock(domain, 150, 100, 0.7, 120);
            EntityUtility.CreateMessage(domain, "Port Royal", 400 + xOffset, -200 + yOffset);
            EntityUtility.CreateRock(domain, 400, -200, 1.0, 40);
            EntityUtility.CreateMessage(domain, "Barataria Bay", 0 + xOffset, 50 + yOffset);
            EntityUtility.CreateRock(domain, 0, 50, 1.2, 300);
            EntityUtility.CreateMessage(domain, "Clew Bay", -300 + xOffset, 75 + yOffset);
            EntityUtility.CreateRock(domain, -300, 75, 1.4, 170);
            EntityUtility.CreateMessage(domain, "New Providence", -100 + xOffset, -200 + yOffset);
            EntityUtility.CreateRock(domain, -100, -200, 1.2, 30);

            // Create ship
            {
                ship = new Entity(domain);
                ship.AddComponent(new Transform());
                ship.AddComponent(new Sprite(Assets.Textures.SHIP, 66, 113));

                var move = ship.AddComponent(new Move());
                move.Resistance = 0.25;
                move.RotationResistance = 0.75;
                move.ForwardVelocity = true;
            }
        }

        public override void update(GameTime gameTime)
        {
            domain.Clean();

            //if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
            //Exit();

            double seconds = gameTime.ElapsedGameTime.TotalSeconds;
            bool shiftPressed = Keyboard.GetState().IsKeyDown(Keys.LeftShift) | Keyboard.GetState().IsKeyDown(Keys.RightShift);

            if (Keyboard.GetState().IsKeyDown(Keys.C))
                domain.Clean();

            if (Keyboard.GetState().IsKeyDown(Keys.R))
                camera.Rotation += 100 * seconds * (shiftPressed ? -1 : 1);

            if (Keyboard.GetState().IsKeyDown(Keys.Z))
                camera.Zoom += 1.0 * seconds * (shiftPressed ? -1 : 1);

            // Update ship movement
            {
                var move = ship.GetComponent<Move>();

                if (Keyboard.GetState().IsKeyDown(Keys.Space))
                {
                    move.Acceleration = move.Acceleration.WithMagnitude(30);
                }
                else
                {
                    move.Acceleration = new Vector2(0, 0);
                }
                
                if (Keyboard.GetState().IsKeyDown(Keys.Right))
                {
                    move.RotationAcceleration = 270;
                }
                else if (Keyboard.GetState().IsKeyDown(Keys.Left))
                {
                    move.RotationAcceleration = -270;
                }
                else
                {
                    move.RotationAcceleration = 0;
                }
            }

            // Update camera to ship
            {
                var transform = ship.GetComponent<Transform>();
                camera.X = transform.X;
                camera.Y = transform.Y;
            }

            Movement.Update(domain, seconds);
        }

        public override void draw(GameTime gameTime)
        {
            Renderer.RenderSprites(domain, camera);
            Renderer.RenderText(domain, camera);
        }

        
    }
}
