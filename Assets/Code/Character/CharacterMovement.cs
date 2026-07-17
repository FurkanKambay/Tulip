using System;
using FK.Common;
using FK.Tulip.Data.Gameplay;
using FK.Tulip.Input;
using UnityEngine;

namespace FK.Tulip.Character
{
    public class CharacterMovement : MonoBehaviour
    {
        [Header("Brain")]
        [SerializeField, Required] private CharacterBrain brain;

        [Header("References")]
        [SerializeField, Required] private Rigidbody2D body;
        [SerializeField, Required] private SurroundsChecker surrounds;

        [Header("Config")]
        public MovementConfig config;

        public Vector2 DesiredVelocity { get; private set; }
        public Vector2 Velocity => velocity;

        // Calculations
        private Vector2 velocity;
        private float maxSpeedChange;
        private float acceleration;
        private float deceleration;
        private float turnSpeed;

        // Current State
        private bool isGrounded;
        private bool hasAnyMovement;

        private void Update()
        {
            if (brain == null) return;
            hasAnyMovement = brain.HorizontalMovement != 0;

            bool isFacingObstacle = brain.HorizontalMovement < 0 ? surrounds.IsLeftBlocked : surrounds.IsRightBlocked;
            float velocityX = brain.HorizontalMovement * Mathf.Max(config.maxSpeed - config.friction, 0f);

            DesiredVelocity = isFacingObstacle ? Vector2.zero : new Vector2(velocityX, 0);
        }

        private void FixedUpdate()
        {
            isGrounded = surrounds.IsGrounded;
            velocity = body.linearVelocity;

            if (config.useAcceleration)
                RunWithAcceleration();
            else if (isGrounded)
                RunWithoutAcceleration();
            else
                RunWithAcceleration();
        }

        private void RunWithAcceleration()
        {
            acceleration = isGrounded ? config.maxAcceleration : config.maxAirAcceleration;
            deceleration = isGrounded ? config.maxDeceleration : config.maxAirDeceleration;
            turnSpeed = isGrounded ? config.maxTurnSpeed : config.maxAirTurnSpeed;

            maxSpeedChange = hasAnyMovement switch
            {
                true when Math.Sign(brain.HorizontalMovement) != Math.Sign(Velocity.x) => turnSpeed * Time.deltaTime,
                true => acceleration * Time.deltaTime,
                _ => deceleration * Time.deltaTime
            };

            velocity.x = Mathf.MoveTowards(Velocity.x, DesiredVelocity.x, maxSpeedChange);
            body.linearVelocity = velocity;
        }

        private void RunWithoutAcceleration()
        {
            velocity.x = DesiredVelocity.x;
            body.linearVelocity = velocity;
        }

        [Serializable]
        internal class AccelerationOptions
        {
            [Range(0f, 100f)] public float maxAcceleration = 50f;
            [Range(0f, 100f)] public float maxDeceleration = 50f;
            [Range(0f, 100f)] public float maxTurnSpeed = 80f;
        }
    }
}
