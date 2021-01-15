using CaptainCombat.json;
using CaptainCombat.network;
using CaptainCombat.singletons;
using CaptainCombat.Source;
using CaptainCombat.Source.Components;
using CaptainCombat.Source.Event;
using CaptainCombat.Source.Scenes;
using CaptainCombat.Source.Utility;
using ECS;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using System;
using System.Threading;
using static ECS.Domain;

using MGTexture = Microsoft.Xna.Framework.Graphics.Texture2D;

namespace CaptainCombat
{
    public class GameController : Game {

        public static GameController Game { get; private set; }
        public static GraphicsDeviceManager Graphics { get; private set; }
        private StateManager Manager { get; set; }

        public GameController() {
            Game = this;
            Graphics = new GraphicsDeviceManager(this);

            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize() {
            base.Initialize();
            for (uint i = 1; i < 5; i++)
                EventController.Send(new HelloEvent($"Hello there from client {Connection.Instance.User_id}!", i));
        }


        protected override void LoadContent() {
            Renderer.Initialize(GraphicsDevice);

            // Set the window size
            Graphics.PreferredBackBufferWidth = 1280;
            Graphics.PreferredBackBufferHeight = 720;
            Graphics.ApplyChanges();

            // Set state. 
            Manager = new StateManager(new Loading(Game));

            // Set asset loaders
            Asset.SetLoader<Source.Texture, MGTexture>((texture) => {
                return Content.Load<MGTexture>(texture.Url);
            });

            Asset.SetLoader<Source.Font, SpriteFont>((font) => {
                return Content.Load<SpriteFont>(font.Url);
            });

            Asset.SetLoader<Source.Track, Song>((number) => {
                return Content.Load<Song>(number.Url);
            });

            // Loading global asset collection
            Assets.Collections.GLOBAL.Load();

            // Load Song 
            Track track = Assets.Music.PirateSong;
            var song = track.GetNative<Song>();

        }

        protected override void Update(GameTime gameTime) {
            Manager._state.update(gameTime); 
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            Manager._state.draw(gameTime); 
            base.Draw(gameTime);
        }
    }
}
