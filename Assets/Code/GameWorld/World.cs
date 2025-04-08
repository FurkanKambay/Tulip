using System;
using System.Collections.Generic;
using System.Linq;
using Furkan.Common;
using Tulip.Character;
using Tulip.Core;
using Tulip.Data;
using Tulip.Data.Items;
using UnityEngine;

namespace Tulip.GameWorld
{
    public class World : MonoBehaviour
    {
        public delegate void PlaceableEvent(TileModification modification);

        public event ProvideWorldEvent OnRefresh;
        public event PlaceableEvent    OnPlaceTile;
        public event PlaceableEvent    OnHitTile;
        public event PlaceableEvent    OnDestroyTile;

        [Header("References")]
        [SerializeField] WorldManager worldManager;
        [SerializeField] WorldVisual worldVisual;

        [Header("Config")]
        [SerializeField] bool isReadonly;
        [SerializeField] LayerMask entityLayers;

        public WorldData WorldData  { get; private set; }
        public bool      IsReadonly => isReadonly;

        private readonly Dictionary<Vector2Int, TangibleEntity> staticEntities   = new();
        private readonly Dictionary<Vector2Int, int>            wallDamageMap    = new();
        private readonly Dictionary<Vector2Int, int>            blockDamageMap   = new();
        private readonly Dictionary<Vector2Int, int>            curtainDamageMap = new();

        private void Awake() => SetWorldData(worldManager.World);

        private void OnEnable()
        {
            GameStateChange.Event       += HandleGameStateChange;
            worldManager.OnProvideWorld += SetWorldData;
        }

        private void OnDisable()
        {
            GameStateChange.Event       -= HandleGameStateChange;
            worldManager.OnProvideWorld -= SetWorldData;
        }

        /// <summary>
        /// Tries to damage a tile of the given type at the given cell coordinates.
        /// </summary>
        /// <returns>The loot from the tile. Empty if the action was not successful.</returns>
        public InventoryModification DamageTile(Vector2Int cell, TileType tileType, int damage)
        {
            if (isReadonly)
                return default;

            TileDictionary tiles = GetTiles(tileType);

            if (!tiles.TryGetValue(cell, out PlaceableData placeableData))
                return default;

            if (placeableData.IsUnbreakable)
            {
                // TODO: feedback for 'unbreakable'
                // change return type to account for this
                return default;
            }

            Dictionary<Vector2Int, int> damageMap = GetDamageMap(tileType);
            damageMap.TryAdd(cell, 0);
            damageMap[cell] += damage;

            if (damageMap[cell] < placeableData.Hardness)
            {
                // the tile was not destroyed
                OnHitTile?.Invoke(TileModification.FromDamaged(cell, placeableData));
                return default;
            }

            tiles.Remove(cell);
            damageMap.Remove(cell);

            OnDestroyTile?.Invoke(TileModification.FromDestroyed(cell, placeableData));

            ItemData loot = placeableData.OreData.Or<ItemData>(placeableData);
            return InventoryModification.ToAdd(loot.Stack(1));
        }

        /// <summary>
        /// Tries to place a tile at the given cell coordinates.
        /// </summary>
        /// <returns>The inventory modification to place the tile. Empty if the action was not successful.</returns>
        public InventoryModification PlaceTile(Vector2Int cell, PlaceableData placeableData)
        {
            if (isReadonly)
                return default;

            TileDictionary tiles = GetTiles(placeableData.TileType);

            if (!tiles.TryAdd(cell, placeableData))
                return default;

            GetDamageMap(placeableData.TileType).Remove(cell);
            OnPlaceTile?.Invoke(TileModification.FromPlaced(cell, placeableData));

            return InventoryModification.ToRemove(placeableData.Stack(1));
        }

        public bool TryAddStaticEntity(Vector2Int baseCell, TangibleEntity entity) =>
            !isReadonly && staticEntities.TryAdd(baseCell, entity);

        public void ClearEntities() => staticEntities.Clear();

#region Tile Helpers
        public bool HasTile(Vector2Int cell, TileType tileType) =>
            GetTiles(tileType).ContainsKey(cell);

        public PlaceableData GetTile(Vector2Int cell, TileType tileType) =>
            GetTiles(tileType).TryGetValue(cell, out PlaceableData placeableData) ? placeableData : null;

        public PlaceableData GetTileAtWorld(Vector3 worldPosition, TileType tileType) =>
            GetTile(WorldToCell(worldPosition), tileType);

        public int GetTileDamage(Vector2Int cell, TileType tileType) =>
            GetDamageMap(tileType).GetValueOrDefault(cell, 0);

        private TileDictionary GetTiles(TileType tileType) => tileType switch
        {
            TileType.Wall    => WorldData.Walls,
            TileType.Block   => WorldData.Blocks,
            TileType.Curtain => WorldData.Curtains,
            _                => throw new ArgumentOutOfRangeException(nameof(tileType))
        };
#endregion

#region Cell Helpers
        public Vector3    CellCenter(Vector2Int cell)          => worldVisual.GetCellCenterWorld(cell);
        public Vector2Int WorldToCell(Vector3   worldPosition) => worldVisual.WorldToCell(worldPosition);

        public Bounds CellBoundsWorld(Vector2Int   cell)               => worldVisual.CellBoundsWorld(cell);
        public bool   DoesCellIntersect(Vector2Int cell, Bounds other) => CellBoundsWorld(cell).Intersects(other);

        /// Checks for entities at the cell. Use <see cref="HasTile"/> for other purposes.
        public bool IsCellEntityFree(Vector2Int cell)
        {
            Bounds  bounds      = CellBoundsWorld(cell);
            Vector2 topLeft     = bounds.center - bounds.extents + (Vector3.one * 0.02f);
            Vector2 bottomRight = bounds.center + bounds.extents - (Vector3.one * 0.02f);

            return !Physics2D.OverlapArea(topLeft, bottomRight, entityLayers);
        }
#endregion

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

        private void SetWorldData(WorldData newWorldData)
        {
            if (!newWorldData || WorldData == newWorldData)
                return;

            WorldData = newWorldData;
            OnRefresh?.Invoke(newWorldData);
        }

        private void HandleGameStateChange(GameStateChange args) =>
            isReadonly = args.NewState == GameState.MainMenu;

        private Dictionary<Vector2Int, int> GetDamageMap(TileType tileType) => tileType switch
        {
            TileType.Wall    => wallDamageMap,
            TileType.Block   => blockDamageMap,
            TileType.Curtain => curtainDamageMap,
            _                => throw new ArgumentOutOfRangeException(nameof(tileType))
        };
    }
}
