using Newtonsoft.Json;
using static CaptainCombat.Common.Domain;

namespace CaptainCombat.Common.Components {

    public class Sprite : Component {

        public bool   Enabled { get; set; } = true;
        public string TextureTag { get; set; }
        public double Width { get; set; }
        public double Height { get; set; }
        public Color Color { get; set; }

        [JsonIgnore]
        public Texture Texture { get => Asset.GetAsset<Texture>(TextureTag); }

        public Sprite() {}

        public Sprite(Texture texture, double width, double height, Color? color = null) {
            TextureTag = texture.Tag;
            Width = width;
            Height = height;
            Color = color ?? Color.White;
        }

        public override void OnUpdate(Component component) {
            var c = (Sprite)component;
            Enabled = c.Enabled;
            TextureTag = c.TextureTag;
            Width = c.Width;
            Height = c.Height;
            Color = c.Color;
        }
    }

}
