using System;
using SaintsField;
using SaintsField.Playa;
using Tulip.Data.Items;
using UnityEngine;

namespace Tulip.Data
{
    [Serializable]
    public class TileDictionary : SaintsDictionary<Vector2Int, PlaceableData>
    {
    }

    [CreateAssetMenu(menuName = "World Data")]
    public class WorldData : ScriptableObject
    {
        public string Name => name;
        public Vector2Int Dimensions => dimensions;

        public TileDictionary Walls => walls;
        public TileDictionary Blocks => blocks;
        public TileDictionary Curtains => curtains;

        [SerializeField, Min(0)] Vector2Int dimensions;

        [LayoutGroup("Tiles", ELayout.Tab)]
        [LayoutGroup("Tiles/Walls")]
        [SaintsDictionary("Cell", "Placeable", searchable: false, numberOfItemsPerPage: 10)]
        [SerializeField] TileDictionary walls;

        [LayoutGroup("Tiles/Blocks")]
        [SaintsDictionary("Cell", "Placeable", searchable: false, numberOfItemsPerPage: 10)]
        [SerializeField] TileDictionary blocks;

        [LayoutGroup("Tiles/Curtains")]
        [SaintsDictionary("Cell", "Placeable", searchable: false, numberOfItemsPerPage: 10)]
        [SerializeField] TileDictionary curtains;
    }
}
