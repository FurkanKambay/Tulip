using System;
using System.Collections.Generic;
using System.Linq;
using FK.Common;
using FK.Common.Extensions;
using FK.Tulip.Combat.Data;
using FK.Tulip.Data.Items;
using FK.Tulip.Gameplay;
using UnityEngine;
using Vertx.Attributes;

namespace FK.Tulip.Combat
{
    public sealed class WeaponWielder : MonoBehaviour, IWeapon
    {
        [SerializeField, Required] private WeaponWielderConfigAsset config;

        [Header("References")]
        [SerializeField, Required] private Health health;
        [SerializeField, Required] private ItemWielder itemWielder;

#if UNITY_EDITOR
        [Header("Debug")]
        [SerializeField, ReadOnlyField] private Collider2D[] hitColliders;
#endif

        public WeaponAsset Asset => weaponAsset;
        public Health Owner => health;

        private ProjectileManager projectileManager;
        private WeaponAsset weaponAsset;
        private RaycastHit2D[] hitResults;
        private readonly List<Health> damagedTargets = new();

        private void Awake()
        {
            hitResults = new RaycastHit2D[config.MaxHitsPerRaycast];
            projectileManager = FindAnyObjectByType<ProjectileManager>();
        }

        private void OnEnable()
        {
            itemWielder.OnSwingPerform += Attack;
            itemWielder.OnThrowPerform += Throw;
        }

        private void OnDisable()
        {
            itemWielder.OnSwingPerform -= Attack;
            itemWielder.OnThrowPerform -= Throw;
        }

        private void Throw(ItemAsset item, Vector3 aimPoint)
        {
            if (item.Is(out weaponAsset) && weaponAsset.IsThrowable)
                projectileManager.Fire(weaponAsset, health, aimPoint);
        }

        private void Attack(ItemAsset item, Vector3 targetPoint)
        {
            if (item.IsNot(out weaponAsset))
                return;

            foreach (Hurtbox target in GetTargets(transform.position, targetPoint))
            {
                if (target.GetHit(this))
                    damagedTargets.Add(target.Owner);
            }
        }

        private IEnumerable<Hurtbox> GetTargets(Vector2 origin, Vector2 aimPoint)
        {
            Vector2 direction = (aimPoint - origin).normalized;
            int maxHits = weaponAsset.IsMultiTarget ? config.MaxHitsPerRaycast : 1;
            int piercedCount = 0;

            int hitCount = Physics2D.Raycast(origin, direction, config.HitContactFilter, hitResults, weaponAsset.Range);

#if UNITY_EDITOR
            hitColliders = hitResults.Select(hit => hit.collider).ToArray();
            Array.Resize(ref hitColliders, hitCount);
#endif

            Debug.DrawRay(origin, direction * weaponAsset.Range, Color.green, 1f);
            damagedTargets.Clear();

            for (int i = 0; i < hitCount; i++)
            {
                if (piercedCount >= maxHits)
                    break; // reached max piercing

                RaycastHit2D hit = hitResults[i];
                if (!hit) continue;

                if (config.IsObstacle(hit.transform))
                    break; // we hit an obstacle (hit results are pre-sorted by distance)

                if (!hit.collider.TryGetComponent(out Hurtbox hurtbox) || !hurtbox.enabled)
                    continue; // skip colliders with no Hurtbox

                if (damagedTargets.Contains(hurtbox.Owner))
                    continue; // we've hit this target already

                piercedCount++;
                yield return hurtbox;
            }

            // Log.Info($"{Owner.Entity.name} detected {piercedCount} target HurtBoxes.");
        }
    }
}
