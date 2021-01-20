
using CaptainCombat.Client.Scenes;
using CaptainCombat.Common;
using CaptainCombat.Common.Components;
using static CaptainCombat.Common.Domain;

namespace CaptainCombat.Client.Layers {

    class Finish : Layer {

        private Domain domain;

        private Camera camera;

        private OnExitCallback exitCallback;
        public delegate void OnExitCallback();

        public object LastPressedKeys { get; private set; }

        public Finish(Player winner, OnExitCallback exitCallback) {
            this.exitCallback = exitCallback;
            domain = new Domain();
            camera = new Camera(domain);

            { // Fade
                var entity = new Entity(domain);
                entity.AddComponent(new Transform());
                entity.AddComponent(new Sprite(Assets.Textures.SQUARE, 1280, 720, new Color(0.0f, 0.0f, 0.0f, 0.5f)));
            }

            // Chat layout
            var scroll = new Entity(domain);
            scroll.AddComponent(new Transform(0, 0));
            scroll.AddComponent(new Sprite(Assets.Textures.Chat, 300, 250));

            EntityUtility.CreateMessage(domain, $"{winner.Name} wins!", 0, -40, 30);
            EntityUtility.CreateMessage(domain, $"Press 'enter' to return to lobby", 0, 10, 14);

            domain.Clean();
        }


        public override void draw(Microsoft.Xna.Framework.GameTime gameTime) {
            Renderer.RenderSprites(domain, camera);
            Renderer.RenderText(domain, camera);
        }


        public override bool OnKeyDown(Microsoft.Xna.Framework.Input.Keys key)
        {
            if (key == Microsoft.Xna.Framework.Input.Keys.Enter)
                exitCallback();
            return true;
        }
    }

}
