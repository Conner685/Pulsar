using UnityEngine;

namespace Pulsar.Ship
{
    public class ThrusterTile : Tile
    {
        public override void OnAttached(ShipGrid grid)
        {
            base.OnAttached(grid);
            // register with ship thruster groups
        }

        public override void OnDetached()
        {
            base.OnDetached();
            // unregister from ship thruster groups
        }
    }
}
