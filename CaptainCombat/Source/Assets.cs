
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

            public static readonly Texture Loader = new Texture(Collections.GLOBAL, "Loader", "Sprites/loading3_trans");

            public static readonly Texture Background = new Texture(Collections.GLOBAL, "Background", "Sprites/loadingBackGround");
            public static readonly Texture Menu = new Texture(Collections.GLOBAL, "Menu", "Sprites/scroll_trans");
        }

        public static class Fonts
        {
            public static readonly Font PIRATE_FONT_12 = new Font(Collections.GLOBAL, "PIRATE_FONT_12", "fonts/Font_12");
            public static readonly Font PIRATE_FONT_14 = new Font(Collections.GLOBAL, "PIRATE_FONT_14", "fonts/Font_14");
            public static readonly Font PIRATE_FONT_16 = new Font(Collections.GLOBAL, "PIRATE_FONT_16", "fonts/Font_16");
            public static readonly Font PIRATE_FONT_18 = new Font(Collections.GLOBAL, "PIRATE_FONT_18", "fonts/Font_18");
            public static readonly Font PIRATE_FONT_20 = new Font(Collections.GLOBAL, "PIRATE_FONT_20", "fonts/Font_20");
        }
        
        
    }

}
