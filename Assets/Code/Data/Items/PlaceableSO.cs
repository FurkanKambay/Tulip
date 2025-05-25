using SaintsField.Playa;
using Tulip.GameWorld;
using UnityEngine;

namespace Tulip.Data.Items
{
    [CreateAssetMenu(menuName = "Items/Placeable", order = 5)]
    public class PlaceableSO : BaseWorldToolSO
    {
        public Color Color => color;

        /// <summary>
        /// The LDtk IntGrid value index.
        /// </summary>
        public int TileIndex => tileIndex;

        public TileType TileType => tileType;
        public GroundMaterial Material => material;

        public bool IsUnsafe => isUnsafe;
        public bool IsUnbreakable => isUnbreakable;
        public int Hardness => hardness;
        public OreSO OreSO => oreSO;

        [Header("World Tile")]
        [SerializeField] protected Color color;

        [Tooltip("1-based LDtk IntGrid value index. 0 is an empty cell.")]
        [Min(1)]
        [SerializeField] protected int tileIndex;

        [SerializeField] protected TileType tileType;
        [SerializeField] protected GroundMaterial material;

        [SerializeField] protected bool isUnsafe;
        [SerializeField] protected bool isUnbreakable;

        [Min(1), PlayaDisableIf(nameof(isUnbreakable))]
        [SerializeField] protected int hardness = 50;

        [SerializeField] protected OreSO oreSO;

#region Static Placeables Cache

        [ShowInInspector]
        private static PlaceableSO[] placeables;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void Init()
        {
            PlaceableSO[] allPlaceableSOs = Resources.FindObjectsOfTypeAll<PlaceableSO>();
            placeables = new PlaceableSO[allPlaceableSOs.Length];

            foreach (PlaceableSO so in allPlaceableSOs)
            {
                if (so.TileType == TileType.Block)
                    placeables[so.TileIndex - 1] = so;
            }
        }

        /// <summary>
        /// Get the tile with the 1-based LDtk IntGrid <see cref="TileIndex"/>.
        /// </summary>
        internal static PlaceableSO FromIndex(int index) =>
            index > 0 && index < placeables.Length ? placeables[index - 1] : null;

#endregion

        private void Reset()
        {
            maxAmount = 999;
            cooldown = 0.25f;
        }
    }
}
