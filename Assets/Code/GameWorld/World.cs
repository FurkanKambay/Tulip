using System.Collections.Generic;
using System.Linq;
using Furkan.Common;
using Tulip.Character;
using Tulip.Data;
using Tulip.Data.Items;
using Tulip.Data.Tiles;
using UnityEngine;

namespace Tulip.GameWorld
{
    public class World : MonoBehaviour
    {
        [Header("References")]
        [SerializeField, Required] WorldVisual worldVisual;

        [Header("Config")]
        [SerializeField] LayerMask entityLayers;

        private readonly Dictionary<Vector2Int, TangibleEntity> staticEntities = new();

        public bool TryAddStaticEntity(Vector2Int baseCell, TangibleEntity entity) =>
            staticEntities.TryAdd(baseCell, entity);

        public void ClearEntities() => staticEntities.Clear();

#region Tile Helpers
        public bool HasTile(Vector2Int cell, TileType tileType)
        {
            CustomRuleTileAsset tile = worldVisual.GetTile(cell, tileType);
            return tile && tile.WorldTileAsset;
        }

        public WorldTileAsset GetTile(Vector2Int cell, TileType tileType)
        {
            CustomRuleTileAsset tile = worldVisual.GetTile(cell, tileType);
            return !tile ? null : tile.WorldTileAsset;
        }

        public WorldTileAsset GetTileAtWorld(Vector3 worldPosition, TileType tileType) =>
            GetTile(WorldToCell(worldPosition), tileType);
#endregion

#region Cell Helpers
        public Vector3 CellCenter(Vector2Int cell) => worldVisual.GetCellCenterWorld(cell);
        public Vector2Int WorldToCell(Vector3 worldPosition) => worldVisual.WorldToCell(worldPosition);

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
