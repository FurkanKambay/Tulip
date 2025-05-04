using UnityEngine;

namespace Tulip.Data.Items
{
    /// <summary>
    /// A world tool such as a pickaxe.
    /// </summary>
    [CreateAssetMenu(menuName = "Items/World Tool", order = 4)]
    public class WorldToolSO : BaseWorldToolSO
    {
        public int Power => power;
        public TileType TileType => tileType;

        [Header("World Tool")]
        [SerializeField, Min(0)] protected int power = 50;
        [SerializeField] protected TileType tileType = TileType.Block;
    }
}
