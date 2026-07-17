using FK.Tulip.Combat;
using UnityEngine;

namespace FK.Tulip.Data.Items
{
    [CreateAssetMenu(menuName = "Items/Weapon", order = 3)]
    public class WeaponAsset : UsableAsset
    {
        public DamageType DamageType => damageType;
        public float Damage => damage;
        public float Range => range;
        public bool IsMultiTarget => isMultiTarget;

        [Header("Weapon")]
        [SerializeField] protected DamageType damageType;
        [SerializeField, Min(0)] protected float damage = 1f;
        [SerializeField, Min(0)] protected float range = 1f;
        [SerializeField] protected bool isMultiTarget;
    }
}
