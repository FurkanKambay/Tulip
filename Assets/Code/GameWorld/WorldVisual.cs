using System;
using FK.Tulip.Data;
using FK.Tulip.Data.Tiles;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace FK.Tulip.GameWorld
{
    public sealed class WorldVisual : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private Tilemap wallTilemap;
        [SerializeField] private Tilemap blockTilemap;

        public CustomRuleTileAsset GetTile(Vector2Int cell, TileType tileType)
        {
            Tilemap tilemap = tileType switch
            {
                TileType.Wall => wallTilemap,
                TileType.Block => blockTilemap,
                _ => throw new ArgumentOutOfRangeException(nameof(tileType))
            };

            return !tilemap ? null : tilemap.GetTile<CustomRuleTileAsset>((Vector3Int)cell);
        }

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
