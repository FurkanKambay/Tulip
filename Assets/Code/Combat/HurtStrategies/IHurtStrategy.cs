using UnityEngine;

namespace FK.Tulip.Combat
{
    public interface IHurtStrategy
    {
        bool Apply(Health victim, IWeapon weapon, DamageType? damageTypeOverride = null);
    }

    public abstract class BaseHurtStrategyAsset : ScriptableObject, IHurtStrategy
    {
        public abstract bool Apply(Health victim, IWeapon weapon, DamageType? damageTypeOverride = null);
    }
}
