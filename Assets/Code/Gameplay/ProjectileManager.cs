using System.Collections.Generic;
using Furkan.Common;
using Tulip.Character;
using Tulip.Data.Items;
using UnityEngine;

namespace Tulip.Gameplay
{
    public sealed class ProjectileManager : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] Projectile projectilePrefab;
        [SerializeField] Transform projectileParent;

        [Header("Config")]
        [SerializeField] ContactFilter2D contactFilter;

        private List<Projectile> projectiles;

        private readonly RaycastHit2D[] results = new RaycastHit2D[1];

        private void Awake() =>
            projectiles = new List<Projectile>();

        private void FixedUpdate()
        {
            // Reversed so we can remove items safely
            for (int projectileIndex = projectiles.Count - 1; projectileIndex >= 0; projectileIndex--)
            {
                Projectile projectile = projectiles[projectileIndex];

                if (!projectile.isActiveAndEnabled)
                    continue;

                Rigidbody2D body = projectile.Body;
                Vector2 velocity = body.linearVelocity;

                body.SetRotation(velocity.ToQuaternion2D());

                Vector2 currentPosition = body.position;
                Vector2 previousPosition = currentPosition - (velocity * Time.deltaTime);

                Debug.DrawLine(currentPosition, previousPosition, Color.magenta);

                int hitCount = Physics2D.Linecast(currentPosition, previousPosition, contactFilter, results);

                if (hitCount == 0)
                    continue;

                if (!projectile.SourceWeapon.IsMultiTarget)
                {
                    DestroyProjectile(projectileIndex);
                    hitCount = 1;
                }

                for (int hitIndex = 0; hitIndex < hitCount; hitIndex++)
                {
                    RaycastHit2D hit = results[hitIndex];

                    // Already hit this target (or no hit)
                    if (!hit || projectile.DamagedTargets.Contains(hit.transform))
                        continue;

                    TangibleEntity entity = hit.collider.GetComponent<TangibleEntity>();

                    // Hit non-living object: destroy the projectile
                    if (!entity || !entity.Health)
                    {
                        DestroyProjectile(projectileIndex);
                        continue;
                    }

                    entity.Health.Damage(projectile.SourceWeapon.Damage, projectile.OwnerHealth);
                    projectile.DamagedTargets.Add(hit.transform);
                }
            }
        }

        public void Fire(WeaponData weaponData, Health owner, Vector3 aimPoint)
        {
            Vector3 origin = owner.transform.position;
            Vector2 aimVector = aimPoint - origin;

            // TODO: pool the projectiles

            Projectile instance = Instantiate(
                original: projectilePrefab,
                position: origin,
                rotation: aimVector.ToQuaternion2D(),
                parent: projectileParent
            );

            instance.OwnerHealth = owner;
            instance.SourceWeapon = weaponData;
            projectiles.Add(instance);

            instance.Body.AddForce(aimVector.normalized * weaponData.ThrowStrength, ForceMode2D.Impulse);
            Debug.DrawRay(origin, aimVector, Color.magenta);
        }

        private void DestroyProjectile(int index)
        {
            projectiles[index].Destroy();
            projectiles.RemoveAt(index);
        }
    }
}
