using FK.Tulip.Combat.Data;
using UnityEngine;

namespace FK.Tulip.Combat
{
    /// <summary>
    /// Accompanies a <see cref="Collider2D"/> on an entity and holds a <see cref="BaseHurtStrategyAsset"/> to use when hit.
    /// </summary>
    /// <remarks>Multiple components may be used on a single entity, e.g. for different body parts.</remarks>
    /// <seealso cref="IHurtStrategy"/>
    public class Hurtbox : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private Health owner;

        [Header("Config")]
        [SerializeField] private BaseHurtStrategyAsset strategyAsset;

        public Health Owner => owner;

        public bool GetHit(IWeapon weapon, DamageType? damageType = null) =>
            strategyAsset && strategyAsset.Apply(owner, weapon, damageType);
    }
}
