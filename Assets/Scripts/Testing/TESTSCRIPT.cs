using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using Pulsar.Ship;
using Pulsar.Building;

namespace Pulsar.Testing
{
    public class TESTSCRIPT : MonoBehaviour
    {
        [Header("Spawnable tiles")]
        [SerializeField] private TileInfoSO[] tileInfos;

        [Header("Spawner")]
        [SerializeField] private float spawnMinRadius = 5f;
        [SerializeField] private float spawnMaxRadius = 8f;
        [SerializeField] private float driftSpeedMin = 0.3f;
        [SerializeField] private float driftSpeedMax = 1.5f;

        public ShipGrid shipGrid;
        private Camera _cam;

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.S))
                SpawnRandomFloater();
        }


        private void SpawnRandomFloater()
        {

            TileInfoSO infor = tileInfos[Random.Range(0, tileInfos.Length)];

            float angle = Random.Range(0f, 360f) * Mathf.Deg2Rad;
            float dist = Random.Range(spawnMinRadius, spawnMaxRadius);
            Vector2 shipPos = shipGrid.transform.position;
            Vector2 pos = shipPos + new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * dist;

            Vector2 toShip = (shipPos - pos).normalized;
            Vector2 velocity = toShip * Random.Range(driftSpeedMin, driftSpeedMax)
                               + Random.insideUnitCircle * 0.3f;

            Tile.SpawnFloating(infor, pos, velocity);
        }


        private void OnGUI()
        {
            GUIStyle style = new GUIStyle(GUI.skin.box)
            {
                fontSize = 30,
                alignment = TextAnchor.UpperLeft,
                richText = true
            };

            string info =
                "<b>BUILD & DESTROY TEST</b>\n\n" +
                "<b>S</b>            — Spawn floating tile\n" +
                "<b>Tab</b>        — Toggle scan zone\n" +
                "<b>Hover</b>    — Mouse over tile in zone\n" +
                "<b>C</b>            — Confirm attach\n" +
                "<b>X</b>            — Destroy ship tile\n\n";

            GUI.Box(new Rect(10, 10, 540, 300), info, style);
        }
    }
}
