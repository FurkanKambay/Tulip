using Tulip.Combat.Data;
using Tulip.Data.Items;
using UnityEngine;

namespace Tulip.Combat
{
    public class Hurtbox : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private Health owner;

        [Header("Config")]
        [SerializeField] private BaseHurtboxStrategySO strategySO;

        public Health Owner => owner;

        public bool GetHit(WeaponSO weapon, Health attacker, DamageType? damageType = null) =>
            strategySO && strategySO.Apply(owner, weapon, attacker, damageType);
    }
}
