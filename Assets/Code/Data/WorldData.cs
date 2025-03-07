using System;
using SaintsField;
using Tulip.Data.Items;
using UnityEngine;

namespace Tulip.Data
{
    [Serializable]
    public class TileDictionary : SaintsDictionary<Vector2Int, PlaceableData>
    {
    }

    [Serializable]
    public class WorldData
    {
        public string Name => name;
        public Vector2Int Dimensions => dimensions;

        public TileDictionary Walls => walls;
        public TileDictionary Blocks => blocks;
        public TileDictionary Curtains => curtains;

        [SerializeField] string name;
        [SerializeField, Min(0)] Vector2Int dimensions;

        [SerializeField, HideInInspector] TileDictionary walls;
        [SerializeField, HideInInspector] TileDictionary blocks;
        [SerializeField, HideInInspector] TileDictionary curtains;

        public WorldData(string name, Vector2Int dimensions,
            TileDictionary walls, TileDictionary blocks, TileDictionary curtains)
        {
            this.name = name;
            this.dimensions = dimensions;
            this.walls = walls;
            this.blocks = blocks;
            this.curtains = curtains;
        }
    }
}
