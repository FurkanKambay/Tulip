using Furkan.Common;
using Tulip.Data.Items;
using Tulip.GameWorld;
using UnityEngine;

namespace Tulip.Data
{
    [CreateAssetMenu(menuName = "Gameplay/Entity")]
    public class EntitySO : ScriptableObject
    {
        [SerializeField, Required] new string name;
        [SerializeField, Required] GameObject prefab;

        [Header("Spawning")]
        [SerializeField] bool isStatic;
        [SerializeField, Required] SpawnConditionSO spawnConditionSO;

        [PostFieldRichLabel("<color=gray>tiles")]
        [SerializeField] Vector2Int size;

        // BUG: only works with Static Entities in the world
        // TODO: make a separate LootTable class
        [Header("Loot")]
        [SerializeField] ItemSO loot;
        [SerializeField] int lootAmount;

        public string Name => name;
        public GameObject Prefab => prefab;

        public bool IsStatic => isStatic;
        public SpawnConditionSO SpawnConditionSO => spawnConditionSO;

        public Vector2Int Size => size;
        public ItemSO Loot => loot;
        public int LootAmount => lootAmount;

        public bool CanSpawnAt(World world, Vector2Int cell) =>
            spawnConditionSO.CanSpawn(this, world, cell);
    }
}
