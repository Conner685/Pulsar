using UnityEngine;

namespace Pulsar.Ship
{
    public class CoreTile : Tile
    {
        public override void OnAttached(ShipGrid grid)
        {
            base.OnAttached(grid);
        }

        public override void TakeDamage(float amount)
        {
            currentHp -= amount;
            if (currentHp <= 0f)
            {
                ShipGrid grid = GetComponentInParent<ShipGrid>();
                if (grid != null) grid.OnCoreDestroyed();
            }
        }
    }
}
