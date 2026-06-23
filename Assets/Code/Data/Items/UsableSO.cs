using Furkan.Common;
using UnityEngine;

namespace Tulip.Data.Items
{
    /// <summary>
    /// A basic item that can be used.
    /// </summary>
    [CreateAssetMenu(menuName = "Items/Usable", order = 2)]
    public class UsableSO : ItemSO
    {
        public float Cooldown => cooldown;
        public ItemSwingConfig SwingConfig => swingConfig;

        public bool IsThrowable => isThrowable;

        /// <seealso cref="IsThrowable"/>
        public float ThrowCooldown => throwableConfig.cooldown;

        /// <seealso cref="IsThrowable"/>
        public float ThrowStrength => throwableConfig.strength;

        /// <seealso cref="IsThrowable"/>
        public float ThrowChargeSpeed => throwableConfig.chargeSpeed;

        [Header("Usable")]
        [SerializeField, Min(0)] protected float cooldown = 0.5f;

        [SerializeField] protected bool isThrowable;

        [ShowIf(nameof(isThrowable))]
        [SerializeField] protected ThrowableConfig throwableConfig;

        [SerializeField] protected ItemSwingConfig swingConfig;

        // TODO: show this value in inspector
        private string SwingTypeLabel() => $"<color=gray>Time to first hit:</color> {SwingConfig.TimeToFirstHit} sec";
    }
}
