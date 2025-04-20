using System;
using SaintsField;
using UnityEngine;

namespace Tulip.Data
{
    [CreateAssetMenu(menuName = "Player/Inventory Preset")]
    public class InventorySO : ScriptableObject, IValidate
    {
        public int Capacity => capacity;
        public ItemStack[] Inventory => inventory;

        [SerializeField] int capacity = 9;

        [SaintsRow(inline: true)]
        [SerializeField] ItemStack[] inventory;

        public void OnValidate()
        {
            if (inventory.Length > capacity)
                Array.Resize(ref inventory, capacity);
        }
    }
}
