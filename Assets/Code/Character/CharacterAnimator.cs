using Tulip.Data.Gameplay;
using UnityEngine;

namespace Tulip.Character
{
    public class CharacterAnimator : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] protected Health health;
        [SerializeField] protected Animator animator;

        private static readonly int animHurt = Animator.StringToHash("hurt");
        private static readonly int animDead = Animator.StringToHash("dead");

        private void OnEnable()
        {
            health.OnHurt += HandleHurt;
            health.OnDie += HandleDied;
            health.OnRevive += HandleRevived;
        }

        private void OnDisable()
        {
            health.OnHurt -= HandleHurt;
            health.OnDie -= HandleDied;
            health.OnRevive -= HandleRevived;
        }

        private void HandleHurt(HealthChangeEventArgs damage) => animator.SetTrigger(animHurt);
        private void HandleDied(HealthChangeEventArgs damage) => animator.SetBool(animDead, true);
        private void HandleRevived(Health reviver) => animator.SetBool(animDead, false);
    }
}
