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

        [Header("Usable Data")]
        [SerializeField, Min(0)] protected float cooldown = 0.5f;

        [BelowRichLabel(nameof(SwingTypeLabel), isCallback: true)]
        [SerializeField] protected ItemSwingConfig swingConfig;

        private string SwingTypeLabel() => $"<color=gray>Time to first hit:</color> {SwingConfig.TimeToFirstHit} sec";
    }
}
