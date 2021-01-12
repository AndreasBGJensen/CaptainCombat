
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

            public static readonly Texture Chat = new Texture(Collections.GLOBAL, "Chat", "Sprites/chat_trans");

            public static readonly Texture Loader = new Texture(Collections.GLOBAL, "Loader", "Sprites/loading_trans");

            public static readonly Texture Loader2 = new Texture(Collections.GLOBAL, "Loader2", "Sprites/loader2");
        }

        public static class Fonts
        {
            public static readonly Font PIRATE_FONT = new Font(Collections.GLOBAL, "PIRATE_FONT", "fonts/Piratefont");
        }
        
        
    }

}
