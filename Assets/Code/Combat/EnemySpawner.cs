using System.Collections.Generic;
using System.Linq;
using Furkan.Common;
using Tulip.Character;
using Tulip.Data;
using Tulip.Data.Sets;
using Tulip.GameWorld;
using UnityEditor;
using UnityEngine;

namespace Tulip.Combat
{
    public class EnemySpawner : MonoBehaviour
    {
        [LayoutGroup("References", ELayout.Background | ELayout.TitleOut)]
        [SerializeField] World world;
        [SerializeField] Transform spawnParent;

        [LayoutGroup("Config", ELayout.Background | ELayout.TitleOut)]
        [LayoutGroup("Config/Spawning", ELayout.TitleOut)]
        [SerializeField, Min(0)] int maxSpawns = 100;

        [OverlayRichLabel("<color=gray>tiles")]
        [SerializeField, Min(0)] int radius = 5;

        [OverlayRichLabel("<color=gray>sec")]
        [SerializeField, Min(0)] float interval = 10f;

        [OverlayRichLabel("<color=gray>sec")]
        [SerializeField, Min(0)] float gracePeriod;

        [SerializeField] EntitySet entitySpawnSet;

        private Camera camera;
        private IEnumerable<Vector2Int> suitableCells;

        private float timeSinceLastSpawn;

        private void Awake()
        {
            camera = Camera.main;
            timeSinceLastSpawn = -gracePeriod;
        }

        private void Update()
        {
            timeSinceLastSpawn += Time.deltaTime;

            if (timeSinceLastSpawn < interval)
                return;

            if (TrySpawnEnemy())
                timeSinceLastSpawn = 0;
        }

        private bool TrySpawnEnemy()
        {
            if (spawnParent.childCount >= maxSpawns)
                return false;

            if (entitySpawnSet.Count == 0)
                return false;

            EntityAsset entityAsset = GetRandomEnemy();
            suitableCells = GetSuitableCells(entityAsset);

            if (!suitableCells.Any())
                return false;

            Vector2Int baseCell = GetRandomSpawnCell();
            var spawnedEnemy = TangibleEntity.SpawnAtCell(entityAsset, world, baseCell, spawnParent);

            if (entityAsset.IsStatic)
                world.TryAddStaticEntity(baseCell, spawnedEnemy);

            return true;
        }

        private EntityAsset GetRandomEnemy() =>
            entitySpawnSet[Random.Range(0, entitySpawnSet.Count)];

        private Vector2Int GetRandomSpawnCell() =>
            suitableCells.ElementAt(Random.Range(0, suitableCells.Count()));

        private IEnumerable<Vector2Int> GetSuitableCells(EntityAsset entityAsset)
        {
            Vector3 cameraExtents = new(camera.orthographicSize * camera.aspect, camera.orthographicSize);
            Vector3 spawnExtents = new(cameraExtents.x + radius, cameraExtents.y + radius);
            Vector3 cameraCenter = camera.transform.position;

            Vector2Int camTopRight = world.WorldToCell(cameraCenter + cameraExtents);
            Vector2Int camBottomLeft = world.WorldToCell(cameraCenter - cameraExtents);
            Vector2Int spawnTopRight = world.WorldToCell(cameraCenter + spawnExtents);
            Vector2Int spawnBottomLeft = world.WorldToCell(cameraCenter - spawnExtents);

            for (int y = spawnBottomLeft.y; y <= spawnTopRight.y; y++)
            {
                for (int x = spawnBottomLeft.x; x <= spawnTopRight.x; x++)
                {
                    if (y <= camTopRight.y && y >= camBottomLeft.y && x <= camTopRight.x && x >= camBottomLeft.x)
                        continue;

                    var cell = new Vector2Int(x, y);

                    if (entityAsset.CanSpawnAt(world, cell))
                        yield return cell;
                }
            }
        }

        [ContextMenu("Destroy All Spawns")]
        private void DestroyAllSpawns()
        {
            for (int i = 0; i < spawnParent.childCount; i++)
                Destroy(spawnParent.GetChild(i).gameObject);

            world.ClearEntities();
        }

#if UNITY_EDITOR
        private void OnDrawGizmosSelected()
        {
            if (entitySpawnSet.Count == 0)
                return;

            if (!camera)
                camera = Camera.main;

            Handles.color = Color.yellow;

            foreach (Vector2Int cell in GetSuitableCells(entitySpawnSet[0]))
                Handles.DrawSolidDisc(world.CellCenter(cell), Vector3.forward, 0.2f);
        }
#endif
    }
}
