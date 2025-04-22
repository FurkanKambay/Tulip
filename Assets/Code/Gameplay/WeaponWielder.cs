using System.Collections.Generic;
using Furkan.Common;
using SaintsField;
using Tulip.Character;
using Tulip.Data;
using Tulip.Data.Gameplay;
using Tulip.Data.Items;
using UnityEngine;

namespace Tulip.Gameplay
{
    public sealed class WeaponWielder : MonoBehaviour
    {
        [Header("References")]
        [SerializeField, Required] Health health;
        [SerializeField, Required] ItemWielder itemWielder;
        [SerializeField] Inventory inventory;

        [Header("Config")]
        [SerializeField] ContactFilter2D hitContactFilter;
        [SerializeField] int maxHitsPerRaycast = 9;

        private ProjectileManager projectileManager;
        private WeaponSO weaponSO;
        private RaycastHit2D[] hitResults;

        private void Awake()
        {
            hitResults = new RaycastHit2D[maxHitsPerRaycast];
            projectileManager = FindFirstObjectByType<ProjectileManager>();
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

        private void Throw(ItemStack stack, Vector3 aimPoint)
        {
            if (stack.itemSO.Is(out weaponSO) && weaponSO.IsThrowable)
                projectileManager.Fire(weaponSO, health, aimPoint);
        }

        private void Attack(ItemStack stack, Vector3 targetPoint)
        {
            if (stack.itemSO.IsNot(out weaponSO))
                return;

            foreach (Health target in GetTargets(transform.position, targetPoint))
            {
                if (!target.enabled)
                    continue;

                InventoryModification loot = target.Damage(weaponSO.Damage, health, DamageType.MeleeWeapon);

                if (inventory)
                    inventory.ApplyModification(loot);
            }
        }

        private IEnumerable<Health> GetTargets(Vector2 origin, Vector2 aimPoint)
        {
            Vector2 direction = (aimPoint - origin).normalized;
            int hitCount = Physics2D.Raycast(origin, direction, hitContactFilter, hitResults, weaponSO.Range);

            int maxHits = weaponSO.IsMultiTarget ? maxHitsPerRaycast : 1;
            hitCount = Mathf.Min(hitCount, maxHits);

            Debug.DrawRay(origin, direction * weaponSO.Range, Color.green, 1f);

            for (int i = 0; i < hitCount; i++)
            {
                RaycastHit2D hit = hitResults[i];

                // We hit an obstacle (hit results are pre-sorted by distance)
                if (!hit || !hit.collider.TryGetComponent(out TangibleEntity entity) || !entity.Health)
                    break;

                yield return entity.Health;
            }
        }
    }
}
