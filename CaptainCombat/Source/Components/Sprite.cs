using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using static ECS.Domain;

namespace CaptainCombat.Source.Components {

    public class Sprite : Component {

        public bool   Enabled { get; set; } = true;
        public string TextureTag { get; set; }
        public double Width { get; set; }
        public double Height { get; set; }

        [JsonIgnore]
        public Texture Texture { get => Asset.GetAsset<Texture>(TextureTag); }

        public Sprite() {}

        public Sprite(Texture texture, double width, double height) {
            TextureTag = texture.Tag;
            Width = width;
            Height = height;
        }

        public override void OnUpdate(Component component) {
            var c = (Sprite)component;
            Enabled = c.Enabled;
            TextureTag = c.TextureTag;
            Width = c.Width;
            Height = c.Height;
        }
    }

}
