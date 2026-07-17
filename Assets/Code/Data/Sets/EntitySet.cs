using UnityEngine;

namespace FK.Tulip.Data.Sets
{
    [CreateAssetMenu(menuName = "Sets/Entity Set")]
    public class EntitySet : DataSet<EntityAsset>
    {
        [ContextMenu("Populate with all assets of type " + nameof(EntityAsset))]
        protected override void PopulateAllAssets() => base.PopulateAllAssets();
    }
}
