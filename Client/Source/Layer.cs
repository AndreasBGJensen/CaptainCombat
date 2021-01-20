using Microsoft.Xna.Framework;

namespace CaptainCombat.Client
{
    abstract class Layer
    {
        public virtual void init() { }

        public virtual void update(GameTime gameTime) { }

        public virtual void draw(GameTime gameTime) { }

    }
}
