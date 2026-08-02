using FK.Common.Extensions;
using UnityEngine;

namespace FK.Tulip.Combat.Data
{
    [CreateAssetMenu(fileName = "Damage Hurtbox Strategy", menuName = "Strategy/Hurtbox/Damage", order = 0)]
    public class DamageHurtboxStrategyAsset : BaseHurtboxStrategyAsset
    {
        [SerializeField, Min(0)] protected float damageMultiplier = 1;

        public override bool Apply(Health victim, IWeapon weapon, DamageType? damageTypeOverride = null)
        {
            if (!victim || weapon.Missing() || !weapon.Owner || !weapon.Asset)
                return false;

            float healthBefore = victim.CurrentHealth;
            // HACK: Health.Damage() needs to return a DamageResult

            victim.Damage(weapon.Asset.Damage, weapon.Owner, damageTypeOverride ?? weapon.Asset.DamageType);
            return !Mathf.Approximately(healthBefore, victim.CurrentHealth);
        }
    }
}
