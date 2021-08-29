
namespace CaptainCombat.Common {

    public class Font : Asset
    {

        public string Url { get; }
        
        public Font(AssetCollection collection, string tag, string url) : base(tag, collection)
        {
            Url = url;
        }


    }
}
