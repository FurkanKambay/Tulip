using SaintsField;
using Tulip.Data.Items;
using Tulip.GameWorld;
using UnityEngine;

namespace Tulip.Data
{
    [CreateAssetMenu(menuName = "Gameplay/Entity")]
    public class EntityData : ScriptableObject
    {
        [SerializeField, Required] new string name;
        [SerializeField, Required] GameObject prefab;

        [Header("Spawning")]
        [SerializeField] bool isStatic;
        [SerializeField, Required] SpawnConditionData spawnConditionData;

        [PostFieldRichLabel("<color=gray>tiles")]
        [SerializeField] Vector2Int size;

        // BUG: only works with Static Entities in the world
        // TODO: make a separate LootTable class
        [Header("Loot")]
        [SerializeField] ItemData loot;
        [SerializeField] int lootAmount;

        public string Name => name;
        public GameObject Prefab => prefab;

        public bool IsStatic => isStatic;
        public SpawnConditionData SpawnConditionData => spawnConditionData;

        public Vector2Int Size => size;
        public ItemData Loot => loot;
        public int LootAmount => lootAmount;

        public bool CanSpawnAt(World world, Vector2Int cell) =>
            spawnConditionData.CanSpawn(this, world, cell);
    }
}
