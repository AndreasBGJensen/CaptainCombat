using CaptainCombat.Common.Components;
using CaptainCombat.Common;
using static CaptainCombat.Common.Domain;

namespace CaptainCombat.Client
{

    class Loader : Layer
    {

        private Camera camera;
        private Domain domain = new Domain();

        public Loader(string loadingText)
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
