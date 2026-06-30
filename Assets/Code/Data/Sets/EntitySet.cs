using UnityEngine;

namespace Tulip.Data.Sets
{
    [CreateAssetMenu(menuName = "Sets/Entity Set")]
    public class EntitySet : DataSet<EntitySO>
    {
        [ContextMenu("Populate with all assets of type " + nameof(EntitySO))]
        protected override void PopulateAllAssets() => base.PopulateAllAssets();
    }
}
