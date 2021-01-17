
using static CaptainCombat.Common.Domain;

namespace CaptainCombat.Common.Components
{

    public class Input : Component
    {

        public string Message { get; set; }
        public string FontTag { get; set; }

        public Font FONT { get => Asset.GetAsset<Font>(FontTag); }

        
        public Input() { }


        public Input(Font font, string message)
        {
            FontTag = font.Tag;
            Message = message;
        }


        public override void OnUpdate(Component component) {
            var c = (Input)component;
            FontTag = c.FontTag;
            Message = c.Message;
        }

    }
}
