using UnityEngine;
using Pulsar.Ship;

namespace Pulsar.Building
{
    public class BuildManager : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private ShipGrid grid;
        [SerializeField] private Camera gameplayCamera;

        [Header("Scan Zone")]
        [SerializeField] private float scanPadding = 2.5f;

        [Header("Hover")]
        [SerializeField] private float hoverRadius = 0.6f;
        [SerializeField] private Color hoverTint = new(0.3f, 1f, 0.8f, 1f);

        private RadialMenu _radialMenu;
        private GhostPreview _ghost;
        private bool _scanActive;

        // hover tile
        private Tile _hoveredTile;
        private Vector2Int _attachCell;
        private int _attachRotation;
        private bool _attachValid;

        // destroy tile
        private Tile _highlightedTile;
        private Color _highlightOrigColor;


        private void Start()
        {
            if (gameplayCamera == null) gameplayCamera = Camera.main;

            // radial menu
            GameObject rmGO = new GameObject("RadialMenu");
            _radialMenu = rmGO.AddComponent<RadialMenu>();

            // ghost preview
            GameObject ghostGO = new GameObject("GhostPreview");
            _ghost = ghostGO.AddComponent<GhostPreview>();
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Tab))
            {
                _scanActive = !_scanActive;
                if (_scanActive)
                {
                    _radialMenu.Show();
                    Debug.Log("Activate radial menu");
                }
                else
                {
                    _radialMenu.Hide();
                    ClearHover();
                    Debug.Log("Inactivate radial menu");
                }
            }

            if (_scanActive)
            {
                UpdateScanZoneShape();
                UpdateHover();
            }

            // destroy is always available when not hovering a floating tile
            if (_hoveredTile == null)
                UpdateDestroyMode();
        }

        private void UpdateScanZoneShape()
        {
            Bounds b = grid.GetLocalBounds();
            float rx = b.extents.x + scanPadding;
            float ry = b.extents.y + scanPadding;
            Vector3 center = grid.transform.TransformPoint(b.center);
            Quaternion rot = grid.transform.rotation;

            _radialMenu.UpdateShape(center, rx, ry, rot);
        }
        

        private void UpdateHover()
        {
            Vector2 cursor = GetCursorWorld();

            // Find the floating tile closest to cursor
            Tile best = null;
            float bestDist = float.MaxValue;

            Tile[] allTiles = FindObjectsByType<Tile>(FindObjectsSortMode.None);
            foreach (Tile t in allTiles)
            {
                if (t.IsAttached) continue;
                if (!_radialMenu.IsInside(t.transform.position)) continue;

                float d = Vector2.Distance(cursor, (Vector2)t.transform.position);
                if (d < bestDist)
                {
                    bestDist = d;
                    best = t;
                }
            }

            if (best == null || bestDist > hoverRadius)
            {
                ClearHover();
                return;
            }

            if (best != _hoveredTile)
            {
                ClearHover();
                _hoveredTile = best;
                _hoveredTile.Highlight(hoverTint);
            }
            
            (Vector2Int cell, int rotation)? result = grid.FindBestAttachment(
                _hoveredTile.tileInfo,
                _hoveredTile.transform.position);

            if (result.HasValue)
            {
                _attachCell = result.Value.cell;
                _attachRotation = result.Value.rotation;
                _attachValid = true;

                Vector3 ghostWorld = grid.transform.TransformPoint((Vector2)_attachCell);
                Quaternion ghostRot = grid.transform.rotation
                                      * Quaternion.Euler(0f, 0f, _attachRotation * 90f);
                _ghost.Show(_hoveredTile.tileInfo.sprite, ghostWorld, true, ghostRot);
            }
            else
            {
                _attachValid = false;
                _ghost.Hide();
            }

            if (Input.GetKeyDown(KeyCode.C) && _attachValid)
            {
                TileInfoSO info = _hoveredTile.tileInfo;
                if (grid.Attach(_attachCell, info, _attachRotation))
                {
                    Destroy(_hoveredTile.gameObject);
                    _hoveredTile = null;
                    _attachValid = false;
                    _ghost.Hide();
                    Debug.Log($"Attached {info.tileName} at {_attachCell}");
                }
            }
        }

        private void ClearHover()
        {
            if (_hoveredTile != null)
            {
                if (_hoveredTile.gameObject != null)
                    _hoveredTile.Unhighlight();
                _hoveredTile = null;
            }
            _attachValid = false;
            _ghost.Hide();
        }


        private void UpdateDestroyMode()
        {
            Vector2 cursorWorld = GetCursorWorld();
            Tile tile = grid.GetTileAtWorldPos(cursorWorld);

            // un-highlight previous
            if (_highlightedTile != null && _highlightedTile != tile)
            {
                SpriteRenderer sr = _highlightedTile.GetComponent<SpriteRenderer>();
                if (sr != null) sr.color = _highlightOrigColor;
                _highlightedTile = null;
            }

            // highlight current 
            if (tile != null && tile != grid.Core && _highlightedTile != tile)
            {
                _highlightedTile = tile;
                SpriteRenderer sr = tile.GetComponent<SpriteRenderer>();
                if (sr != null)
                {
                    _highlightOrigColor = sr.color;
                    sr.color = Color.red;
                }
            }

            if (Input.GetKeyDown(KeyCode.X) && _highlightedTile != null)
            {
                Vector2Int cell = _highlightedTile.cell;
                _highlightedTile = null;
                grid.DestroyTile(cell);
            }
        }


        private Vector2 GetCursorWorld()
        {
            Vector3 screen = Input.mousePosition;
            screen.z = -gameplayCamera.transform.position.z;
            return gameplayCamera.ScreenToWorldPoint(screen);
        }
    }
}
