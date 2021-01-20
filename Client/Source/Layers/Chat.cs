using CaptainCombat.Client.Scenes;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using CaptainCombat.Common.Components;
using CaptainCombat.Common;
using static CaptainCombat.Common.Domain;
using CaptainCombat.Client.NetworkEvent;
using CaptainCombat.Client.Layers.Events;

namespace CaptainCombat.Client.GameLayers
{

    class Chat : Layer
    {
        private Domain Domain = new Domain();  
        private Camera Camera;

        private Entity ChatBox;
        private Entity InputBox;

        private List<ChatMessage> messages = new List<ChatMessage>();

        private bool DisplayChat = false;
        private Keys[] LastPressedKeys = new Keys[5];
        private string InputMessage = string.Empty;

        public Chat(Game game, State state)
        {
            Camera = new Camera(Domain);

            // Listen for chat message events 
            EventController.AddListener<MessageEvent>((e) => {
                lock(messages)
                {
                    messages.Add(new ChatMessage(GameInfo.Current.GetPlayer(e.Sender), e.Message));
                }
                return true;
            });

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


            lock (messages)
            {
                if (messageInDomain < messages.Count)
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
                    var LastMessages = messages.Skip(Math.Max(0, messages.Count() - maxDisplayedMesseges)).Take(maxDisplayedMesseges);
                    foreach (var message in messages)
                    {
                        EntityUtility.CreateIcon(Domain, (int)message.Sender.Id);
                        EntityUtility.CreateMessage(Domain, (string)message.Message, 0, 0, 14);
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
                EventController.Broadcast(new MessageEvent(InputMessage), includeLocal: true);
                InputMessage = string.Empty;
            }
            else if (key == Keys.Back)
            {
                if (InputMessage.Length > 0) {
                    InputMessage = InputMessage.Remove(InputMessage.Length - 1);
                } 
            }
            else if (key == Keys.Space)
            {
                InputMessage += " "; 
            }
            else
            {
                string keyData = key.ToString();
                if (KeyboardInputValidator.isValid(keyData) && InputMessage.Length < 20)
                {
                    InputMessage = (KeyboardInputValidator.dict.ContainsKey(keyData) ? InputMessage += KeyboardInputValidator.dict[keyData] : InputMessage += keyData); 
                }

            }
        }

        public void Display()
        {
            // Display input box
            var input = InputBox.GetComponent<Input>();
            input.Message = InputMessage;


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


        // Just represents a message
        private struct ChatMessage
        {
            public Player Sender;
            public string Message;

            public ChatMessage(Player sender, string message)
            {
                Sender = sender;
                Message = message;
            }
        }

    }
       
}
