using Furkan.Common;
using Tulip.Character;
using Tulip.Combat;
using Tulip.Input;
using UnityEngine;

namespace Tulip.AI
{
    public sealed class SimpleFlightAI : CharacterBrain
    {
        [Header("References")]
        [SerializeField, Required] Health health;

        [Header("Movement")]
        [SerializeField] Vector2 stopDistance;

        public override float HorizontalMovement { get; protected set; }
        public override float VerticalMovement { get; protected set; }
        public override Vector2? AimPosition { get; protected set; }
        public override bool WantsToAttack { get; protected set; }
        public override bool WantsToTakeAim { get; protected set; }
        public override bool WantsToHook { get; protected set; }

        private Health targetHealth;

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
                VerticalMovement = default;
                WantsToAttack = false;
                WantsToTakeAim = false;
                return;
            }

            AimPosition = (Vector2)targetHealth.transform.position;
            Vector2 targetVector = AimPosition.Value - (Vector2)transform.position;

            bool reachedX = Mathf.Abs(targetVector.x) < stopDistance.x;
            bool reachedY = Mathf.Abs(targetVector.y) < stopDistance.y;

            HorizontalMovement = reachedX ? default : targetVector.normalized.x;
            VerticalMovement = reachedY ? default : targetVector.normalized.y;
            WantsToAttack = reachedX && reachedY;
        }
    }
}
