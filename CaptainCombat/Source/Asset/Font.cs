using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaptainCombat.Source
{
    public class Font : Asset
    {

        public string Url { get; }
        
        public Font(AssetCollection collection, string tag, string url) : base(tag, collection)
        {
            Url = url;
        }


    }
}
