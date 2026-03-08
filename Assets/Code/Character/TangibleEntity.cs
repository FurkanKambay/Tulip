using SaintsField;
using Tulip.Data;
using Tulip.Data.Gameplay;
using Tulip.GameWorld;
using UnityEngine;

namespace Tulip.Character
{
    public class TangibleEntity : MonoBehaviour
    {
        [Header("Injectable State")]
        [SerializeField] World world;

        [Header("References")]
        [SerializeField, Required] EntitySO entitySO;
        [SerializeField] Rigidbody2D body;
        [SerializeField] Health health;

        public string Name => entitySO.Name;
        public EntitySO EntitySO => entitySO;
        public Health Health => health;

        public World World => world;
        public Vector2Int Cell { get; private set; }
        public RectInt Rect => new(Cell, EntitySO.Size);

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

        public void SetResidence(World homeWorld, Vector2Int baseCell)
        {
            world = homeWorld;
            Cell = baseCell;
        }

        public override string ToString() => Name;
    }
}
