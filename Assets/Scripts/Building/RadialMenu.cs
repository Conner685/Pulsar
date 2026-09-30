using UnityEngine;

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
        private float _rx, _ry;
        private Quaternion _rotation;

        private void Awake()
        {
            // scan area
            GameObject fillGO = new GameObject("ScanZone_Fill");
            fillGO.transform.SetParent(transform, false);
            _fillTransform = fillGO.transform;
            _fillSr = fillGO.AddComponent<SpriteRenderer>();
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
            _outline.material = new Material(Shader.Find("Sprites/Default"));
            _outline.startColor = Color.white;
            _outline.endColor = Color.white;
            _outline.sortingOrder = -9;

            gameObject.SetActive(false);
        }
        
        public void UpdateShape(Vector3 worldCenter, float rx, float ry, Quaternion rotation)
        {
            _center = worldCenter;
            _rx = rx;
            _ry = ry;
            _rotation = rotation;

            transform.SetPositionAndRotation(worldCenter, rotation);

            _fillTransform.localScale = new Vector3(rx, ry, 1f);

            for (int i = 0; i < Segments; i++)
            {
                float angle = i * Mathf.PI * 2f / Segments;
                _outline.SetPosition(i, new Vector3(
                    Mathf.Cos(angle) * rx,
                    Mathf.Sin(angle) * ry,
                    0f));
            }
        }

        public bool IsInside(Vector2 worldPoint)
        {
            if (_rx <= 0f || _ry <= 0f) return false;
            Vector2 local = (Vector2)(Quaternion.Inverse(_rotation) * ((Vector3)worldPoint - _center));
            float nx = local.x / _rx;
            float ny = local.y / _ry;
            return (nx * nx + ny * ny) <= 1f;
        }

        public void Show() => gameObject.SetActive(true);
        public void Hide() => gameObject.SetActive(false);

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
