using SaintsField;
using Tulip.Character;
using Tulip.Data.Gameplay;
using UnityEngine;

namespace Tulip.Gameplay
{
    public class Knockbackable : MonoBehaviour
    {
        [Header("References")]
        [SerializeField, Required] Rigidbody2D body;
        [SerializeField, Required] Health health;

        [Header("Config")]
        [SerializeField] float hurtForceAmount;
        [SerializeField] float deathForceAmount;

        private void HandleHurt(CombatPacket combatPacket) =>
            ApplyKnockback(hurtForceAmount, combatPacket.SourcePosition);

        private void HandleDeath(CombatPacket combatPacket) =>
            ApplyKnockback(deathForceAmount, combatPacket.SourcePosition);

        private void HandleRevived(Health reviver) =>
            body.linearVelocity = Vector2.zero;

        private void ApplyKnockback(float forceAmount, Vector3 sourcePosition)
        {
            Vector3 direction = (transform.position - sourcePosition).normalized;
            body.linearVelocity = direction * forceAmount;
        }

        private void OnEnable()
        {
            health.OnHurt += HandleHurt;
            health.OnDie += HandleDeath;
            health.OnRevive += HandleRevived;
        }

        private void OnDisable()
        {
            health.OnHurt -= HandleHurt;
            health.OnDie -= HandleDeath;
            health.OnRevive -= HandleRevived;
        }
    }
}
