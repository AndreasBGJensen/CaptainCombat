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
using System.Threading.Tasks;
using System.Threading;

namespace CaptainCombat.Client.Source.Layers
{
    class LobbyList : Layer
    {

        private Camera Camera;
        private Domain Domain = new Domain();

        private bool DisableKeyboard = false;

        private State ParentState;
        private Game Game;
        private int selectionIndex = 0;
        private List<Entity> lobbyEntities = new List<Entity>();
        private List<string> allURLs = new List<string>();

        private Entity left_pointer;
        private Entity right_pointer;
        private Entity clientInformation;

        private Loader<bool> loader;

        private bool updateLobbies = false;

        private bool lobbiesWereUpdated = false;
        private Dictionary<string, LobbyInfo> currentLobies = new Dictionary<string, LobbyInfo>();

        private List<LobbyInfo> menuList = new List<LobbyInfo>();


        public LobbyList(Game game, State state)
        {
            ParentState = state;
            Game = game;
            Camera = new Camera(Domain);

            clientInformation = EntityUtility.CreateMessage(Domain, "", 0, 150, 16);

            EntityUtility.CreateMessage(Domain, "Select lobby", 0, -180, 16);

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

            UpdateLobbies();
        }


        // Task which repeatedly fetches the current lobbies,
        // and signal for an update of the UI in case they've changed
        private async void UpdateLobbies()
        {
            updateLobbies = true;
            await Task.Run(() => {
                while(updateLobbies)
                {
                    var lobbies = ClientProtocol.GetLobbies();
                    lock(currentLobies)
                    {
                        // Check if lobbies requires update
                        var matched = lobbies.Count == currentLobies.Count;
                        if( matched )
                            foreach (var lobby in lobbies)
                                if( !currentLobies.ContainsKey(lobby.Id))
                                {
                                    matched = false;
                                    break;
                                }

                        // Update lobbies
                        if( !matched )
                        {
                            currentLobies.Clear();
                            foreach (var lobby in lobbies)
                                currentLobies[lobby.Id] = lobby;
                            lobbiesWereUpdated = true;
                        }

                        // Since the GetLobbies() function creates a new connection to the server for
                        // each lobby that exists, it may overload the server. Reducing the frequency
                        // of the connections fixes the problem as of now.
                        Thread.Sleep(500);
                    }
                }
            });
        }


        public override void update(GameTime gameTime)
        {
            // Clear domain 
            Domain.Clean();

            if( lobbiesWereUpdated )
            {
                lock(currentLobies)
                {
                    // Clear current lobby entities (menu)
                    foreach (var lobby in lobbyEntities)
                        lobby.Delete();
                    allURLs.Clear();
                    lobbyEntities.Clear();

                    // Update selection index
                    if (selectionIndex < currentLobies.Count)
                        selectionIndex = currentLobies.Count-1;

                    // Create new lobby entities
                    menuList.Clear();
                    foreach (var lobby in currentLobies.Values)
                    {
                        lobbyEntities.Add(EntityUtility.CreateMessage(Domain, $"{lobby.OwnerName}'s lobby ({lobby.NumPlayers}/{Settings.LOBBY_SLOTS})", 0, 0, 18));
                        menuList.Add(lobby);
                        menuList.Sort((l1, l2) => string.Compare(l2.Id, l1.Id));
                    }

                    lobbiesWereUpdated = false;
                }
            }

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
                DisableKeyboard = !DisableKeyboard;
                RunCurrentselected();
                return true;
            }
            
            if (key == Keys.Up)
            {
                if (!(selectionIndex == 0))
                {
                    selectionIndex--;
                }
                return true;
            }
            
            if (key == Keys.Down)
            {
                if (!(selectionIndex == lobbyEntities.Count - 1))
                {
                    selectionIndex++;
                }
                return true;
            }
            return false;
        }


        public void RunCurrentselected()
        {
            loader = new Loader<bool>("Connecting to lobby",
                    () => ClientProtocol.SubscribeForLobby(menuList[selectionIndex].Url),
                    (success) => {
                        if( !success )
                        {
                            var info = clientInformation.GetComponent<Text>();
                            info.Message = "Connection failed";
                            loader = null;
                            DisableKeyboard = false;
                        }
                        else
                        {
                            updateLobbies = false;
                            ParentState._context.TransitionTo(new LobbyState(Game));
                        }
                    }
                );
        }


        public void displayCurrentIndex()
        {

            int placement_Y = -100;
            for (int i = 0; i < lobbyEntities.Count(); i++)
            {
                {
                    var transform = lobbyEntities[i].GetComponent<Transform>();
                    transform.Position.X = 0;
                    transform.Position.Y = placement_Y;

                }
                if (i == selectionIndex)
                {
                    {
                        var transform = left_pointer.GetComponent<Transform>();
                        transform.Position.X = -125;
                        transform.Position.Y = placement_Y + 10;
                    }
                    {
                        var transform = right_pointer.GetComponent<Transform>();
                        transform.Position.X = 125;
                        transform.Position.Y = placement_Y + 10;
                    }
                }
                placement_Y += 50;
            }
        }
    }
}

