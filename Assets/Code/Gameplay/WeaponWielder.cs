using System;
using System.Collections.Generic;
using System.Linq;
using Furkan.Common;
using SaintsField;
using Tulip.Character;
using Tulip.Data;
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
        [SerializeField] int maxMultiTargetAmount = 9;

        private ProjectileManager projectileManager;
        private WeaponSO weaponSO;
        private Collider2D[] hits = Array.Empty<Collider2D>();

        private void Awake() =>
            projectileManager = FindFirstObjectByType<ProjectileManager>();

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

            Array.Resize(ref hits, weaponSO.IsMultiTarget ? maxMultiTargetAmount : 1);

            foreach (Health target in GetTargets(transform.position, targetPoint))
            {
                if (!target.enabled)
                    continue;

                InventoryModification loot = target.Damage(weaponSO.Damage, health);

                if (inventory)
                    inventory.ApplyModification(loot);
            }
        }

        private IEnumerable<Health> GetTargets(Vector2 origin, Vector2 aimPoint)
        {
            Vector2 direction = (aimPoint - origin).normalized;

            var results = new RaycastHit2D[hits.Length];
            int hitCount = Physics2D.Raycast(origin, direction, hitContactFilter, results, weaponSO.Range);
            hits = results.Select(hit => hit.collider).ToArray();

            Debug.DrawRay(origin, direction * weaponSO.Range, Color.green, 1f);

            return hits
                .Take(hitCount)
                .TakeWhile(hit => (bool)hit)
                .Select(hit => hit.GetComponentInChildren<Health>())
                .TakeWhile(hitHealth => (bool)hitHealth);
        }
    }
}
