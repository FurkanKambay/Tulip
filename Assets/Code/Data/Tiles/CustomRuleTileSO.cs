using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Tulip.Data.Tiles
{
    [CreateAssetMenu(menuName = "Tiles/Rule Tile")]
    public sealed class CustomRuleTileSO : RuleTile<CustomRuleTileSO.Neighbor>
    {
        public override bool RuleMatch(int neighbor, TileBase tile) => neighbor switch
        {
            Neighbor.Null => tile == null,
            Neighbor.NotNull => tile != null,
            _ => base.RuleMatch(neighbor, tile)
        };

        [UsedImplicitly]
        public class Neighbor : TilingRuleOutput.Neighbor
        {
            public const int Null = 3;
            public const int NotNull = 4;
        }
    }
}
