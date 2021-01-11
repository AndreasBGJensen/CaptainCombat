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


            /*
            Upload upload = new Upload();
            Thread uploadThread = new Thread(new ThreadStart(upload.RunProtocol));
            uploadThread.Start();

            DownLoad download = new DownLoad();
            Thread downloadThread = new Thread(new ThreadStart(download.RunProtocol));
            downloadThread.Start();
            */
            
        }


        protected override void Initialize() {
            base.Initialize();
        }


        protected override void LoadContent() {
            Renderer.Initialize(GraphicsDevice);

            // Set statemanager 
            stateManager = new StateManager(); 
            

            // Set the window size
            Graphics.PreferredBackBufferWidth = 1280;
            Graphics.PreferredBackBufferHeight = 720;
            Graphics.ApplyChanges();


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


            //DomainState.Instance.Upload = JsonBuilder.createJsonString();

            stateManager._state.update(gameTime); 

            base.Update(gameTime);
        }


        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            stateManager._state.draw(gameTime); 
            base.Draw(gameTime);
        }
    }
}
