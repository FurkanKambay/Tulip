using UnityEngine;

namespace FK.Tulip.Data.Items
{
    [CreateAssetMenu(menuName = "Items/Hook", order = 3)]
    public class HookAsset : UsableAsset
    {
        public float Range => range;
        public float RopeLaunchSpeed => ropeLaunchSpeed;
        public float PullStrength => pullStrength;

        [Header("Hook")]
        [SerializeField, Min(0)] private float range;
        [SerializeField, Min(0)] private float ropeLaunchSpeed;
        [SerializeField, Min(0)] private float pullStrength;
    }
}
