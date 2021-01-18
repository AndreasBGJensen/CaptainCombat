using CaptainCombat.Client.protocols;
using CaptainCombat.Client.Scenes;
using dotSpace.Interfaces.Space;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.Linq;
using CaptainCombat.Common.Components;
using CaptainCombat.Common;
using static CaptainCombat.Common.Domain;
using Microsoft.Xna.Framework.Input;
using CaptainCombat.Common.Singletons;
using CaptainCombat.Client.Source.Scenes;

namespace CaptainCombat.Client.Source.Layers
{
    class GameLobby : Layer
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
        private List<Entity> players = new List<Entity>();
        private List<Entity> playerIcons = new List<Entity>();
        private Entity left_pointer;
        private Entity right_pointer;
        private Entity clientInformation;
        

        public GameLobby(Game game, State state)
        {
            ParentState = state;
            Game = game;
            Camera = new Camera(Domain);
            init();
        }

        public override void init()
        {

            // Static message to client 
            if (Connection.Instance.Space_owner)
            {
                menuItems.Add(EntityUtility.CreateMessage(Domain, "Start game", 0, 0, 20));
                clientInformation = EntityUtility.CreateMessage(Domain, "", -70, 150, 16);
            }
            else
            {
                clientInformation = EntityUtility.CreateMessage(Domain, "Waiting for host to start game", -70, 150, 16);
            }
            
            

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

            IEnumerable<ITuple> AllClientsIngame = ClientProtocol.GetAllClients();
            if (AllClientsIngame != null)
            {
                
                foreach (Entity icon in playerIcons)
                {
                    icon.Delete();
                }
                foreach (Entity playerName in players)
                {
                    playerName.Delete();
                }
                playerIcons.Clear();
                players.Clear();

                foreach (ITuple client in AllClientsIngame)
                {
                    playerIcons.Add(EntityUtility.CreateIcon(Domain, (int)client[1]));
                    players.Add(EntityUtility.CreateMessage(Domain, (string)client[2], 0, 0, 16));
                }
                
            }

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
                if (Connection.Instance.Space_owner)
                {
                    RunCurrentselected();
                }
                //DisableKeyboard = !DisableKeyboard;
                
                /*
                Task.Factory.StartNew(async () =>
                {
                    await Task.Delay(2000);
                    ChangeState = true;
                });
                */
            }
        }

        public void RunCurrentselected()
        {
            switch (currentIndex)
            {
                case 0:
                    {
                        var info = clientInformation.GetComponent<Text>();
                        info.Message = "Starting game";
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

        public void Display()
        {

            // Display client names
            {
                int placement_Y = -245;
                foreach (Entity playerName in players)
                {
                    var transform = playerName.GetComponent<Transform>();
                    transform.Position.X = -565;
                    transform.Position.Y = placement_Y;
                    placement_Y += 30;
                }
            }

            // Display client icons
            {
                int placement_Y = -230;
                foreach (Entity icon in playerIcons)
                {
                    var transform = icon.GetComponent<Transform>();
                    transform.Position.X = -585;
                    transform.Position.Y = placement_Y;
                    placement_Y += 30;
                }
            }
        }
    }
}

