using UnityEngine;
using UnityEngine.Tilemaps;
using IntGrid = LDtkUnity.LDtkComponentLayerIntGridValues;

namespace Tulip.GameWorld
{
    public sealed class WorldVisual : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] IntGrid worldIntGrid;
        [SerializeField] Tilemap wallTilemap;
        [SerializeField] Tilemap blockTilemap;
        [SerializeField] Tilemap curtainTilemap;

        internal IntGrid WorldIntGrid => worldIntGrid;

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
