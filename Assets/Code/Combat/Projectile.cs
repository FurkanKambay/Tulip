using System.Collections.Generic;
using FK.Common;
using FK.Common.Extensions;
using FK.Tulip.Data.Items;
using UnityEngine;

namespace FK.Tulip.Combat
{
    public sealed class Projectile : MonoBehaviour, IWeapon
    {
        [Header("Config")]
        [SerializeField] private float gravityScale = 1;
        [SerializeField] private LayerMask obstacleLayers;

        [LayoutGroup("State", ELayout.TitleOut)]
        [ShowInInspector] private Health ownerHealth;
        [ShowInInspector] private WeaponAsset sourceWeapon;
        [ShowInInspector] private Vector2 velocity;
        [ShowInInspector] private ContactFilter2D contactFilter;
        [ShowInInspector] private readonly List<Health> damagedTargets = new();

        public WeaponAsset Asset => sourceWeapon;
        public Health Owner => ownerHealth;

        internal void Launch(Vector2 origin, Vector2 direction, Health owner, WeaponAsset weapon, ContactFilter2D filter)
        {
            ownerHealth = owner;
            sourceWeapon = weapon;
            contactFilter = filter;
            velocity = direction.normalized * sourceWeapon.ThrowStrength;

            transform.SetPositionAndRotation(origin, direction.ToQuaternion2D());
        }

        /// <summary>
        /// Move, rotate, and handle collisions between the previous and current position.
        /// </summary>
        /// <returns>Whether the projectile should be destroyed.</returns>
        internal bool MoveAndCollide(RaycastHit2D[] hitResults)
        {
            velocity += Physics2D.gravity * (gravityScale * Time.deltaTime);

            Vector2 previousPosition = transform.position;
            Vector2 currentPosition = previousPosition + (velocity * Time.deltaTime);
            transform.SetPositionAndRotation(currentPosition, velocity.ToQuaternion2D());

            Debug.DrawLine(currentPosition, previousPosition, Color.magenta);
            int hitCount = Physics2D.Linecast(currentPosition, previousPosition, contactFilter, hitResults);

            if (hitCount == 0)
                return false;

            bool shouldDestroy = false;

            if (!sourceWeapon.IsMultiTarget)
            {
                shouldDestroy = true;
                hitCount = 1;
            }

            for (int hitIndex = 0; hitIndex < hitCount; hitIndex++)
            {
                RaycastHit2D hit = hitResults[hitIndex];
                Collider2D hitCollider = hit.collider;

                if (hitCollider.TryGetComponent(out Hurtbox hurtbox))
                {
                    if (hurtbox.Owner == ownerHealth) continue;
                    bool undamagedTarget = !damagedTargets.Contains(hurtbox.Owner);

                    if (undamagedTarget && hurtbox.GetHit(this, DamageType.RangedWeapon))
                        damagedTargets.Add(hurtbox.Owner);
                }
                else if (obstacleLayers.Includes(hitCollider.gameObject))
                {
                    shouldDestroy = true;
                    break; // hit an obstacle: stop here
                }
            }

            return shouldDestroy;
        }

        internal void ResetState()
        {
            velocity = Vector2.zero;
            damagedTargets.Clear();
            contactFilter = default;
        }
    }
}
