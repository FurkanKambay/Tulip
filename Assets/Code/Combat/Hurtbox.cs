using FK.Tulip.Combat.Data;
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

        public bool GetHit(IWeapon weapon, DamageType? damageType = null) =>
            strategyAsset && strategyAsset.Apply(owner, weapon, damageType);
    }
}
