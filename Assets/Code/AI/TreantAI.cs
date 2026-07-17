using FK.Common;
using FK.Tulip.Combat;
using FK.Tulip.Input;
using UnityEngine;

namespace FK.Tulip.AI
{
    public sealed class TreantAI : CharacterBrain
    {
        [Header("References")]
        [SerializeField, Required] Health health;

        [Header("Movement")]
        [SerializeField] Vector2 attackDistance;

        public override Vector2? AimPosition { get; protected set; }
        public override bool WantsToAttack { get; protected set; }
        public override bool WantsToTakeAim { get; protected set; }
        public override bool WantsToHook { get; protected set; }

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
                WantsToAttack = false;
                WantsToTakeAim = false;
                return;
            }

            AimPosition = (Vector2)targetHealth.transform.position;
            Vector2 targetVector = AimPosition.Value - (Vector2)transform.position;

            bool reachedX = Mathf.Abs(targetVector.x) < attackDistance.x;
            bool reachedY = Mathf.Abs(targetVector.y) < attackDistance.y;
            WantsToAttack = reachedX && reachedY;
        }
    }
}
