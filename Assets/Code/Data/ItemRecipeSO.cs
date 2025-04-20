using Tulip.Data.Items;
using UnityEngine;

namespace Tulip.Data
{
    [CreateAssetMenu(menuName = "Player/Item Recipe")]
    public class ItemRecipeSO : ScriptableObject
    {
        public ItemSO ResultItemSO => resultItemSO;
        public ItemStack[] Ingredients => ingredients;

        [SerializeField] protected ItemSO resultItemSO;
        [SerializeField] protected ItemStack[] ingredients;
    }
}
