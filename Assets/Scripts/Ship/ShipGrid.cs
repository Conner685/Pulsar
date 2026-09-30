using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

namespace Pulsar.Ship
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class ShipGrid : MonoBehaviour
    {
        public static readonly Vector2Int[] Dirs =
        {
            Vector2Int.up,    
            Vector2Int.right, 
            Vector2Int.down,  
            Vector2Int.left  
        };

        public static int Opposite(int d) => (d + 2) % 4;

        [SerializeField] private TileInfoSO coreInfoSo;

        private readonly Dictionary<Vector2Int, Tile> _cells = new();
        private Tile _core;
        private Rigidbody2D _rb;

        public IReadOnlyDictionary<Vector2Int, Tile> Cells => _cells;
        public Tile Core => _core;
        public Rigidbody2D Rb => _rb;

        public UnityEvent onShipChanged;
        public UnityEvent onShipDestroyed;


        private void Awake()
        {
            _rb = GetComponent<Rigidbody2D>();
            // _rb.useAutoMass = true; // auto center gravity
            _rb.gravityScale = 0f;
            _rb.linearDamping = 0.5f;
            _rb.angularDamping = 0.8f;

            if (coreInfoSo != null) SpawnCore();
        }

        private void SpawnCore()
        {
            Tile tile = CreateTileGO(coreInfoSo, Vector2Int.zero, 0);
            _cells[Vector2Int.zero] = tile;
            _core = tile;
            tile.OnAttached(this);
        }


        public Tile CreateTileGO(TileInfoSO infoSo, Vector2Int cell, int rotation)
        {
            GameObject go = new GameObject($"Tile_{infoSo.tileName}_{cell}");
            go.transform.SetParent(transform, false);
            go.AddComponent<SpriteRenderer>();
            go.AddComponent<BoxCollider2D>();

            Tile tile = Tile.CreateTyped(go, infoSo.type);
            tile.Init(infoSo, cell, rotation);
            return tile;
        }

        #region ATTACH-DETACH

        // Requires cell empty + at least one mutual edge connection.
        public bool CanAttach(Vector2Int pos, TileInfoSO infoSo, int rotation)
        {
            if (_cells.ContainsKey(pos)) return false;

            for (int d = 0; d < 4; d++)
            {
                int localDir = (d + rotation) % 4;
                if (!infoSo.connectableEdges[localDir]) continue;

                Vector2Int neighborPos = pos + Dirs[d];
                if (_cells.TryGetValue(neighborPos, out Tile neighbor)
                    && neighbor.EdgeConnectable(Opposite(d)))
                    return true;
            }
            return false;
        }
        
        public int CountConnections(Vector2Int pos, TileInfoSO infoSo, int rotation)
        {
            int count = 0;
            for (int d = 0; d < 4; d++)
            {
                int localDir = (d + rotation) % 4;
                if (!infoSo.connectableEdges[localDir]) continue;

                Vector2Int neighborPos = pos + Dirs[d];
                if (_cells.TryGetValue(neighborPos, out Tile neighbor)
                    && neighbor.EdgeConnectable(Opposite(d)))
                    count++;
            }
            return count;
        }

        public bool Attach(Vector2Int pos, TileInfoSO infoSo, int rotation)
        {
            if (!CanAttach(pos, infoSo, rotation)) return false;
            Tile tile = CreateTileGO(infoSo, pos, rotation);
            _cells[pos] = tile;
            tile.OnAttached(this);
            onShipChanged?.Invoke();
            return true;
        }

        public void DestroyTile(Vector2Int pos)
        {
            if (!_cells.TryGetValue(pos, out Tile tile)) return;
            if (tile == _core) { OnCoreDestroyed(); return; }

            _cells.Remove(pos);
            tile.OnDetached();
            Destroy(tile.gameObject);
            DetachOrphans();
            onShipChanged?.Invoke();
        }

        public void OnCoreDestroyed()
        {
            onShipDestroyed?.Invoke();
        }
        
        public (Vector2Int cell, int rotation)? FindBestAttachment(TileInfoSO infoSo, Vector2 worldPos)
        {
            Vector2 localPos = transform.InverseTransformPoint(worldPos);

            float bestDist = float.MaxValue;
            int bestRot = -1;
            int bestConns = 0;
            Vector2Int bestCell = default;

            // gather all candidate empty cells adjacent to existing tiles
            HashSet<Vector2Int> candidates = new();
            foreach (Vector2Int occupied in _cells.Keys)
                for (int d = 0; d < 4; d++)
                    candidates.Add(occupied + Dirs[d]);

            foreach (Vector2Int candidate in candidates)
            {
                if (_cells.ContainsKey(candidate)) continue;

                float dist = Vector2.Distance(localPos, (Vector2)candidate);

                for (int rot = 0; rot < 4; rot++)
                {
                    if (!CanAttach(candidate, infoSo, rot)) continue;

                    int conns = CountConnections(candidate, infoSo, rot);

                    if (bestRot == -1
                        || dist < bestDist - 0.01f
                        || (Mathf.Abs(dist - bestDist) < 0.01f && conns > bestConns))
                    {
                        bestDist = dist;
                        bestCell = candidate;
                        bestRot = rot;
                        bestConns = conns;
                    }
                }
            }

            if (bestRot == -1) return null;
            return (bestCell, bestRot);
        }



        private void DetachOrphans()
        {
            HashSet<Vector2Int> reachable = Flood(_core.cell);
            List<Vector2Int> orphanKeys = _cells.Keys.Where(c => !reachable.Contains(c)).ToList();
            if (orphanKeys.Count == 0) return;

            // Release each orphan as a free-floating tile
            foreach (Vector2Int c in orphanKeys)
            {
                if (!_cells.TryGetValue(c, out Tile tile)) continue;

                // capture data before destroying the grid tile
                TileInfoSO info = tile.tileInfo;
                Vector3 worldPos = tile.transform.position;

                // compute velocity: ship linear + tangential from angular
                Vector2 velocity = _rb.linearVelocity;
                Vector2 offset = (Vector2)worldPos - (Vector2)transform.position;
                float angRad = _rb.angularVelocity * Mathf.Deg2Rad;
                velocity += new Vector2(-offset.y, offset.x) * angRad;

                tile.OnDetached();
                _cells.Remove(c);
                Destroy(tile.gameObject);

                // spawn a free-floating tile with inherited velocity
                Tile.SpawnFloating(info, worldPos, velocity);
            }
        }

        private HashSet<Vector2Int> Flood(Vector2Int start)
        {
            Queue<Vector2Int> open = new Queue<Vector2Int>();
            HashSet<Vector2Int> closed = new HashSet<Vector2Int> { start };
            open.Enqueue(start);

            while (open.Count > 0)
            {
                Vector2Int c = open.Dequeue();
                Tile tile = _cells[c];
                for (int d = 0; d < 4; d++)
                {
                    Vector2Int n = c + Dirs[d];
                    if (closed.Contains(n) || !_cells.ContainsKey(n)) continue;
                    if (!tile.EdgeConnectable(d) || !_cells[n].EdgeConnectable(Opposite(d))) continue;
                    closed.Add(n);
                    open.Enqueue(n);
                }
            }
            return closed;
        }
        #endregion


        public Bounds GetLocalBounds()
        {
            if (_cells.Count == 0) return new Bounds(Vector3.zero, Vector3.one);

            Vector2 min = new(float.MaxValue, float.MaxValue);
            Vector2 max = new(float.MinValue, float.MinValue);
            foreach (Vector2Int c in _cells.Keys)
            {
                min = Vector2.Min(min, c);
                max = Vector2.Max(max, c);
            }
            return new Bounds((Vector3)(min + max) * 0.5f, (Vector3)(max - min + Vector2.one));
        }

        public Tile GetTileAtWorldPos(Vector2 worldPos)
        {
            Vector2 local = transform.InverseTransformPoint(worldPos);
            Vector2Int cell = Vector2Int.RoundToInt(local);
            _cells.TryGetValue(cell, out Tile tile);
            return tile;
        }
    }
}
