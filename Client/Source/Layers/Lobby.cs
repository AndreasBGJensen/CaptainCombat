﻿using CaptainCombat.Client.protocols;
using CaptainCombat.Client.Scenes;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CaptainCombat.Common.Components;
using CaptainCombat.Common;
using static CaptainCombat.Common.Domain;
using CaptainCombat.Client.Source.Scenes;

namespace CaptainCombat.Client.Source.Layers
{
    class Lobby : Layer
    {

        private Camera Camera;
        private Domain Domain = new Domain();

        private bool DisableKeyboard = false;
        private bool ChangeState = false;

        private Keys[] LastPressedKeys = new Keys[5];

        private State ParentState;
        private Game Game;
        private int currentIndex = 0;
        private List<Entity> menuItems = new List<Entity>();
        private Entity left_pointer;
        private Entity right_pointer;
        private Entity clientInformation;


        public Lobby(Game game, State state)
        {
            ParentState = state;
            Game = game;
            Camera = new Camera(Domain);
            init();
        }

        public override void init()
        {

            // Static message to client 
            menuItems.Add(EntityUtility.CreateMessage(Domain, "Lobby 1: 1-4", 0, 0, 16));
            menuItems.Add(EntityUtility.CreateMessage(Domain, "Lobby 2: 1-4", 0, 0, 16));
            menuItems.Add(EntityUtility.CreateMessage(Domain, "Lobby 3: 1-4", 0, 0, 16));
            clientInformation = EntityUtility.CreateMessage(Domain, "", -70, 150, 16);
            EntityUtility.CreateMessage(Domain, "Select lobby", -70, -180, 16);

            // Background
            Entity backGround = new Entity(Domain);
            backGround.AddComponent(new Transform());
            backGround.AddComponent(new Sprite(Assets.Textures.Background, 1280, 720));

            // Menu
            Entity Menu = new Entity(Domain);
            Menu.AddComponent(new Transform());
            Menu.AddComponent(new Sprite(Assets.Textures.Menu, 600, 600));

            // pointer
            left_pointer = EntityUtility.MenuArrow(Domain, false);
            right_pointer = EntityUtility.MenuArrow(Domain, true);
        }

        public override void update(GameTime gameTime)
        {
            // Clear domain 
            Domain.Clean();

            // Handles keyboard input
            GetKeys();

            // Displays list of all clients in server
            displayCurrentIndex();


            // Changes state when condition is true 
            if (ChangeState)
            {
                ParentState._context.TransitionTo(new LobbyState(Game));
            }
        }

        public override void draw(GameTime gameTime)
        {
            Renderer.RenderSprites(Domain, Camera);
            Renderer.RenderText(Domain, Camera);
            Renderer.RenderInput(Domain, Camera);
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
            if (DisableKeyboard)
            {
                return;
            }
            else if (key == Keys.Enter)
            {
                //DisableKeyboard = !DisableKeyboard;
                RunCurrentselected();
                /*
                Task.Factory.StartNew(async () =>
                {
                    await Task.Delay(2000);
                    ChangeState = true;
                });
                */
            }
            else if (key == Keys.Up)
            {
                if (!(currentIndex == 0))
                {
                    currentIndex--;
                }
            }
            else if (key == Keys.Down)
            {
                if (!(currentIndex == menuItems.Count - 1))
                {
                    currentIndex++;
                }
            }
        }

        public void RunCurrentselected()
        {
            var info = clientInformation.GetComponent<Text>();
            info.Message = "Current lobby is full";
        }

        public void displayCurrentIndex()
        {

            int placement_Y = -140;
            for (int i = 0; i < menuItems.Count(); i++)
            {
                {
                    var transform = menuItems[i].GetComponent<Transform>();
                    transform.Position.X = -70;
                    transform.Position.Y = placement_Y;

                }
                if (i == currentIndex)
                {
                    {
                        var transform = left_pointer.GetComponent<Transform>();
                        transform.Position.X = -120;
                        transform.Position.Y = placement_Y + 10;
                    }
                    {
                        var transform = right_pointer.GetComponent<Transform>();
                        transform.Position.X = 115;
                        transform.Position.Y = placement_Y + 10;
                    }
                }
                placement_Y += 50;
            }
        }
    }
}

