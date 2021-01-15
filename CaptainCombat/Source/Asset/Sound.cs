using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
