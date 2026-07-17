using System;
using FK.Common;
using FK.Tulip.Input;
using UnityEngine;

namespace FK.Tulip.Character
{
    public class Dasher : MonoBehaviour
    {
        public event Action OnDash;

        [Header("Brain")]
        [SerializeField, Required] private CharacterBrain brain;

        [Header("References")]
        [SerializeField, Required] private Rigidbody2D body;

        [Header("Config")]
        public float dashSpeed = 10f;
        public float dashCooldown = 0.5f;
        [SerializeField] private ForceMode2D forceMode;

        private float timeSinceLastDash;

        private void Update()
        {
            timeSinceLastDash += Time.deltaTime;

            if (brain.WantsToDash && timeSinceLastDash >= dashCooldown)
                Dash();
        }

        private void Dash()
        {
            float direction = brain.HorizontalMovement;

            if (Mathf.Abs(direction) < 0.1f)
                return;

            timeSinceLastDash = 0f;
            body.AddForce(Vector2.right * (direction * dashSpeed), forceMode);
            OnDash?.Invoke();
        }
    }
}
