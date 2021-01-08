
using System;
using System.Collections.Generic;

namespace CaptainCombat.Source {

    public abstract class Asset {

        private readonly static Dictionary<string, Asset> allAssets = new Dictionary<string, Asset>();

        public AssetCollection Collection { get; }
        public string Tag { get; }
        public bool Loaded { get; private set; }



        public Asset(string tag, AssetCollection collection = null) {
            if (allAssets.ContainsKey(tag))
                throw new ArgumentException($"Asset tag '{tag}' has already been used");
            allAssets.Add(tag, this);
            Tag = tag;

            if (collection != null) {
                Collection = collection;
                Collection.Add(this);
            }
        }


        public static T GetAsset<T>(string tag) where T : Asset {
            return (T) allAssets[tag];
        }

        public static Asset GetAsset(string tag ) {
            return allAssets[tag];
        }

        public abstract void Load();



        public class AssetCollection {

            private Dictionary<string, Asset> assets = new Dictionary<string, Asset>();

            public void Add(Asset asset) {
                assets[asset.Tag] = asset;
            }

            public void Load() {
                foreach (var pair in assets) {
                    var asset = pair.Value;
                    asset.Load();
                    asset.Loaded = true;
                }
            }
        }

    }

}
