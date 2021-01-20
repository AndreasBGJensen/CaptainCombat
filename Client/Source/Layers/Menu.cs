using CaptainCombat.Client.protocols;
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

namespace CaptainCombat.Client.MenuLayers
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
            for(int i = 0; i < users.Count(); i++)
            {
                EntityUtility.CreateMessage(Domain, users[i], 0, 0, 14);
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
            InputBox = EntityUtility.CreateInput(Domain, "Enter Name", -70, 100, 18);
        }

        public override void update(GameTime gameTime)
        {
            // Clear domain 
            Domain.Clean();

            // Displays list of all clients in server
            DisplayPlayerNames();

        
            // Changes state when condition is true 
            if (ChangeState)
            {
                ParentState._context.TransitionTo(new SelectLobbyState(Game));
            }
        }

        public override void draw(GameTime gameTime)
        {
            Renderer.RenderSprites(Domain, Camera);
            Renderer.RenderText(Domain, Camera);
            Renderer.RenderInput(Domain, Camera);
        }

        public override bool OnKeyDown(Keys key)
        {
            if (DisableKeyboard)
            {
                return false; 
            }
            else if (key == Keys.Enter)
            {
                if (ClientProtocol.Join(PlayerName))
                {
                    // Disables keyboard
                    DisableKeyboard = !DisableKeyboard;

                    // Adds player name to domain 
                    EntityUtility.CreateMessage(Domain, PlayerName, 0, 0, 16);

                    // Enablers state change after delay 
                    Task.Factory.StartNew(async () =>
                    {
                        await Task.Delay(2000);
                        ChangeState = true;
                    });

                    // Display a message to the client 
                    var input = InputBox.GetComponent<Input>();
                    input.Message = "Joined server succesful";
                }
                else
                {
                    PlayerName = string.Empty; 
                    var input = InputBox.GetComponent<Input>();
                    input.Message = "Invalid username";
                }
                return true;
            }
            else if (key == Keys.Back)
            {
                if (PlayerName.Length > 0)
                {
                    PlayerName = PlayerName.Remove(PlayerName.Length - 1);
                    var input = InputBox.GetComponent<Input>();
                    input.Message = PlayerName;
                }
                return true;
            }
            else if (key == Keys.Space)
            {
                PlayerName += " ";
                var input = InputBox.GetComponent<Input>();
                input.Message = PlayerName;
                return true;
            }
            else
            {
                string keyData = key.ToString();
                if (KeyboardInputValidator.isValid(keyData) && PlayerName.Length < 20)
                {
                    PlayerName = (KeyboardInputValidator.dict.ContainsKey(keyData) ? PlayerName += KeyboardInputValidator.dict[keyData] : PlayerName += keyData);
                    var input = InputBox.GetComponent<Input>();
                    input.Message = PlayerName;
                    return true;
                }
                return false;
            }
        }

        public void DisplayPlayerNames()
        {
            int placement_Y = -150;
            Domain.ForMatchingEntities<Text, Transform>((entity) => {
                var transform = entity.GetComponent<Transform>();
                transform.Position.X = 0; 
                transform.Position.Y = placement_Y;
                placement_Y += 25;
            });
        }

    }
}
