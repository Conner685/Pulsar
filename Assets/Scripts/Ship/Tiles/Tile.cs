using UnityEngine;

namespace Pulsar.Ship
{
    [RequireComponent(typeof(SpriteRenderer), typeof(BoxCollider2D))]
    public class Tile : MonoBehaviour
    {
        public TileInfoSO tileInfo;
        public Vector2Int cell;
        public float currentHp;
        public int rotation; 

        private SpriteRenderer _sr;
        private BoxCollider2D _col;

        private Rigidbody2D _floatRb;
        private Color _baseColor = Color.white;

        public bool IsAttached { get; private set; }

        public virtual void Init(TileInfoSO tileInfoSo, Vector2Int gridCell, int rot = 0)
        {
            tileInfo = tileInfoSo;
            cell = gridCell;
            currentHp = tileInfoSo.hp;
            rotation = rot;

            _sr = GetComponent<SpriteRenderer>();
            _col = GetComponent<BoxCollider2D>();

            _sr.sprite = tileInfoSo.sprite;
            _col.size = Vector2.one;

            transform.localPosition = (Vector2)gridCell;
            transform.localRotation = Quaternion.Euler(0f, 0f, rot * 90f);
        }
        
        public virtual void InitFloating(TileInfoSO tileInfoSo, Vector2 position, Vector2 velocity)
        {
            tileInfo = tileInfoSo;
            currentHp = tileInfoSo.hp;
            rotation = 0;

            _sr = GetComponent<SpriteRenderer>();
            _col = GetComponent<BoxCollider2D>();

            _sr.sprite = tileInfoSo.sprite;
            _sr.sortingOrder = 10;
            _baseColor = Color.white;
            _sr.color = _baseColor;

            _col.size = Vector2.one;
            _col.isTrigger = true; // pass through ship, no physical collision

            _floatRb = gameObject.AddComponent<Rigidbody2D>();
            _floatRb.gravityScale = 0f;
            _floatRb.linearDamping = 0.05f;
            _floatRb.angularDamping = 0.1f;
            _floatRb.linearVelocity = velocity;
            _floatRb.angularVelocity = Random.Range(-45f, 45f);

            transform.position = (Vector3)position;
            IsAttached = false;
        }
        
        public bool EdgeConnectable(int worldDir)
        {
            int localDir = (worldDir + rotation) % 4;
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

        public static Tile SpawnFloating(TileInfoSO info, Vector2 position, Vector2 velocity)
        {
            GameObject go = new GameObject($"Tile_{info.tileName}_floating");
            Tile tile = CreateTyped(go, info.type);
            tile.InitFloating(info, position, velocity);
            return tile;
        }
        #endregion
    }
}
