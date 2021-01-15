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

        private bool DisableKeyboard = false;
        private bool ChangeState = false; 

        private Keys[] LastPressedKeys = new Keys[5];
        private string PlayerName = string.Empty;

        private Entity InputBox;

        private State ParentState;
        private Game Game; 


        public Menu(Game game, State state)
        {
            ParentState = state;
            Game = game; 
            Camera = new Camera(Domain);
            init();
        }

        public override void init()
        {
            
            // Static message to client 
            EntityUtility.CreateMessage(Domain, "Players in server", 0, 0, 16);

            // Creates a list of all clients in server
            List<string> users = ClientProtocol.GetAllUsers();
            foreach (string user in users)
            {
                EntityUtility.CreateMessage(Domain, user, 0, 0, 14);
            }

            // Background
            Entity backGround = new Entity(Domain);
            backGround.AddComponent(new Transform());
            backGround.AddComponent(new Sprite(Assets.Textures.Background, 1280, 720));

            // Menu
            Entity Menu = new Entity(Domain);
            Menu.AddComponent(new Transform());
            Menu.AddComponent(new Sprite(Assets.Textures.Menu, 600, 600));

            // InputBox
            InputBox = EntityUtility.CreateInput(Domain, "Enter Name", -70, 150, 16);
        }

        public override void update(GameTime gameTime)
        {
            // Clear domain 
            Domain.Clean();

            // Handles keyboard input
            GetKeys();

            // Displays list of all clients in server
            DisplayPlayerNames();

        
            // Changes state when condition is true 
            if (ChangeState)
            {
                ParentState._context.TransitionTo(new GameState(Game));
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
                

                if (ClientProtocol.isValidName(PlayerName) && PlayerName.Length > 0)
                {
                    // Disables keyboard
                    DisableKeyboard = !DisableKeyboard;

                    // Adds player name to domain 
                    EntityUtility.CreateMessage(Domain, PlayerName, 0, 0, 16);

                    // Adds playername to server 
                    ClientProtocol.Join(PlayerName);

                    // Enablers state change after delay 
                    Task.Factory.StartNew(async () =>
                    {
                        await Task.Delay(2000);
                        ChangeState = true;
                    });

                    // Display a message to the client 
                    var input = InputBox.GetComponent<Input>();
                    input.Message = "Game is staring...";
                }
                else
                {
                    PlayerName = string.Empty; 
                    var input = InputBox.GetComponent<Input>();
                    input.Message = "Invalid username";
                }
            }
            else if (key == Keys.Back)
            {
                if (PlayerName.Length > 0)
                {
                    PlayerName = PlayerName.Remove(PlayerName.Length - 1);
                    var input = InputBox.GetComponent<Input>();
                    input.Message = PlayerName;
                }
            }
            else if (key == Keys.Space)
            {
                PlayerName += " ";
                var input = InputBox.GetComponent<Input>();
                input.Message = PlayerName;
            }
            else
            {
                string keyData = key.ToString();
                if (KeyboardInputValidator.isValid(keyData) && PlayerName.Length < 20)
                {
                    PlayerName = (KeyboardInputValidator.dict.ContainsKey(keyData) ? PlayerName += KeyboardInputValidator.dict[keyData] : PlayerName += keyData);
                    var input = InputBox.GetComponent<Input>();
                    input.Message = PlayerName;
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

    }
}
