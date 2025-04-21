using System.Collections.Generic;
using Tulip.Character;
using Tulip.Data.Items;
using UnityEngine;
using UnityEngine.Pool;

namespace Tulip.Gameplay
{
    public sealed class ProjectileManager : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] Projectile projectilePrefab;
        [SerializeField] Transform projectileParent;

        [Header("Config")]
        [SerializeField] ContactFilter2D contactFilter;
        [SerializeField] int initialPoolCapacity = 10;
        [SerializeField] int maxPoolSize = 100;

        private ObjectPool<Projectile> pool;
        private List<Projectile> allProjectiles;

        private readonly RaycastHit2D[] results = new RaycastHit2D[1];

        private void Awake()
        {
            allProjectiles = new List<Projectile>(maxPoolSize);

            pool = new ObjectPool<Projectile>(
                createFunc: () =>
                {
                    Projectile instance = Instantiate(projectilePrefab, projectileParent);
                    allProjectiles.Add(instance);
                    return instance;
                },
                actionOnDestroy: projectile =>
                {
                    allProjectiles.Remove(projectile);
                    Destroy(projectile.gameObject);
                },
                collectionCheck: true,
                defaultCapacity: initialPoolCapacity,
                maxSize: maxPoolSize
            );
        }

        private void FixedUpdate()
        {
            for (int projectileIndex = allProjectiles.Count - 1; projectileIndex >= 0; projectileIndex--)
            {
                Projectile projectile = allProjectiles[projectileIndex];

                if (!projectile.isActiveAndEnabled)
                    continue;

                bool shouldDestroy = projectile.MoveAndCollide(in contactFilter, results);

                if (shouldDestroy)
                    ReleaseProjectile(projectileIndex);
            }
        }

        internal void Fire(WeaponSO weaponSO, Health owner, Vector3 aimPoint)
        {
            Vector3 origin = owner.transform.position;
            Vector2 aimVector = aimPoint - origin;

            Projectile projectile = pool.Get();
            projectile.Launch(origin, aimVector, owner, weaponSO);
            projectile.gameObject.SetActive(true);

            Debug.DrawRay(origin, aimVector.normalized, Color.yellow);
        }

        private void ReleaseProjectile(int index)
        {
            Projectile projectile = allProjectiles[index];
            pool.Release(projectile);
            projectile.gameObject.SetActive(false);
            projectile.ResetState();
        }
    }
}
