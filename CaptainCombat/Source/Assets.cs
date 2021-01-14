
using CaptainCombat.Source.Components;


namespace CaptainCombat.Source {

    public static class Assets {

        public static class Collections {
            public static readonly Asset.AssetCollection GLOBAL = new Asset.AssetCollection();

        }

        public static class Textures {
            public static readonly Texture SHIP = new Texture(Collections.GLOBAL, "ship", "Sprites/ShipRed", rotation: 180 );

            public static readonly Texture SHIP_BLUE = new Texture(Collections.GLOBAL, "ship_blue", "Sprites/ShipBlue");

            public static readonly Texture ROCK = new Texture(Collections.GLOBAL, "rock1", "Sprites/Rock1");

            public static readonly Texture CANNON_BALL = new Texture(Collections.GLOBAL, "cannon_ball", "Sprites/CannonBall");

            public static readonly Texture LINE_SQUARE = new Texture(Collections.GLOBAL, "line_square", "Misc/LineSquare");
            public static readonly Texture LINE_CIRCLE = new Texture(Collections.GLOBAL, "line_circle", "Misc/LineCircle");
        }



        public static class ColliderTypes {
            public static readonly ColliderTag PROJECTILE = new ColliderTag("projectile");
            public static readonly ColliderTag SHIP = new ColliderTag("ship");
            public static readonly ColliderTag ROCK = new ColliderTag("rock");
        }

    }
}
