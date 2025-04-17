using SaintsField;
using UnityEngine;

namespace Tulip.Data.Items
{
    /// <summary>
    /// A basic item that can be used.
    /// </summary>
    [CreateAssetMenu(menuName = "Items/Usable", order = 2)]
    public class UsableData : ItemData
    {
        public float Cooldown => cooldown;
        public float ThrowCooldown => throwCooldown;
        public float ThrowStrength => throwStrength;
        public float AimChargeSpeed => aimChargeSpeed;
        public ItemSwingConfig SwingConfig => swingConfig;

        [Header("Usable Data")]
        [SerializeField, Min(0)] protected float cooldown = 0.5f;
        [SerializeField, Min(0)] protected float throwCooldown;
        [SerializeField, Min(0)] protected float throwStrength;
        [SerializeField, Min(0)] protected float aimChargeSpeed;

        [BelowRichLabel(nameof(SwingTypeLabel), isCallback: true)]
        [SerializeField] protected ItemSwingConfig swingConfig;

        private string SwingTypeLabel() => $"<color=gray>Time to first hit:</color> {SwingConfig.TimeToFirstHit} sec";
    }
}
