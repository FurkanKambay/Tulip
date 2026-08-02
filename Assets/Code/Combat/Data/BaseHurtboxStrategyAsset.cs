using UnityEngine;

namespace FK.Tulip.Combat.Data
{
    public interface IHurtboxStrategy
    {
        bool Apply(Health victim, IWeapon weapon, DamageType? damageTypeOverride = null);
    }

    public abstract class BaseHurtboxStrategyAsset : ScriptableObject, IHurtboxStrategy
    {
        public abstract bool Apply(Health victim, IWeapon weapon, DamageType? damageTypeOverride = null);
    }
}
