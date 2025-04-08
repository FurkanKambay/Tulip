using Furkan.Common;
using SaintsField;
using Tulip.Data;
using Tulip.Data.Items;
using Tulip.Gameplay;
using Tulip.GameWorld;
using UnityEngine;
using UnityEngine.Assertions;

namespace Tulip.Player
{
    /// <summary>
    /// * Highlight the focused cell with an unlit sprite (smooth Lerp) <br/>
    /// * If <see cref="PlaceableData"/>, use its sprite as a preview <br/>
    /// * Change color if the item can't be used <br/>
    /// * Visual impact effect on item swing
    /// </summary>
    /// <remarks>Depends on <see cref="World"/>, <see cref="Terraformer"/>, <see cref="IItemWielder"/></remarks>
    public sealed class CellHighlighter : MonoBehaviour
    {
        [Header("References")]
        [SerializeField, Required] World world;
        [SerializeField, Required] Terraformer terraformer;
        [SerializeField, Required] ItemWielder itemWielder;
        [SerializeField] new SpriteRenderer renderer;

        [Header("Config")]
        [SerializeField] float trackingSpeed = 100;
        [SerializeField] Color validColor = Color.white;
        [SerializeField] Color invalidColor = Color.red;
        [SerializeField] Vector2 impactTargetSize;

        private Vector3 targetPosition;
        private Vector2Int? focusedCell;
        private Sprite defaultSprite;
        private float impactLerp;

        private void Awake() => defaultSprite = renderer.sprite;

        private void OnEnable()
        {
            terraformer.OnChangeCellFocus += HandleCellFocusChanged;
            itemWielder.OnSwingStart += HandleSwingStarted;
            itemWielder.OnSwingPerform += HandleSwingPerformed;
        }

        private void OnDisable()
        {
            terraformer.OnChangeCellFocus -= HandleCellFocusChanged;
            itemWielder.OnSwingStart -= HandleSwingStarted;
            itemWielder.OnSwingPerform -= HandleSwingPerformed;
        }

        private void Update()
        {
            ItemData itemData = itemWielder.CurrentStack.itemData;

            if (!focusedCell.HasValue || itemData.IsNot(out BaseWorldToolData worldToolData))
            {
                renderer.enabled = false;
                impactLerp = 0;
                return;
            }

            Assert.IsNotNull(worldToolData);

            Color? color = worldToolData.GetUsability(world, focusedCell.Value) switch
            {
                ToolUsability.Available => validColor,
                ToolUsability.Invalid => invalidColor,
                ToolUsability.NotNow => invalidColor,
                _ => null
            };

            renderer.enabled = color.HasValue;
            renderer.sprite = worldToolData.CellHighlightSprite;
            renderer.color = color.GetValueOrDefault();

            // update marker state
            targetPosition = world.CellCenter(focusedCell.Value);

            // BUG: doesn't support multi-hit swing types
            impactLerp = Mathf.MoveTowards(impactLerp, 1, Time.deltaTime / worldToolData.GetTimeToFirstHit());

            // I once got a NaN error that I couldn't reproduce so just in case
            if (!float.IsFinite(impactLerp))
                impactLerp = 0;
        }

        private void LateUpdate()
        {
            transform.position = Vector3.Lerp(transform.position, targetPosition, trackingSpeed * Time.deltaTime);
            renderer.transform.localScale = Vector3.Lerp(Vector3.one, impactTargetSize, impactLerp);

            // reset scale when impact is done
            if (impactLerp >= 1)
                renderer.transform.localScale = Vector3.one;
        }

        private void HandleSwingStarted(ItemStack stack, Vector3 aimPoint) => impactLerp = 0;
        private void HandleSwingPerformed(ItemStack stack, Vector3 aimPoint) => impactLerp = 1;

        private void HandleCellFocusChanged(Vector2Int? cell)
        {
            focusedCell = cell;

            // TODO: this feels like a hack. handle this better
            if (impactLerp > 0)
                impactLerp = 1;
        }
    }
}
