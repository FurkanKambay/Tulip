using UnityEngine;

namespace FK.Tulip.Combat
{
    [CreateAssetMenu(menuName = "Strategies/Get Hurt/Ignore", order = 0)]
    public class IgnoreHurtStrategyAsset : BaseHurtStrategyAsset
    {
        public override bool Apply(Health victim, IWeapon weapon, DamageType? damageTypeOverride = null) => false;
    }
}
