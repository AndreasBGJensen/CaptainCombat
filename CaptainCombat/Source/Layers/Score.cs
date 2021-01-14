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

namespace CaptainCombat.Source.GameLayers
{
    class Score : Layer
    {

        private Domain Domain = new Domain(); 
        private Camera Camera;
        private Game Game;
        private State ParentState; 
        public Score(Game game, State state)
        {
            ParentState = state; 
            Game = game; 
            Camera = new Camera(Domain);
            init();
        }

        public override void init()
        {
           
        }

        public override void update(GameTime gameTime)
        {
            Domain.Clean();

            // Updates chat if new message is added to remote space on server 
            int clientInDomain = 0;
            Domain.ForMatchingEntities<Sprite, Transform>((entity) => {
                clientInDomain++;
            });

            IEnumerable<ITuple> AllClientsIngame = ClientProtocol.GetAllClients();
            if (AllClientsIngame != null)
            {
                if (clientInDomain < AllClientsIngame.Count())
                {
                    Domain.ForMatchingEntities<Text, Transform>((entity) => {
                        entity.Delete();
                    });

                    Domain.ForMatchingEntities<Sprite, Transform>((entity) =>
                    {
                        entity.Delete();
                    });

                    foreach (ITuple client in AllClientsIngame)
                    {
                        EntityUtility.CreateIcon(Domain, (int)client[1]);
                        EntityUtility.CreateMessage(Domain, (string)client[2], 0, 0, 16);
                    }
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
                Domain.ForMatchingEntities<Text, Transform>((entity) => {
                    var transform = entity.GetComponent<Transform>();
                    transform.X = -565;
                    transform.Y = placement_Y;
                    placement_Y += 30;
                });
            }

            // Display client icons
            {
                int placement_Y = -230;
                Domain.ForMatchingEntities<Sprite, Transform>((entity) => {
                    var transform = entity.GetComponent<Transform>();
                    transform.X = -585;
                    transform.Y = placement_Y;
                    placement_Y += 30;
                });
            }
        }
    }
}
