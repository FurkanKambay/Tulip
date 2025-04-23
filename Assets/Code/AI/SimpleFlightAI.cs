using SaintsField;
using Tulip.Character;
using Tulip.Data;
using UnityEngine;

namespace Tulip.AI
{
    public sealed class SimpleFlightAI : MonoBehaviour, IFlightBrain
    {
        [Header("References")]
        [SerializeField, Required] Health health;

        [Header("Movement")]
        [SerializeField] Vector2 stopDistance;

        public float HorizontalMovement { get; private set; }
        public float VerticalMovement { get; private set; }
        public Vector2? AimPosition { get; private set; }
        public bool WantsToUse { get; private set; }
        public bool WantsToThrow { get; private set; }
        public bool WantsToHook { get; private set; }

        private Health targetHealth;

        private void Awake()
        {
            // TODO: better targeting AI
            targetHealth = GameObject.FindGameObjectWithTag("Player").GetComponentInChildren<Health>();
        }

        private void Update()
        {
            if (!health || health.IsDead || !targetHealth || targetHealth.IsDead)
            {
                AimPosition = default;
                HorizontalMovement = default;
                VerticalMovement = default;
                WantsToUse = false;
                WantsToThrow = false;
                return;
            }

            AimPosition = (Vector2)targetHealth.transform.position;
            Vector2 targetVector = AimPosition.Value - (Vector2)transform.position;

            bool reachedX = Mathf.Abs(targetVector.x) < stopDistance.x;
            bool reachedY = Mathf.Abs(targetVector.y) < stopDistance.y;

            HorizontalMovement = reachedX ? default : targetVector.normalized.x;
            VerticalMovement = reachedY ? default : targetVector.normalized.y;
            WantsToUse = reachedX && reachedY;
        }
    }
}
