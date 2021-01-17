using CaptainCombat.Client.protocols;
using CaptainCombat.Client.Scenes;
using dotSpace.Interfaces.Space;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using CaptainCombat.Common.Components;
using CaptainCombat.Common;
using static CaptainCombat.Common.Domain;

namespace CaptainCombat.Client.GameLayers
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

            IEnumerable<ITuple> AllUsersMessages = ClientProtocol.GetAllUsersMessages();
            if(AllUsersMessages != null)
            {
                if (messageInDomain < AllUsersMessages.Count())
                {
                    Domain.ForMatchingEntities<Text, Transform>((entity) => {
                        entity.Delete();
                    });

                    Domain.ForMatchingEntities<Sprite, Transform>((entity) =>
                    {
                        if (!(entity == ChatBox))
                        {
                            entity.Delete();
                        }
                    }); 

                    int maxDisplayedMesseges = 15;
                    var LastMessages = AllUsersMessages.Skip(Math.Max(0, AllUsersMessages.Count() - maxDisplayedMesseges)).Take(maxDisplayedMesseges);
                    foreach (ITuple chatMessages in LastMessages)
                    {
                        EntityUtility.CreateIcon(Domain, (int)chatMessages[1]); 
                        EntityUtility.CreateMessage(Domain, (string)chatMessages[2], 0, 0, 14);
                    }
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


            // Display messages in chat box 
            { 
            int placement_Y = -230; 
            Domain.ForMatchingEntities<Text, Transform>((entity) => {
                var transform = entity.GetComponent<Transform>();
                transform.Position.X = 385;
                transform.Position.Y = placement_Y;
                placement_Y += 30; 
            });
            }

            // Display icons in chat box 
            {
                int placement_Y = -245;
                Domain.ForMatchingEntities<Sprite, Transform>((entity) => {
                    var transform = entity.GetComponent<Transform>();
                    transform.Position.X = 365;
                    transform.Position.Y = placement_Y;
                    placement_Y += 30;
                });
            }
            

            // Display chat box
            {
                var transform = ChatBox.GetComponent<Transform>();
                var sprite = ChatBox.GetComponent<Sprite>();
                transform.Position.X = 450;
                transform.Position.Y = 0;
                sprite.Height = 700;
                sprite.Width = 300;
            }
        }

        public void Hide()
        {
            // Hide input box
            var input = InputBox.GetComponent<Input>();
            input.Message = string.Empty;

            // Display icons in chat box 
            {
                Domain.ForMatchingEntities<Sprite, Transform>((entity) => {
                    var transform = entity.GetComponent<Transform>();
                    transform.Position.Y = -1000;
                });
            }

            // Shrink chat box
            {
            var transform = ChatBox.GetComponent<Transform>();
            var sprite = ChatBox.GetComponent<Sprite>();
            transform.Position.X = 580;
            transform.Position.Y = 300;
            sprite.Height = 40;
            sprite.Width = 40;
            }

            // Hide messages from view 
            Domain.ForMatchingEntities<Text, Transform>((entity) => {
                var transform = entity.GetComponent<Transform>();
                transform.Position.Y = -1000; 
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
