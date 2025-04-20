using System;
using System.Linq;
using Tulip.Data;
using Tulip.Data.Items;
using UnityEngine;

namespace Tulip.Character
{
    public sealed class Inventory : InventoryBase
    {
        // TODO: provide affected indexes
        public override event Action OnModify;

        [Header("Config")]
        [SerializeField] InventorySO inventorySO;
        [SerializeField, Min(0)] int capacity = 9;

        public override int Capacity => capacity;
        public override ItemStack[] Items { get; protected set; }

        private void Awake()
        {
            ItemStack[] startingInventory = inventorySO.Inventory.ToArray();
            Array.Resize(ref startingInventory, capacity);

            Items = startingInventory;
        }

        /// <summary>
        /// Applies the <see cref="InventoryModification"/> to the inventory.
        /// </summary>
        /// <param name="modification">The item stack to remove from / add to the inventory.</param>
        /// <returns>The remaining item stack.</returns>
        public InventoryModification ApplyModification(InventoryModification modification)
        {
            if (!modification.IsValid)
                return default;

            int remainder = modification.WouldAdd
                ? AddItem(modification.Stack)
                : RemoveItem(modification.Stack);

            ItemStack remainingStack = modification.Stack.itemSO.Stack(remainder);

            return modification.WouldAdd
                ? InventoryModification.ToAdd(remainingStack)
                : InventoryModification.ToRemove(remainingStack);
        }

        private int RemoveItem(ItemStack itemStack)
        {
            if (!itemStack.IsValid)
                return 0;

            int remaining = itemStack.Amount;

            while (remaining > 0)
            {
                // TODO: first remove from selected hotbar slot
                int? foundIndex = GetFirstSlotWith(itemStack.itemSO, intentToRemove: true);

                if (!foundIndex.HasValue)
                    break;

                ItemStack foundStack = Items[foundIndex.Value];
                int oldAmount = foundStack.Amount;

                int newAmount = Items[foundIndex.Value].Amount -= remaining;
                remaining -= oldAmount - newAmount;
            }

            if (remaining != itemStack.Amount)
                OnModify?.Invoke();

            return remaining;
        }

        private int AddItem(ItemStack itemStack)
        {
            if (!itemStack.IsValid)
                return 0;

            int remaining = itemStack.Amount;

            while (remaining > 0)
            {
                int? foundIndex = GetFirstSlotWith(itemStack.itemSO, intentToRemove: false);

                if (!foundIndex.HasValue)
                {
                    foundIndex = CreateNewStackWith(itemStack.itemSO);

                    if (!foundIndex.HasValue)
                    {
                        // Inventory is full
                        break;
                    }
                }

                remaining = AddToExistingSlot(foundIndex.Value, remaining);
            }

            if (remaining != itemStack.Amount)
                OnModify?.Invoke();

            return remaining;
        }

        private int AddToExistingSlot(int stackIndex, int amount)
        {
            ItemStack stack = Items[stackIndex];
            int wouldTotal = stack.Amount + amount;

            Items[stackIndex].Amount += amount;

            bool hasOverflow = wouldTotal > stack.itemSO.MaxAmount;
            int overflowAmount = wouldTotal - stack.itemSO.MaxAmount;
            return hasOverflow ? overflowAmount : 0;
        }

        private int? CreateNewStackWith(ItemSO itemSO)
        {
            int? firstEmptyIndex = GetFirstEmptySlot();

            if (!firstEmptyIndex.HasValue)
                return null;

            Items[firstEmptyIndex.Value] = new ItemStack(itemSO, 0);
            return firstEmptyIndex;
        }

        private int? GetFirstSlotWith(ItemSO itemSO, bool intentToRemove)
        {
            if (!itemSO)
                return null;

            for (int itemIndex = 0; itemIndex < Items.Length; itemIndex++)
            {
                ItemStack currentItem = Items[itemIndex];

                if (currentItem.itemSO != itemSO)
                    continue;

                // don't allow full stacks when adding
                if (!intentToRemove && currentItem.Amount >= itemSO.MaxAmount)
                    continue;

                // don't allow empty stacks when removing
                if (intentToRemove && currentItem.Amount < 1)
                    continue;

                return itemIndex;
            }

            return null;
        }

        private int? GetFirstEmptySlot()
        {
            for (int i = 0; i < Items.Length; i++)
            {
                if (!Items[i].itemSO)
                    return i;
            }

            return null;
        }
    }
}
