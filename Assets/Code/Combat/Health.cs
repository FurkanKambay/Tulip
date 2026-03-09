using Furkan.Common;
using Tulip.Character;
using Tulip.Data;
using UnityEngine;

namespace Tulip.Combat
{
    [SelectionBase]
    public class Health : MonoBehaviour
    {
        public delegate void DamageEvent(CombatPacket combatPacket);
        public delegate void DeathEvent(CombatPacket combatPacket);
        public delegate void HealEvent(CombatPacket healPacket);
        public delegate void ReviveEvent(Health reviver);

        public event DamageEvent OnHurt;
        public event DeathEvent OnDie;
        public event HealEvent OnHeal;
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

        public float MaxHealth => maxHealth;
        public float InvulnerabilityDuration => invulnerabilityDuration;

        /// <summary>
        /// Remaining seconds of invulnerability.
        /// </summary>
        public float InvulnerabilityRemaining { get; private set; }

        public float Ratio => CurrentHealth / MaxHealth;
        public bool IsAlive => CurrentHealth > 0;
        public bool IsDead => CurrentHealth <= 0;
        public bool IsFull => CurrentHealth >= MaxHealth;
        public bool IsHurt => CurrentHealth < MaxHealth && !IsDead;
        public bool IsInvulnerable => InvulnerabilityRemaining > 0;

        public TangibleEntity Entity => entity;
        public Health LatestDamageSource { get; private set; }
        public Health LatestDeathSource { get; private set; }

        private void Update() =>
            InvulnerabilityRemaining = Mathf.Max(0, InvulnerabilityRemaining - Time.deltaTime);

        public InventoryModification Damage(float amount, Health source, DamageType damageType)
        {
            if (IsDead || amount < 0)
                return default;

            // Damage from status effects bypass invulnerability checks
            bool bypassInvulnerability = damageType is DamageType.StatusEffect;

            if (IsInvulnerable && !bypassInvulnerability)
                return default;

            CurrentHealth -= amount;
            LatestDamageSource = source;

            if (!bypassInvulnerability)
                InvulnerabilityRemaining = invulnerabilityDuration;

            Vector3 sourcePosition = source.Is(out Health sourceHealth)
                ? sourceHealth.transform.position
                : transform.position;

            var packet = new CombatPacket
            {
                Amount = amount,
                Source = source,
                Target = this,
                SourcePosition = sourcePosition,
                DamageType = damageType
            };

            OnHurt?.Invoke(packet);

            if (IsAlive)
                return default;

            LatestDeathSource = source;
            OnDie?.Invoke(packet);
            enabled = false;

            // TODO: fix whatever this is later
            EntitySO entitySO = Entity.EntitySO;

            if (!entitySO || !entitySO.Loot)
                return default;

            return InventoryModification.ToAdd(entitySO.Loot.Stack(entitySO.LootAmount));
        }

        public void Heal(float amount, Health source)
        {
            if (IsDead || amount < 0)
                return;

            CurrentHealth += amount;

            Vector3 sourcePosition = source.Is(out Health sourceHealth)
                ? sourceHealth.transform.position
                : transform.position;

            var healPacket = new CombatPacket
            {
                Amount = amount,
                Source = source,
                Target = this,
                SourcePosition = sourcePosition,
                DamageType = DamageType.StatusEffect
            };
            OnHeal?.Invoke(healPacket);
        }

        public void Revive(Health reviver = null)
        {
            CurrentHealth = maxHealth;
            enabled = true;
            OnRevive?.Invoke(reviver.Or(this));
        }

        private void OnValidate() => CurrentHealth = currentHealth;
    }
}
