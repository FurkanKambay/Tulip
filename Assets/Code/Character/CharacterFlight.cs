using Furkan.Common;
using Tulip.Data;
using Tulip.Data.Gameplay;
using UnityEngine;

namespace Tulip.Character
{
    public class CharacterFlight : MonoBehaviour, ICharacterMovement
    {
        [Header("Brain")]
        [SerializeField, Required] SaintsInterface<Component, IFlightBrain> brain;

        [Header("References")]
        [SerializeField, Required] Rigidbody2D body;

        [Header("Config")]
        public MovementConfig config;

        public Vector2 DesiredVelocity { get; private set; }
        public Vector2 Velocity { get; private set; }

        private bool hasAnyMovement;

        private void Update()
        {
            // TODO: float up & down
            var movement = new Vector2(brain.I.HorizontalMovement, brain.I.VerticalMovement);
            DesiredVelocity = movement * Mathf.Max(config.maxSpeed - config.friction, 0f);
        }

        private void FixedUpdate()
        {
            Velocity = body.linearVelocity;

            if (config.useAcceleration)
                RunWithAcceleration();
            else
                RunWithoutAcceleration();
        }

        private void RunWithAcceleration()
        {
            float acceleration = config.maxAirAcceleration;
            float deceleration = config.maxAirDeceleration;

            float maxSpeedChange = hasAnyMovement switch
            {
                true => acceleration * Time.deltaTime,
                _ => deceleration * Time.deltaTime
            };

            Velocity = Vector2.MoveTowards(Velocity, DesiredVelocity, maxSpeedChange);
            body.linearVelocity = Velocity;
        }

        private void RunWithoutAcceleration()
        {
            Velocity = DesiredVelocity;
            body.linearVelocity = Velocity;
        }
    }
}
