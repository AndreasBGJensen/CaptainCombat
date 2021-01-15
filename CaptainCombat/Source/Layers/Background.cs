﻿using CaptainCombat.Source.protocols;
using CaptainCombat.Source.Scenes;
using StaticGameLogic_Library.Source.ECS;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System.Linq;
using static StaticGameLogic_Library.Source.ECS.Domain;
using StaticGameLogic_Library.Singletons;
using Source.EntityUtility;
using StaticGameLogic_Library.Source.Components;
using StaticGameLogic_Library.Source;
using StaticGameLogic_Library.JsonBuilder;

namespace CaptainCombat.Source.GameLayers
{

    class Background : Layer
    {
        private Domain Domain = new Domain();
        private Camera Camera;

        private bool DisableKeyboard = false;
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
            Ship = EntityUtility.CreateShip(Domain, Connection.Instance.User_id, 1); 
        }

        public override void update(GameTime gameTime)
        {

            if (DomainState.Instance.Download != null) {
                Domain.update(DomainState.Instance.Download);
                DomainState.Instance.Download = null;
            }

            Domain.Clean();

            // TODO: Fix this
            //EventController.HandleEvents();

            // Clear domain

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
                        move.Acceleration = new Vector(30, 0);
                    }
                    else
                    {
                        move.Acceleration = new Vector(0, 0);
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
                Camera.Position = transform.Position;
            }


            // Update movement in domain 
            Movement.Update(Domain, seconds);

            Domain.Clean();

            DomainState.Instance.Upload = JsonBuilder.createJsonString();
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
            }
        }
    }
}
