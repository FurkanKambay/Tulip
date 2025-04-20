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
            InitializeTilemaps(world.WorldSO);

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

        private void InitializeTilemaps(WorldSO worldSO)
        {
            if (!worldSO)
                return;

            Vector3Int tilemapSize = worldSO.Dimensions.WithZ(1);
            wallTilemap.size = tilemapSize;
            blockTilemap.size = tilemapSize;
            curtainTilemap.size = tilemapSize;

            // TODO: Apply world delta to tilemaps from the save file

            // TileChangeData[] wallChanges = worldSO.Walls.Select(selector).ToArray();
            // TileChangeData[] blockChanges = worldSO.Blocks.Select(selector).ToArray();
            // TileChangeData[] curtainChanges = worldSO.Curtains.Select(selector).ToArray();
            // wallTilemap.SetTiles(wallChanges, ignoreLockFlags: true);
            // blockTilemap.SetTiles(blockChanges, ignoreLockFlags: true);
            // curtainTilemap.SetTiles(curtainChanges, ignoreLockFlags: true);

            // TileChangeData selector(KeyValuePair<Vector2Int, PlaceableSO> kvp)
            // {
            //     (Vector2Int cell, PlaceableSO placeableSO) = kvp;
            //
            //     return new TileChangeData(
            //         (Vector3Int)cell,
            //         (bool)placeableSO ? placeableSO.RuleTileSO : null,
            //         (bool)placeableSO ? placeableSO.Color : Color.white,
            //         Matrix4x4.identity
            //     );
            // }
        }

#region World Events
        private void World_TilePlaced(TileModification modification)
        {
            PlaceableSO placeableSO = modification.PlaceableSO;
            Tilemap tilemap = GetTilemap(placeableSO.TileType);

            var cell = (Vector3Int)modification.Cell;
            tilemap.SetTile(cell, placeableSO.RuleTileSO);

            if (!placeableSO)
            {
                tilemap.SetTile(cell, null);
                return;
            }

            tilemap.SetTile(cell, placeableSO.RuleTileSO);
            tilemap.SetColor(cell, placeableSO.Color);
        }

        private void World_TileDestroyed(TileModification modification)
        {
            Tilemap tilemap = GetTilemap(modification.PlaceableSO.TileType);
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

                CustomRuleTileSO wallTile = wallTilemap.GetTile<CustomRuleTileSO>((Vector3Int)cell);
                CustomRuleTileSO blockTile = blockTilemap.GetTile<CustomRuleTileSO>((Vector3Int)cell);
                CustomRuleTileSO curtainTile = curtainTilemap.GetTile<CustomRuleTileSO>((Vector3Int)cell);

                if (wallTile)
                    world.WorldSO.Walls[cell] = wallTile.PlaceableSO;
                else if (world.WorldSO.Walls.ContainsKey(cell))
                    world.WorldSO.Walls.Remove(cell);

                if (blockTile)
                    world.WorldSO.Blocks[cell] = blockTile.PlaceableSO;
                else if (world.WorldSO.Blocks.ContainsKey(cell))
                    world.WorldSO.Blocks.Remove(cell);

                if (curtainTile)
                    world.WorldSO.Curtains[cell] = curtainTile.PlaceableSO;
                else if (world.WorldSO.Curtains.ContainsKey(cell))
                    world.WorldSO.Curtains.Remove(cell);
            }

            EditorUtility.SetDirty(world.WorldSO);
        }

        private void OnDrawGizmos()
        {
            WorldSO worldSO = world ? world.WorldSO : Resources.Load<WorldSO>("Worlds/World");

            Handles.color = Color.white;
            Handles.DrawWireCube(Vector3.zero, (Vector3Int)worldSO.Dimensions);
        }
#endif
    }
}
