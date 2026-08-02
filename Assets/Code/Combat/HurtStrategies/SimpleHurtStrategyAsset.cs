using FK.Common.Extensions;
using UnityEngine;

namespace FK.Tulip.Combat
{
    [CreateAssetMenu(menuName = "Strategies/Get Hurt/Simple Damage", order = 1)]
    public class SimpleHurtStrategyAsset : BaseHurtStrategyAsset
    {
        [SerializeField, Min(0)] protected float damageMultiplier = 1;

        public override bool Apply(Health victim, IWeapon weapon, DamageType? damageTypeOverride = null)
        {
            if (!victim || weapon.Missing() || !weapon.Owner || !weapon.Asset)
                return false;

            float damageAmount = weapon.Asset.Damage * damageMultiplier;
            DamageType damageType = damageTypeOverride ?? weapon.Asset.DamageType;
            DamageResult result = victim.Damage(damageAmount, weapon.Owner, damageType);

            return result is DamageResult.Damaged or DamageResult.Killed;
        }
    }
}
