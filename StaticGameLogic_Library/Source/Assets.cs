

using CaptainCombat.Source;
using StaticGameLogic_Library.Source.Components;

namespace StaticGameLogic_Library.Source {

    public static class Assets {

        public static class Collections {
            public static readonly Asset.AssetCollection GLOBAL = new Asset.AssetCollection();
        }


        public static class Textures {
            public static readonly Texture SHIP = new Texture(Collections.GLOBAL, "ship", "Sprites/ShipRed", rotation: 180 );

            public static readonly Texture CANNON_BALL = new Texture(Collections.GLOBAL, "cannon_ball", "Sprites/CannonBall");

            public static readonly Texture LINE_SQUARE = new Texture(Collections.GLOBAL, "line_square", "Misc/LineSquare");
            public static readonly Texture LINE_CIRCLE = new Texture(Collections.GLOBAL, "line_circle", "Misc/LineCircle");

            public static readonly Texture SHIP_BLUE = new Texture(Collections.GLOBAL, "ship_blue", "Sprites/ShipBlue");
            public static readonly Texture ROCK = new Texture(Collections.GLOBAL, "rock1", "Sprites/Rock1");
            public static readonly Texture Chat = new Texture(Collections.GLOBAL, "Chat", "Sprites/scroll_trans");
            public static readonly Texture Loader = new Texture(Collections.GLOBAL, "Loader", "Sprites/loading3_trans");
            public static readonly Texture Background = new Texture(Collections.GLOBAL, "Background", "Sprites/loadingBackGround");
            public static readonly Texture Menu = new Texture(Collections.GLOBAL, "Menu", "Sprites/scroll_trans");

            // Icons 
            public static readonly Texture blue_icon = new Texture(Collections.GLOBAL, "blue_icon", "Sprites/blue_icon");
            public static readonly Texture red_icon = new Texture(Collections.GLOBAL, "red_icon", "Sprites/red_icon");
            public static readonly Texture black_icon = new Texture(Collections.GLOBAL, "black_icon", "Sprites/black_icon");
            public static readonly Texture yellow_icon = new Texture(Collections.GLOBAL, "yellow_icon", "Sprites/yellow_icon");
            public static readonly Texture green_icon = new Texture(Collections.GLOBAL, "green_icon", "Sprites/green_icon");
            public static readonly Texture white_icon = new Texture(Collections.GLOBAL, "white_icon", "Sprites/white_icon");

            // Ships 
            public static readonly Texture blue_ship_1 = new Texture(Collections.GLOBAL, "blue_ship_1", "Sprites/blue_ship_1", rotation: 180);
            public static readonly Texture blue_ship_2 = new Texture(Collections.GLOBAL, "blue_ship_2", "Sprites/blue_ship_2", rotation: 180);
            public static readonly Texture blue_ship_3 = new Texture(Collections.GLOBAL, "blue_ship_3", "Sprites/blue_ship_3", rotation: 180);
            public static readonly Texture blue_ship_4 = new Texture(Collections.GLOBAL, "blue_ship_4", "Sprites/blue_ship_4", rotation: 180);

            public static readonly Texture red_ship_1 = new Texture(Collections.GLOBAL, "red_ship_1", "Sprites/red_ship_1", rotation: 180);
            public static readonly Texture red_ship_2 = new Texture(Collections.GLOBAL, "red_ship_2", "Sprites/red_ship_2", rotation: 180);
            public static readonly Texture red_ship_3 = new Texture(Collections.GLOBAL, "red_ship_3", "Sprites/red_ship_3", rotation: 180);
            public static readonly Texture red_ship_4 = new Texture(Collections.GLOBAL, "red_ship_4", "Sprites/red_ship_4", rotation: 180);


            public static readonly Texture black_ship_1 = new Texture(Collections.GLOBAL, "black_ship_1 ", "Sprites/black_ship_1", rotation: 180);
            public static readonly Texture black_ship_2 = new Texture(Collections.GLOBAL, "black_ship_2 ", "Sprites/black_ship_2", rotation: 180);
            public static readonly Texture black_ship_3 = new Texture(Collections.GLOBAL, "black_ship_3 ", "Sprites/black_ship_3", rotation: 180);
            public static readonly Texture black_ship_4 = new Texture(Collections.GLOBAL, "black_ship_4 ", "Sprites/black_ship_4", rotation: 180);


            public static readonly Texture yellow_ship_1 = new Texture(Collections.GLOBAL, "yellow_ship_1", "Sprites/yellow_ship_1", rotation: 180);
            public static readonly Texture yellow_ship_2 = new Texture(Collections.GLOBAL, "yellow_ship_2", "Sprites/yellow_ship_2", rotation: 180);
            public static readonly Texture yellow_ship_3 = new Texture(Collections.GLOBAL, "yellow_ship_3", "Sprites/yellow_ship_3", rotation: 180);
            public static readonly Texture yellow_ship_4 = new Texture(Collections.GLOBAL, "yellow_ship_4", "Sprites/yellow_ship_4", rotation: 180);

            public static readonly Texture green_ship_1 = new Texture(Collections.GLOBAL, "green_ship_1", "Sprites/green_ship_1", rotation: 180);
            public static readonly Texture green_ship_2 = new Texture(Collections.GLOBAL, "green_ship_2", "Sprites/green_ship_2", rotation: 180);
            public static readonly Texture green_ship_3 = new Texture(Collections.GLOBAL, "green_ship_3", "Sprites/green_ship_3", rotation: 180);
            public static readonly Texture green_ship_4 = new Texture(Collections.GLOBAL, "green_ship_4", "Sprites/green_ship_4", rotation: 180);


            public static readonly Texture white_ship_1 = new Texture(Collections.GLOBAL, "white_ship_1", "Sprites/white_ship_1", rotation: 180);
            public static readonly Texture white_ship_2 = new Texture(Collections.GLOBAL, "white_ship_2", "Sprites/white_ship_2", rotation: 180);
            public static readonly Texture white_ship_3 = new Texture(Collections.GLOBAL, "white_ship_3", "Sprites/white_ship_3", rotation: 180);
            public static readonly Texture white_ship_4 = new Texture(Collections.GLOBAL, "white_ship_4", "Sprites/white_ship_4", rotation: 180);
        }


        public static class Fonts {
            public static readonly Font PIRATE_FONT_12 = new Font(Collections.GLOBAL, "PIRATE_FONT_12", "fonts/Font_12");
            public static readonly Font PIRATE_FONT_14 = new Font(Collections.GLOBAL, "PIRATE_FONT_14", "fonts/Font_14");
            public static readonly Font PIRATE_FONT_16 = new Font(Collections.GLOBAL, "PIRATE_FONT_16", "fonts/Font_16");
            public static readonly Font PIRATE_FONT_18 = new Font(Collections.GLOBAL, "PIRATE_FONT_18", "fonts/Font_18");
            public static readonly Font PIRATE_FONT_20 = new Font(Collections.GLOBAL, "PIRATE_FONT_20", "fonts/Font_20");
        }

        public static class Music
        {
            public static readonly Track PirateSong = new Track(Collections.GLOBAL, "PirateSong", "music/song");
        }

        public static class Sounds
        {
            public static readonly Sound KanonSound = new Sound(Collections.GLOBAL, "CannonSound", "sound/Cannon");
        }

        public static class ColliderTags {
            public static readonly ColliderTag PROJECTILE = new ColliderTag("projectile");
            public static readonly ColliderTag SHIP = new ColliderTag("ship");
            public static readonly ColliderTag ROCK = new ColliderTag("rock");
        }        
        
    }

}
