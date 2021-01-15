using CaptainCombat.Source.NetworkEvent;
using CaptainCombat.Source.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaptainCombat.Source.Events {
    

    public class ProjectileCollisionEvent : Event {

        public GlobalId ProjectileId { get; set; }
        public GlobalId TargetId { get; set; }

        public ProjectileCollisionEvent() { }

        public ProjectileCollisionEvent(uint receiver, GlobalId projectileId, GlobalId targetId) : base(receiver) {
            ProjectileId = projectileId;
            TargetId = targetId;
        }

        
    }

}
