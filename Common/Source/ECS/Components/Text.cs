using static CaptainCombat.Common.Domain;

namespace CaptainCombat.Common.Components
{

    public class Text : Component
    {
        public string Message { get; set; } = "";
        public string FontTag { get; set; }
        public TextOrigin Origin { get; set; } = TextOrigin.Center;

        public Font FONT { get => Asset.GetAsset<Font>(FontTag); }

        public Text() { }

        public Text(Font font, string message, TextOrigin origin = TextOrigin.Center)
        {
            FontTag = font.Tag;
            Message = message;
            Origin = origin;
        }

        public override void OnUpdate(Component component) {
            var c = (Text)component;
            FontTag = c.FontTag;
            Message = c.Message;
            Origin = c.Origin;
        }
    }


    public enum TextOrigin
    {
        Center,
        Left,
        Right       
    }

}
