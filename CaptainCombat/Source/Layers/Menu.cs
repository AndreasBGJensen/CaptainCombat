using CaptainCombat.Source.Components;
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

namespace CaptainCombat.Source.MenuLayers
{
    class Menu : Layer
    {
        
        private Camera Camera;
        private Domain Domain = new Domain();
        private KeyboardController keyboardController;

        private bool disableKeyboard = false;
        private Keys[] lastPressedKeys = new Keys[5];
        private string message = string.Empty;

        private Entity inputBox;
        private State ParentState;
        private Game Game; 

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

        public Menu(Game game, State state)
        {
            Game = game; 
            ParentState = state; 
            Camera = new Camera(Domain);
            keyboardController = new KeyboardController(); 
            init();
        }

        public override void init()
        {
            // Static text 
            

            // Players
            EntityUtility.CreateMessage(Domain, "Players in server", 0, 0, 14);

            List<string> users = ClientProtocol.GetAllUsers();

            foreach (String user in users)
            {
                EntityUtility.CreateMessage(Domain, user, 0, 0, 14);
            }

            // Background
            Entity backGround = new Entity(Domain);
            backGround.AddComponent(new Transform());
            backGround.AddComponent(new Sprite(Assets.Textures.Background, 1280, 720));

            Entity Menu = new Entity(Domain);
            Menu.AddComponent(new Transform());
            Menu.AddComponent(new Sprite(Assets.Textures.Menu, 600, 600));

            // InputBox
            inputBox = EntityUtility.CreateInput(Domain, "Enter Name", -70, 150, 14);
        }

        public override void update(GameTime gameTime)
        {
            Domain.Clean();
            GetKeys();
            DisplayPlayerNames(); 
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
            if (disableKeyboard)
            {
                return; 
            }
            
            if (key == Keys.Enter)
            {
                disableKeyboard = !disableKeyboard;
                EntityUtility.CreateMessage(Domain, message, 0, 0, 14);
                ClientProtocol.Join(message); 
                Task.Factory.StartNew(async () =>
                {
                    await Task.Delay(2000);
                    changeState();
                });
                message = "Game is staring...";
                var input = inputBox.GetComponent<Input>();
                input.Message = message;
            }
            else if (key == Keys.Back)
            {
                if (message.Length > 0)
                {
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
                if ((Regex.IsMatch(keyData, pattern) && keyData.Length <= 1 || dict.ContainsKey(keyData)) && message.Length < 20)
                {
                    if (dict.ContainsKey(keyData))
                    {
                        message += dict[keyData];
                    }
                    else
                    {
                        message += keyData;
                    }

                    var input = inputBox.GetComponent<Input>();
                    input.Message = message;
                }

            }
        }

        public void DisplayPlayerNames()
        {
            int placement_Y = -180;

            Domain.ForMatchingEntities<Text, Transform>((entity) => {
                var transform = entity.GetComponent<Transform>();
                transform.Y = placement_Y;
                transform.X = -70; 
                placement_Y += 25;
            });
        }


     
    public void changeState()
    {
        ParentState._context.TransitionTo(new GameState(Game));
    }

    }
}
