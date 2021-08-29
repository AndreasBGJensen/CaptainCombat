
using static CaptainCombat.Common.Domain;

namespace CaptainCombat.Common.Components {

    public class ShipHealth : Component {
        public double Max { get; set; } = 1.0;
        public double Current { get; set; } = 1.0;

        public bool DeathHandled { get; set; } = false;

        public override void OnUpdate(Component component) {
            var c = (ShipHealth)(component);
            Max = c.Max;
            Current = c.Current;
        }
    }
}
