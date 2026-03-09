using System.Collections.Generic;
using Furkan.Common;
using SaintsField.Playa;
using Tulip.Character;
using Tulip.Data.Items;
using UnityEngine;

namespace Tulip.Combat
{
    public sealed class Projectile : MonoBehaviour
    {
        [Header("Config")]
        [SerializeField] float gravityScale = 1;

        [LayoutGroup("State", ELayout.TitleOut)]
        [ShowInInspector] Health ownerHealth;
        [ShowInInspector] WeaponSO sourceWeapon;
        [ShowInInspector] Vector2 velocity;
        [ShowInInspector] readonly List<Health> damagedTargets = new();

        internal void Launch(Vector2 origin, Vector2 direction, Health owner, WeaponSO weapon)
        {
            ownerHealth = owner;
            sourceWeapon = weapon;
            velocity = direction.normalized * sourceWeapon.ThrowStrength;

            transform.SetPositionAndRotation(origin, direction.ToQuaternion2D());
        }

        /// <summary>
        /// Move, rotate, and handle collisions between the previous and current position.
        /// </summary>
        /// <returns>Whether the projectile should be destroyed.</returns>
        internal bool MoveAndCollide(in ContactFilter2D contactFilter, RaycastHit2D[] hitResults)
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

                // Hit an obstacle: destroy projectile
                if (!hit || !hit.collider.TryGetComponent(out Hurtbox hurtbox))
                {
                    shouldDestroy = true;
                    break;
                }

                // Already hit this target
                if (damagedTargets.Contains(hurtbox.Owner))
                    continue;

                hurtbox.GetHit(sourceWeapon, ownerHealth, DamageType.RangedWeapon);
                damagedTargets.Add(hurtbox.Owner);
            }

            return shouldDestroy;
        }

        internal void ResetState()
        {
            velocity = Vector2.zero;
            damagedTargets.Clear();
        }
    }
}
