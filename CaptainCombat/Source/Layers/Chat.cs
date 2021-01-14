﻿using CaptainCombat.Source.Components;
using CaptainCombat.Source.protocols;
using CaptainCombat.Source.Scenes;
using ECS;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using static ECS.Domain;

namespace CaptainCombat.Source.GameLayers
{
    class Chat : Layer
    {
        private Domain Domain = new Domain();  
        private Camera Camera;

        private Entity ChatBox;
        private Entity InputBox;

        private bool DisplayChat = false;
        private Keys[] LastPressedKeys = new Keys[5];
        private string ChatMessage = string.Empty;

        private Game Game;
        private State ParentState;

        public Chat(Game game, State state)
        {
            ParentState = state; 
            Game = game; 
            Camera = new Camera(Domain);
            init();
        }

        public override void init()
        {
            // Input box 
            InputBox = EntityUtility.CreateInput(Domain, "", 360, 200, 14);

            // Chat layout
            ChatBox = new Entity(Domain);
            ChatBox.AddComponent(new Transform());
            ChatBox.AddComponent(new Sprite(Assets.Textures.Chat, 200, 200));
            var transform = ChatBox.GetComponent<Transform>();
            transform.X = 200;
            transform.Y = 200;
        }

        

        public override void update(GameTime gameTime)
        {
            // Clearing domain 
            Domain.Clean();

            // Updates chat if new message is added to remote space on server 
            int messageInDomain = 0; 
            Domain.ForMatchingEntities<Text, Transform>((entity) => {
                messageInDomain++; 
            });

            List<string> AllUsersMessages = ClientProtocol.GetAllUsersMessages();
            if (messageInDomain < AllUsersMessages.Count)
            {
                Domain.ForMatchingEntities<Text, Transform>((entity) => {
                    entity.Delete(); 
                });
                int maxDisplayedMesseges = 15;
                var LastMessages = AllUsersMessages.Skip(Math.Max(0, AllUsersMessages.Count() - maxDisplayedMesseges)).Take(maxDisplayedMesseges); 
                foreach (string chatMessages in LastMessages)
                {
                    EntityUtility.CreateMessage(Domain, chatMessages, 0, 0, 14);
                }
            }
            
            // Handles keyboard inputs 
            GetKeys();

            // Display chat 
            (DisplayChat ? new Action(Display) : Hide)();
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
                DisplayChat = !DisplayChat;
            }
            else if (!DisplayChat)
            {
                return; 
            }
            else if (key == Keys.Enter)
            {
                ClientProtocol.AddMessageToServer(ChatMessage); 
                ChatMessage = string.Empty; 
            }
            else if (key == Keys.Back)
            {
                if (ChatMessage.Length > 0) {
                    ChatMessage = ChatMessage.Remove(ChatMessage.Length - 1);
                } 
            }
            else if (key == Keys.Space)
            {
                ChatMessage += " "; 
            }
            else
            {
                string keyData = key.ToString();
                if (KeyboardInputValidator.isValid(keyData) && ChatMessage.Length < 20)
                {
                    ChatMessage = (KeyboardInputValidator.dict.ContainsKey(keyData) ? ChatMessage += KeyboardInputValidator.dict[keyData] : ChatMessage += keyData); 
                }

            }
        }

        public void Display()
        {
            // Display input box
            var input = InputBox.GetComponent<Input>();
            input.Message = ChatMessage;

            // Display chat box
            { 
            var transform = ChatBox.GetComponent<Transform>();
            var sprite = ChatBox.GetComponent<Sprite>(); 
            transform.X = 450;
            transform.Y = 0;
            sprite.Height = 700;
            sprite.Width = 300;
            }

            // Display messages in chat box 
            int placement_Y = -230; 
            Domain.ForMatchingEntities<Text, Transform>((entity) => {
                var transform = entity.GetComponent<Transform>();
                transform.X = 360;
                transform.Y = placement_Y;
                placement_Y += 25; 
            });
        }

        public void Hide()
        {
            // Hide input box
            var input = InputBox.GetComponent<Input>();
            input.Message = string.Empty;

            // Shrink chat box
            {
                var transform = ChatBox.GetComponent<Transform>();
            var sprite = ChatBox.GetComponent<Sprite>();
            transform.X = 580;
            transform.Y = 300;
            sprite.Height = 40;
            sprite.Width = 40;
            }

            // Hide messages from view 
            Domain.ForMatchingEntities<Text, Transform>((entity) => {
                var transform = entity.GetComponent<Transform>();
                transform.Y = -1000; 
            });

        }


        public override void draw(GameTime gameTime)
        {
            Renderer.RenderSprites(Domain, Camera);
            Renderer.RenderText(Domain, Camera);
            Renderer.RenderInput(Domain, Camera);
        }
    }
}
