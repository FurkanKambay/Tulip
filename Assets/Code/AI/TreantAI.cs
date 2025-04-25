using SaintsField;
using Tulip.Character;
using Tulip.Data;
using Tulip.Data.Gameplay;
using UnityEngine;

namespace Tulip.AI
{
    public sealed class TreantAI : MonoBehaviour, IWielderBrain
    {
        [Header("References")]
        [SerializeField, Required] Health health;

        [Header("Movement")]
        [SerializeField] Vector2 attackDistance;

        public Vector2? AimPosition { get; private set; }
        public bool WantsToUse { get; private set; }
        public bool WantsToThrow { get; private set; }
        public bool WantsToHook { get; private set; }

        private Health targetHealth;

        private void OnEnable() => health.OnHurt += HandleHurt;
        private void OnDisable() => health.OnHurt -= HandleHurt;

        private void HandleHurt(CombatPacket combatPacket)
        {
            // always target the last attacker (if any)
            targetHealth = combatPacket.Source;
        }

        private void Update()
        {
            if (!health || health.IsDead || !targetHealth || targetHealth.IsDead)
            {
                AimPosition = default;
                WantsToUse = false;
                WantsToThrow = false;
                return;
            }

            AimPosition = (Vector2)targetHealth.transform.position;
            Vector2 targetVector = AimPosition.Value - (Vector2)transform.position;

            bool reachedX = Mathf.Abs(targetVector.x) < attackDistance.x;
            bool reachedY = Mathf.Abs(targetVector.y) < attackDistance.y;
            WantsToUse = reachedX && reachedY;
        }
    }
}
