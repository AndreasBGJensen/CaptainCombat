
using CaptainCombat.Client.Scenes;
using CaptainCombat.Common;
using CaptainCombat.Common.Components;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
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

            // Chat layout
            var scroll = new Entity(domain);
            scroll.AddComponent(new Transform(0, 0));
            scroll.AddComponent(new Sprite(Assets.Textures.Chat, 300, 250));

            EntityUtility.CreateMessage(domain, $"{winner.Name} wins!", -60, -10, 45);
            EntityUtility.CreateMessage(domain, $"Press 'enter' to return to lobby", -30, -10, 20);

            domain.Clean();
        }


        public override void draw(GameTime gameTime) {
            Renderer.RenderSprites(domain, camera);
            Renderer.RenderText(domain, camera);
        }

        public override void init()
        {
        }



        public override void update(GameTime gameTime)
        {
            KeyboardState kbState = Keyboard.GetState();
            
            if( kbState.IsKeyDown(Keys.Enter) )
                exitCallback();
        }
    }

}
