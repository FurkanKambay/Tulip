using System.Linq;
using Furkan.Common;
using UnityEngine;
using Vertx.Attributes;

namespace Tulip.Data.Items
{
    [CreateAssetMenu(menuName = "Items/Ore", order = 1)]
    public class OreAsset : ItemAsset
    {
        public GameObject Prefab => prefab;

        [Header("Ore")]
        [SerializeField] GameObject prefab;

        // ReSharper disable NotAccessedField.Global
        [LayoutGroup("Referenced By", ELayout.Background | ELayout.TitleOut | ELayout.Foldout)]
        [SerializeField, ReadOnlyField] protected EntityAsset[] entityLoot;
        // ReSharper restore NotAccessedField.Global

        protected override void OnValidate()
        {
            base.OnValidate();

            entityLoot = Resources.FindObjectsOfTypeAll<EntityAsset>()
                .Where(entityAsset => entityAsset.Loot == this)
                .ToArray();
        }
    }
}
