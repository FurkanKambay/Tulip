using FK.Common;
using FK.Tulip.Data.Items;
using FK.Tulip.GameWorld;
using UnityEngine;

namespace FK.Tulip.Data
{
    [CreateAssetMenu(menuName = "Gameplay/Entity")]
    public class EntityAsset : ScriptableObject
    {
        [SerializeField, Required] private new string name;
        [SerializeField, Required] private GameObject prefab;

        [Header("Spawning")]
        [SerializeField] private bool isStatic;
        [SerializeField, Required] private SpawnConditionAsset spawnConditionAsset;

        [PostFieldRichLabel("<color=gray>tiles")]
        [SerializeField] private Vector2Int size;

        // BUG: only works with Static Entities in the world
        // TODO: make a separate LootTable class
        [Header("Loot")]
        [SerializeField] private ItemAsset loot;
        [SerializeField] private int lootAmount;

        public string Name => name;
        public GameObject Prefab => prefab;

        public bool IsStatic => isStatic;
        public SpawnConditionAsset SpawnConditionAsset => spawnConditionAsset;

        public Vector2Int Size => size;
        public ItemAsset Loot => loot;
        public int LootAmount => lootAmount;

        public bool CanSpawnAt(World world, Vector2Int cell) =>
            spawnConditionAsset.CanSpawn(this, world, cell);
    }
}
