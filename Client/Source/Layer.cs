using Microsoft.Xna.Framework;

namespace CaptainCombat.Client
{
    abstract class Layer
    {
        public abstract void init();

        public abstract void update(GameTime gameTime);

        public abstract void draw(GameTime gameTime);

    }
}
