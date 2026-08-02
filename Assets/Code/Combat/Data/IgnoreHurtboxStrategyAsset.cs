using UnityEngine;

namespace FK.Tulip.Combat.Data
{
    [CreateAssetMenu(fileName = "Ignore Hurtbox Strategy", menuName = "Strategy/Hurtbox/Ignore", order = 0)]
    public class IgnoreHurtboxStrategyAsset : BaseHurtboxStrategyAsset
    {
        public override bool Apply(Health victim, IWeapon weapon, DamageType? damageTypeOverride = null) => false;
    }
}
