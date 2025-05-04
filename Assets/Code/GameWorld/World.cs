using System;
using System.Collections.Generic;
using System.Linq;
using SaintsField;
using Tulip.Character;
using Tulip.Data;
using Tulip.Data.Items;
using UnityEngine;
using UnityEngine.Assertions;
using IntGrid = LDtkUnity.LDtkComponentLayerIntGridValues;

namespace Tulip.GameWorld
{
    public class World : MonoBehaviour
    {
        public delegate void PlaceableEvent(TileModification modification);

        public event PlaceableEvent OnPlaceTile;
        public event PlaceableEvent OnHitTile;
        public event PlaceableEvent OnDestroyTile;

        [Header("References")]
        [SerializeField, Required] WorldVisual worldVisual;

        [Header("Config")]
        [SerializeField] bool isReadonly;
        [SerializeField] LayerMask entityLayers;

        public bool IsReadonly => isReadonly;

        private readonly Dictionary<Vector2Int, TangibleEntity> staticEntities = new();

        public bool TryAddStaticEntity(Vector2Int baseCell, TangibleEntity entity) =>
            !isReadonly && staticEntities.TryAdd(baseCell, entity);

        public void ClearEntities() => staticEntities.Clear();

#region Tile Helpers
        public bool HasTile(Vector2Int cell, TileType tileType)
        {
            IntGrid intGrid = GetIntGrid(tileType);

            if (!intGrid)
                return false;

            return intGrid.GetValue((Vector3Int)cell) != 0;
        }

        public PlaceableSO GetTile(Vector2Int cell, TileType tileType)
        {
            IntGrid intGrid = GetIntGrid(tileType);
            Assert.IsNotNull(intGrid);

            int tileIndex = intGrid.GetValue((Vector3Int)cell);
            return tileIndex == 0 ? null : PlaceableSO.FromIndex(tileIndex);
        }

        public PlaceableSO GetTileAtWorld(Vector3 worldPosition, TileType tileType) =>
            GetTile(WorldToCell(worldPosition), tileType);

        public int GetTileDamage(Vector2Int cell, TileType tileType) => 0;

        private IntGrid GetIntGrid(TileType tileType) => tileType switch
        {
            TileType.Wall => null,
            TileType.Block => worldVisual.WorldIntGrid,
            TileType.Curtain => null,
            _ => throw new ArgumentOutOfRangeException(nameof(tileType))
        };
#endregion

#region Cell Helpers
        public Vector3 CellCenter(Vector2Int cell) => worldVisual.GetCellCenterWorld(cell);
        public Vector2Int WorldToCell(Vector3 worldPosition) => worldVisual.WorldToCell(worldPosition);

        public Bounds CellBoundsWorld(Vector2Int cell) => worldVisual.CellBoundsWorld(cell);
        public bool DoesCellIntersect(Vector2Int cell, Bounds other) => CellBoundsWorld(cell).Intersects(other);

        /// Checks for entities at the cell. Use <see cref="HasTile"/> for other purposes.
        public bool IsCellEntityFree(Vector2Int cell)
        {
            Bounds bounds = CellBoundsWorld(cell);
            Vector2 topLeft = bounds.center - bounds.extents + (Vector3.one * 0.02f);
            Vector2 bottomRight = bounds.center + bounds.extents - (Vector3.one * 0.02f);

            return !Physics2D.OverlapArea(topLeft, bottomRight, entityLayers);
        }

        /// <param name="baseCell">The bottom-left cell, NOT center or pivot</param>
        /// <param name="entitySize"></param>
        public bool CanAccommodate(Vector2Int baseCell, Vector2Int entitySize)
        {
            if (staticEntities.ContainsKey(baseCell))
                return false;

            var entityRect = new RectInt(baseCell, entitySize);

            foreach (Vector2Int position in entityRect.allPositionsWithin)
            {
                if (HasTile(position, TileType.Block))
                    return false;
            }

            return staticEntities.Values.All(entity => !entity.Rect.Overlaps(entityRect));
        }
#endregion
    }
}
