using ECS;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaptainCombat.Source
{
    abstract class Layer
    {
        public abstract void init();

        public abstract void update(GameTime gameTime);

        public abstract void draw(GameTime gameTime);

    }
}
