using Tulip.Data.Items;
using UnityEngine;

namespace Tulip.Data
{
    public readonly struct TileModification
    {
        public readonly Vector2Int Cell;
        public readonly PlaceableSO PlaceableSO;
        public readonly TileModificationKind Kind;

        public static TileModification FromPlaced(Vector2Int cell, PlaceableSO placeableSO) =>
            new(cell, placeableSO, TileModificationKind.Placed);

        public static TileModification FromDamaged(Vector2Int cell, PlaceableSO placeableSO) =>
            new(cell, placeableSO, TileModificationKind.Damaged);

        public static TileModification FromDestroyed(Vector2Int cell, PlaceableSO placeableSO) =>
            new(cell, placeableSO, TileModificationKind.Destroyed);

        private TileModification(Vector2Int cell, PlaceableSO placeableSO, TileModificationKind kind)
        {
            Cell = cell;
            PlaceableSO = placeableSO;
            Kind = kind;
        }
    }
}
