using System.Linq;
using SaintsField;
using SaintsField.Playa;
using UnityEngine;

namespace Tulip.Data.Items
{
    [CreateAssetMenu(menuName = "Items/Ore", order = 1)]
    public class OreSO : ItemSO
    {
        public GameObject Prefab => prefab;

        [Header("Ore")]
        [SerializeField] GameObject prefab;

        // ReSharper disable NotAccessedField.Global
        [LayoutGroup("Referenced By", ELayout.Background | ELayout.TitleOut | ELayout.Foldout, marginTop: 16)]
        [SerializeField, ReadOnly] protected EntitySO[] entityLoot;
        // ReSharper restore NotAccessedField.Global

        protected override void OnValidate()
        {
            base.OnValidate();

            entityLoot = Resources.FindObjectsOfTypeAll<EntitySO>()
                .Where(entitySO => entitySO.Loot == this)
                .ToArray();
        }
    }
}
