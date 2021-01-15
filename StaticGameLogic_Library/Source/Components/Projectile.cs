using static StaticGameLogic_Library.Source.ECS.Domain;

namespace StaticGameLogic_Library.Source.Components {
    
    public class Projectile : Component {

        public bool HasHit { get; set; } = false;

        public Projectile() { }

        public override void OnUpdate(Component component) {
            var c = (Projectile)component;
            HasHit = c.HasHit;
        }
    }

}
