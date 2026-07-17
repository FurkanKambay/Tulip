using FK.Common;
using FK.Tulip.Data;
using FK.Tulip.Data.Items;
using FK.Tulip.GameWorld;
using UnityEngine;

namespace FK.Tulip.Character
{
    public class SurroundsChecker : MonoBehaviour
    {
        [Header("References")]
        [SerializeField, Required] Collider2D collider;

        [Header("Ground")]
        [SerializeField] LayerMask groundLayer;
        [SerializeField] float groundRange = .5f;
        [SerializeField] float groundWidth = .5f;

        [Header("Sides")]
        [SerializeField] LayerMask sidesLayer;
        [SerializeField] float sideRange = .5f;
        [SerializeField] float sideHeight = .5f;

        public bool IsGrounded { get; private set; }
        public bool IsLeftBlocked { get; private set; }
        public bool IsRightBlocked { get; private set; }
        public GroundMaterial GroundMaterial { get; private set; }

        private Transform entityTransform;
        private World world;

        private Vector2 MinBounds => collider.bounds.min;
        private Vector2 MaxBounds => collider.bounds.max;

        private Vector2 GroundLeft => new(collider.bounds.center.x - groundWidth, MinBounds.y);
        private Vector2 GroundRight => new(collider.bounds.center.x + groundWidth, MinBounds.y);

        private Vector2 LeftTop => new Vector2(MinBounds.x, MinBounds.y + sideHeight) + safetyLeft;
        private Vector2 LeftBottom => MinBounds + safetyLeft;

        private Vector2 RightTop => new Vector2(MaxBounds.x, MinBounds.y + sideHeight) + safetyRight;
        private Vector2 RightBottom => new Vector2(MaxBounds.x, MinBounds.y) + safetyRight;

        private static readonly Vector2 safetyLeft = new(-0.001f, 0.5f);
        private static readonly Vector2 safetyRight = new(0.001f, 0.5f);

        private void Awake() => entityTransform = collider.transform;
        private void Start() => world = entityTransform.GetComponent<TangibleEntity>().World;

        private void Update()
        {
            if (!world) return;

            Vector3 position = entityTransform.position + (Vector3.down * 0.2f);
            WorldTileAsset groundTile = world.GetTileAtWorld(position, TileType.Block);
            GroundMaterial = groundTile ? groundTile.Material : GroundMaterial.Nothing;
        }

        private void FixedUpdate()
        {
            UpdateGrounded();
            UpdateSides();
        }

        private void UpdateGrounded()
        {
            bool groundLeft = Physics2D.Raycast(GroundLeft, Vector2.down, groundRange, groundLayer);
            bool groundRight = Physics2D.Raycast(GroundRight, Vector2.down, groundRange, groundLayer);
            IsGrounded = groundLeft || groundRight;
        }

        private void UpdateSides()
        {
            bool leftTop = Physics2D.Raycast(LeftTop, Vector2.left, sideRange, sidesLayer);
            bool leftBottom = Physics2D.Raycast(LeftBottom, Vector2.left, sideRange, sidesLayer);
            bool rightTop = Physics2D.Raycast(RightTop, Vector2.right, sideRange, sidesLayer);
            bool rightBottom = Physics2D.Raycast(RightBottom, Vector2.right, sideRange, sidesLayer);
            IsLeftBlocked = leftTop && leftBottom;
            IsRightBlocked = rightTop && rightBottom;
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = IsGrounded ? Color.green : Color.gray;
            Gizmos.DrawRay(GroundLeft, Vector2.down * groundRange);
            Gizmos.DrawRay(GroundRight, Vector2.down * groundRange);

            Gizmos.color = IsLeftBlocked ? Color.green : Color.gray;
            Gizmos.DrawRay(LeftTop, Vector2.left * sideRange);
            Gizmos.DrawRay(LeftBottom, Vector2.left * sideRange);

            Gizmos.color = IsRightBlocked ? Color.green : Color.gray;
            Gizmos.DrawRay(RightTop, Vector2.right * sideRange);
            Gizmos.DrawRay(RightBottom, Vector2.right * sideRange);
        }
    }
}
