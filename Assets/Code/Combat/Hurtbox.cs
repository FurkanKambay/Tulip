using FK.Tulip.Combat.Data;
using FK.Tulip.Data.Items;
using UnityEngine;

namespace FK.Tulip.Combat
{
    public class Hurtbox : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private Health owner;

        [Header("Config")]
        [SerializeField] private BaseHurtboxStrategyAsset strategyAsset;

        public Health Owner => owner;

        public bool GetHit(WeaponAsset weapon, Health attacker, DamageType? damageType = null) =>
            strategyAsset && strategyAsset.Apply(owner, weapon, attacker, damageType);
    }
}
