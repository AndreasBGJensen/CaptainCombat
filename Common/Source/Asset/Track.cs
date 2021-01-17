
namespace CaptainCombat.Common
{

    public class Track : Asset
    {
        public string Url { get; }

        public Track(AssetCollection collection, string tag, string url) : base(tag, collection)
        {
            Url = url;
        }

    }
}
