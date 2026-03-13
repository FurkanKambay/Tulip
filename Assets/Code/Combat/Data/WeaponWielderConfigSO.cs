using UnityEngine;

namespace Tulip.Combat.Data
{
    [CreateAssetMenu(fileName = "Weapon Wielder Config", menuName = "Config/WeaponWielder", order = 0)]
    public class WeaponWielderConfigSO : ScriptableObject
    {
        [SerializeField, Min(0)] int maxHitsPerRaycast = 9;
        [SerializeField] ContactFilter2D hitContactFilter;

        public int MaxHitsPerRaycast => maxHitsPerRaycast;
        public ContactFilter2D HitContactFilter => hitContactFilter;
    }
}
