using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static ECS.Domain;

namespace CaptainCombat.game
{
    class Transform : Component
    {
        int x = 4;
        int y = 4;


        public override Object getData()
        {
            var obj = new { x = this.x, y = this.y };
            return obj;
        }

        public override void update(JObject json)
        {
            this.x = (int)json.SelectToken("x")+1;
            this.y = (int)json.SelectToken("y")+1;
        }
    }
}
