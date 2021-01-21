using CaptainCombat.Client.protocols;
using CaptainCombat.Client.Scenes;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using System.Linq;
using CaptainCombat.Common.Components;
using CaptainCombat.Common;
using static CaptainCombat.Common.Domain;
using CaptainCombat.Client.Source.Scenes;


namespace CaptainCombat.Client.Source.Layers
{
    class Select : Layer
    {
        private Camera Camera;
        private Domain Domain = new Domain();

        private bool DisableKeyboard = false;
        
        private State ParentState;
        private Game Game;
        private int currentIndex = 0;
        private List<Entity> menuItems = new List<Entity>();
        private Entity left_pointer;
        private Entity right_pointer;
        private Entity clientInformation;

        private Loader<List<LobbyInfo>> loader;

        public Select(Game game, State state)
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
            clientInformation = EntityUtility.CreateMessage(Domain, "", 0, 150, 16);

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

            // Displays list of all clients in server
            displayCurrentIndex(); 

            loader?.update(gameTime);
        }

        public override void draw(GameTime gameTime)
        {
            Renderer.RenderSprites(Domain, Camera);
            Renderer.RenderText(Domain, Camera);
            Renderer.RenderInput(Domain, Camera);
            loader?.draw(gameTime);
        }


        public override bool OnKeyDown(Keys key)
        {
            if (DisableKeyboard) return false;

            if (key == Keys.Enter)
            {
                RunCurrentselected();
                return true;
            }
            
            if (key == Keys.Up)
            {
                if (!(currentIndex == 0))
                {
                    currentIndex--;
                }
                return true;
            }

            if (key == Keys.Down)
            {
                if (!(currentIndex == menuItems.Count - 1))
                {
                    currentIndex++;
                }
                return true;
            }
            return false;
        }

        public void RunCurrentselected()
        {

            switch (currentIndex)
            {
                case 0:
                    {
                        // Create new lobby
                        DisableKeyboard = !DisableKeyboard;
                        loader = new Loader<List<LobbyInfo>>("Creating lobby",
                            () => {
                                ClientProtocol.CreateLobby();
                                return null;
                            },
                            (success) => ParentState._context.TransitionTo(new LobbyState(Game))
                        );
                    }
                    break;
                case 1:
                    {
                        // Join existing lobby
                        loader = new Loader<List<LobbyInfo>>("Loading lobbies",
                            () => ClientProtocol.GetLobbies(),
                            (lobbies) => {
                                if (lobbies.Count != 0)
                                {
                                    DisableKeyboard = !DisableKeyboard;
                                    ParentState._context.TransitionTo(new SelectLobbyState(Game));
                                }
                                else
                                {
                                    var info = clientInformation.GetComponent<Text>();
                                    info.Message = "No existing lobbies";
                                    loader = null;
                                }
                            }
                        );
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
                    transform.Position.Y = placement_Y;
                }
                if (i == currentIndex)
                {
                    {
                        var transform = left_pointer.GetComponent<Transform>();
                        transform.Position.X = -125;
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
