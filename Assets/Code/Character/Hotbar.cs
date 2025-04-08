using System;
using SaintsField;
using Tulip.Data;
using UnityEngine;

namespace Tulip.Character
{
    public class Hotbar : MonoBehaviour
    {
        public event Action OnModify;
        public event Action<int> OnChangeSelection;

        [Header("References")]
        [SerializeField, Required] InventoryBase inventory;

        [Header("Config")]
        [SerializeField, Min(0)] int size = 9;

        public int Size => size;
        public ItemStack[] Items => inventory[..Size];
        public ItemStack SelectedStack => this[SelectedIndex];

        public int SelectedIndex
        {
            get => selectedIndex;
            private set => selectedIndex = Mathf.Clamp(value, 0, Size - 1);
        }

        public ItemStack this[int index] => index >= Items.Length ? default : Items[index];

        private int selectedIndex;

        private void Awake()
        {
            OnModify?.Invoke();
            OnChangeSelection?.Invoke(SelectedIndex);
        }

        public void Select(int index)
        {
            if (index == SelectedIndex)
                return;

            SelectedIndex = index;
            OnChangeSelection?.Invoke(SelectedIndex);
        }

        private void OnEnable() => inventory.OnModify += HandleInventoryModified;
        private void OnDisable() => inventory.OnModify -= HandleInventoryModified;

        private void HandleInventoryModified() => OnModify?.Invoke();

        private void OnValidate() =>
            size = Mathf.Clamp(size, 0, inventory.Capacity);
    }
}
