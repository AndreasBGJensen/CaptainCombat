using CaptainCombat.Client.Scenes;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;

using SpriteFont = Microsoft.Xna.Framework.Graphics.SpriteFont;
using MGTexture = Microsoft.Xna.Framework.Graphics.Texture2D;
using CaptainCombat.Common;

namespace CaptainCombat.Client
{


    public class GameController : Game {

        public static GameController Game { get; private set; }
        public static GraphicsDeviceManager Graphics { get; private set; }

        public static CollisionController collisionController = new CollisionController();
        private StateManager Manager { get; set; }

        
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

            // Set state. 
            Manager = new StateManager(new Intro(this));

            // Set asset loaders
            Asset.SetLoader<Texture, MGTexture>((texture) => {
                return Content.Load<MGTexture>(texture.Url);
            });

            Asset.SetLoader<Font, SpriteFont>((font) => {
                return Content.Load<SpriteFont>(font.Url);
            });

            Asset.SetLoader<Track, Song>((number) => {
                return Content.Load<Song>(number.Url);
            });

            Asset.SetLoader<Sound, SoundEffect>((sound) => {
                return Content.Load<SoundEffect>(sound.Url);
            });


            // Loading global asset collection
            Assets.Collections.GLOBAL.Load();

            // Load Song 
            Track track = Assets.Music.PirateSong;
            var song = track.GetNative<Song>();

            // Load Song 
            Sound soundEffect = Assets.Sounds.KanonSound; 
            var effect = track.GetNative<Song>();

        }

        protected override void Update(GameTime gameTime) {
            Manager._state.update(gameTime); 
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            //Graphics.GraphicsDevice.Clear(ClearOptions.Target, Microsoft.Xna.Framework.Color.CornflowerBlue, 0, 0);
            Graphics.GraphicsDevice.Clear(Microsoft.Xna.Framework.Color.CornflowerBlue);
            Manager._state.draw(gameTime); 
            base.Draw(gameTime);
        }
    }
}
