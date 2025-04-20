using System.Linq;
using SaintsField;
using SaintsField.Playa;
using Tulip.Gameplay;
using UnityEngine;

namespace Tulip.Data.Items
{
    /// <summary>
    /// A base item that can be stored in an inventory.
    /// </summary>
    [CreateAssetMenu(menuName = "Items/Item", order = 0)]
    public class ItemSO : ScriptableObject
    {
        public virtual Sprite Icon => icon;
        public virtual float IconScale => iconScale;
        public virtual string Name => name;
        public virtual string Description => description;
        public virtual int MaxAmount => maxAmount;

        [Header("Item")]
        [AssetPreview(width: 64, align: EAlign.FieldStart)]
        [SerializeField] protected Sprite icon;

        [SerializeField] protected float iconScale = 1f;
        [SerializeField] protected new string name;
        [SerializeField, Multiline] protected string description;
        [SerializeField, Min(1)] protected int maxAmount = 1;

        // ReSharper disable NotAccessedField.Global
        [LayoutGroup("Referenced By", ELayout.Background | ELayout.TitleOut | ELayout.Foldout, marginTop: 16)]
        [SerializeField, ReadOnly] protected ItemRecipeSO[] craftedBy;
        [SerializeField, ReadOnly] protected ItemRecipeSO[] usedInCrafting;
        // ReSharper restore NotAccessedField.Global

        public ItemStack Stack(int amount) => new(this, amount);

        public override string ToString() => Name;

        protected virtual void OnValidate()
        {
            craftedBy = Resources.FindObjectsOfTypeAll<ItemRecipeSO>()
                .Where(recipeSO => recipeSO.ResultItemSO == this)
                .ToArray();

            usedInCrafting = Resources.FindObjectsOfTypeAll<ItemRecipeSO>()
                .Where(recipeSO => recipeSO.Ingredients.Any(stack => stack.itemSO == this))
                .ToArray();
        }
    }
}
