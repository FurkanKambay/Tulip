using System;
using FK.Common;
using UnityEngine;
using UnityEngine.Tilemaps;
using Vertx.Debugging;

namespace FK.Tulip.Character
{
    public class AutoStepper : MonoBehaviour
    {
        [Header("References")]
        [SerializeField, Required] private TangibleEntity entity;
        [SerializeField, Required] private Rigidbody2D body;
        [SerializeField, Required] private CharacterMovement movement;
        [SerializeField, Required] private SurroundsChecker surrounds;

        [Header("Step")]
        [SerializeField] private float stepHeight = 1f;
        [SerializeField] private float stepWidth;
        [SerializeField] private Vector2 stepSpeed = Vector2.one;

        [Header("Thresholds")]
        [SerializeField] private float velocityThreshold = .1f;
        [SerializeField] private float range = .5f;
        [SerializeField] private Vector3 offset = Vector3.up * .5f;

        private Vector2? targetPosition;

        private void Update()
        {
            AutoStepDirection stepDirection = CanStepUp();

            if (stepDirection == AutoStepDirection.None)
            {
                targetPosition = null;
                return;
            }

            if (!surrounds.IsGrounded)
                return;

            Vector2 position = body.position;
            float width = stepDirection == AutoStepDirection.Left ? -stepWidth : stepWidth;
            targetPosition = new Vector2(position.x + width, position.y + stepHeight);
        }

        private void FixedUpdate()
        {
            if (!targetPosition.HasValue)
                return;

            Vector2 position = body.position;
            Vector2 moveDirection = (targetPosition.Value - position).normalized;
            Vector2 moveDelta = moveDirection * stepSpeed * Time.deltaTime;
            body.MovePosition(position + moveDelta);
        }

        private AutoStepDirection CanStepUp()
        {
            float velocity = movement.DesiredVelocity.x;

            if (Mathf.Abs(velocity) < velocityThreshold)
                return AutoStepDirection.None;

            Vector2 direction = Vector2.right * Math.Sign(velocity);
            Vector2 hotspot = transform.position + offset;

            RaycastHit2D hit = DrawPhysics2D.Raycast(hotspot, direction, range, LayerMask.GetMask("World"));
            if (!hit || !hit.collider.TryGetComponent(out Tilemap tilemap))
                return AutoStepDirection.None;

            Vector3 hitPoint = hit.point - (hit.normal * 0.1f);
            Vector3Int cell1 = tilemap.WorldToCell(hitPoint) + Vector3Int.up;
            Vector3Int cell2 = cell1 + Vector3Int.up;
            Vector3Int cell3 = cell2 + (velocity < 0 ? Vector3Int.right : Vector3Int.left);

            bool hasObstruction = tilemap.HasTile(cell1) || tilemap.HasTile(cell2) || tilemap.HasTile(cell3);
            if (hasObstruction)
                return AutoStepDirection.None;

            return velocity > 0 ? AutoStepDirection.Right : AutoStepDirection.Left;
        }

        private void OnDrawGizmosSelected()
        {
            if (!Application.isPlaying)
                return;

            if (!targetPosition.HasValue)
                return;

            Gizmos.color = Color.green;
            Gizmos.DrawSphere(targetPosition.Value, .3f);
        }

        private enum AutoStepDirection
        {
            None,
            Left,
            Right
        }
    }
}
