
using CaptainCombat.Source.Components;
using Microsoft.Xna.Framework.Graphics;

namespace CaptainCombat.Source {
    
    public static class Assets {


        public static class Collections {
            public static readonly Asset.AssetCollection GLOBAL = new Asset.AssetCollection();

        }




        public static class Textures {

            public static readonly Texture SHIP = new Texture(Collections.GLOBAL, "ship", "Sprites/ShipRed", rotation: 180 );

            public static readonly Texture SHIP_BLUE = new Texture(Collections.GLOBAL, "ship_blue", "Sprites/ShipBlue");

            public static readonly Texture ROCK = new Texture(Collections.GLOBAL, "rock1", "Sprites/Rock1");


            public static readonly Texture LINE_SQUARE = new Texture(Collections.GLOBAL, "line_square", "Misc/LineSquare");
            public static readonly Texture LINE_CIRCLE = new Texture(Collections.GLOBAL, "line_circle", "Misc/LineCircle");
        }



        public static class Colliders {
            public static readonly ColliderType SHIP = new ColliderType("ship");
            public static readonly ColliderType TEST = new ColliderType("test");
            public static readonly ColliderType ROCK = new ColliderType("rock");
        }


        public static class Misc {
            
        }

    }



}
