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

            EntityUtility.CreateMessage(Domain, "Players in current lobby", -70, -180, 16);
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
            if (Connection.Instance.Space_owner)
            {
                left_pointer = EntityUtility.MenuArrow(Domain, false);
                right_pointer = EntityUtility.MenuArrow(Domain, true);
            }
               
        }

        public override void update(GameTime gameTime)
        {
            // Clear domain 
            Domain.Clean();

            foreach (Entity playerName in players)
            {
                playerName.Delete();
            }
            players.Clear();

            IEnumerable<ITuple> users = ClientProtocol.GetAllClientInLobby();
            foreach (ITuple user in users)
            {
                if (((string)user[2]).Contains("No user"))
                {
                    players.Add(EntityUtility.CreateMessage(Domain, "---------------", 0, 0, 14));
                }
                else
                {
                    players.Add(EntityUtility.CreateMessage(Domain, (string)user[2], 0, 0, 14));
                }
               
            }

            // Handles keyboard input
            GetKeys();

            // Displays list of all clients in server
            if (Connection.Instance.Space_owner)
            {
                displayCurrentIndex();
            }
            
            Display();

            // No host
           
            if (!Connection.Instance.Space_owner)
            {
                if (ClientProtocol.ListenForMatchBegin())
                {
                    ChangeState = true; 
                }
            }
       

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
                if (Connection.Instance.Space_owner)
                {
                    RunCurrentselected();
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
                        ClientProtocol.BeginMatch(); 
                        var info = clientInformation.GetComponent<Text>();
                        info.Message = "Starting game";
                        ChangeState = true;
                    }
                    break;
                default:
                    break;
            }
        }

        public void displayCurrentIndex()
        {

            int placement_Y = 100;
            for (int i = 0; i < menuItems.Count(); i++)
            {
                {
                    var transform = menuItems[i].GetComponent<Transform>();
                    transform.Position.X = -70;
                    transform.Position.Y = placement_Y;

                }
                if (i == currentIndex)
                {
                    {
                        var transform = left_pointer.GetComponent<Transform>();
                        transform.Position.X = -100;
                        transform.Position.Y = placement_Y + 20;
                    }
                    {
                        var transform = right_pointer.GetComponent<Transform>();
                        transform.Position.X = 100;
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
                int placement_Y = -160;
                foreach (Entity playerName in players)
                {
                    var transform = playerName.GetComponent<Transform>();
                    transform.Position.X = -70;
                    transform.Position.Y = placement_Y;
                    placement_Y += 30;
                }
            }

        }
    }
}

