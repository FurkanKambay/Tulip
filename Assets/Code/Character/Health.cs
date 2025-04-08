using Furkan.Common;
using Tulip.Data;
using Tulip.Data.Gameplay;
using UnityEngine;

namespace Tulip.Character
{
    [SelectionBase]
    public class Health : MonoBehaviour
    {
        public delegate void DamageEvent(HealthChangeEventArgs damage);
        public delegate void DeathEvent(HealthChangeEventArgs damage);
        public delegate void HealEvent(HealthChangeEventArgs healing);
        public delegate void ReviveEvent(Health reviver);

        public event DamageEvent OnHurt;
        public event DeathEvent  OnDie;
        public event HealEvent   OnHeal;
        public event ReviveEvent OnRevive;

        [Header("References")]
        [SerializeField] TangibleEntity entity;

        [Header("Config")]
        [SerializeField, Min(0)] float maxHealth = 100f;
        [SerializeField, Min(0)] float currentHealth = 100f;
        [SerializeField, Min(0)] float invulnerabilityDuration;

        public float CurrentHealth
        {
            get => currentHealth;
            private set => currentHealth = Mathf.Clamp(value, 0, MaxHealth);
        }

        public float MaxHealth               => maxHealth;
        public float InvulnerabilityDuration => invulnerabilityDuration;

        /// <summary>
        /// Remaining seconds of invulnerability.
        /// </summary>
        public float InvulnerabilityRemaining { get; private set; }

        public float Ratio          => CurrentHealth / MaxHealth;
        public bool  IsAlive        => CurrentHealth > 0;
        public bool  IsDead         => CurrentHealth <= 0;
        public bool  IsFull         => CurrentHealth >= MaxHealth;
        public bool  IsHurt         => CurrentHealth < MaxHealth && !IsDead;
        public bool  IsInvulnerable => InvulnerabilityRemaining > 0;

        public TangibleEntity Entity             => entity;
        public Health         LatestDamageSource { get; private set; }
        public Health         LatestDeathSource  { get; private set; }

        private void Update() =>
            InvulnerabilityRemaining = Mathf.Max(0, InvulnerabilityRemaining - Time.deltaTime);

        public InventoryModification Damage(float amount, Health source, bool checkInvulnerable = true)
        {
            if (IsDead || amount < 0)
                return default;

            if (checkInvulnerable && IsInvulnerable)
                return default;

            CurrentHealth      -= amount;
            LatestDamageSource =  source;

            if (checkInvulnerable)
                InvulnerabilityRemaining = invulnerabilityDuration;

            Vector3 sourcePosition = source.Is(out Health sourceHealth)
                ? sourceHealth!.transform.position
                : transform.position;

            var damageArgs = new HealthChangeEventArgs(amount, source, this, sourcePosition);
            OnHurt?.Invoke(damageArgs);

            if (IsAlive)
                return default;

            LatestDeathSource = source;
            OnDie?.Invoke(damageArgs);
            enabled = false;

            // TODO: fix whatever this is later
            EntityData entityData = Entity.EntityData;

            if (!entityData || !entityData.Loot)
                return default;

            return InventoryModification.ToAdd(entityData.Loot.Stack(entityData.LootAmount));
        }

        public void Heal(float amount, Health source)
        {
            if (IsDead || amount < 0)
                return;

            CurrentHealth += amount;

            Vector3 sourcePosition = source.Is(out Health sourceHealth)
                ? sourceHealth!.transform.position
                : transform.position;

            var healArgs = new HealthChangeEventArgs(amount, source, this, sourcePosition);
            OnHeal?.Invoke(healArgs);
        }

        public void Revive(Health reviver = null)
        {
            CurrentHealth = maxHealth;
            enabled       = true;
            OnRevive?.Invoke(reviver.Or(this));
        }

        private void OnValidate() => CurrentHealth = currentHealth;
    }
}
