using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace CaptainCombat.Client
{
    abstract class Layer
    {
        public virtual void init() { }

        public virtual void update(GameTime gameTime) { }

        public virtual void draw(GameTime gameTime) { }

        public virtual bool OnKeyDown(Keys key) {
            return false;
        }

    }
}
