using CaptainCombat.json;
using CaptainCombat.network;
using CaptainCombat.singletons;
using CaptainCombat.Source;
using CaptainCombat.Source.Components;
using CaptainCombat.Source.Events;
using CaptainCombat.Source.NetworkEvent;
using CaptainCombat.Source.Utility;
using ECS;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Threading;
using static ECS.Domain;


using MGTexture = Microsoft.Xna.Framework.Graphics.Texture2D;


namespace CaptainCombat {

    public class GameController : Game {

        public static GameController Game { get; private set; }
        public static GraphicsDeviceManager Graphics { get; private set; }

        private static Domain domain = new Domain();

        public static Camera camera;

        public static Entity ship;

        public static CollisionController collisionController = new CollisionController();

        
        public GameController() {
            Game = this;
            Graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;

            DomainState.Instance.Domain = domain;

            
            Upload upload = new Upload();
            Thread uploadThread = new Thread(new ThreadStart(upload.RunProtocol));
            uploadThread.Start();

            DownLoad download = new DownLoad();
            Thread downloadThread = new Thread(new ThreadStart(download.RunProtocol));
            downloadThread.Start();

            

            EventController.Start();
        }


        protected override void Initialize() {
            base.Initialize();
        }


        protected override void LoadContent() {
            Renderer.Initialize(GraphicsDevice);

            // Create some test rocks
            EntityUtility.CreateRock(domain, 150, 100, 0.7, 120);
            EntityUtility.CreateRock(domain, 400, -200, 1.0, 40);
            EntityUtility.CreateRock(domain, 50, 400, 1.5, 45);
            EntityUtility.CreateRock(domain, -300, 75, 1.4, 170);
            EntityUtility.CreateRock(domain, -100, -200, 1.2, 30);

            // Create ship
            {
                ship = new Entity(domain);
                ship.AddComponent(new Transform());
                ship.AddComponent(new Sprite(Assets.Textures.SHIP, 66, 113));
                
                var move = ship.AddComponent(new Move());
                move.Resistance = 0.25;
                move.RotationResistance = 0.75;
                move.ForwardVelocity = true;

                //ship.AddComponent(new CircleCollider(Assets.ColliderTypes.SHIP, 20));
                var collider = ship.AddComponent(new BoxCollider());
                collider.Width = 40;
                collider.Height = 80;
                collider.Tag = Assets.ColliderTags.SHIP;
            }

            camera = new Camera(domain);

            // Set the window size
            Graphics.PreferredBackBufferWidth = 1280;
            Graphics.PreferredBackBufferHeight = 720;
            Graphics.ApplyChanges();

            // Set asset loaders
            Asset.SetLoader<Texture, MGTexture>((texture) => {
                return Content.Load<MGTexture>(texture.Url);
            });

            // Loading global asset collection
            Assets.Collections.GLOBAL.Load();


            EventController.AddListener<ProjectileCollisionEvent>((e) => {
                Console.WriteLine($"Collision event from Client {e.Sender}");

                var projectile = domain.GetEntity(e.ProjectileId);
                if (projectile == null) return false;

                Collider collider = projectile.GetComponent<BoxCollider>();
                if (collider == null) collider = projectile.GetComponent<CircleCollider>();

                if (!collider.Enabled) return false;

                collider.Enabled = false;
                projectile.Delete();
                EventController.Send(new ProjectileEffectEvent(e.Sender, e.TargetId, 100));

                return true;
            });

            EventController.AddListener<ProjectileEffectEvent>((e) => {
                Console.WriteLine($"Damage event from client {e.Sender}: {e.Damage} damage on Entity {e.TargetId.objectId}");
                return true;
            });


            collisionController.AddListener(Assets.ColliderTags.ROCK, Assets.ColliderTags.PROJECTILE, (rock, projectile) => {
                if (projectile.IsLocal) projectile.Delete();
                return true;
            });

            collisionController.AddListener(Assets.ColliderTags.SHIP, Assets.ColliderTags.PROJECTILE, (ship, projectile) => {
                // ship.IsLocal basically checks if its the local players own ship
                if (ship.IsLocal && !projectile.IsLocal) {
                    Collider collider = projectile.GetComponent<BoxCollider>();
                    if (collider == null) collider = projectile.GetComponent<CircleCollider>();
                    collider.Enabled = false;
                    projectile.GetComponent<Sprite>().Enabled = false;
                    projectile.GetComponent<Move>().Enabled = false;
                    EventController.Send(new ProjectileCollisionEvent(projectile.ClientId, projectile.Id, ship.Id));
                    Console.WriteLine("Sent collision event");
                    return true;
                }
                return false;
            });
        }


        
        private const double FIRE_COOLDOWN = 0.25;
        private static double fireCooldownCurrent = 0.0;

        protected override void Update(GameTime gameTime) {

            if (DomainState.Instance.Download != null) {
                domain.update(DomainState.Instance.Download);
                DomainState.Instance.Download = null;
            }

            EventController.HandleEvents();

            double seconds = gameTime.ElapsedGameTime.TotalSeconds;
            bool shiftPressed = Keyboard.GetState().IsKeyDown(Keys.LeftShift) | Keyboard.GetState().IsKeyDown(Keys.RightShift);

            if (fireCooldownCurrent > 0)
                fireCooldownCurrent -= seconds;

            if( Keyboard.GetState().IsKeyDown(Keys.E)) {
                if( fireCooldownCurrent <= 0) {
                    EntityUtility.FireCannonBall(ship);
                    fireCooldownCurrent = FIRE_COOLDOWN;
                }
            }

            if (Keyboard.GetState().IsKeyDown(Keys.R))
                camera.Rotation += 100 * seconds * (shiftPressed ? -1 : 1);

            if (Keyboard.GetState().IsKeyDown(Keys.Z))
                camera.Zoom += 1.0 * seconds * (shiftPressed ? -1 : 1);

            // Update ship movement
            {
                var move = ship.GetComponent<Move>();

                if (Keyboard.GetState().IsKeyDown(Keys.Space)) {
                    move.Acceleration = move.Acceleration.WithMagnitude(30);
                } else {
                    move.Acceleration = new Vector2(0, 0);
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
                camera.X = transform.X;
                camera.Y = transform.Y;
            }

            Movement.Update(domain, seconds);
            collisionController.CheckCollisions(domain);

            domain.Clean();
            
            DomainState.Instance.Upload = JsonBuilder.createJsonString();

            base.Update(gameTime);
        }


        protected override void Draw(GameTime gameTime)
        {


            GraphicsDevice.Clear(Color.CornflowerBlue);

            Renderer.RenderSprites(domain, camera);
            Renderer.RenderColliders(domain, camera);

            base.Draw(gameTime);
        }
    }
}
