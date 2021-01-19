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
using CaptainCombat.Common.Singletons;
using dotSpace.Interfaces.Space;

namespace CaptainCombat.Client.Source.Layers
{
    class SelectLobby : Layer
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


        public SelectLobby(Game game, State state)
        {
            ParentState = state;
            Game = game;
            Camera = new Camera(Domain);
            init();
        }

        public override void init()
        {

            // Static message to client 
            menuItems.Add(EntityUtility.CreateMessage(Domain, "Create new Lobby", 0, 0, 20));
            menuItems.Add(EntityUtility.CreateMessage(Domain, "Join existing lobby", 0, 0, 20));
            clientInformation = EntityUtility.CreateMessage(Domain, "", -70, 150, 16);

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
                RunCurrentselected(); 
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

            switch (currentIndex)
            {
                case 0:
                    {
                        DisableKeyboard = !DisableKeyboard;
                        var info = clientInformation.GetComponent<Text>();
                        info.Message = "Creating new lobby";
                        Connection.Instance.Space_owner = true;
                        ClientProtocol.CreateLobby();

                        Task.Factory.StartNew(async () =>
                        {
                            await Task.Delay(2000);
                            ParentState._context.TransitionTo(new GameLobbyState(Game));
                        });
                    }
                    break;
                case 1:
                    {
                        IEnumerable<ITuple> allLobies = ClientProtocol.GetAllLobbys();
                        
                        if (allLobies.Count() != 0)
                        {
                            DisableKeyboard = !DisableKeyboard;
                            var info = clientInformation.GetComponent<Text>();
                            info.Message = "Going to lobbies";
                            Connection.Instance.Space_owner = false;
                           // ClientProtocol.CreateLobby();
                            Task.Factory.StartNew(async () =>
                            {
                                await Task.Delay(2000);
                                ParentState._context.TransitionTo(new LobbyState(Game));
                            });
                        }
                        else
                        {
                            var info = clientInformation.GetComponent<Text>();
                            info.Message = "No existing lobbies";
                        }
                    }
                    break;
                default:
                    break;
            }
        }

        public void displayCurrentIndex()
        {
           
            int placement_Y = -50;
            for (int i = 0; i < menuItems.Count(); i++)
            {
                {
                    var transform = menuItems[i].GetComponent<Transform>();
                    transform.Position.X = -80;
                    transform.Position.Y = placement_Y;
                    
                }
                if (i == currentIndex)
                {
                    {
                        var transform = left_pointer.GetComponent<Transform>();
                        transform.Position.X = -110;
                        transform.Position.Y = placement_Y + 20;
                    }
                    {
                        var transform = right_pointer.GetComponent<Transform>();
                        transform.Position.X = 125;
                        transform.Position.Y = placement_Y + 20;
                    }
                }
                placement_Y += 70;
            }
        }
    }

}
