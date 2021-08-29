using CaptainCombat.Common;

namespace CaptainCombat.Client
{

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