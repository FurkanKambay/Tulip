using Tulip.Data.Items;
using UnityEngine;

namespace Tulip.Combat.Data
{
    [CreateAssetMenu(fileName = "Damage Hurtbox Strategy", menuName = "Strategy/Hurtbox/Damage", order = 0)]
    public class DamageHurtboxStrategySO : BaseHurtboxStrategySO
    {
        [SerializeField, Min(0)] protected float damageMultiplier = 1;

        public override bool Apply(Health victim, WeaponSO weapon, Health attacker, DamageType? damageTypeOverride = null)
        {
            if (!victim || !weapon || !attacker)
                return false;

            float healthBefore = victim.CurrentHealth;
            // HACK: Health.Damage() needs to return a DamageResult

            victim.Damage(weapon.Damage, attacker, damageTypeOverride ?? weapon.DamageType);
            return !Mathf.Approximately(healthBefore, victim.CurrentHealth);
        }
    }
}
