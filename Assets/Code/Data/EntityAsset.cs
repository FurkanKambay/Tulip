using Furkan.Common;
using Tulip.Data.Items;
using Tulip.GameWorld;
using UnityEngine;

namespace Tulip.Data
{
    [CreateAssetMenu(menuName = "Gameplay/Entity")]
    public class EntityAsset : ScriptableObject
    {
        [SerializeField, Required] new string name;
        [SerializeField, Required] GameObject prefab;

        [Header("Spawning")]
        [SerializeField] bool isStatic;
        [SerializeField, Required] SpawnConditionAsset spawnConditionAsset;

        [PostFieldRichLabel("<color=gray>tiles")]
        [SerializeField] Vector2Int size;

        // BUG: only works with Static Entities in the world
        // TODO: make a separate LootTable class
        [Header("Loot")]
        [SerializeField] ItemAsset loot;
        [SerializeField] int lootAmount;

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
