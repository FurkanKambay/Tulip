using UnityEngine;

namespace FK.Tulip.Combat.Data
{
    [CreateAssetMenu(fileName = "Weapon Wielder Config", menuName = "Config/WeaponWielder", order = 0)]
    public class WeaponWielderConfigAsset : ScriptableObject
    {
        [SerializeField, Min(0)] private int maxHitsPerRaycast = 9;
        [SerializeField] private ContactFilter2D hitContactFilter;

        public int MaxHitsPerRaycast => maxHitsPerRaycast;
        public ContactFilter2D HitContactFilter => hitContactFilter;
    }
}
