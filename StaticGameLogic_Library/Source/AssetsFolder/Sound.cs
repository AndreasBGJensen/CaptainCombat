
using StaticGameLogic_Library.Source;

namespace CaptainCombat.Source
{

    public class Sound : Asset
    {

        public string Url { get; }

        public Sound(AssetCollection collection, string tag, string url) : base(tag, collection)
        {
            Url = url;
        }
    }
}
