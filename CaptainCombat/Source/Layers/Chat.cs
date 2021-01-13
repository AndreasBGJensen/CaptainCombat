using CaptainCombat.Source.Components;
using CaptainCombat.Source.protocols;
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

        
      
        private Entity chat;
        private Entity inputBox;

        private bool disableKeyboard = true;
        private Keys[] lastPressedKeys = new Keys[5];
        private string message = string.Empty;

        Dictionary<string, string> dict = new Dictionary<string, string>
        {
            { "D1", "1" },
            { "D2", "2" },
            { "D3", "3" },
            { "D4", "4" },
            { "D5", "5" },
            { "D6", "6" },
            { "D7", "7" },
            { "D8", "8" },
            { "D9", "9" },
            { "D0", "0" },
            { "OemComma", "," },
            { "OemPeriod", "." },
            { "OemMinus", "-" },
        };

        private Game Game; 

        public Chat(Game game)
        {
            Game = game; 
            Camera = new Camera(Domain);
            init();
        }

        public override void init()
        {
            // Chat messages
            inputBox = EntityUtility.CreateInput(Domain, "", 360, 200, 14);


            chat = new Entity(Domain);
            chat.AddComponent(new Transform());
            chat.AddComponent(new Sprite(Assets.Textures.Chat, 200, 200));
            var transform = chat.GetComponent<Transform>();
            transform.X = 200;
            transform.Y = 200;
        }

        

        public override void update(GameTime gameTime)
        {
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
                int nummerOfElements = 15;
                var LastMessages = AllUsersMessages.Skip(Math.Max(0, AllUsersMessages.Count() - nummerOfElements)).Take(nummerOfElements); 
                foreach (string chatMessages in LastMessages)
                {
                    EntityUtility.CreateMessage(Domain, chatMessages, 0, 0, 14);
                }
                
            }
            

            Domain.Clean();
            GetKeys(); 

            if (!disableKeyboard)
            {
                var input = inputBox.GetComponent<Input>();
                input.Message = message;
                ShowChat(); 
            }
            else
            {
                var input = inputBox.GetComponent<Input>();
                input.Message = string.Empty;
                HideChat(); 
            }
        }

        public void GetKeys()
        {
            KeyboardState kbState = Keyboard.GetState();
            Keys[] pressedKeys = kbState.GetPressedKeys(); 

            foreach(Keys key in lastPressedKeys)
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
            else if (disableKeyboard)
            {
                return; 
            }
            else if (key == Keys.Enter)
            {
                ClientProtocol.AddMessageToServer(message); 
                message = string.Empty; 
                var input = inputBox.GetComponent<Input>();
                input.Message = message;
               
            }
            else if (key == Keys.Back)
            {
                if (message.Length > 0) {
                    message = message.Remove(message.Length - 1);
                    var input = inputBox.GetComponent<Input>();
                    input.Message = message;
                } 
            }
            else if (key == Keys.Space)
            {
                message += " "; 
            }
            else
            {
                string pattern = @"[A-Z]";
                string keyData = key.ToString();
                if ((Regex.IsMatch(keyData, pattern) && keyData.Length <= 1 || dict.ContainsKey(keyData)) && message.Length < 20 )
                {
                    if (dict.ContainsKey(keyData))
                    {
                        message += dict[keyData];
                    }
                    else
                    {
                        message += keyData; 
                    }
                }

            }
        }


        public void ShowChat()
        {
            { 
            var transform = chat.GetComponent<Transform>();
            var sprite = chat.GetComponent<Sprite>();

            // Placement 
            transform.X = 450;
            transform.Y = 0;
            sprite.Height = 700;
            sprite.Width = 300;
            }
            int placement_Y = -230; 

            Domain.ForMatchingEntities<Text, Transform>((entity) => {
                var transform = entity.GetComponent<Transform>();
                transform.X = 360;
                transform.Y = placement_Y;
                placement_Y += 25; 
            });
        }

        public void HideChat()
        {
            { 
            var transform = chat.GetComponent<Transform>();
            var sprite = chat.GetComponent<Sprite>();

            // Placement 
            transform.X = 580;
            transform.Y = 300;
            sprite.Height = 40;
            sprite.Width = 40;
            }


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
