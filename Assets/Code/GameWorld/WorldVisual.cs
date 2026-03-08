using UnityEngine;
using UnityEngine.Tilemaps;
using IntGrid = LDtkUnity.LDtkComponentLayerIntGridValues;

namespace Tulip.GameWorld
{
    public sealed class WorldVisual : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] IntGrid wallIntGrid;
        [SerializeField] IntGrid blocksIntGrid;
        [SerializeField] Tilemap wallTilemap;
        [SerializeField] Tilemap blockTilemap;

        internal IntGrid WallIntGrid => wallIntGrid;
        internal IntGrid BlocksIntGrid => blocksIntGrid;

#region Tilemap APIs
        internal Vector2Int WorldToCell(Vector3 worldPosition) =>
            (Vector2Int)blockTilemap.WorldToCell(worldPosition);

        internal Vector3 GetCellCenterWorld(Vector2Int cell) =>
            blockTilemap.GetCellCenterWorld((Vector3Int)cell);

        internal Bounds CellBoundsWorld(Vector2Int cell)
        {
            return new Bounds(
                center: GetCellCenterWorld(cell),
                size: blockTilemap.GetBoundsLocal((Vector3Int)cell).size
            );
        }
#endregion
    }
}
