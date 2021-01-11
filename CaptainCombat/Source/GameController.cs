using CaptainCombat.json;
using CaptainCombat.network;
using CaptainCombat.singletons;
using CaptainCombat.Source;
using CaptainCombat.Source.Components;
using CaptainCombat.Source.Utility;
using ECS;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Threading;
using static ECS.Domain;

namespace CaptainCombat
{
    public class GameController : Game {

        public static GameController Game { get; private set; }
        public static GraphicsDeviceManager Graphics { get; private set; }

        private static Domain domain = new Domain();

        public static Camera camera;


        public static Entity ship;


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
            
        }


        protected override void Initialize() {
            base.Initialize();
        }


        protected override void LoadContent() {
            Renderer.Initialize(GraphicsDevice);

            int xOffset = -40;
            int yOffset = -70;

            // Create some test rocks
            EntityUtility.CreateMessage(domain, "Tortuga", 150+xOffset, 100 + yOffset, false);
            EntityUtility.CreateRock(domain, 150, 100, 0.7, 120);
            EntityUtility.CreateMessage(domain, "Port Royal", 400 + xOffset, -200 + yOffset, false);
            EntityUtility.CreateRock(domain, 400, -200, 1.0, 40);
            EntityUtility.CreateMessage(domain, "Barataria Bay", 0 + xOffset, 50 + yOffset, false);
            EntityUtility.CreateRock(domain, 0, 50, 1.2, 300);
            EntityUtility.CreateMessage(domain, "Clew Bay", -300 + xOffset, 75 + yOffset, false);
            EntityUtility.CreateRock(domain, -300, 75, 1.4, 170);
            EntityUtility.CreateMessage(domain, "New Providence", -100 + xOffset, -200 + yOffset, false);
            EntityUtility.CreateRock(domain, -100, -200, 1.2, 30);

            // Game score static 
            EntityUtility.CreateMessage(domain, "Score: 1", 0, 0, true);



            // Create ship
            {
                ship = new Entity(domain);
                ship.AddComponent(new Transform());
                ship.AddComponent(new Sprite(Assets.Textures.SHIP, 66, 113));
                
                var move = ship.AddComponent(new Move());
                move.Resistance = 0.25;
                move.RotationResistance = 0.75;
                move.ForwardVelocity = true;
            }

            camera = new Camera(domain);

            // Set the window size
            Graphics.PreferredBackBufferWidth = 1280;
            Graphics.PreferredBackBufferHeight = 720;
            Graphics.ApplyChanges();


            // Set asset loaders
            Asset.SetLoader<Source.Texture, Texture2D>((texture) => {
                return Content.Load<Texture2D>(texture.Url);
            });

            // Loading global asset collection
            Assets.Collections.GLOBAL.Load();

        }


        protected override void Update(GameTime gameTime) {

            //System.Console.WriteLine("Frame: " + gameTime.ElapsedGameTime.TotalMilliseconds);

            // Clean the Domain before each frame
            domain.Clean();

            //if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
            //Exit();

            double seconds = gameTime.ElapsedGameTime.TotalSeconds;
            bool shiftPressed = Keyboard.GetState().IsKeyDown(Keys.LeftShift) | Keyboard.GetState().IsKeyDown(Keys.RightShift);

            if (Keyboard.GetState().IsKeyDown(Keys.C))
                domain.Clean();

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

            DomainState.Instance.Upload = JsonBuilder.createJsonString();

            base.Update(gameTime);
        }


        protected override void Draw(GameTime gameTime)
        {

            GraphicsDevice.Clear(Color.CornflowerBlue);

            Renderer.RenderSprites(domain, camera);
            Renderer.RenderText(domain, camera); 

            base.Draw(gameTime);
        }
    }
}
