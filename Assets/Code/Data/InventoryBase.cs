using System;
using UnityEngine;

namespace Tulip.Data
{
    public abstract class InventoryBase : MonoBehaviour
    {
        public abstract event Action OnModify;

        public abstract int Capacity { get; }
        public abstract ItemStack[] Items { get; protected set; }

        public virtual ItemStack this[int index] =>
            Items == null || index < 0 || index >= Items.Length ? default : Items[index];

        public virtual ItemStack[] this[Range range] =>
            Items == null || range.Start.Value < 0 || range.End.Value >= Items.Length
                ? Array.Empty<ItemStack>()
                : Items[range];
    }
}
