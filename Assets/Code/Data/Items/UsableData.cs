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
        public ItemSwingConfig SwingConfig => swingConfig;

        public bool IsThrowable => isThrowable;
        public float ThrowCooldown => throwableConfig.cooldown;
        public float ThrowStrength => throwableConfig.strength;
        public float AimChargeSpeed => throwableConfig.chargeSpeed;

        [Header("Usable Data")]
        [SerializeField, Min(0)] protected float cooldown = 0.5f;

        [SerializeField] protected bool isThrowable;

        [ShowIf(nameof(isThrowable))]
        [SerializeField] protected ThrowableConfig throwableConfig;

        [BelowRichLabel(nameof(SwingTypeLabel), isCallback: true)]
        [SerializeField] protected ItemSwingConfig swingConfig;

        private string SwingTypeLabel() => $"<color=gray>Time to first hit:</color> {SwingConfig.TimeToFirstHit} sec";
    }
}
