﻿using CaptainCombat.Client.protocols;
using CaptainCombat.Client.Scenes;
using dotSpace.Interfaces.Space;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.Linq;
using CaptainCombat.Common.Components;
using CaptainCombat.Common;
using static CaptainCombat.Common.Domain;
using CaptainCombat.Client.Layers;

namespace CaptainCombat.Client.GameLayers
{
    class Score : Layer
    {

        private Domain Domain = new Domain(); 
        private Camera Camera;
        private List<Entity> playerNames = new List<Entity>();
        private List<Entity> playerScores = new List<Entity>();
        private List<Entity> playerIcons = new List<Entity>();
        private LifeController lifeController;

        private bool started = false;

        public Score(LifeController lifeController)
        {
            this.lifeController = lifeController;
            Camera = new Camera(Domain);
        }

        public override void init()
        {
        }

        public void Start() {
            started = true;
        }

        public override void update(GameTime gameTime)
        {
            if (!started) return;

            Domain.Clean();

            // Updates player names and icons if player is added to remote space on server 
            int clientInDomain = 0;
            Domain.ForMatchingEntities<Sprite, Transform>((entity) => {
                clientInDomain++;
            });

            var currentLives = lifeController.Lives;

            //System.Console.WriteLine("Current lives: " + currentLives[(uint)Connection.Instance.User_id]);
            
            foreach(Entity icon in playerIcons)   
                icon.Delete(); 
            foreach (Entity playerName in playerNames)
                playerName.Delete();
            foreach (Entity score in playerScores)
                score.Delete();

            playerScores.Clear();
            playerIcons.Clear();
            playerNames.Clear();

            var clients = GameInfo.Current.Clients;
            foreach (var client in clients)
            {
                var lives = lifeController.GetClientLives(client.Id);

                playerIcons.Add(EntityUtility.CreateIcon(Domain, (int)client.Id)); 
                playerNames.Add(EntityUtility.CreateMessage(Domain, client.Name, 0, 0, 16));
                playerScores.Add(EntityUtility.CreateMessage(Domain, lives.ToString(), 0, 0, 16));

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

            // Display client scores
            {
                int placement_Y = -245;
               
                foreach (Entity score in playerScores)
                {
                    var transform = score.GetComponent<Transform>();
                    transform.Position.X = -625;
                    transform.Position.Y = placement_Y;
                    placement_Y += 30;
                }
            }
        }
    }


}
