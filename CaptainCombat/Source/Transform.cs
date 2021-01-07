using System;
using System.Collections.Generic;
using System.Text;
using static ECS.Domain;

namespace PirateCombatTemp {

    class Transform : Component {

        public string s;

        public Transform(Entity e, string s) : base(e) {
            this.s = s;
        }

    }





    class TestComponent2 : Component {

        public string s;

        public TestComponent2(Entity e) : base(e) {

        }

    }
}
