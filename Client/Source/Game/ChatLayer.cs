using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using CaptainCombat.Common.Components;
using CaptainCombat.Common;
using static CaptainCombat.Common.Domain;
using CaptainCombat.Client.NetworkEvent;

namespace CaptainCombat.Client.GameLayers
{

    class ChatLayer : Layer
    {
        private Domain domain = new Domain();  
        private Camera camera;
        private EventController eventController;

        private Entity ChatBox;
        private Entity InputBox;

        private List<ChatMessage> messages = new List<ChatMessage>();

        private bool DisplayChat = false;
        private string InputMessage = string.Empty;

        public ChatLayer(EventController eventController)
        {
            camera = new Camera(domain);
            this.eventController = eventController;
            
            // Listen for chat message events 
            eventController.AddListener<MessageEvent>((e) => {
                lock(messages)
                {
                    messages.Add(new ChatMessage(Connection.Lobby.GetPlayer(e.Sender), e.Message));
                }
                return true;
            });

            init();
        }

        public override void init()
        {
            // Input box 
            InputBox = EntityUtility.CreateInput(domain, "", 360, 200, 14);
                
            // Chat layout
            ChatBox = new Entity(domain);
            ChatBox.AddComponent(new Transform());
            ChatBox.AddComponent(new Sprite(Assets.Textures.Chat, 200, 200));
        }


        public override void update(GameTime gameTime)
        {
            // Clearing domain 
            domain.Clean();

            // Updates chat if new message is added to remote space on server 
            int messageInDomain = 0; 
            domain.ForMatchingEntities<Text, Transform>((entity) => {
                messageInDomain++; 
            });


            lock (messages)
            {
                if (messageInDomain < messages.Count)
                {
                    domain.ForMatchingEntities<Text, Transform>((entity) => {
                        entity.Delete();
                    });

                    domain.ForMatchingEntities<Sprite, Transform>((entity) =>
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
                        EntityUtility.CreateIcon(domain, (int)message.Sender.Id);
                        EntityUtility.CreateMessage(domain, (string)message.Message, 0, 0, 14, origin: TextOrigin.Left);
                    }
                }
            }
            
            // Display chat 
            (DisplayChat ? new Action(Display) : Hide)();
        }


        public override bool OnKeyDown(Keys key)
        {
            if (key == Keys.Tab)
            {
                DisplayChat = !DisplayChat;
            }
            else if (!DisplayChat)
            {
                return false; 
            }
            else if (key == Keys.Enter)
            {
                eventController.Broadcast(new MessageEvent(InputMessage), includeLocal: true);
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
                return true;
            }
            return true;
        }

        public void Display()
        {
            // Display input box
            var input = InputBox.GetComponent<Input>();
            input.Message = InputMessage;


            // Display messages in chat box 
            { 
            int placement_Y = -230; 
            domain.ForMatchingEntities<Text, Transform>((entity) => {
                var transform = entity.GetComponent<Transform>();
                transform.Position.X = 385;
                transform.Position.Y = placement_Y;
                placement_Y += 30; 
            });
            }

            // Display icons in chat box 
            {
                int placement_Y = -245;
                domain.ForMatchingEntities<Sprite, Transform>((entity) => {
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
                domain.ForMatchingEntities<Sprite, Transform>((entity) => {
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
            domain.ForMatchingEntities<Text, Transform>((entity) => {
                var transform = entity.GetComponent<Transform>();
                transform.Position.Y = -1000; 
            });

        }


        public override void draw(GameTime gameTime)
        {
            Renderer.RenderSprites(domain, camera);
            Renderer.RenderText(domain, camera);
            Renderer.RenderInput(domain, camera);
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
