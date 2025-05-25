using System.Linq;
using SaintsField;
using SaintsField.Playa;
using Tulip.Data.Items;
using Tulip.GameWorld;
using UnityEngine;

namespace Tulip.Data
{
    [CreateAssetMenu(menuName = "Gameplay/Spawn Condition")]
    public class SpawnConditionSO : ScriptableObject
    {
        [Header("Ground")]
        [SerializeField] bool needsGround;

        [EnableIf(nameof(needsGround))]
        [SerializeField] bool needsSafeGround;

        [EnableIf(nameof(needsGround))]
        [SerializeField] WorldTileSO[] groundTiles;

        [Header("Clearance")]
        [SerializeField, Min(0)] int clearanceAbove;

        [DisableIf(nameof(needsGround))]
        [SerializeField, Min(0)] int clearanceBelow;

        // ReSharper disable NotAccessedField.Global
        [LayoutGroup("Referenced By", ELayout.Background | ELayout.TitleOut | ELayout.Foldout, marginTop: 16)]
        [SerializeField, ReadOnly] protected EntitySO[] assignedEntities;
        // ReSharper restore NotAccessedField.Global

        /// <param name="entitySO"></param>
        /// <param name="world"></param>
        /// <param name="baseCell">The bottom-left cell, NOT center or pivot</param>
        public bool CanSpawn(EntitySO entitySO, World world, Vector2Int baseCell)
        {
            if (!world.CanAccommodate(baseCell, entitySO.Size))
                return false;

            // Check tiles above
            for (int y = 0; y < clearanceAbove; y++)
            for (int x = 0; x < entitySO.Size.x; x++)
            {
                if (world.HasTile(baseCell + new Vector2Int(x, entitySO.Size.y + y), TileType.Block))
                    return false;
            }

            // Check tiles below
            if (!needsGround)
            {
                for (int y = 1; y <= clearanceBelow; y++)
                for (int x = 0; x < entitySO.Size.x; x++)
                {
                    if (world.HasTile(baseCell + new Vector2Int(x, -y), TileType.Block))
                        return false;
                }
            }

            if (!needsGround)
                return true;

            // Check ground only
            for (int x = 0; x < entitySO.Size.x; x++)
            {
                Vector2Int floorCell = baseCell + new Vector2Int(x, -1);
                WorldTileSO floorTile = world.GetTile(floorCell, TileType.Block);

                if (!floorTile || (needsSafeGround && floorTile.IsUnsafe))
                    return false;

                if (groundTiles.Length > 0 && !groundTiles.Contains(floorTile))
                    return false;
            }

            return true;
        }

        private void OnValidate() =>
            assignedEntities = Resources.FindObjectsOfTypeAll<EntitySO>()
                .Where(entitySO => entitySO.SpawnConditionSO == this)
                .ToArray();
    }
}
