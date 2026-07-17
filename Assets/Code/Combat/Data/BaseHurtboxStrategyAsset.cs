using FK.Tulip.Data.Items;
using UnityEngine;

namespace FK.Tulip.Combat.Data
{
    public interface IHurtboxStrategy
    {
        public bool Apply(Health victim, WeaponAsset weapon, Health attacker, DamageType? damageTypeOverride = null);
    }

    public abstract class BaseHurtboxStrategyAsset : ScriptableObject, IHurtboxStrategy
    {
        public abstract bool Apply(Health victim, WeaponAsset weapon, Health attacker, DamageType? damageTypeOverride = null);
    }
}
