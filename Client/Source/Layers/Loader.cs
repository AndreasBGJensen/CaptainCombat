using CaptainCombat.Common.Components;
using CaptainCombat.Common;
using static CaptainCombat.Common.Domain;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Input;

namespace CaptainCombat.Client
{

    /// <summary>
    /// Layer displays a loading window with a given message,
    /// while it asynchrounously runs the given loading function
    /// </summary>
    class Loader<T> : Layer
    {

        private Camera camera;
        private Domain domain = new Domain();

        public delegate T LoadCallback();
        public delegate void OnFinish(T result);

        public Loader(string loadingText, LoadCallback loadingCallback, OnFinish finishCallback)
        {
            domain = new Domain();
            camera = new Camera(domain);

            { // Fade
                var entity = new Entity(domain);
                entity.AddComponent(new Transform());
                entity.AddComponent(new Sprite(Assets.Textures.SQUARE, 1280, 720, new Color(0.0f, 0.0f, 0.0f, 0.5f)));
            }

            { // Create window 
                var entity = new Entity(domain);
                entity.AddComponent(new Transform());
                entity.AddComponent(new Sprite(Assets.Textures.Chat, 550, 400));
            }

            // Create text
            EntityUtility.CreateMessage(domain, loadingText, 0, -70, 22);
            
            { // Create loading spinner
                var entity = new Entity(domain);
                entity.AddComponent(new Transform(0, 40));
                entity.AddComponent(new Sprite(Assets.Textures.Loader, 120, 120));
                var move = entity.AddComponent(new Move());
                move.RotationVelocity = 230;
            }

            // Start loading
            Task.Factory.StartNew(async () => {
                // Just to allow the loader to at least show for short amount of time
                await Task.Delay(500);
                await Task.Run(() => {
                    var success = loadingCallback();
                    finishCallback(success);
                });
            });
        }

        public override bool OnKeyDown(Keys key)
        {
            return true;
        }


        public override void update(Microsoft.Xna.Framework.GameTime gameTime)
        {
            domain.Clean();
            double seconds = gameTime.ElapsedGameTime.TotalSeconds;
            Movement.Update(domain, seconds);
        }

        public override void draw(Microsoft.Xna.Framework.GameTime gameTime)
        {
            Renderer.RenderSprites(domain, camera);
            Renderer.RenderText(domain, camera);
        }
    }
}
