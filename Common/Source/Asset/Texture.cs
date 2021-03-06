
namespace CaptainCombat.Common
{


    public class Texture : Asset {

        public string Url { get; } 
        public float Rotation { get; }

        public Texture(AssetCollection collection, string tag, string url, double rotation = 0) : base(tag, collection) {
            Url = url;
            Rotation = (float)rotation;
        }

    }


}
