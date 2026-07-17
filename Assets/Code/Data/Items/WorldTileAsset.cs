using FK.Common;
using UnityEngine;

namespace FK.Tulip.Data.Items
{
    [CreateAssetMenu(menuName = "World Tile", order = 1)]
    public class WorldTileAsset : ScriptableObject
    {
        /// <summary>
        /// The tile ID.
        /// </summary>
        public int TileIndex => tileIndex;

        public TileType TileType => tileType;
        public GroundMaterial Material => material;

        public bool IsUnsafe => isUnsafe;

        [Header("World Tile")]
        [Min(1), Tooltip("Tile ID (1-based). 0 is an empty cell.")]
        [SerializeField] protected int tileIndex;

        [Tooltip("What layer of the world does this tile belong to?")]
        [SerializeField] protected TileType tileType;

        [Header("Details")]
        [SerializeField] protected GroundMaterial material;
        [SerializeField] protected bool isUnsafe;

#region Static - World Tiles Cache
        [ShowInInspector]
        private static WorldTileAsset[] worldTiles;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void Init()
        {
            WorldTileAsset[] allTileAssets = Resources.LoadAll<WorldTileAsset>("World Tiles");
            worldTiles = new WorldTileAsset[allTileAssets.Length];

            foreach (WorldTileAsset tileAsset in allTileAssets)
            {
                if (tileAsset.TileType == TileType.Block)
                    worldTiles[tileAsset.TileIndex - 1] = tileAsset;
            }
        }

        /// <summary>
        /// Get the tile with the 1-based LDtk IntGrid <see cref="TileIndex"/>.
        /// </summary>
        internal static WorldTileAsset FromIndex(int index) =>
            index > 0 && index < worldTiles.Length ? worldTiles[index - 1] : null;
#endregion
    }
}
