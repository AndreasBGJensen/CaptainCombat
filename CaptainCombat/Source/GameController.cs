using CaptainCombat.dotSpaceConnection;
using CaptainCombat.Source;
using CaptainCombat.Source.Components;
using ECS;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using static ECS.Domain;

namespace CaptainCombat
{
    public class GameController : Game
    {

        public static GameController Game { get; private set; }
        public static GraphicsDeviceManager Graphics { get; private set;  }

        private static Domain domain = new Domain();

        public static Camera camera;

        public GameController()
        {
            Game = this;
            Graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {

            // Create test entities
            {
                var entity = new Entity(domain);
                var transform = entity.AddComponent<Transform>();
            }


            {
                var entity = new Entity(domain);
                var transform = entity.AddComponent<Transform>();
                transform.X = 100; transform.Y = 50;
                entity.AddComponent<Sprite>(Assets.Textures.SHIP_BLUE, 100, 100);
            }

            var entity3 = new Entity(domain);
            entity3.AddComponent<Transform>();

            domain.Clean();

            entity3.AddComponent<Sprite>(Assets.Textures.SHIP_BLUE, 100, 100);

            camera = new Camera(domain);

            base.Initialize();
        }

        protected override void LoadContent()
        {
            Renderer.Initialize(GraphicsDevice);


            // Loading global asset collection
            Assets.Collections.GLOBAL.Load();
        
        }

        protected override void Update(GameTime gameTime)
        {

            //if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
            //Exit();

            if (Keyboard.GetState().IsKeyDown(Keys.C))
                domain.Clean();

            if (Keyboard.GetState().IsKeyDown(Keys.A))
                camera.Rotation += 100 * gameTime.ElapsedGameTime.TotalSeconds;

            if (Keyboard.GetState().IsKeyDown(Keys.Up))
                camera.Y += 200 * gameTime.ElapsedGameTime.TotalSeconds;

            if (Keyboard.GetState().IsKeyDown(Keys.Down))
                camera.Y -= 200 * gameTime.ElapsedGameTime.TotalSeconds;

            if (Keyboard.GetState().IsKeyDown(Keys.Right))
                camera.X += 200 * gameTime.ElapsedGameTime.TotalSeconds;

            if (Keyboard.GetState().IsKeyDown(Keys.Left))
                camera.X -= 200 * gameTime.ElapsedGameTime.TotalSeconds;


            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {

            System.Console.WriteLine("Frame: " + gameTime.ElapsedGameTime.TotalMilliseconds);

            GraphicsDevice.Clear(Color.CornflowerBlue);

            Renderer.RenderSprites(domain, camera);

            base.Draw(gameTime);
        }
    }
}
