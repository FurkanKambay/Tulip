using UnityEngine;

namespace FK.Tulip.Combat
{
    [CreateAssetMenu(menuName = "Strategies/Get Hurt/Strategy Sequence", order = 100)]
    public class HurtStrategySequenceAsset : BaseHurtStrategyAsset, IHurtStrategy
    {
        [SerializeField] private BaseHurtStrategyAsset[] strategies;

        public override bool Apply(Health victim, IWeapon weapon, DamageType? damageTypeOverride = null)
        {
            bool allSucceeded = true;

            foreach (BaseHurtStrategyAsset strategy in strategies)
            {
                if (strategy == this) continue; // avoid recursions

                if (!strategy.Apply(victim, weapon, damageTypeOverride))
                    allSucceeded = false;
            }

            return allSucceeded;
        }
    }
}
