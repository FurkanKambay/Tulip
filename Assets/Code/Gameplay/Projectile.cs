using System.Collections.Generic;
using Furkan.Common;
using Tulip.Character;
using Tulip.Data.Items;
using UnityEngine;

namespace Tulip.Gameplay
{
    public sealed class Projectile : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] Rigidbody2D body;

        [Header("State")]
        [SerializeField] Health ownerHealth;
        [SerializeField] WeaponSO sourceWeapon;
        [SerializeField] List<Transform> damagedTargets;

        internal void Launch(Vector2 direction, Health owner, WeaponSO weapon)
        {
            ownerHealth = owner;
            sourceWeapon = weapon;
            body.AddForce(direction.normalized * sourceWeapon.ThrowStrength, ForceMode2D.Impulse);
        }

        /// <summary>
        /// Handle collisions between the previous and current position and update rotation.
        /// </summary>
        /// <returns>Whether the projectile should be destroyed.</returns>
        internal bool HandleCollisions(in ContactFilter2D contactFilter, RaycastHit2D[] hitResults)
        {
            body.SetRotation(body.linearVelocity.ToQuaternion2D());

            Vector2 currentPosition = body.position;
            Vector2 previousPosition = currentPosition - (body.linearVelocity * Time.deltaTime);

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

                // Already hit this target (or no hit)
                if (!hit || damagedTargets.Contains(hit.transform))
                    continue;

                TangibleEntity entity = hit.collider.GetComponent<TangibleEntity>();

                // Hit an obstacle: destroy projectile
                if (!entity || !entity.Health)
                {
                    shouldDestroy = true;
                    continue;
                }

                entity.Health.Damage(sourceWeapon.Damage, ownerHealth);
                damagedTargets.Add(entity.transform);
            }

            return shouldDestroy;
        }

        internal void Destroy()
        {
            // TODO: disable and reset state instead (for pooling)
            damagedTargets.Clear();
            Destroy(gameObject);
        }
    }
}
