using System.Linq;
using Furkan.Common;
using Tulip.Data.Items;
using Tulip.GameWorld;
using UnityEngine;
using Vertx.Attributes;

namespace Tulip.Data
{
    [CreateAssetMenu(menuName = "Gameplay/Spawn Condition")]
    public class SpawnConditionAsset : ScriptableObject
    {
        [Header("Ground")]
        [SerializeField] bool needsGround;

        [EnableIf(nameof(needsGround))]
        [SerializeField] bool needsSafeGround;

        [EnableIf(nameof(needsGround))]
        [SerializeField] WorldTileAsset[] groundTiles;

        [Header("Clearance")]
        [SerializeField, Min(0)] int clearanceAbove;

        [DisableIf(nameof(needsGround))]
        [SerializeField, Min(0)] int clearanceBelow;

        // ReSharper disable NotAccessedField.Global
        [LayoutGroup("Referenced By", ELayout.Background | ELayout.TitleOut | ELayout.Foldout)]
        [SerializeField, ReadOnlyField] protected EntityAsset[] assignedEntities;
        // ReSharper restore NotAccessedField.Global

        /// <param name="entityAsset"></param>
        /// <param name="world"></param>
        /// <param name="baseCell">The bottom-left cell, NOT center or pivot</param>
        public bool CanSpawn(EntityAsset entityAsset, World world, Vector2Int baseCell)
        {
            if (!world.CanAccommodate(baseCell, entityAsset.Size))
                return false;

            // Check tiles above
            for (int y = 0; y < clearanceAbove; y++)
            for (int x = 0; x < entityAsset.Size.x; x++)
            {
                if (world.HasTile(baseCell + new Vector2Int(x, entityAsset.Size.y + y), TileType.Block))
                    return false;
            }

            // Check tiles below
            if (!needsGround)
            {
                for (int y = 1; y <= clearanceBelow; y++)
                for (int x = 0; x < entityAsset.Size.x; x++)
                {
                    if (world.HasTile(baseCell + new Vector2Int(x, -y), TileType.Block))
                        return false;
                }
            }

            if (!needsGround)
                return true;

            // Check ground only
            for (int x = 0; x < entityAsset.Size.x; x++)
            {
                Vector2Int floorCell = baseCell + new Vector2Int(x, -1);
                WorldTileAsset floorTile = world.GetTile(floorCell, TileType.Block);

                if (!floorTile || (needsSafeGround && floorTile.IsUnsafe))
                    return false;

                if (groundTiles.Length > 0 && !groundTiles.Contains(floorTile))
                    return false;
            }

            return true;
        }

        private void OnValidate() =>
            assignedEntities = Resources.FindObjectsOfTypeAll<EntityAsset>()
                .Where(entityAsset => entityAsset.SpawnConditionAsset == this)
                .ToArray();
    }
}
