using System.Collections.Generic;
using FK.Common;
using FK.Common.Extensions;
using FK.Tulip.Combat.Data;
using FK.Tulip.Data.Items;
using FK.Tulip.Gameplay;
using UnityEngine;

namespace FK.Tulip.Combat
{
    public sealed class WeaponWielder : MonoBehaviour, IWeapon
    {
        [SerializeField, Required] private WeaponWielderConfigAsset config;

        [Header("References")]
        [SerializeField, Required] private Health health;
        [SerializeField, Required] private ItemWielder itemWielder;

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
            int hitCount = Physics2D.Raycast(origin, direction, config.HitContactFilter, hitResults, weaponAsset.Range);

            int maxHits = weaponAsset.IsMultiTarget ? config.MaxHitsPerRaycast : 1;
            hitCount = Mathf.Min(hitCount, maxHits);

            Debug.DrawRay(origin, direction * weaponAsset.Range, Color.green, 1f);
            damagedTargets.Clear();

            for (int i = 0; i < hitCount; i++)
            {
                RaycastHit2D hit = hitResults[i];

                // We hit an obstacle (hit results are pre-sorted by distance)
                if (!hit || !hit.collider.TryGetComponent(out Hurtbox hurtbox))
                    break; // stop piercing further

                // Already hit this target
                if (damagedTargets.Contains(hurtbox.Owner))
                    continue;

                if (hurtbox.enabled)
                    yield return hurtbox;
            }
        }
    }
}
