using UnityEngine;

namespace FK.Tulip.Data.Sets
{
    [CreateAssetMenu(menuName = "Sets/Entity Set")]
    public class EntitySet : DataSet<EntityAsset>
    {
#if UNITY_EDITOR
        [ContextMenu("Populate with all assets of type " + nameof(EntityAsset))]
        protected override void PopulateAllAssets() => base.PopulateAllAssets();
#endif
    }
}
