using FK.Common;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace FK.Tulip.GameWorld
{
    public class RealmShader : MonoBehaviour
    {
        [Header("Tilemaps")]
        [SerializeField, Required] private Tilemap wallTilemap;
        [SerializeField, Required] private Tilemap blockTilemap;

        private void Awake()
        {
            TilemapRenderer wallRenderer = wallTilemap.GetComponent<TilemapRenderer>();
            TilemapRenderer blockRenderer = blockTilemap.GetComponent<TilemapRenderer>();

            // Hardcode sorting layers and materials for layers imported from LDtk
            wallRenderer.sortingLayerID = SortingLayer.NameToID("Wall");
            wallRenderer.sortingOrder = 0;

            blockRenderer.sortingLayerID = SortingLayer.NameToID("Default");
            blockRenderer.sortingOrder = 0;

            // Add sprite mask to the wall tilemap for the dust particles
            SpriteMask dustMask = wallRenderer.gameObject.AddComponent<SpriteMask>();
            dustMask.maskSource = SpriteMask.MaskSource.SupportedRenderers;
        }
    }
}
