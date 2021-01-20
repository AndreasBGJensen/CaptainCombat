using CaptainCombat.Client.protocols;
using CaptainCombat.Common;
using CaptainCombat.Common.Components;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System.Linq;
using System.Threading.Tasks;
using static CaptainCombat.Common.Domain;


namespace CaptainCombat.Client.Scenes
{
    class Intro : State
    {

        private bool connecting = false;
        private Keys[] LastPressedKeys;
        private Game game;
        private Domain domain;
        private Camera camera;

        private Loader<bool> loader;

        public Intro(Game game)
        {
            this.game = game;

            domain = new Domain();
            camera = new Camera(domain);


            // Background entity
            var backGround = new Entity(domain);
            backGround.AddComponent(new Transform());
            backGround.AddComponent(new Sprite(Assets.Textures.Background, 1280, 720));

            // Scroll entity
            var menu = new Entity(domain);
            menu.AddComponent(new Transform());
            menu.AddComponent(new Sprite(Assets.Textures.Menu, 600, 600));


            EntityUtility.CreateMessage(domain, "Captain Combat", 0, -75, 40);
            EntityUtility.CreateMessage(domain, "Press 'enter' to connect", 0, 50, 20);
        }


        public override void update(GameTime gameTime)
        {
            domain.Clean();
            loader?.update(gameTime);
        }


        public override void draw(GameTime gameTime)
        {
            Renderer.RenderSprites(domain, camera);
            Renderer.RenderText(domain, camera);
            loader?.draw(gameTime);
        }


        public override void OnKeyDown(Keys key)
        {
            if (key == Keys.Enter)
                Connect();
        }

        private void Connect()
        {
            if (connecting) return;
            connecting = true;

            loader = new Loader<bool>("Connecting to server",
                () => {
                    ClientProtocol.Connect();
                    return true;
                },
                (success) => _context.TransitionTo(new JoinState(game))
            );
        }
    }
}
