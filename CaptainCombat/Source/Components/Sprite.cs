﻿using Newtonsoft.Json.Linq;
using static ECS.Domain;

namespace CaptainCombat.Source.Components {

    public class Sprite : Component {

        public string TextureTag { get; }
        public double Width { get; set; }
        public double Height { get; set; }

        public Texture Texture { get => Asset.GetAsset<Texture>(TextureTag); }

        public Sprite(Texture texture, double width, double height) {
            TextureTag = texture.Tag;
            Width = width;
            Height = height;
        }
       

        public override object getData()
        {
            throw new System.NotImplementedException();
        }

        public override void update(JObject json)
        {
            throw new System.NotImplementedException();
        }
    }

}
