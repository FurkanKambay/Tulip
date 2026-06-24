using System;
using Furkan.Common;
using Tulip.Character;
using Tulip.Combat;
using Tulip.Input;
using UnityEngine;

namespace Tulip.AI
{
    public sealed class SimpleFollowerAI : CharacterBrain
    {
        public override event Action OnJump;
        public override event Action OnJumpReleased;

        [Header("References")]
        [SerializeField, Required] Health health;

        [Header("Movement")]
        [SerializeField] float stopDistance;

        [Header("Jump")]
        [SerializeField] float heightThresholdToJump;
        [SerializeField] float jumpCooldown;

        public override Vector2? AimPosition { get; protected set; }
        public override float HorizontalMovement { get; protected set; }

        public override bool WantsToJump { get; protected set; }
        public override bool WantsToAttack { get; protected set; }
        public override bool WantsToTakeAim { get; protected set; }
        public override bool WantsToHook { get; protected set; }

        private Health targetHealth;
        private float timeSinceLastJump;

        private void Awake()
        {
            // TODO: better targeting AI
            targetHealth = GameObject.FindGameObjectWithTag("Player").GetComponent<TangibleEntity>().Health;
        }

        private void Update()
        {
            if (!health || health.IsDead || !targetHealth || targetHealth.IsDead)
            {
                AimPosition = default;
                HorizontalMovement = default;
                WantsToAttack = false;
                WantsToTakeAim = false;

                OnJumpReleased?.Invoke();
                return;
            }

            timeSinceLastJump += Time.deltaTime;

            AimPosition = (Vector2)targetHealth.transform.position;
            Vector2 distanceToTarget = AimPosition.Value - (Vector2)transform.position;
            bool withinAttackingRange = distanceToTarget.sqrMagnitude < stopDistance * stopDistance;

            WantsToAttack = withinAttackingRange;
            HorizontalMovement = withinAttackingRange ? default : Mathf.Sign(distanceToTarget.x);

            // TODO: some enemies should be able to jump
            // TryJump(distanceToTarget.y);
        }

        private void TryJump(float heightDifference)
        {
            if (timeSinceLastJump < jumpCooldown) return;
            if (heightDifference <= heightThresholdToJump) return;

            timeSinceLastJump = 0f;
            OnJump?.Invoke();
        }
    }
}
