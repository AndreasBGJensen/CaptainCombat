using CaptainCombat.json;
using CaptainCombat.network;
using CaptainCombat.singletons;
using CaptainCombat.Source;
using CaptainCombat.Source.Components;
using CaptainCombat.Source.Scenes;
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

        private static StateManager stateManager; 

        

        public GameController() {
            Game = this;
            Graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }


        protected override void Initialize() {
            base.Initialize();
        }


        protected override void LoadContent() {
            Renderer.Initialize(GraphicsDevice);

            // Set the window size
            Graphics.PreferredBackBufferWidth = 1280;
            Graphics.PreferredBackBufferHeight = 720;
            Graphics.ApplyChanges();

            // Needed for loading assets 
            new Loading(Game);
            new MenuState(Game);
            new GameState(Game);

            stateManager = new StateManager(new Loading(Game));


            // Set asset loaders
            Asset.SetLoader<Source.Texture, Texture2D>((texture) => {
                return Content.Load<Texture2D>(texture.Url);
            });


            Asset.SetLoader<Source.Font, SpriteFont>((font) => {
                return Content.Load<SpriteFont>(font.Url);
            });

            // Loading global asset collection
            Assets.Collections.GLOBAL.Load();

            
        }





        protected override void Update(GameTime gameTime) {
            stateManager._state.update(gameTime); 
            base.Update(gameTime);
        }


        protected override void Draw(GameTime gameTime)
        {
            stateManager._state.draw(gameTime); 
            base.Draw(gameTime);
        }
    }
}
