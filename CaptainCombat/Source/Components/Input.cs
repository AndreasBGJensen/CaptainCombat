using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static ECS.Domain;

namespace CaptainCombat.Source.Components
{
    class Input : Component
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


        public override object getData()
        {
            var obj = new
            {
                FontTag = this.FontTag,
                Message = this.Message,
            };
            return obj;
        }

        public override void update(JObject json)
        {
            this.FontTag = (string)json.SelectToken("FontTag");
            this.Message = (string)json.SelectToken("Message");
        }
    }
}
