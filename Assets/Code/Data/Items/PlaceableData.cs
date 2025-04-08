using SaintsField.Playa;
using Tulip.Data.Tiles;
using Tulip.GameWorld;
using UnityEngine;

namespace Tulip.Data.Items
{
    [CreateAssetMenu(menuName = "Items/Placeable", order = 5)]
    public class PlaceableData : BaseWorldToolData
    {
        public override Sprite Icon => ruleTileData.m_DefaultSprite;

        public Color Color => color;
        public CustomRuleTileData RuleTileData => ruleTileData;
        public TileType TileType => tileType;
        public PlaceableMaterial Material => material;

        public bool IsUnsafe => isUnsafe;
        public bool IsUnbreakable => isUnbreakable;
        public int Hardness => hardness;
        public OreData OreData => oreData;

        [Header("World Tile Data")]
        [SerializeField] protected Color color;
        [SerializeField] protected CustomRuleTileData ruleTileData;
        [SerializeField] protected TileType tileType;
        [SerializeField] protected PlaceableMaterial material;

        [SerializeField] protected bool isUnsafe;
        [SerializeField] protected bool isUnbreakable;

        [Min(1), PlayaDisableIf(nameof(isUnbreakable))]
        [SerializeField] protected int hardness = 50;

        [SerializeField] protected OreData oreData;

        public override InventoryModification UseOn(World world, Vector2Int cell)
        {
            ToolUsability usability = GetUsability(world, cell);
            return usability == ToolUsability.Available ? world.PlaceTile(cell, this) : default;
        }

        public override ToolUsability GetUsability(World world, Vector2Int cell)
        {
            // TODO: check if cell is out of world bounds
            // return ToolUsability.Never;

            PlaceableData tile = world.GetTile(cell, tileType);
            bool cellHasEntity = tileType is TileType.Block && !world.IsCellEntityFree(cell);

            return (bool)tile switch
            {
                true when tile == this => ToolUsability.NoEffect,
                true => ToolUsability.Invalid,
                false when cellHasEntity => ToolUsability.NotNow,
                _ => ToolUsability.Available
            };
        }

        private void OnEnable()
        {
            if (ruleTileData)
                ruleTileData.PlaceableData = this;
        }

        protected override void OnValidate()
        {
            if (ruleTileData)
                ruleTileData.PlaceableData = this;
        }

        private void Reset()
        {
            maxAmount = 999;
            cooldown = 0.25f;
        }
    }
}
