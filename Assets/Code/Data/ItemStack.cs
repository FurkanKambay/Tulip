using System;
using SaintsField;
using Tulip.Data.Items;
using Unity.Properties;
using UnityEngine;

namespace Tulip.Data
{
    [Serializable]
    public struct ItemStack
    {
        public ItemSO itemSO;
        public bool isLocked;

        [Min(0), MaxValue(nameof(MaxAmount))]
        [SerializeField] int amount;

        public int MaxAmount => itemSO ? itemSO.MaxAmount : 0;

        public int Amount
        {
            get => amount;
            set
            {
                amount = Mathf.Clamp(value, 0, MaxAmount);

                if (amount == 0 && !isLocked)
                    itemSO = null;
            }
        }

        [CreateProperty]
        public bool IsValid => itemSO && amount > 0;

        // ReSharper disable UnusedMember.Local
        [CreateProperty] bool ShowAmount => MaxAmount > 1;
        [CreateProperty] bool ShowIcon => isLocked || IsValid;
        [CreateProperty] float IconHeight => itemSO ? itemSO.IconScale * 24f : 0f;
        [CreateProperty] float IconOpacity => isLocked && amount == 0 ? 0.5f : 1f;
        // ReSharper restore UnusedMember.Local

        public ItemStack(ItemSO itemSO, int amount) : this()
        {
            this.itemSO = itemSO;
            this.amount = Mathf.Clamp(amount, 0, MaxAmount);
        }

        public ItemStack(ItemStack other) : this(other.itemSO, other.Amount) { }

        public override string ToString() => $"{Amount} {itemSO}";
    }
}
