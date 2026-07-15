using System;
using Furkan.Common;
using Tulip.Combat;
using Tulip.Data;
using Tulip.GameWorld;
using UnityEngine;

namespace Tulip.Character
{
    public class TangibleEntity : MonoBehaviour
    {
        [Header("Injectable State")]
        [SerializeField] World world;

        [Header("References")]
        [SerializeField, Required] EntityAsset entityAsset;
        [SerializeField] Rigidbody2D body;
        [SerializeField] Health health;

        public string Name => entityAsset.Name;
        public EntityAsset EntityAsset => entityAsset;
        public Health Health => health;

        public World World => world;
        public Vector2Int Cell { get; private set; }
        public RectInt Rect => new(Cell, EntityAsset.Size);

        private void OnEnable()
        {
            if (!health || !body)
                return;

            health.OnDie += HandleDied;
            health.OnRevive += HandleRevived;
        }

        private void OnDisable()
        {
            if (!health || !body)
                return;

            health.OnDie -= HandleDied;
            health.OnRevive -= HandleRevived;
        }

        private void HandleDied(CombatPacket combatPacket) => body.simulated = false;
        private void HandleRevived(Health reviver) => body.simulated = true;

        public override string ToString() => Name;

        public static TangibleEntity SpawnAtCell(EntityAsset entityAsset, World world, Vector2Int baseCell, Transform parent)
        {
            Vector2Int cellCenter = new(baseCell.x + (entityAsset.Size.x / 2), baseCell.y);
            Vector3 position = world.CellCenter(cellCenter);

            TangibleEntity entity = Spawn(entityAsset, world, position, parent);
            entity.Cell = baseCell;

            return entity;
        }

        public static TangibleEntity Spawn(EntityAsset entityAsset, World world, Vector3 position, Transform parent)
        {
            if (!entityAsset) throw new ArgumentNullException(nameof(entityAsset));
            if (!world) throw new ArgumentNullException(nameof(world));
            if (!entityAsset.Prefab) throw new ArgumentException("Entity lacks an assigned Prefab.", nameof(entityAsset));

            GameObject instance = Instantiate(entityAsset.Prefab, position, Quaternion.identity, parent);

            if (!instance.TryGetComponent(out TangibleEntity tangible))
                throw new ArgumentException($"Entity lacks a {nameof(TangibleEntity)} component.", nameof(entityAsset));

            tangible.world = world;
            return tangible;
        }
    }
}
