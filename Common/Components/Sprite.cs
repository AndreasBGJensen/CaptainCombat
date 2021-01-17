﻿using Newtonsoft.Json.Linq;
using StaticGameLogic_Library.Source;
using StaticGameLogic_Library.Source.Assets;
using static Source.ECS.Domain;

namespace StaticGameLogic_Library.Components {

    public class Sprite : Component {

        public string TextureTag { get; set; }
        public double Width { get; set; }
        public double Height { get; set; }

        public Texture Texture { get => Asset.GetAsset<Texture>(TextureTag); }

        public Sprite() {}

        public Sprite(Texture texture, double width, double height) {
            TextureTag = texture.Tag;
            Width = width;
            Height = height;
        }
       

        public override object getData()
        {
            var obj = new { 
                TextureTag = this.TextureTag,
                Width = this.Width,
                Height = this.Height,
            };
            return obj;
        }

        public override void update(JObject json)
        {
            this.TextureTag = (string)json.SelectToken("TextureTag");
            this.Width = (double)json.SelectToken("Width");
            this.Height = (double)json.SelectToken("Height");
        }
    }

}