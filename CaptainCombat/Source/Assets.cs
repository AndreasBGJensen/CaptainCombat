
namespace CaptainCombat.Source {
    
    public static class Assets {


        public static class Collections {
            public static readonly Asset.AssetCollection GLOBAL = new Asset.AssetCollection();

        }




        public static class Textures {

            public static readonly Texture SHIP = new Texture(Collections.GLOBAL, "ship", "Sprites/ShipRed");


            public static readonly Texture SHIP_BLUE = new Texture(Collections.GLOBAL, "ship_blue", "Sprites/ShipBlue");
        }
        




    }

}
