using UnityEngine;

namespace Pulsar.Ship
{
    [RequireComponent(typeof(BoxCollider))]
    public class Tile : MonoBehaviour
    {
        public TileInfoSO tileInfo;
        public Vector2Int cell;
        public float currentHp;
        public int rotation; 

        private SpriteRenderer _sr;
        private BoxCollider _col;

        private Rigidbody _floatRb;
        private Color _baseColor = Color.white;

        public bool IsAttached { get; private set; }

        public virtual void Init(TileInfoSO tileInfoSo, Vector2Int gridCell, int rot = 0)
        {
            tileInfo = tileInfoSo;
            cell = gridCell;
            currentHp = tileInfoSo.hp;
            rotation = rot;

            _sr = CreateVisual(transform);
            _col = GetComponent<BoxCollider>();

            _sr.sprite = tileInfoSo.sprite;
            _col.size = new Vector3(1f, ShipUtilities.TileColliderHeight, 1f);

            transform.localPosition = ShipUtilities.GridToLocal(gridCell);
            transform.localRotation = ShipUtilities.RotateQuarterOnYAxis(rot);
        }
        
        public virtual void InitFloating(TileInfoSO tileInfoSo, Vector3 position, Vector3 velocity)
        {
            tileInfo = tileInfoSo;
            currentHp = tileInfoSo.hp;
            rotation = 0;

            _sr = CreateVisual(transform);
            _col = GetComponent<BoxCollider>();

            _sr.sprite = tileInfoSo.sprite;
            _baseColor = Color.white;
            _sr.color = _baseColor;

            _col.size = new Vector3(1f, ShipUtilities.TileColliderHeight, 1f);

            transform.position = position;
            ReleaseToFloating(velocity, Vector3.up * (Random.Range(-45f, 45f) * Mathf.Deg2Rad));
        }

        public void AttachTo(ShipGrid grid, Vector2Int gridCell, int rot)
        {
            if (_floatRb != null)
            {
                _floatRb.linearVelocity = Vector3.zero;
                _floatRb.angularVelocity = Vector3.zero;
                _floatRb.isKinematic = true;
                _floatRb.detectCollisions = false;
                Destroy(_floatRb);
                _floatRb = null;
            }

            cell = gridCell;
            rotation = rot;
            transform.SetParent(grid.transform, false);
            transform.localPosition = ShipUtilities.GridToLocal(gridCell);
            transform.localRotation = ShipUtilities.RotateQuarterOnYAxis(rot);
            _col.isTrigger = false;
            _sr.sortingOrder = 0;
            Unhighlight();
        }

        public void ReleaseToFloating(Vector3 velocity, Vector3 angularVelocity)
        {
            transform.SetParent(null, true); // preserve world position and orientation
            IsAttached = false;
            _col.isTrigger = true; // pass through ship, no physical collision
            _sr.sortingOrder = 10;
            Unhighlight();
            EnableFloatingPhysics(velocity, angularVelocity);
        }

        private void EnableFloatingPhysics(Vector3 velocity, Vector3 angularVelocity)
        {
            if (_floatRb == null) _floatRb = gameObject.AddComponent<Rigidbody>();
            ShipUtilities.Constrain(_floatRb);
            _floatRb.linearDamping = 0.05f;
            _floatRb.angularDamping = 0.1f;
            _floatRb.linearVelocity = new Vector3(velocity.x, 0f, velocity.z);
            _floatRb.angularVelocity = Vector3.up * angularVelocity.y;
            _floatRb.maxAngularVelocity = Mathf.Max(_floatRb.maxAngularVelocity, Mathf.Abs(angularVelocity.y));
        }

        public bool EdgeConnectable(int gridDir)
        {
            int localDir = (gridDir + rotation) % 4;
            return tileInfo != null && tileInfo.connectableEdges[localDir];
        }
        
        public virtual void OnAttached(ShipGrid grid)
        {
            IsAttached = true;
        }

        public virtual void OnDetached()
        {
            IsAttached = false;
        }

        public virtual void TakeDamage(float amount)
        {
            currentHp -= amount;
            if (currentHp <= 0f)
            {
                ShipGrid grid = GetComponentInParent<ShipGrid>();
                if (grid != null) grid.DestroyTile(cell);
            }
        }

        // Highlight on radial menu hover 

        public void Highlight(Color tint)
        {
            if (_sr != null) _sr.color = tint;
        }

        public void Unhighlight()
        {
            if (_sr != null) _sr.color = _baseColor;
        }

        #region FACTORY

        

        public static Tile CreateTyped(GameObject go, TileType type)
        {
            switch (type)
            {
                case TileType.Core:     return go.AddComponent<CoreTile>();
                case TileType.Chassis:  return go.AddComponent<ChassisTile>();
                case TileType.Thruster: return go.AddComponent<ThrusterTile>();
                case TileType.Weapon:   return go.AddComponent<GunTile>();
                default:                return go.AddComponent<Tile>();
            }
        }

        public static Tile SpawnFloating(TileInfoSO info, Vector3 position, Vector3 velocity)
        {
            GameObject go = new GameObject($"Tile_{info.tileName}_floating");
            Tile tile = CreateTyped(go, info.type);
            tile.InitFloating(info, position, velocity);
            return tile;
        }
        
        public static SpriteRenderer CreateVisual(Transform parent, int sortingOrder = 0)
        {
            GameObject visual = new GameObject("Visual");
            visual.transform.SetParent(parent, false);
            // Sprite +Y becomes ship-local +Z. Keep this tilt off the physics root.
            visual.transform.localRotation = Quaternion.Euler(90f, 0f, 0f);
            SpriteRenderer renderer = visual.AddComponent<SpriteRenderer>();
            renderer.sortingOrder = sortingOrder;
            return renderer;
        }
        #endregion
    }
}
