using SaintsField.Playa;
using UnityEngine;

namespace Tulip.Data.Items
{
    [CreateAssetMenu(menuName = "World Tile", order = 1)]
    public class WorldTileSO : ScriptableObject
    {
        /// <summary>
        /// The LDtk IntGrid value index.
        /// </summary>
        public int TileIndex => tileIndex;

        public TileType TileType => tileType;
        public GroundMaterial Material => material;

        public bool IsUnsafe => isUnsafe;

        [Header("World Tile")]

        [Min(1), Tooltip("LDtk IntGrid value index (1-based). 0 is an empty cell.")]
        [SerializeField] protected int tileIndex;

        [Tooltip("What layer of the world does this tile belong to?")]
        [SerializeField] protected TileType tileType;

        [Header("Details")]
        [SerializeField] protected GroundMaterial material;
        [SerializeField] protected bool isUnsafe;

#region Static - World Tiles Cache

        [ShowInInspector]
        private static WorldTileSO[] worldTiles;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void Init()
        {
            WorldTileSO[] allTileAssets = Resources.FindObjectsOfTypeAll<WorldTileSO>();
            worldTiles = new WorldTileSO[allTileAssets.Length];

            foreach (WorldTileSO so in allTileAssets)
            {
                if (so.TileType == TileType.Block)
                    worldTiles[so.TileIndex - 1] = so;
            }
        }

        /// <summary>
        /// Get the tile with the 1-based LDtk IntGrid <see cref="TileIndex"/>.
        /// </summary>
        internal static WorldTileSO FromIndex(int index) =>
            index > 0 && index < worldTiles.Length ? worldTiles[index - 1] : null;

#endregion
    }
}
