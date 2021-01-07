


using CaptainCombat.Source;
using static ECS.Domain;

namespace CaptainCombat.Source.Components {

    public class Sprite : Component {

        public string TextureTag { get; }
        public double Width { get; set; }
        public double Height { get; set; }


        public Texture Texture { get => Asset.GetAsset<Texture>(TextureTag); }

        public Sprite(Entity entity, Texture texture, double width, double height) : base(entity) {
            TextureTag = texture.Tag;
            Width = width;
            Height = height;
        }




        
        
    }

}
