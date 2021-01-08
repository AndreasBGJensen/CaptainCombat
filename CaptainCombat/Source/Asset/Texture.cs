using Microsoft.Xna.Framework.Graphics;

namespace CaptainCombat.Source {

    public class Texture : Asset {

        public string Url { get; } 
        public Texture2D MGTexture { get; private set; }

        public Texture(AssetCollection collection, string tag, string url) : base(tag, collection) {
            Url = url;
        }

        public override void Load() {
            MGTexture = GameController.Game.Content.Load<Texture2D>(Url);
        }

    }

}
