using Microsoft.Xna.Framework.Graphics;

namespace CaptainCombat.Source {

    public class Texture : Asset {

        public string Url { get; } 
        public Texture2D MGTexture { get; private set; }
        public float Rotation { get; }

        public Texture(AssetCollection collection, string tag, string url, double rotation = 0) : base(tag, collection) {
            Url = url;
            Rotation = (float)rotation;
        }

        public override void Load() {
            MGTexture = GameController.Game.Content.Load<Texture2D>(Url);
        }

    }

}
