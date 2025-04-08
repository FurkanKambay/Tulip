using Tulip.GameWorld;
using UnityEngine;

namespace Tulip.Data.Items
{
    /// <summary>
    /// A world tool such as a pickaxe.
    /// </summary>
    [CreateAssetMenu(menuName = "Items/World Tool", order = 4)]
    public class WorldToolData : BaseWorldToolData
    {
        public int Power => power;
        public TileType TileType => tileType;

        [Header("World Tool Data")]
        [SerializeField, Min(0)] protected int power = 50;
        [SerializeField] protected TileType tileType = TileType.Block;

        public override ToolUsability GetUsability(World world, Vector2Int cell)
        {
            bool hasTile = world.HasTile(cell, TileType.Block);
            bool cellHasEntity = !world.IsCellEntityFree(cell);

            return !hasTile ? ToolUsability.NoEffect
                : cellHasEntity ? ToolUsability.NotNow
                : ToolUsability.Available;
        }

        public override InventoryModification UseOn(World world, Vector2Int cell) =>
            world.DamageTile(cell, tileType, power);
    }
}
