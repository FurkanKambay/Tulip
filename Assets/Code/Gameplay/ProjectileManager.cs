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

                bool shouldDestroy = projectile.MoveAndCollide(in contactFilter, results);

                if (shouldDestroy)
                    DestroyProjectile(projectileIndex);
            }
        }

        internal void Fire(WeaponSO weaponSO, Health owner, Vector3 aimPoint)
        {
            Vector3 origin = owner.transform.position;
            Vector2 aimVector = aimPoint - origin;

            // TODO: pool the projectiles

            Projectile projectile = Instantiate(
                original: projectilePrefab,
                position: origin,
                rotation: aimVector.ToQuaternion2D(),
                parent: projectileParent
            );

            projectiles.Add(projectile);
            projectile.Launch(aimVector, owner, weaponSO);

            Debug.DrawRay(origin, aimVector, Color.magenta);
        }

        private void DestroyProjectile(int index)
        {
            projectiles[index].Destroy();
            projectiles.RemoveAt(index);
        }
    }
}
