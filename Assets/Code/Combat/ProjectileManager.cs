using System.Collections.Generic;
using FK.Tulip.Data.Items;
using UnityEngine;
using UnityEngine.Pool;

namespace FK.Tulip.Combat
{
    public sealed class ProjectileManager : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private Projectile projectilePrefab;
        [SerializeField] private Transform projectileParent;

        [Header("Config")]
        [SerializeField] private ContactFilter2D contactFilter;
        [SerializeField] private int initialPoolCapacity = 10;
        [SerializeField] private int maxPoolSize = 100;

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
                actionOnGet: projectile => projectile.gameObject.SetActive(true),
                actionOnRelease: projectile =>
                {
                    projectile.gameObject.SetActive(false);
                    projectile.ResetState();
                },
                actionOnDestroy: projectile =>
                {
                    allProjectiles.Remove(projectile);
                    if (projectile) Destroy(projectile.gameObject);
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

                bool shouldDestroy = projectile.MoveAndCollide(results);
                if (shouldDestroy)
                    pool.Release(projectile);
            }
        }

        internal void Fire(WeaponAsset weaponAsset, Health owner, Vector3 aimPoint)
        {
            Vector3 origin = owner.transform.position;
            Vector2 aimVector = aimPoint - origin;

            Projectile projectile = pool.Get();
            projectile.Launch(origin, aimVector, owner, weaponAsset, contactFilter);

            Debug.DrawRay(origin, aimVector.normalized, Color.yellow);
        }
    }
}
