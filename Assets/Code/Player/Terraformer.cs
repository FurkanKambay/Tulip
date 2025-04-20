using System;
using Furkan.Common;
using SaintsField;
using Tulip.Character;
using Tulip.Core;
using Tulip.Data;
using Tulip.Data.Items;
using Tulip.Gameplay;
using UnityEngine;
using UnityEngine.Assertions;

namespace Tulip.Player
{
    public class Terraformer : MonoBehaviour
    {
        [Header("References")]
        [SerializeField, Required] TangibleEntity entity;
        [SerializeField, Required] Inventory inventory;
        [SerializeField, Required] ItemWielder itemWielder;

        [Header("Config")]
        [SerializeField, Range(0, 1)] float centerOffset = 0.5f;
        [SerializeField, Min(0)] float coyoteTime = 0.15f;
        [SerializeField, Min(0)] float range = 5f;

        public event Action<Vector2Int?> OnChangeCellFocus;

        public Vector2Int MouseCell { get; private set; }

        public Vector2Int? FocusedCell
        {
            get => focusedCell;
            private set
            {
                if (focusedCell == value)
                    return;

                focusedCell = value;
                OnChangeCellFocus?.Invoke(focusedCell);
            }
        }

        private Vector2Int? focusedCell;
        private Vector3 hitPoint;
        private Vector2 topOrigin;
        private Vector2 bottomOrigin;

        private ItemStack latestSwungStack;
        private float coyoteTimeCounter;

        private void OnEnable() => itemWielder.OnSwingPerform += ItemWielder_Swing;
        private void OnDisable() => itemWielder.OnSwingPerform -= ItemWielder_Swing;

        private void Update()
        {
            coyoteTimeCounter += Time.deltaTime;

            AssignCells();
            AttemptUse();
        }

        private void ItemWielder_Swing(ItemStack stack, Vector3 aimPoint)
        {
            latestSwungStack = stack;
            coyoteTimeCounter = 0;
        }

        /// Respects coyote time
        private void AttemptUse()
        {
            if (!latestSwungStack.IsValid || coyoteTimeCounter > coyoteTime)
                return;

            if (UseTool(latestSwungStack))
                latestSwungStack = default;
        }

        private bool UseTool(ItemStack stack)
        {
            if (!FocusedCell.HasValue || stack.itemSO.IsNot(out BaseWorldToolSO tool))
                return false;

            Assert.IsNotNull(tool);

            ToolUsability usability = tool.GetUsability(entity.World, FocusedCell.Value);

            if (usability != ToolUsability.Available)
                return false;

            InventoryModification modification = tool.UseOn(entity.World, FocusedCell.Value);

            // TODO: drop items
            inventory.ApplyModification(modification);

            return true;
        }

        private void AssignCells()
        {
            if (itemWielder.CurrentStack.itemSO.IsNot(out BaseWorldToolSO _))
                return;

            Vector2 hotspot = transform.position;
            Vector2 aimPoint = hotspot + itemWielder.AimVector;

            MouseCell = entity.World.WorldToCell(aimPoint);

            if (entity.World.IsReadonly)
            {
                FocusedCell = null;
                return;
            }

            if (!Settings.Gameplay.UseSmartCursor || itemWielder.CurrentStack.itemSO.IsNot(out WorldToolSO _))
            {
                // holding a placeable OR not smart cursor
                float distance = Vector3.Distance(hotspot, aimPoint);
                FocusedCell = distance <= range ? MouseCell : null;
                return;
            }

            // holding a tool
            Vector2 aimDirection = itemWielder.AimVector.normalized;
            Vector2 offset = Vector3.Cross(aimDirection, Vector3.forward) * centerOffset;
            topOrigin = hotspot + offset;
            bottomOrigin = hotspot - offset;

            RaycastHit2D topHit = Physics2D.Raycast(topOrigin, aimDirection, range, LayerMask.GetMask("World"));
            RaycastHit2D bottomHit = Physics2D.Raycast(bottomOrigin, aimDirection, range, LayerMask.GetMask("World"));

            RaycastHit2D activeHit = ((bool)topHit, (bool)bottomHit) switch
            {
                (true, true) when bottomHit.distance <= topHit.distance => bottomHit,
                (true, _) => topHit,
                (false, true) => bottomHit,
                (false, false) => default
            };

            hitPoint = activeHit.point - (activeHit.normal * 0.1f);
            FocusedCell = activeHit ? entity.World.WorldToCell(hitPoint) : null;
        }

        private void OnDrawGizmosSelected()
        {
            if (!Settings.Gameplay.UseSmartCursor)
                return;

            Vector2 aimVector = itemWielder.AimVector.normalized * range;

            Gizmos.color = Color.red;
            Gizmos.DrawRay(topOrigin, aimVector);

            Gizmos.color = Color.red;
            Gizmos.DrawRay(bottomOrigin, aimVector);

            Gizmos.color = Color.green;
            Gizmos.DrawSphere(hitPoint, 0.1f);
        }
    }
}
