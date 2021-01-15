using CaptainCombat.network;
using CaptainCombat.singletons;
using CaptainCombat.Source.Components;
using CaptainCombat.Source.protocols;
using CaptainCombat.Source.Scenes;
using CaptainCombat.Source.Utility;
using ECS;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static ECS.Domain;

namespace CaptainCombat.Source.GameLayers
{
    class Background : Layer
    {
        private Domain Domain = new Domain();
        private Camera Camera;

        private bool DisableKeyboard = false;
        private bool playMusic = true;
        private Keys[] LastPressedKeys = new Keys[5];

        private Entity Ship;
        private int currentScore = 0;

        private State ParentState;
        Game Game;

        

        public Background(Game game, State state)
        {
            ParentState = state; 
            Game = game; 
            Camera = new Camera(Domain);
            DomainState.Instance.Domain = Domain; 
            init(); 
        }

        public override void init()
        {
       

            // Create some test rocks
            int xOffset = -40;
            int yOffset = -70;
            EntityUtility.CreateMessage(Domain, "Tortuga", 150 + xOffset, 100 + yOffset, 12);
            EntityUtility.CreateRock(Domain, 150, 100, 0.7, 120);
            EntityUtility.CreateMessage(Domain, "Port Royal", 400 + xOffset, -200 + yOffset, 12);
            EntityUtility.CreateRock(Domain, 400, -200, 1.0, 40);
            EntityUtility.CreateMessage(Domain, "Barataria Bay", 0 + xOffset, 50 + yOffset, 12);
            EntityUtility.CreateRock(Domain, 0, 50, 1.2, 300);
            EntityUtility.CreateMessage(Domain, "Clew Bay", -300 + xOffset, 75 + yOffset, 12);
            EntityUtility.CreateRock(Domain, -300, 75, 1.4, 170);
            EntityUtility.CreateMessage(Domain, "New Providence", -100 + xOffset, -200 + yOffset, 12);
            EntityUtility.CreateRock(Domain, -100, -200, 1.2, 30);

            // Create ship
            /*
            {
                Ship = new Entity(Domain);
                Ship.AddComponent(new Transform());
                Ship.AddComponent(new Sprite(Assets.Textures.SHIP, 66, 113));

                var move = Ship.AddComponent(new Move());
                move.Resistance = 0.25;
                move.RotationResistance = 0.75;
                move.ForwardVelocity = true;
            }
            */
            Ship = EntityUtility.CreateShip(Domain, Connection.Instance.User_id, 1); 
        }

        public override void update(GameTime gameTime)
        {
            // Clear domain 
            Domain.Clean();

            // Handles keyboard input 
            GetKeys();

            // Update ship movement
            double seconds = gameTime.ElapsedGameTime.TotalSeconds;
           
            if (!DisableKeyboard)
            {
                {
                    var move = Ship.GetComponent<Move>();

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
                var transform = Ship.GetComponent<Transform>();
                Camera.X = transform.X;
                Camera.Y = transform.Y;
            }

            // Update movement in domain 
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

            foreach (Keys key in pressedKeys)
            {
                if (!LastPressedKeys.Contains(key))
                {
                    OnKeyDown(key);
                }
            }
            LastPressedKeys = pressedKeys;

        }

        public void OnKeyDown(Keys key)
        {
            if (key == Keys.Tab)
            {
                DisableKeyboard = !DisableKeyboard;
            }else if(key == Keys.Add)
            {
                currentScore++;
                ClientProtocol.AddClientScoreToServer(currentScore);
            }else if (key == Keys.M)
            {
                if (playMusic)
                {
                    playMusic = !playMusic; 
                    // Start song 
                    Track track = Assets.Music.PirateSong;
                    var song = track.GetNative<Song>();
                    MediaPlayer.IsRepeating = true;
                    MediaPlayer.Play(song);
                }
                else
                {
                    playMusic = !playMusic;
                    MediaPlayer.Stop();
                }
                
            }
        }
    }
}
