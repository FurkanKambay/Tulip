using Tulip.Data.Items;
using UnityEngine;

namespace Tulip.Combat.Data
{
    public interface IHurtboxStrategy
    {
        public bool Apply(Health victim, WeaponSO weapon, Health attacker, DamageType? damageTypeOverride = null);
    }

    public abstract class BaseHurtboxStrategySO : ScriptableObject, IHurtboxStrategy
    {
        public abstract bool Apply(Health victim, WeaponSO weapon, Health attacker, DamageType? damageTypeOverride = null);
    }
}
