using SaintsField.Playa;
using Tulip.Data.Tiles;
using Tulip.GameWorld;
using UnityEngine;

namespace Tulip.Data.Items
{
    [CreateAssetMenu(menuName = "Items/Placeable", order = 5)]
    public class PlaceableSO : BaseWorldToolSO
    {
        public override Sprite Icon => ruleTileSO.m_DefaultSprite;

        public Color Color => color;
        public CustomRuleTileSO RuleTileSO => ruleTileSO;
        public TileType TileType => tileType;
        public PlaceableMaterial Material => material;

        public bool IsUnsafe => isUnsafe;
        public bool IsUnbreakable => isUnbreakable;
        public int Hardness => hardness;
        public OreSO OreSO => oreSO;

        [Header("World Tile")]
        [SerializeField] protected Color color;
        [SerializeField] protected CustomRuleTileSO ruleTileSO;
        [SerializeField] protected TileType tileType;
        [SerializeField] protected PlaceableMaterial material;

        [SerializeField] protected bool isUnsafe;
        [SerializeField] protected bool isUnbreakable;

        [Min(1), PlayaDisableIf(nameof(isUnbreakable))]
        [SerializeField] protected int hardness = 50;

        [SerializeField] protected OreSO oreSO;

        public override InventoryModification UseOn(World world, Vector2Int cell)
        {
            ToolUsability usability = GetUsability(world, cell);
            return usability == ToolUsability.Available ? world.PlaceTile(cell, this) : default;
        }

        public override ToolUsability GetUsability(World world, Vector2Int cell)
        {
            // TODO: check if cell is out of world bounds
            // return ToolUsability.Never;

            PlaceableSO tile = world.GetTile(cell, tileType);
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
            if (ruleTileSO)
                ruleTileSO.PlaceableSO = this;
        }

        protected override void OnValidate()
        {
            if (ruleTileSO)
                ruleTileSO.PlaceableSO = this;
        }

        private void Reset()
        {
            maxAmount = 999;
            cooldown = 0.25f;
        }
    }
}
