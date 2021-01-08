using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Text;
using static ECS.Domain;

namespace PirateCombatTemp {

    class Transform : Component {

        public string s;

        public Transform(Entity e, string s) : base(e) {
            this.s = s;
        }

        public override object getData()
        {
            var obj = new { a = "aaa", b = "bbb" };
            return obj; 
        }
    }


}
