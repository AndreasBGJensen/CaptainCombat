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
    public class Game1 : Game
    {

        public static Game1 Game { get; private set; }
        private GraphicsDeviceManager _graphics;
        private static SpriteBatch SpriteBatch;

        private static Domain domain = new Domain();

        public Game1()
        {
            Game = this;
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {

            // Create test entities
            var entity = new Entity(domain);
            entity.AddComponent<Transform>();
            entity.AddComponent<Sprite>(Assets.Textures.SHIP, 100, 100);

            domain.Clean();

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
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            Renderer.RenderSprites(domain);

            base.Draw(gameTime);
        }
    }
}
