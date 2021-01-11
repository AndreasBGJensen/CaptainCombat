
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
            return (T)allAssets[tag];
        }


        public static Asset GetAsset(string tag) {
            return allAssets[tag];
        }


        /// <summary>
        /// Collection for Assets, which allows for batch loading of them
        /// </summary>
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



        // - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -
        // Asset loading

        /// <summary>
        /// Returns the native asset object for this Asset.
        /// The asset loader for this type of Asset must must.
        /// </summary>
        /// <typeparam name="N">Type of the native asset</typeparam>
        public N GetNative<N>(){
            return (N) handlers[GetType()].Get(this);
        }

        public delegate N Loader<T, N>(T asset) where T : Asset;


        /// <summary>
        /// Sets the callback which is to load the native asset for a given Asset type
        /// </summary>
        /// <typeparam name="T">Type of the Asset</typeparam>
        /// <typeparam name="N">Type of the native ASset</typeparam>
        /// <param name="loader">Delegate to load the native asset</param>
        public static void SetLoader<T, N>(Asset.Loader<T, N> loader) where T : Asset {
            var assetType = typeof(T);
            if (handlers.ContainsKey(typeof(T)))
                throw new InvalidOperationException($"Asset type '{assetType.Name}' already has assigned a loader");

            handlers[assetType] = new Handler<T, N>(loader);
        }


        

        /// <summary>
        /// Loads this Asset by using the Loader assigned to this Asset's type
        /// </summary>
        public void Load() {
            if (Loaded) return;

            var type = GetType();

            if (!handlers.ContainsKey(type))
                throw new NullReferenceException($"No AssetLoader found for asset type '{type.Name}'");

            handlers[type].Load(this);
        }


        // List of all Asset handlers
        private static Dictionary<Type, IHandler> handlers = new Dictionary<Type, IHandler>();

        // Interface for Handler, so that they may be stored in the same
        // list while still being generic types
        private interface IHandler {
            void Load(Asset asset);
            object Get(Asset asset);
        }
              
        private class Handler<T, N> : IHandler where T : Asset {
            
            // Map of Assets to their native asset counterparts
            private Dictionary<T, N> nativeAssetMap = new Dictionary<T, N>();
            private Loader<T,N> loader;

            public Handler(Loader<T,N> loader) {
                handlers[typeof(T)] = this;
                this.loader = loader;
            }           

            // Loads the Assset (of type T)
            public void Load(Asset asset) {
                if (asset.GetType() != typeof(T))
                    throw new ArgumentException($"AssetLoader<{typeof(T).Name}> cannot load assets of type '{asset.GetType().Name}'");

                if (nativeAssetMap.ContainsKey((T)asset))
                    return;

                N nativeAsset = loader((T)asset);

                if (nativeAsset != null)
                    nativeAssetMap[(T)asset] = nativeAsset;
            }

            // Retrieves the native Asset for the given ASset
            public object Get(Asset asset) {
                if (!nativeAssetMap.ContainsKey((T)asset))
                    throw new InvalidOperationException($"Asset '{asset.Tag}' has not been loaded");
                return nativeAssetMap[(T)asset];
            }

        }

    }

}
