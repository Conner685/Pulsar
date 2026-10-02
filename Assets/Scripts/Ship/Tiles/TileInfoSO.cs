using UnityEngine;

namespace Pulsar.Ship
{
    [CreateAssetMenu(fileName = "NewTileDef", menuName = "Pulsar/Tile Definition")]
    public class TileInfoSO : ScriptableObject
    {
        [Header("Common Info")]
        public string tileName;

        public TileType type;
        public Sprite sprite;
        public float hp = 10f;
        public float mass = 1f;

        [Header("Edges: FORWARD (+Z), RIGHT (+X), BACK (-Z), LEFT (-X)")]
        public bool[] connectableEdges = { true, true, true, true };
    }

    public enum TileType
    {
        Core,
        Chassis,
        Thruster,
        Weapon
    }
}
