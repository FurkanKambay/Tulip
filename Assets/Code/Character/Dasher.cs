using System;
using Furkan.Common;
using Tulip.Data;
using UnityEngine;

namespace Tulip.Character
{
    public class Dasher : MonoBehaviour
    {
        public event Action OnDash;

        [Header("Brain")]
        [SerializeField, Required] SaintsInterface<Component, IDasherBrain> brain;

        [Header("References")]
        [SerializeField, Required] Rigidbody2D body;

        [Header("Config")]
        public float dashSpeed = 10f;
        public float dashCooldown = 0.5f;
        [SerializeField] ForceMode2D forceMode;

        private float timeSinceLastDash;

        private void Update()
        {
            timeSinceLastDash += Time.deltaTime;

            if (brain.I.WantsToDash && timeSinceLastDash >= dashCooldown)
                Dash();
        }

        private void Dash()
        {
            float direction = brain.I.HorizontalMovement;

            if (Mathf.Abs(direction) < 0.1f)
                return;

            timeSinceLastDash = 0f;
            body.AddForce(Vector2.right * (direction * dashSpeed), forceMode);
            OnDash?.Invoke();
        }
    }
}
