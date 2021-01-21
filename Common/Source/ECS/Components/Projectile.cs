using static CaptainCombat.Common.Domain;

namespace CaptainCombat.Common.Components {
    
    public class Projectile : Component {

        public bool HasHit { get; set; } = false;

        public Projectile() { }

        public override void OnUpdate(Component component) {
            var c = (Projectile)component;
            HasHit = c.HasHit;
        }
    }

}
