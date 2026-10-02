using UnityEngine;
using Pulsar.Ship;

namespace Pulsar.Building
{
    public class RadialMenu : MonoBehaviour
    {
        private LineRenderer _outline;
        private Transform _fillTransform;
        private SpriteRenderer _fillSr;
        private const int Segments = 64;

        // cached oval params 
        private Vector3 _center;
        private float _rx, _rz;
        private Quaternion _rotation;

        private void Awake()
        {
            // scan area
            GameObject fillGO = new GameObject("ScanZone_Fill");
            fillGO.transform.SetParent(transform, false);
            _fillTransform = fillGO.transform;
            _fillTransform.localRotation = Quaternion.Euler(90f, 0f, 0f);
            _fillSr = fillGO.AddComponent<SpriteRenderer>();
            _fillSr.sharedMaterial = ShipUtilities.UnlitSpriteMaterial;
            _fillSr.sprite = GenerateCircleSprite();
            _fillSr.color = new Color(0f, 1f, 0f, 0.10f);
            _fillSr.sortingOrder = -10;

            // scan area ouline
            GameObject outlineGO = new GameObject("ScanZone_Outline");
            outlineGO.transform.SetParent(transform, false);
            _outline = outlineGO.AddComponent<LineRenderer>();
            _outline.useWorldSpace = false;
            _outline.loop = true;
            _outline.positionCount = Segments;
            _outline.widthMultiplier = 0.05f;
            _outline.sharedMaterial = ShipUtilities.UnlitSpriteMaterial;
            _outline.startColor = Color.white;
            _outline.endColor = Color.white;
            _outline.sortingOrder = -9;

            gameObject.SetActive(false);
        }
        
        public void UpdateShape(Vector3 worldCenter, float rx, float rz, Quaternion rotation)
        {
            _center = worldCenter;
            _rx = rx;
            _rz = rz;
            _rotation = rotation;

            transform.SetPositionAndRotation(worldCenter, rotation);

            _fillTransform.localScale = new Vector3(rx, rz, 1f);

            for (int i = 0; i < Segments; i++)
            {
                float angle = i * Mathf.PI * 2f / Segments;
                _outline.SetPosition(i, new Vector3(
                    Mathf.Cos(angle) * rx,
                    0f,
                    Mathf.Sin(angle) * rz));
            }
        }

        public bool IsInside(Vector3 worldPoint)
        {
            if (_rx <= 0f || _rz <= 0f) return false;
            Vector3 local = Quaternion.Inverse(_rotation) * (worldPoint - _center);
            float nx = local.x / _rx;
            float nz = local.z / _rz;
            return (nx * nx + nz * nz) <= 1f;
        }

        public void Show() => gameObject.SetActive(true);
        public void Hide() => gameObject.SetActive(false);

        private void OnDestroy()
        {
            if (_fillSr == null || _fillSr.sprite == null) return;
            Destroy(_fillSr.sprite.texture);
            Destroy(_fillSr.sprite);
        }

        private static Sprite GenerateCircleSprite()
        {
            const int size = 64;
            Texture2D tex = new Texture2D(size, size, TextureFormat.RGBA32, false);
            tex.filterMode = FilterMode.Bilinear;

            Color fill = Color.white;   // actual tint from SpriteRenderer.color
            Color clear = new Color(0, 0, 0, 0);
            float half = size * 0.5f;

            for (int x = 0; x < size; x++)
            {
                for (int y = 0; y < size; y++)
                {
                    float dx = (x - half + 0.5f) / half;
                    float dy = (y - half + 0.5f) / half;
                    tex.SetPixel(x, y, (dx * dx + dy * dy) <= 1f ? fill : clear);
                }
            }
            tex.Apply();

            return Sprite.Create(tex, new Rect(0, 0, size, size), new Vector2(0.5f, 0.5f), size / 2f);
        }
    }
}
