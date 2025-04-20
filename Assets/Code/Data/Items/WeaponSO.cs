using UnityEngine;

namespace Tulip.Data.Items
{
    [CreateAssetMenu(menuName = "Items/Weapon", order = 3)]
    public class WeaponSO : UsableSO
    {
        public float Damage => damage;
        public float Range => range;
        public bool IsMultiTarget => isMultiTarget;

        [Header("Weapon")]
        [SerializeField, Min(0)] protected float damage = 1f;
        [SerializeField, Min(0)] protected float range = 1f;
        [SerializeField] protected bool isMultiTarget;
    }
}
