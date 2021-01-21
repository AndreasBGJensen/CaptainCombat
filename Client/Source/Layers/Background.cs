using CaptainCombat.Client.protocols;
using CaptainCombat.Client.Scenes;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using System.Linq;
using CaptainCombat.Common.Singletons;
using CaptainCombat.Common.Components;
using CaptainCombat.Common;
using CaptainCombat.Common.JsonBuilder;
using static CaptainCombat.Common.Domain;
using CaptainCombat.Client.NetworkEvent;
using System;
using System.Threading.Tasks;
using CaptainCombat.Client.Layers;
using CaptainCombat.Source.Events;
using CaptainCombat.network;

namespace CaptainCombat.Client.GameLayers
{

    class Background : Layer
    {

        public bool UpdateEnabled { get; set; } = true;
        public bool DrawEnabled { get; set; } = true;

        private Domain Domain = new Domain();
        private Camera Camera;

        private bool DisableKeyboard = false;
        private bool playMusic = true;
        private Keys[] LastPressedKeys = new Keys[5];

        private bool gameStarted = false;

        private Entity ship;

        private bool renderColliders = false;

        private const double FIRE_COOLDOWN = 0.25;
        private static double fireCooldownCurrent = 0.0;

        private LifeController lifeController;
        private GameDataUploader gameDataUploader = new GameDataUploader();
        private GameDataDownloader gameDataDownloader = new GameDataDownloader();

        private CollisionController collisionController = new CollisionController();
        private EventController eventController;


        public Background(LifeController lifeController, EventController eventController)
        {
            Camera = new Camera(Domain);
            this.lifeController = lifeController;
            this.eventController = eventController;
            DomainState.Instance.Domain = Domain;
        }

        public void Terminate()
        {
            gameDataUploader.Stop();
            gameDataDownloader.Stop();
        }


        public override void init()
        {

            collisionController.AddListener(Assets.ColliderTags.SHIP, Assets.ColliderTags.ROCK, (ship, rock) => {
                // Only detect this with own collision
                if (ship != this.ship) return false;

                var health = ship.GetComponent<ShipHealth>();
                if (health.Current <= 0) return false;

                health.Current = 0;
                ship.GetComponent<Sprite>().Enabled = false;                
                ship.GetComponent<BoxCollider>().Enabled = false;
                ship.GetComponent<Move>().Enabled = false;


                var numLives = lifeController.DecrementLocalLives();
                if (numLives > 0)
                    Respawn();

                return true;
            });


            eventController.AddListener<ProjectileCollisionEvent>((e) => {
                Console.WriteLine($"Collision event from Client {e.Sender}");

                var projectile = Domain.GetEntity(e.ProjectileId);
                if (projectile == null) return false;

                var projectileComp = projectile.GetComponent<Projectile>();
                if (projectileComp.HasHit) return false;
                projectileComp.HasHit = true;

                Collider collider = projectile.GetComponent<BoxCollider>();
                if (collider == null) collider = projectile.GetComponent<CircleCollider>();
                collider.Enabled = false;

                projectile.Delete();
                eventController.Send(new ProjectileEffectEvent(e.TargetId.clientId, e.TargetId, 34));

                return true;
            });


            eventController.AddListener<ProjectileEffectEvent>((e) => {
                Console.WriteLine($"Damage event from client {e.Sender}: {e.Damage} damage on Entity {e.TargetId.objectId}");
                var health = ship.GetComponent<ShipHealth>();
                if (health.Current <= 0) return false;

                health.Current -= e.Damage;

                if (health.Current > 0) return true;

                ship.GetComponent<Sprite>().Enabled = false;
                ship.GetComponent<BoxCollider>().Enabled = false;
                ship.GetComponent<Move>().Enabled = false;

                var numLives = lifeController.DecrementLocalLives();
                if (numLives > 0)
                    Respawn();

                return true;
            });


            collisionController.AddListener(Assets.ColliderTags.ROCK, Assets.ColliderTags.PROJECTILE, (rock, projectile) => {
                if (projectile.IsLocal) projectile.Delete();
                return true;
            });


            collisionController.AddListener(Assets.ColliderTags.SHIP, Assets.ColliderTags.PROJECTILE, (ship, projectile) => {
                if ((ship.IsLocal && !projectile.IsLocal) || (!ship.IsLocal && projectile.IsLocal)) {
                    Collider collider = projectile.GetComponent<BoxCollider>();
                    if (collider == null) collider = projectile.GetComponent<CircleCollider>();
                    collider.Enabled = false;
                    projectile.GetComponent<Sprite>().Enabled = false;
                    projectile.GetComponent<Move>().Enabled = false;
                    eventController.Send(new ProjectileCollisionEvent(projectile.ClientId, projectile.Id, ship.Id));
                    Console.WriteLine("Sent collision event");
                    return true;
                }
                return false;
            });

            
            ship = EntityUtility.CreateShip(Domain, Connection.Instance.User_id, 1);
            SpawnShip();
        }


        public void Start() {
            gameStarted = true;
            gameDataUploader.Start();
            gameDataDownloader.Start();
        }


        public override void update(GameTime gameTime)
        {
            double seconds = gameTime.ElapsedGameTime.TotalSeconds;

            if (!UpdateEnabled) return;

            { // Update Domain to remote data
                var data = gameDataDownloader.GetData();
                if (data != null)
                    Domain.update(data);
            }

            if( gameStarted)
            {
                eventController.Flush();
            }

            Domain.Clean();
            // Clear domain

            if( gameStarted ) {

                // Handles keyboard input 
                GetKeys();

                // Update ship movement

                if (fireCooldownCurrent > 0)
                    fireCooldownCurrent -= seconds;

                if (gameStarted && !DisableKeyboard) {
                    if (Keyboard.GetState().IsKeyDown(Keys.E)) {
                        if (fireCooldownCurrent <= 0) {
                            EntityUtility.FireCannonBall(ship);
                            fireCooldownCurrent = FIRE_COOLDOWN;
                        }
                    }

                    var move = ship.GetComponent<Move>();

                    if (Keyboard.GetState().IsKeyDown(Keys.Space)) {
                        move.Acceleration = new Vector(30, 0);
                    }
                    else {
                        move.Acceleration = new Vector(0, 0);
                    }

                    if (Keyboard.GetState().IsKeyDown(Keys.Right)) {
                        move.RotationAcceleration = 270;
                    }
                    else if (Keyboard.GetState().IsKeyDown(Keys.Left)) {
                        move.RotationAcceleration = -270;
                    }
                    else {
                        move.RotationAcceleration = 0;
                    }
                }

                // Update camera to ship
                {
                    var transform = ship.GetComponent<Transform>();
                    var difference = transform.Position - Camera.Position;
                    Camera.Position += difference * 0.05;
                }
            }

            // Update movement in domain 
            Movement.Update(Domain, seconds);
            collisionController.CheckCollisions(Domain);

            Domain.Clean();
            gameDataUploader.UploadData(JsonBuilder.createJsonString());
        }
        

        

        
        public override void draw(GameTime gameTime)
        {
            if (!DrawEnabled) return;

            Renderer.RenderSprites(Domain, Camera);
            Renderer.RenderText(Domain, Camera);
            if (renderColliders)
                Renderer.RenderColliders(Domain, Camera);
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
            if (key == Keys.Tab)
            {
                DisableKeyboard = !DisableKeyboard;
            }
            
            if (key == Keys.M)
            {
                if (playMusic)
                {
                    playMusic = !playMusic; 
                    // Start song 
                    Track track = Assets.Music.PirateSong;
                    var song = track.GetNative<Song>();
                    MediaPlayer.IsRepeating = true;
                    MediaPlayer.Play(song);
                }
                else
                {
                    playMusic = !playMusic;
                    MediaPlayer.Stop();
                }
            }
            
            if(key == Keys.L)
            {
                Sound sound = Assets.Sounds.KanonSound;
                var effect = sound.GetNative<SoundEffect>();
                effect.Play(); 
            }

            if (key == Keys.C)
                renderColliders = !renderColliders;

        }



        private async void Respawn() {
            await Task.Delay((int)(Settings.RESPAWN_DELAY*1000));
            SpawnShip();
        }


        /// <summary>
        /// Spawns the Player's ship at some location (center for now)
        /// </summary>
        private void SpawnShip() {
            var health = ship.GetComponent<ShipHealth>();
            health.Current = health.Max;
            health.DeathHandled = false;    

            var transform = ship.GetComponent<Transform>();
            transform.Position = Vector.Zero;
            transform.Rotation = 0;

            var move = ship.GetComponent<Move>();
            move.Velocity = Vector.Zero;
            move.RotationVelocity = 0;
            move.Acceleration = Vector.Zero;
            move.Enabled = true;

            var sprite = ship.GetComponent<Sprite>();
            sprite.Enabled = true;
            sprite.Depth = 0.5f;

            var collider = ship.GetComponent<BoxCollider>();
            collider.Enabled = true;
        }
    }
}
