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
        private Domain Domain = new Domain();
        private Camera Camera;

        private Entity ship;

        private bool disableKeyboard = false;

        private Keys[] lastPressedKeys = new Keys[5];

        public Background()
        {
            
            Camera = new Camera(Domain);
            init(); 
        }

        public override void init()
        {
            int xOffset = -40;
            int yOffset = -70;

            // Create some test rocks
            EntityUtility.CreateMessage(Domain, "Tortuga", 150 + xOffset, 100 + yOffset);
            EntityUtility.CreateRock(Domain, 150, 100, 0.7, 120);
            EntityUtility.CreateMessage(Domain, "Port Royal", 400 + xOffset, -200 + yOffset);
            EntityUtility.CreateRock(Domain, 400, -200, 1.0, 40);
            EntityUtility.CreateMessage(Domain, "Barataria Bay", 0 + xOffset, 50 + yOffset);
            EntityUtility.CreateRock(Domain, 0, 50, 1.2, 300);
            EntityUtility.CreateMessage(Domain, "Clew Bay", -300 + xOffset, 75 + yOffset);
            EntityUtility.CreateRock(Domain, -300, 75, 1.4, 170);
            EntityUtility.CreateMessage(Domain, "New Providence", -100 + xOffset, -200 + yOffset);
            EntityUtility.CreateRock(Domain, -100, -200, 1.2, 30);

            // Create ship
            {
                ship = new Entity(Domain);
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
            Domain.Clean();
            GetKeys(); 
            

            double seconds = gameTime.ElapsedGameTime.TotalSeconds;
            bool shiftPressed = Keyboard.GetState().IsKeyDown(Keys.LeftShift) | Keyboard.GetState().IsKeyDown(Keys.RightShift);


            if (!disableKeyboard)
            {
                if (Keyboard.GetState().IsKeyDown(Keys.C))
                    Domain.Clean();

                if (Keyboard.GetState().IsKeyDown(Keys.R))
                    Camera.Rotation += 100 * seconds * (shiftPressed ? -1 : 1);

                if (Keyboard.GetState().IsKeyDown(Keys.Z))
                    Camera.Zoom += 1.0 * seconds * (shiftPressed ? -1 : 1);

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
            }

            // Update camera to ship
            {
                var transform = ship.GetComponent<Transform>();
                Camera.X = transform.X;
                Camera.Y = transform.Y;
            }

            Movement.Update(Domain, seconds);
        }

        

        public override void draw(GameTime gameTime)
        {
            Renderer.RenderSprites(Domain, Camera);
            Renderer.RenderText(Domain, Camera);
        }

        public void GetKeys()
        {
            KeyboardState kbState = Keyboard.GetState();
            Keys[] pressedKeys = kbState.GetPressedKeys();

            foreach (Keys key in lastPressedKeys)
            {
                if (!pressedKeys.Contains(key))
                {
                    OnKeyUp(key);
                }
            }
            foreach (Keys key in pressedKeys)
            {
                if (!lastPressedKeys.Contains(key))
                {
                    OnKeyDown(key);
                }
            }
            lastPressedKeys = pressedKeys;

        }

        public void OnKeyUp(Keys key)
        {

        }

        public void OnKeyDown(Keys key)
        {
            if (key == Keys.Tab)
            {
                disableKeyboard = !disableKeyboard;
            }
        }


    }
}
