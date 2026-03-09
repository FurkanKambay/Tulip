using Tulip.Data.Items;
using UnityEngine;

namespace Tulip.Combat.Data
{
    [CreateAssetMenu(fileName = "Ignore Hurtbox Strategy", menuName = "Strategy/Hurtbox/Ignore", order = 0)]
    public class IgnoreHurtboxStrategySO : BaseHurtboxStrategySO
    {
        public override bool Apply(Health victim, WeaponSO weapon, Health attacker, DamageType? damageTypeOverride = null) => false;
    }
}
