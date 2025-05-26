using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Furkan.Common;
using JetBrains.Annotations;
using LDtkUnity;
using UnityEditor;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Tilemaps;
using Object = UnityEngine.Object;

namespace Tulip.Editor.LDtk
{
    [UsedImplicitly]
    public sealed class LDtkPostprocessor : LDtkUnity.Editor.LDtkPostprocessor
    {
        protected override void OnPostprocessLevel(GameObject root, LdtkJson projectJson)
        {
            LDtkComponentLevel level = root.GetComponent<LDtkComponentLevel>();

            int mergedLayersCount = 0;
            int maxTileCount = (int)(level.Size.x * level.Size.y);

            var positions = new Vector3Int[maxTileCount];
            var tiles = new TileBase[maxTileCount];
            var tileChanges = new List<TileChangeData>();

            // Go through auto-layers with the tag "merge_into_{index}"
            foreach (LDtkComponentLayer sourceLayer in level.LayerInstances)
            {
                // possible values: IntGrid, Entities, Tiles or AutoLayer
                if (sourceLayer.Type != "AutoLayer")
                    continue;

                string[] tags = sourceLayer.LayerDef.UiFilterTags;
                int mergeTargetIndex = -1;

                foreach (string tag in tags)
                {
                    Match match = Regex.Match(tag, @"^merge_into_(?<index>\d+)$");

                    if (!match.Success)
                        continue;

                    string indexMatch = match.Groups["index"].Value;

                    if (int.TryParse(indexMatch, out mergeTargetIndex))
                        break;

                    LogWarning($"Invalid merge_into tag in auto-layer {sourceLayer.Identifier}: {indexMatch}");
                }

                // No valid tag was found for this auto-layer
                if (mergeTargetIndex < 0)
                    continue;

                Tilemap sourceTilemap = sourceLayer.AutoLayerTiles.Tilemap;
                BoundsInt sourceBounds = sourceTilemap.cellBounds;

                int tileCount = sourceTilemap.GetTilesRangeNonAlloc(
                    new Vector3Int(sourceBounds.xMin, sourceBounds.yMin, 0),
                    new Vector3Int(sourceBounds.xMax, sourceBounds.yMax, 0),
                    positions,
                    tiles
                );

                // Build the tile change data array from the source Tilemap
                for (int i = 0; i < tileCount; i++)
                {
                    Vector3Int position = positions[i];
                    TileBase tileBase = tiles[i];

                    // Remove this tile and continue
                    if (!tileBase || tileBase.IsNot(out LDtkTilesetTile ldtkTile))
                    {
                        tileChanges.Add(new TileChangeData(position, null, Color.white, Matrix4x4.identity));
                        continue;
                    }

                    // Assign collider type per tile based on enum tags
                    Collider_Type[] tileEnumTagValues = ldtkTile.GetEnumTagValues<Collider_Type>();
                    ldtkTile.Type = tileEnumTagValues.Contains(Collider_Type.Grid)
                        ? Tile.ColliderType.Grid
                        : Tile.ColliderType.None;

                    // Preserve the tile's color and offset
                    Color color = sourceTilemap.GetColor(position);
                    Matrix4x4 matrix = sourceTilemap.GetTransformMatrix(position);

                    tileChanges.Add(new TileChangeData(position, ldtkTile, color, matrix));
                }

                LDtkComponentLayer targetLayer = level.LayerInstances[mergeTargetIndex];
                Tilemap targetTilemap = targetLayer.transform.GetChild(0).GetComponent<Tilemap>();

                // Log($"Merging {sourceLayer.Identifier} into {targetLayer.Identifier}. ({tileCount} tiles)");

                TileChangeData[] tileChangeDataArray = tileChanges.ToArray();
                targetTilemap.SetTiles(tileChangeDataArray, ignoreLockFlags: true);

                if (!targetTilemap.TryGetComponent(out TilemapRenderer _))
                    targetTilemap.gameObject.AddComponent<TilemapRenderer>();

                mergedLayersCount++;

                // Clean up
                tileChanges.Clear();
                positions = new Vector3Int[maxTileCount];
                tiles = new TileBase[maxTileCount];

                // Delete the source auto-layer after moving the tiles to the target tilemap
                Object.DestroyImmediate(sourceLayer.gameObject);
            }

            Log($"Done. {mergedLayersCount} layers were merged. Removing LDtk-related components.");

            int nextOrderInLayer = 0;

            // Sort tilemaps and remove LDtk-related components.
            foreach (LDtkComponentLayer layer in level.LayerInstances)
            {
                if (!layer)
                    continue;

                Transform transform = layer.transform;
                Transform tilemap = transform.GetChild(0);
                tilemap.name = $"{layer.name} Tilemap";

                // Sort the tilemaps
                TilemapRenderer tilemapRenderer = tilemap.GetComponent<TilemapRenderer>();
                tilemapRenderer.sortingOrder = nextOrderInLayer;
                nextOrderInLayer--;

                bool hasCollision = false;

                foreach (string tag in layer.LayerDef.UiFilterTags)
                {
                    if (tag != "collision")
                        continue;

                    hasCollision = true;
                    break;
                }

                Object.DestroyImmediate(tilemap.GetComponent<CompositeCollider2D>());

                if (hasCollision)
                {
                    // Rebuild composite collider
                    tilemap.gameObject.AddComponent<CompositeCollider2D>();
                }
                else
                {
                    // Remove colliders for the wall and curtain layers
                    Object.DestroyImmediate(tilemap.GetComponent<TilemapCollider2D>());
                    Object.DestroyImmediate(tilemap.GetComponent<Rigidbody2D>());
                }

                SetEditorClassIdentifier(layer.GetComponent<LDtkComponentLayerIntGridValues>());
                Object.DestroyImmediate(layer.GetComponent<LDtkIid>());
                Object.DestroyImmediate(layer.GetComponent<Grid>());
                Object.DestroyImmediate(layer);
            }

            Object.DestroyImmediate(level.GetComponent<LDtkIid>());
            Object.DestroyImmediate(level);
        }

        /// <summary>
        /// Set the m_EditorClassIdentifier property on the object so the com.needle.missing-component-info package
        /// doesn't create an override for it on the prefab.
        /// </summary>
        private static void SetEditorClassIdentifier(Object targetObject)
        {
            var serializedObject = new SerializedObject(targetObject);
            SerializedProperty editorClassIdProp = serializedObject.FindProperty("m_EditorClassIdentifier");

            Type type = targetObject.GetType();
            string identifier = type.AssemblyQualifiedName;
            Assert.IsNotNull(identifier);

            identifier = string.Join(",", identifier.Split(',').Take(2));

            if (editorClassIdProp == null)
                return;

            editorClassIdProp.stringValue = identifier;
            serializedObject.ApplyModifiedProperties();
        }

        private static void Log(string message) =>
            Debug.Log("[LDtk Postprocessor] " + message);

        private static void LogWarning(string message) =>
            Debug.LogWarning("[LDtk Postprocessor] " + message);
    }
}
