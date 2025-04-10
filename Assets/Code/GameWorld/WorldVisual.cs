using System;
using System.Collections.Generic;
using System.Linq;
using Furkan.Common;
using SaintsField.Playa;
using Tulip.Data;
using Tulip.Data.Items;
using Tulip.Data.Tiles;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Tulip.GameWorld
{
    public class WorldVisual : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] World world;
        [SerializeField] Tilemap wallTilemap;
        [SerializeField] Tilemap blockTilemap;
        [SerializeField] Tilemap curtainTilemap;

#region Unity Lifecycle
        private void Awake() =>
            InitializeTilemaps(world.WorldData);

        private void OnEnable()
        {
            if (!world)
                return;

            world.OnPlaceTile += World_TilePlaced;
            world.OnDestroyTile += World_TileDestroyed;
        }

        private void OnDisable()
        {
            if (!world)
                return;

            world.OnPlaceTile -= World_TilePlaced;
            world.OnDestroyTile -= World_TileDestroyed;
        }
#endregion

#region Tilemap APIs
        public Vector2Int WorldToCell(Vector3 worldPosition) =>
            (Vector2Int)blockTilemap.WorldToCell(worldPosition);

        public Vector3 GetCellCenterWorld(Vector2Int cell) =>
            blockTilemap.GetCellCenterWorld((Vector3Int)cell);

        public Bounds CellBoundsWorld(Vector2Int cell) =>
            new(GetCellCenterWorld(cell), blockTilemap.GetBoundsLocal((Vector3Int)cell).size);
#endregion

        private void InitializeTilemaps(WorldData worldData)
        {
            if (!worldData)
                return;

            Vector3Int tilemapSize = worldData.Dimensions.WithZ(1);
            wallTilemap.size = tilemapSize;
            blockTilemap.size = tilemapSize;
            curtainTilemap.size = tilemapSize;

            // TODO: Apply world delta to tilemaps from the save file

            // TileChangeData[] wallChanges = worldData.Walls.Select(selector).ToArray();
            // TileChangeData[] blockChanges = worldData.Blocks.Select(selector).ToArray();
            // TileChangeData[] curtainChanges = worldData.Curtains.Select(selector).ToArray();
            // wallTilemap.SetTiles(wallChanges, ignoreLockFlags: true);
            // blockTilemap.SetTiles(blockChanges, ignoreLockFlags: true);
            // curtainTilemap.SetTiles(curtainChanges, ignoreLockFlags: true);

            // TileChangeData selector(KeyValuePair<Vector2Int, PlaceableData> kvp)
            // {
            //     (Vector2Int cell, PlaceableData placeableData) = kvp;
            //
            //     return new TileChangeData(
            //         (Vector3Int)cell,
            //         (bool)placeableData ? placeableData.RuleTileData : null,
            //         (bool)placeableData ? placeableData.Color : Color.white,
            //         Matrix4x4.identity
            //     );
            // }
        }

#region World Events
        private void World_TilePlaced(TileModification modification)
        {
            PlaceableData placeableData = modification.PlaceableData;
            Tilemap tilemap = GetTilemap(placeableData.TileType);

            var cell = (Vector3Int)modification.Cell;
            tilemap.SetTile(cell, placeableData.RuleTileData);

            if (!placeableData)
            {
                tilemap.SetTile(cell, null);
                return;
            }

            tilemap.SetTile(cell, placeableData.RuleTileData);
            tilemap.SetColor(cell, placeableData.Color);
        }

        private void World_TileDestroyed(TileModification modification)
        {
            Tilemap tilemap = GetTilemap(modification.PlaceableData.TileType);
            tilemap.SetTile((Vector3Int)modification.Cell, null);
        }

        private Tilemap GetTilemap(TileType tileType) => tileType switch
        {
            TileType.Wall => wallTilemap,
            TileType.Block => blockTilemap,
            TileType.Curtain => curtainTilemap,
            _ => throw new ArgumentOutOfRangeException(nameof(tileType))
        };
#endregion

#if UNITY_EDITOR
        [Button]
        private void SaveWorldToAsset()
        {
            foreach (Vector3Int position in blockTilemap.cellBounds.allPositionsWithin)
            {
                var cell = (Vector2Int)position;

                CustomRuleTileData wallTile = wallTilemap.GetTile<CustomRuleTileData>((Vector3Int)cell);
                CustomRuleTileData blockTile = blockTilemap.GetTile<CustomRuleTileData>((Vector3Int)cell);
                CustomRuleTileData curtainTile = curtainTilemap.GetTile<CustomRuleTileData>((Vector3Int)cell);

                if (wallTile)
                    world.WorldData.Walls[cell] = wallTile.PlaceableData;
                else if (world.WorldData.Walls.ContainsKey(cell))
                    world.WorldData.Walls.Remove(cell);

                if (blockTile)
                    world.WorldData.Blocks[cell] = blockTile.PlaceableData;
                else if (world.WorldData.Blocks.ContainsKey(cell))
                    world.WorldData.Blocks.Remove(cell);

                if (curtainTile)
                    world.WorldData.Curtains[cell] = curtainTile.PlaceableData;
                else if (world.WorldData.Curtains.ContainsKey(cell))
                    world.WorldData.Curtains.Remove(cell);
            }

            EditorUtility.SetDirty(world.WorldData);
        }

        private void OnDrawGizmos()
        {
            WorldData worldData = world ? world.WorldData : Resources.Load<WorldData>("Worlds/World");

            Handles.color = Color.white;
            Handles.DrawWireCube(Vector3.zero, (Vector3Int)worldData.Dimensions);
        }
#endif
    }
}
