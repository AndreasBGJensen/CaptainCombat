using static CaptainCombat.Common.Domain;

namespace CaptainCombat.Common.Components
{

    public class Text : Component
    {
        public string Message { get; set; }
        public string FontTag { get; set; }

        public Font FONT { get => Asset.GetAsset<Font>(FontTag); }

        public Text() { }

        public Text(Font font, string message)
        {
            FontTag = font.Tag;
            Message = message;
        }

        public override void OnUpdate(Component component) {
            var c = (Text)component;
            FontTag = c.FontTag;
            Message = c.Message;
        }
    }

}
