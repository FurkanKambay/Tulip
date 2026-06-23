using Furkan.Common;
using UnityEngine;

namespace Tulip.Combat
{
    [CreateAssetMenu(menuName = "Gameplay/Status Effect")]
    public class StatusEffectSO : ScriptableObject
    {
        public bool IsPermanent => isPermanent;
        public float Duration => duration;
        public float Amount => amount;
        public float Rate => rate;

        [Header("Duration")]
        [SerializeField] bool isPermanent;

        [DisableIf(nameof(isPermanent))]
        [SerializeField, Min(0)] float duration;

        [Header("Rate")]
        [SerializeField] float amount;
        [SerializeField, Min(0.01f)] float rate;

        public StatusEffect Create(Health source, Health target) => new(this, source, target);

        // TODO: show this value in inspector
        private string AmountPerSecond => $"<color=green>{amount / rate} per second</color>";
    }
}
