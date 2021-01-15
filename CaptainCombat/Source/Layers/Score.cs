using CaptainCombat.Source.Components;
using CaptainCombat.Source.protocols;
using CaptainCombat.Source.Scenes;
using dotSpace.Interfaces.Space;
using ECS;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static ECS.Domain;

namespace CaptainCombat.Source.GameLayers
{
    class Score : Layer
    {

        private Domain Domain = new Domain(); 
        private Camera Camera;
        private Game Game;
        private State ParentState;
        private List<Entity> playerNames = new List<Entity>();
        private List<Entity> playerScores = new List<Entity>();
        private List<Entity> playerIcons = new List<Entity>();
        public Score(Game game, State state)
        {
            ParentState = state; 
            Game = game; 
            Camera = new Camera(Domain);
            init();
        }

        public override void init()
        {
            ClientProtocol.AddClientScoreToServer(0); 
        }

        public override void update(GameTime gameTime)
        {
            Domain.Clean();

            // Updates player names and icons if player is added to remote space on server 
            int clientInDomain = 0;
            Domain.ForMatchingEntities<Sprite, Transform>((entity) => {
                clientInDomain++;
            });

            IEnumerable<ITuple> AllClientsIngame = ClientProtocol.GetAllClients();
            if (AllClientsIngame != null)
            {
                if (clientInDomain < AllClientsIngame.Count())
                {
                    foreach(Entity icon in playerIcons)
                    {
                        icon.Delete(); 
                    }
                    foreach (Entity playerName in playerNames)
                    {
                        playerName.Delete();
                    }
                    playerIcons.Clear();
                    playerNames.Clear(); 

                    foreach (ITuple client in AllClientsIngame)
                    {
                        playerIcons.Add(EntityUtility.CreateIcon(Domain, (int)client[1])); 
                        playerNames.Add(EntityUtility.CreateMessage(Domain, (string)client[2], 0, 0, 16)); 
                    }
                }
            }

            IEnumerable<ITuple> AllPlayerScores = ClientProtocol.GetAllClientScores();
            if (AllPlayerScores != null)
            {   
                foreach (Entity score in playerScores)
                {
                    score.Delete();
                }
                playerScores.Clear();

                AllPlayerScores = AllPlayerScores.OrderBy(client => (int)client[1]); 
                foreach (ITuple client in AllPlayerScores)
                {
                    playerScores.Add(EntityUtility.CreateMessage(Domain, ((int)client[2]).ToString(), 0, 0, 16));
                }
               
            }





            Display(); 
        }

        public override void draw(GameTime gameTime)
        {
            Renderer.RenderSprites(Domain, Camera);
            Renderer.RenderText(Domain, Camera);
        }

        public void Display()
        {
            
            // Display client names
            {
                int placement_Y = -245;
                foreach (Entity playerName in playerNames)
                {
                    var transform = playerName.GetComponent<Transform>();
                    transform.X = -565;
                    transform.Y = placement_Y;
                    placement_Y += 30;
                }
            }

            // Display client icons
            {
                int placement_Y = -230;
                foreach (Entity icon in playerIcons)
                {
                    var transform = icon.GetComponent<Transform>();
                    transform.X = -585;
                    transform.Y = placement_Y;
                    placement_Y += 30;
                }
            }

            // Display client scores
            {
                int placement_Y = -245;
               
                foreach (Entity score in playerScores)
                {
                    var transform = score.GetComponent<Transform>();
                    transform.X = -625;
                    transform.Y = placement_Y;
                    placement_Y += 30;
                }
            }


        }
    }
}
