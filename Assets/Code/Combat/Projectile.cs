using System.Collections.Generic;
using Furkan.Common;
using SaintsField.Playa;
using Tulip.Data.Items;
using UnityEngine;

namespace Tulip.Combat
{
    public sealed class Projectile : MonoBehaviour
    {
        [Header("Config")]
        [SerializeField] float gravityScale = 1;
        [SerializeField] LayerMask obstacleLayers;

        [LayoutGroup("State", ELayout.TitleOut)]
        [ShowInInspector] Health ownerHealth;
        [ShowInInspector] WeaponSO sourceWeapon;
        [ShowInInspector] Vector2 velocity;
        [ShowInInspector] ContactFilter2D contactFilter;
        [ShowInInspector] readonly List<Health> damagedTargets = new();

        internal void Launch(Vector2 origin, Vector2 direction, Health owner, WeaponSO weapon, ContactFilter2D filter)
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
                    if (undamagedTarget && hurtbox.GetHit(sourceWeapon, ownerHealth, DamageType.RangedWeapon))
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
