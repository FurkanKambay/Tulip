using SaintsField;
using Tulip.Data;
using Tulip.Data.Gameplay;
using Tulip.GameWorld;
using UnityEngine;

namespace Tulip.Character
{
    public class TangibleEntity : MonoBehaviour
    {
        [Header("References")]
        [SerializeField, Required] EntityData entityData;
        [SerializeField] Rigidbody2D body;
        [SerializeField] Health health;
        [SerializeField] World world;

        public string Name => entityData.Name;
        public EntityData EntityData => entityData;
        public Health Health => health;

        public World World => world;
        public Vector2Int Cell { get; private set; }
        public RectInt Rect => new (Cell, EntityData.Size);

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

        private void HandleDied(HealthChangeEventArgs damage) => body.simulated = false;
        private void HandleRevived(Health reviver) => body.simulated = true;

        public void SetResidence(World homeWorld, Vector2Int baseCell)
        {
            world = homeWorld;
            Cell = baseCell;
        }

        public override string ToString() => Name;
    }
}
