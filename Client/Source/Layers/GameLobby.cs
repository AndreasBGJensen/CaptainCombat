using CaptainCombat.Client.protocols;
using CaptainCombat.Client.Scenes;
using dotSpace.Interfaces.Space;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using CaptainCombat.Common.Components;
using CaptainCombat.Common;
using static CaptainCombat.Common.Domain;
using Microsoft.Xna.Framework.Input;

namespace CaptainCombat.Client.Source.Layers
{

    class GameLobby : Layer
    {

        private Camera Camera;
        private Domain Domain = new Domain();

        private bool DisableKeyboard = false;
        private bool ChangeState = false;

        private State ParentState;
        private Game Game;

        private List<Entity> players = new List<Entity>();
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

            EntityUtility.CreateMessage(Domain, "Players in lobby", 0, -180, 16);
            if (Connection.Lobby.Owner == Connection.LocalPlayer)
            {
                clientInformation = EntityUtility.CreateMessage(Domain, "Press 'enter' to start game", 0, 100, 16);
            }
            else
            {
                clientInformation = EntityUtility.CreateMessage(Domain, "Waiting for host to start game", 0, 100, 16);
            }
            
            
            // Background
            Entity backGround = new Entity(Domain);
            backGround.AddComponent(new Transform());
            backGround.AddComponent(new Sprite(Assets.Textures.Background, 1280, 720));

            // Menu
            Entity Menu = new Entity(Domain);
            Menu.AddComponent(new Transform());
            Menu.AddComponent(new Sprite(Assets.Textures.Menu, 600, 600));
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

            IEnumerable<ITuple> users = ClientProtocol.GetClientsInLobby();
            foreach (ITuple user in users)
            {
                if (((string)user[2]).Contains("No user"))
                {
                    players.Add(EntityUtility.CreateMessage(Domain, "- - - - -", 0, 0, 16));
                }
                else
                {
                    players.Add(EntityUtility.CreateMessage(Domain, (string)user[2], 0, 0, 16));
                }
               
            }
            players.Reverse();

            SetPlayerNamesPosition();

            // No host
            if (Connection.LocalPlayer != Connection.Lobby.Owner)
            {
                if (ClientProtocol.IsGameStarted())
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


        public override bool OnKeyDown(Keys key)
        {
            if (DisableKeyboard) return false;
            
            if (key == Keys.Enter)
            {
                if (Connection.LocalPlayer == Connection.Lobby.Owner)
                {
                    // Start the game
                    if (ClientProtocol.StartGame())
                    {
                        DisableKeyboard = !DisableKeyboard;
                        var info = clientInformation.GetComponent<Text>();
                        info.Message = "Starting game";
                        ChangeState = true;
                    }
                    else
                    {
                        var info = clientInformation.GetComponent<Text>();
                        info.Message = "Not enough players!";
                    }
                }
                return true;
            }
            return false;
        }


        private void SetPlayerNamesPosition()
        {
            // Display client names
            int placement_Y = -100;
            foreach (Entity playerName in players)
            {
                var transform = playerName.GetComponent<Transform>();
                transform.Position.X = 0;
                transform.Position.Y = placement_Y;
                placement_Y += 30;
            }
        }
    }
}

